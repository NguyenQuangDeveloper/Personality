using LiveCharts.Definitions.Series;
using LiveCharts.Wpf.Charts.Base;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing.Segments;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace VSLibrary.UIComponent.VsCharts
{
    public class VsRTChart : Control
    {
        private CartesianChart? _chart;
        private ContextMenu? _seriesContextMenu;

        private const int YLeftStepCount = 5;
        private const int YRightStepCount = 10;

        private double _initialXMin;
        private double _initialXMax;

        public DateTime _startTime;

        static VsRTChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(VsRTChart),
                new FrameworkPropertyMetadata(typeof(VsRTChart)));

            if(Application.Current != null)
            {
                try
                {
                    var dict = new ResourceDictionary
                    {
                        Source = new Uri("/VSLibrary;component/UIComponent/Styles/VsRTChartStyle.xaml",
                                UriKind.RelativeOrAbsolute)
                    };
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }
                catch { };
            }
        }

        public VsRTChart()
        {
            // 1) 시리즈 기본 생성
            Series = new ObservableCollection<ISeries>();
            Series.CollectionChanged += (s, e) => BuildContextMenu();

            // 2) DP 기본값으로 축 생성
            XAxes = new[]
            {
                new Axis { Name = XAxisTitle, Labeler = v => v.ToString("F1") }
            };
            YAxes = new[]
            {
                new Axis { Name = YLeftAxisTitle, Labeler = v => v.ToString("F2") },
                new Axis { Name = YRightAxisTitle, Position = AxisPosition.End, Labeler = v => v.ToString("F2") }
            };

            DrawMarginFrame = new DrawMarginFrame
            {
                Fill = new SolidColorPaint(SKColors.White),
                Stroke = new SolidColorPaint(SKColors.Black, 1)
            };

           // InitDesignMode();

            // 컨트롤이 Loaded 될 때 축 설정 초기화
            this.Loaded += (sender, e) =>
            {
                //InitDesignMode();
                //InitChart();
            };
        }


        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    _chart = GetTemplateChild("PART_Chart") as CartesianChart;

        //    if (_chart != null)
        //    {

        //        _chart.Series = Series;

        //        //_initialXMin = XAxisMin;
        //        //_initialXMax = XAxisMax;

        //        _chart.LegendTextSize = LegendTextSize;

        //        InitDesignMode();

        //        InitChart( Convert.ToInt32(XAxisMax) );

        //    }
        //}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _chart = GetTemplateChild("PART_Chart") as CartesianChart;

            if (_chart != null)
            {
                // 1. 컨트롤의 DP들을 내부 차트(_chart)의 속성에 연결합니다.
                _chart.Series = Series;
                _chart.XAxes = XAxes;
                _chart.YAxes = YAxes;
                _chart.LegendTextSize = LegendTextSize;

                // 2. 디자이너 모드일 때만 샘플 데이터를 채웁니다.
                InitDesignMode();

                // 3. DP에 설정된 값들로 축 서식을 최종 적용합니다.
                ApplyAxisSettings();
            }
        }

        // ───────────────────────────────────────────────
        // Series DP
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series),
                typeof(ObservableCollection<ISeries>),
                typeof(VsRTChart),
                new PropertyMetadata(null));

        public ObservableCollection<ISeries> Series
        {
            get => (ObservableCollection<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        // XAxes DP
        public static readonly DependencyProperty XAxesProperty =
            DependencyProperty.Register(
                nameof(XAxes),
                typeof(Axis[]),
                typeof(VsRTChart),
                new PropertyMetadata(null));

        public Axis[] XAxes
        {
            get => (Axis[])GetValue(XAxesProperty);
            set => SetValue(XAxesProperty, value);
        }

        // YAxes DP
        public static readonly DependencyProperty YAxesProperty =
            DependencyProperty.Register(
                nameof(YAxes),
                typeof(Axis[]),
                typeof(VsRTChart),
                new PropertyMetadata(null));

        public Axis[] YAxes
        {
            get => (Axis[])GetValue(YAxesProperty);
            set => SetValue(YAxesProperty, value);
        }

        // DrawMarginFrame DP
        public static readonly DependencyProperty DrawMarginFrameProperty =
            DependencyProperty.Register(
                nameof(DrawMarginFrame),
                typeof(DrawMarginFrame),
                typeof(VsRTChart),
                new PropertyMetadata(null));

        public DrawMarginFrame DrawMarginFrame
        {
            get => (DrawMarginFrame)GetValue(DrawMarginFrameProperty);
            set => SetValue(DrawMarginFrameProperty, value);
        }

        // ───────────────────────────────────────────────
        // ChartTitle DP
        public static readonly DependencyProperty ChartTitleProperty =
            DependencyProperty.Register(
                nameof(ChartTitle),
                typeof(string),
                typeof(VsRTChart),
                new PropertyMetadata("Chamber A"));

        public string ChartTitle
        {
            get => (string)GetValue(ChartTitleProperty);
            set => SetValue(ChartTitleProperty, value);
        }

        // XAxisTitle DP
        public static readonly DependencyProperty XAxisTitleProperty =
            DependencyProperty.Register(
                nameof(XAxisTitle),
                typeof(string),
                typeof(VsRTChart),
                new PropertyMetadata("Time", OnAxisTitleChanged));

        public string XAxisTitle
        {
            get => (string)GetValue(XAxisTitleProperty);
            set => SetValue(XAxisTitleProperty, value);
        }

        // YLeftAxisTitle DP
        public static readonly DependencyProperty YLeftAxisTitleProperty =
            DependencyProperty.Register(
                nameof(YLeftAxisTitle),
                typeof(string),
                typeof(VsRTChart),
                new PropertyMetadata("Temp.(°C)", OnAxisTitleChanged));

        public string YLeftAxisTitle
        {
            get => (string)GetValue(YLeftAxisTitleProperty);
            set => SetValue(YLeftAxisTitleProperty, value);
        }

        // YRightAxisTitle DP
        public static readonly DependencyProperty YRightAxisTitleProperty =
            DependencyProperty.Register(
                nameof(YRightAxisTitle),
                typeof(string),
                typeof(VsRTChart),
                new PropertyMetadata("O2(ppm)", OnAxisTitleChanged));

        public string YRightAxisTitle
        {
            get => (string)GetValue(YRightAxisTitleProperty);
            set => SetValue(YRightAxisTitleProperty, value);
        }

        // --- X축 설정용 DP ---
        public static readonly DependencyProperty XStepMinutesProperty =
            DependencyProperty.Register(
                nameof(XStepMinutes), typeof(double), typeof(VsRTChart),
                new PropertyMetadata(30d, OnAxisSettingChanged));
        public double XStepMinutes
        {
            get => (double)GetValue(XStepMinutesProperty);
            set => SetValue(XStepMinutesProperty, value);
        }

        public static readonly DependencyProperty XAxisMinProperty =
            DependencyProperty.Register(
                nameof(XAxisMin), typeof(double), typeof(VsRTChart),
                new PropertyMetadata(0d, OnAxisSettingChanged));
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double XAxisMin
        {
            get => (double)GetValue(XAxisMinProperty);
            set => SetValue(XAxisMinProperty, value);
        }

        public static readonly DependencyProperty XAxisMaxProperty =
            DependencyProperty.Register(
                nameof(XAxisMax), typeof(double), typeof(VsRTChart),
                new PropertyMetadata(180d, OnAxisSettingChanged));
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double XAxisMax
        {
            get => (double)GetValue(XAxisMaxProperty);
            set => SetValue(XAxisMaxProperty, value);
        }

        // --- Y축 설정용 DP (왼/오른쪽) ---
        public static readonly DependencyProperty YLeftMaxProperty =
            DependencyProperty.Register(
                nameof(YLeftMax), typeof(double), typeof(VsRTChart),
                new PropertyMetadata(250d, OnAxisSettingChanged));
        public double YLeftMax
        {
            get => (double)GetValue(YLeftMaxProperty);
            set => SetValue(YLeftMaxProperty, value);
        }


        public static readonly DependencyProperty YRightMaxProperty =
            DependencyProperty.Register(
                nameof(YRightMax), typeof(double), typeof(VsRTChart),
                new PropertyMetadata(200d, OnAxisSettingChanged));
        public double YRightMax
        {
            get => (double)GetValue(YRightMaxProperty);
            set => SetValue(YRightMaxProperty, value);
        }

        // 차트 제목 폰트 크기
        public static readonly DependencyProperty ChartTitleFontSizeProperty =
            DependencyProperty.Register(
                nameof(ChartTitleFontSize),
                typeof(double),
                typeof(VsRTChart),
                new PropertyMetadata(16d, OnAxisSettingChanged));

        public double ChartTitleFontSize
        {
            get => (double)GetValue(ChartTitleFontSizeProperty);
            set => SetValue(ChartTitleFontSizeProperty, value);
        }

        public static readonly DependencyProperty LegendTextSizeProperty = DependencyProperty.Register(
           nameof(LegendTextSize), typeof(double), typeof(VsRTChart), new PropertyMetadata(15d, OnLegendSettingChanged));
        public double LegendTextSize
        {
            get => (double)GetValue(LegendTextSizeProperty);
            set => SetValue(LegendTextSizeProperty, value);
        }

        private static void OnLegendSettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VsRTChart chart && chart._chart != null)
            {
                // 레전드 텍스트 크기와 도형 크기 동기화
                chart._chart.LegendTextSize = chart.LegendTextSize;
             
            }
        }

        private static void OnAxisSettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VsRTChart chart) chart.ApplyAxisSettings();
        }

        #region Private Field
        private bool IsInDesignMode()
        {
            //return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            return (bool)DesignerProperties.IsInDesignModeProperty
                .GetMetadata(typeof(FrameworkElement)).DefaultValue;
        }
        private void InitDesignMode()
        {
            // 🎯 디자인 모드라면 시리즈 없으면 샘플 데이터 삽입
            if (IsInDesignMode() && (Series == null || Series.Count == 0))
            {
                var values = new ObservableCollection<ObservablePoint>
                {
                    new ObservablePoint(0, 10),
                    new ObservablePoint(1, 20),
                    new ObservablePoint(2, 15)
                };

                Series = new ObservableCollection<ISeries>
                {
                    new LineSeries<ObservablePoint>
                    {
                        Name = "Preview",
                        Values = values,
                        Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                        Fill = null,
                        GeometrySize = 6
                    }
                };

                _chart.Series = Series;
            }
            //if (DesignerProperties.GetIsInDesignMode(this))
            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //{
            //    var values = new ObservableCollection<ObservablePoint>
            //    {
            //        new ObservablePoint(0, 10),
            //        new ObservablePoint(1, 20),
            //        new ObservablePoint(2, 15)
            //    };

            //    Series.Add(new LineSeries<ObservablePoint>
            //    {
            //        Name = "DesignTime",
            //        Values = values,
            //        Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
            //        Fill = null,
            //        GeometrySize = 6
            //    });
        }

        private void ApplyAxisSettings()
        {
            // _chart가 null이거나, 축 DP가 초기화되지 않았으면 아무것도 하지 않음
            if (_chart == null || XAxes == null || XAxes.Length == 0 || YAxes == null || YAxes.Length < 2)
            {
                return;
            }

            // --- X축 업데이트 ---
            // 기존 XAxes 배열의 첫 번째 Axis 객체를 가져와 속성만 변경
            var xAxis = XAxes[0];
            var segmentCount = (int)Math.Floor((XAxisMax - XAxisMin) / XStepMinutes);
            var ticks = Enumerable
                .Range(0, segmentCount + 1)
                .Select(i => XAxisMin + i * XStepMinutes)
                .ToArray();

            xAxis.MinLimit = XAxisMin;
            xAxis.MaxLimit = XAxisMax;
            xAxis.CustomSeparators = ticks;
            xAxis.Name = XAxisTitle;
            xAxis.SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 };
            xAxis.Labeler = value =>
            {
                var ts = TimeSpan.FromMinutes(value);
                return ts.ToString(@"hh\:mm");
            };


            // --- Y축 업데이트 ---
            // 왼쪽 Y축 객체를 가져와 속성만 변경
            var yLeftAxis = YAxes[0];
            double leftStep = YLeftMax / YLeftStepCount;
            yLeftAxis.MinLimit = 0;
            yLeftAxis.MaxLimit = YLeftMax;
            yLeftAxis.CustomSeparators = Enumerable.Range(0, YLeftStepCount + 1).Select(i => i * (YLeftMax / YLeftStepCount)).ToArray();
            yLeftAxis.Labeler = v => FormatSmart(v);
            yLeftAxis.Name = YLeftAxisTitle;
            yLeftAxis.SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 };

            // 오른쪽 Y축 객체를 가져와 속성만 변경
            var yRightAxis = YAxes[1];
            double rightStep = YRightMax / YRightStepCount;
            yRightAxis.MinLimit = 0;
            yRightAxis.MaxLimit = YRightMax;
            yRightAxis.CustomSeparators = Enumerable.Range(0, YRightStepCount + 1).Select(i => i * (YRightMax / YRightStepCount)).ToArray();
            yRightAxis.Labeler = v => FormatSmart(v);
            yRightAxis.Name = YRightAxisTitle;
            yRightAxis.Position = AxisPosition.End;
            yRightAxis.SeparatorsPaint = new SolidColorPaint(SKColors.Transparent); // 오른쪽 축 라인은 보통 숨깁니다. 필요시 색상 변경.


            // _chart에 DP를 다시 할당하여 변경 사항을 알림
            _chart.XAxes = XAxes;
            _chart.YAxes = YAxes;
        }

        //private void ApplyAxisSettings()
        //{
        //    if (_chart == null) return;

        //    var segmentCount = (int)Math.Floor((XAxisMax - XAxisMin) / XStepMinutes);
        //    var ticks = Enumerable
        //        .Range(0, segmentCount + 1)
        //        .Select(i => XAxisMin + i * XStepMinutes)
        //        .ToArray();
        //    // X축
        //    _chart.XAxes = new[]
        //    {
        //        new Axis
        //        {
        //            MinLimit  = XAxisMin,
        //            MaxLimit  = XAxisMax,
        //            //UnitWidth = XStepSeconds,
        //            CustomSeparators = ticks,
        //            Name = XAxisTitle,
        //            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray){ StrokeThickness=1 },
        //            //Labeler  = v => FormatSmart(v),
        //            Labeler   = value =>
        //            {
        //                //var ts = TimeSpan.FromSeconds(value);
        //                //return ts.ToString(@"mm\:ss");

        //                var ts = TimeSpan.FromMinutes(value);
        //                return ts.ToString(@"hh\:mm");
        //            },

        //        }
        //    };

        //    double leftStep = YLeftMax / YLeftStepCount;   // 5칸 분할
        //    double rightStep = YRightMax / YRightStepCount;  // 10칸 분할
        //    var globalMax = Math.Max(YLeftMax, YRightMax);
        //    // Y축: 왼쪽, 오른쪽
        //    _chart.YAxes = new[]
        //    {
        //        // 왼쪽 축
        //        new Axis
        //        {
        //            MinLimit = 0,
        //            MaxLimit = YLeftMax,    //globalMax,
        //            //MinStep = leftStep,
        //            //UnitWidth = leftStep,
        //            CustomSeparators = Enumerable.Range(0, YLeftStepCount + 1).Select(i => i * (YLeftMax / YLeftStepCount)).ToArray(),
        //            Labeler  = v => FormatSmart(v),
        //            Name = YLeftAxisTitle,
        //            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray){ StrokeThickness=1 }
        //        },
        //        // 오른쪽 축
        //        new Axis
        //        {
        //            MinLimit = 0,
        //            MaxLimit = YRightMax,   //globalMax,
        //            //MinStep = rightStep,
        //            //UnitWidth = rightStep,
        //            CustomSeparators = Enumerable.Range(0, YRightStepCount + 1).Select(i => i * (YRightMax / YRightStepCount)).ToArray(),
        //            Labeler  = v => FormatSmart(v),
        //            Name = YRightAxisTitle,
        //            Position  = AxisPosition.End,
        //            SeparatorsPaint = new SolidColorPaint(SKColors.Yellow){ StrokeThickness=1 }
        //        }
        //    };

        //}

        private static string FormatSmart(double value) =>
            Math.Abs(value % 1) < Double.Epsilon * 100
                ? value.ToString("N0")
                : value.ToString("N1");
       

        private static void OnAxisTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VsRTChart chart)
                chart.UpdateAxisTitles();
        }

        private void UpdateAxisTitles()
        {
            // X축
            if (XAxes != null && XAxes.Length > 0)
                XAxes[0].Name = XAxisTitle;
            // Y축 좌
            if (YAxes != null && YAxes.Length > 0)
                YAxes[0].Name = YLeftAxisTitle;
            // Y축 우
            if (YAxes != null && YAxes.Length > 1)
                YAxes[1].Name = YRightAxisTitle;
        }

        private ContextMenu BuildContextMenu()
        {
            var cm = new ContextMenu();
           

            // Separator 하나 추가 (원하시면 뺄 수도 있습니다)
            cm.Items.Add(new Separator());

            // Series 목록만큼 메뉴 아이템 생성
            for (int i = 0; i < Series.Count; i++)
            {
                var series = Series[i];
                var mi = new MenuItem
                {
                    Header = series.Name,
                    IsCheckable = true,
                    IsChecked = series.IsVisible,
                    Focusable = false,
                    StaysOpenOnClick = true,
                    Tag = i
                };

                int idx = i; // 클로저 캡쳐
                mi.Checked += (_, __) => SetSeriesVisibility(idx, true);
                mi.Unchecked += (_, __) => SetSeriesVisibility(idx, false);

                cm.Items.Add(mi);
            }
            _seriesContextMenu = cm;
            if(_chart != null)
            {
                _chart.ContextMenu = _seriesContextMenu;
            }
               

            return _seriesContextMenu;
        }

        /// <summary>
        /// 전체 시리즈에 (x,y) 포인트를 추가합니다.
        /// 시리즈 생성 Index 만큼의 ys 값을 넘겨받습니다.
        /// </summary>
        private void AddPoint(double x, double[] ys)
        {
            if (Series == null || ys.Length > Series.Count) return;

            // 새 포인트 추가
            for (int i = 0; i < ys.Length; i++)
            {
                if (Series[i] is LineSeries<ObservablePoint> ls &&
                    ls.Values is ObservableCollection<ObservablePoint> col)
                {
                    col.Add(new ObservablePoint(x, ys[i]));
                }
            }

            // x가 현재 MaxLimit을 넘으면 윈도우를 스텝만큼 민다
            if (x > XAxisMax)
            {
                XAxisMin += XStepMinutes;
                XAxisMax += XStepMinutes;

                // 축에 반영
                if (_chart?.XAxes.FirstOrDefault() is Axis axis)
                {
                    axis.MinLimit = XAxisMin;
                    axis.MaxLimit = XAxisMax;
                }

                // 각 시리즈에서 X < XAxisMin 인 포인트들 전부 제거
                foreach (var s in Series.OfType<LineSeries<ObservablePoint>>())
                {
                    var col = s.Values as ObservableCollection<ObservablePoint>;
                    if (col == null) continue;
                    // 보이는 구간 이전의 모든 포인트 삭제
                    while (col.Count > 0 && col[0].X < XAxisMin)
                        col.RemoveAt(0);
                }


            }
        }

        /// <summary>
        /// 지정한 시리즈에 (X, Y) 데이터 점을 추가합니다.
        /// </summary>
        /// <param name="seriesIndex">추가할 시리즈의 인덱스</param>
        /// <param name="x">X축 값 (경과 시간: 분 단위)</param>
        /// <param name="y">Y축 값</param>
        private void AddPoint(int seriesIndex, double x, double y)
        {
            if (_chart == null || Series == null) return;
            if (seriesIndex < 0 || seriesIndex >= Series.Count) return;

            // 시리즈 가져오기 및 값 추가
            if (Series[seriesIndex] is LineSeries<ObservablePoint> lineSeries &&
                lineSeries.Values is IList<ObservablePoint> values)
            {
                values.Add(new ObservablePoint(x, y));
            }

            // X축 범위를 넘었는지 확인
            if (x > XAxisMax)
            {
                XAxisMin += XStepMinutes;
                XAxisMax += XStepMinutes;

                // 축 갱신
                if (_chart?.XAxes.FirstOrDefault() is Axis axis)
                {
                    axis.MinLimit = XAxisMin;
                    axis.MaxLimit = XAxisMax;
                }

                // 시리즈 내 낡은 포인트 제거
                foreach (var s in Series.OfType<LineSeries<ObservablePoint>>())
                {
                    if (s.Values is not ObservableCollection<ObservablePoint> c) continue;

                    while (c.Count > 0 && c[0].X < XAxisMin)
                        c.RemoveAt(0);
                }
            }
        }

        #endregion




        // ───────────────────────────────────────────────
        #region 실시간 API
        public void InitChart(int cureTime)
        {
            XAxisMax = cureTime;

            _initialXMin = XAxisMin;
            _initialXMax = XAxisMax;

            ClearAllSeries();
            ApplyAxisSettings();

            _startTime = default;
        }
        /// <summary>
        /// 새 LineSeries 를 생성해서 컬렉션에 추가합니다.
        /// Returns the Values 컬렉션을 호출자에게 줘서 데이터 추가를 허용.
        /// </summary>
        public ObservableCollection<ObservablePoint> AddSeries(string name, SKColor color, bool useRightAxis = false)
        {
            var values = new ObservableCollection<ObservablePoint>();
            var s = new LineSeries<ObservablePoint>
            {
                Name = name,
                Values = values,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
                Fill = null,    // 면 채우기 없음
                GeometrySize = 0,        // 마커(점) 숨기기
                ScalesYAt = useRightAxis ? 1 : 0,
                Tag = Series.Count
            };
            Series.Add(s);

          
            return values;
        }

        /// <summary>
        /// DateTime 기준으로 각 시리즈에 데이터를 추가합니다.
        /// 시간이 새로운 시(시간)로 넘어가면 차트를 초기화(클리어)하고,
        /// x축은 기준 시점부터 지난 분 단위로 계산됩니다.
        /// </summary>
        public void AddXY(DateTime time, double[] ys)
        {
            // 차트 시작 시점을 “해당 시의 0분 0초”로 초기화
            if (_startTime == default)
            {
                _startTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
            }

            // 경과 분 단위로 X값 계산
            var elapsedMin = (time - _startTime).TotalMinutes;

            // 기존 +(x, ys) 호출
            AddPoint(elapsedMin, ys);
        }

        /// <summary>
        /// DateTime 기준으로 단일 시리즈에만 데이터를 추가합니다.
        /// </summary>
        /// <param name="seriesIndex">0-based 시리즈 인덱스</param>
        /// <param name="time">현재 시각</param>
        /// <param name="y">추가할 Y값</param>
        public void AddXY(int seriesIndex, DateTime time, double y)
        {
            // _startTime이 기본값(default)이면, time의 "시:분:초"를 기준점으로 설정
            if (_startTime == default)
                _startTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

            // 시리즈 인덱스 유효성 검사
            if (Series == null || seriesIndex < 0 || seriesIndex >= Series.Count) return;

            // 경과 분 단위로 x 계산
            var elapsedMin = (time - _startTime).TotalMinutes;

            // 해당 시리즈에만 AddPoint 호출
            AddPoint(seriesIndex, elapsedMin, y);
        }


        /// <summary>
        /// 모든 시리즈의 데이터를 지웁니다.
        /// </summary>
        public void ClearAllSeries()
        {
            if (Series == null) return;

            foreach (var s in Series)
            {
                switch (s)
                {
                    case LineSeries<ObservablePoint> lsPoint
                        when lsPoint.Values is ObservableCollection<ObservablePoint> colPoint:
                        colPoint.Clear();
                        break;

                    case LineSeries<double> lsDouble
                        when lsDouble.Values is ObservableCollection<double> colDouble:
                        colDouble.Clear();
                        break;

                    default:
                        break;
                }
            }

            XAxisMin = _initialXMin;
            XAxisMax = _initialXMax;

            if (_chart?.XAxes?.FirstOrDefault() is Axis xAxis)
            {
                xAxis.MinLimit = XAxisMin;
                xAxis.MaxLimit = XAxisMax;
            }
        }

        /// <summary>
        /// 특정 seriesIndex의 시리즈만 지웁니다.
        /// </summary>
        public void ClearSeries(int seriesIndex)
        {
            if (Series == null
             || seriesIndex < 0
             || seriesIndex >= Series.Count) return;

            var s = Series[seriesIndex];
            switch (s)
            {
                case LineSeries<ObservablePoint> lsPoint
                    when lsPoint.Values is ObservableCollection<ObservablePoint> colPoint:
                    colPoint.Clear();
                    break;

                case LineSeries<double> lsDouble
                    when lsDouble.Values is ObservableCollection<double> colDouble:
                    colDouble.Clear();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 지정한 인덱스의 Series의 보임/숨김을 설정합니다.
        /// </summary>
        /// <param name="seriesIndex">0부터 시작하는 Series 컬렉션 인덱스</param>
        /// <param name="visible">true면 보이기, false면 숨기기</param>
        public void SetSeriesVisibility(int seriesIndex, bool visible)
        {
            if (Series == null
             || seriesIndex < 0
             || seriesIndex >= Series.Count) return;

            var s = Series[seriesIndex];
            s.IsVisible = visible;
            s.IsVisibleAtLegend = visible;

            // ContextMenu 체크 상태도 동기화
            if (_seriesContextMenu != null)
            {
                foreach (var item in _seriesContextMenu.Items.OfType<MenuItem>())
                {
                    if (item.Tag is int idx && idx == seriesIndex)
                    {
                        if(item.IsChecked != visible)
                            item.IsChecked = visible;

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 지정한 이름(Name)을 가진 Series의 보임/숨김을 설정합니다.
        /// </summary>
        /// <param name="seriesName">Series.Name 프로퍼티와 같은 문자열</param>
        /// <param name="visible">true면 보이기, false면 숨기기</param>
        public void SetSeriesVisibility(string seriesName, bool visible)
        {
            if (Series == null) return;
            var s = Series.FirstOrDefault(x => x.Name == seriesName);
            if (s == null) return;

            s.IsVisible = visible;
            s.IsVisibleAtLegend = visible;

            // ContextMenu 체크 상태도 동기화
            if (_seriesContextMenu != null)
            {
                foreach (var item in _seriesContextMenu.Items.OfType<MenuItem>())
                {
                    if (item.Header is string name && name == seriesName)
                    {
                        if (item.IsChecked != visible)
                            item.IsChecked = visible;

                        break;
                    }
                }
            }
        }

        
        #endregion
    }
}
