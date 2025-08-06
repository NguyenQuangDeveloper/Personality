using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Separator = LiveCharts.Wpf.Separator;

namespace VSLibrary.UIComponent.VsCharts
{
    public class VsChart : Control
    {
        #region Formatter Functions
        public static Func<double, string> FormatDateTime { get; } = value =>
        {
            if (double.IsNaN(value) || value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                return "";
            return new DateTime((long)value).ToString("MM-dd\nHH:mm:ss");
        };

        public static Func<double, string> FormatSmart { get; } = value =>
        {
            if (Math.Abs(value % 1) < (Double.Epsilon * 100))
                return value.ToString("N0");
            else
                return value.ToString("N1");
        };

        public static Func<double, string> FormatN0 { get; } = value => value.ToString("N0");
        public static Func<double, string> FormatN1 { get; } = value => value.ToString("N1");
        #endregion

        #region Private Field
        // PART_Chart 참조용
        private CartesianChart? _chart;
        private bool _drawFull = false;
        // 1) 원본 데이터를 보관할 리스트
        private readonly List<DateTimePoint> _rawRfFwd = new();
        private readonly List<DateTimePoint> _rawRfRef = new();
        private readonly List<DateTimePoint>[] _rawGas = new[] {
            new List<DateTimePoint>(),
            new List<DateTimePoint>(),
            new List<DateTimePoint>(),
            new List<DateTimePoint>()
        };
        private readonly List<DateTimePoint> _rawVac = new();

        // ─── 2) CSV 로드 (한 번만 호출) ────────────────────────────────────────
        private void LoadCsv(string[] csvLines)
        {
            _rawRfFwd.Clear();
            _rawRfRef.Clear();
            foreach (var g in _rawGas) g.Clear();
            _rawVac.Clear();

            foreach (var line in csvLines)
            {
                var p = line.Split(',');
                if (p.Length < 6) continue;

                // datetime 파싱
                var dt = p[0].Split(' ');
                var d = dt[0].Split('-');
                var t = dt[1].Split(':');
                if (d.Length != 3 || t.Length != 4) continue;
                var ts = new DateTime(
                    int.Parse(d[0]), int.Parse(d[1]), int.Parse(d[2]),
                    int.Parse(t[0]), int.Parse(t[1]), int.Parse(t[2]), int.Parse(t[3])
                );

                // RF
                if (double.TryParse(p[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var fwd))
                    _rawRfFwd.Add(new DateTimePoint(ts, fwd));
                if (double.TryParse(p[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var refl))
                    _rawRfRef.Add(new DateTimePoint(ts, refl));

                // GAS1~4
                for (int i = 0; i < 4 && p.Length > 3 + i; i++)
                {
                    if (!((new[] { Gas1Use, Gas2Use, Gas3Use, Gas4Use })[i])) continue;
                    if (double.TryParse(p[3 + i], NumberStyles.Float, CultureInfo.InvariantCulture, out var gv))
                        _rawGas[i].Add(new DateTimePoint(ts, gv));
                }

                // VACUUM
                if (double.TryParse(p[^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var vv))
                    _rawVac.Add(new DateTimePoint(ts, vv));
            }

            // LoadCsv 끝에 추가
            Debug.WriteLine($"[Raw Counts] RFfwd={_rawRfFwd.Count}, RFref={_rawRfRef.Count}, "
                + $"G1={_rawGas[0].Count}, G2={_rawGas[1].Count}, G3={_rawGas[2].Count}, G4={_rawGas[3].Count}, Vac={_rawVac.Count}");

        }

        /// <summary>
        /// 현재 XAxisMin/XAxisMax 범위에 들어오는 원본 데이터만 Series.Values로 채웁니다.
        /// </summary>
        private void DrawRangeData()
        {
            // 1) 현재 X축 범위 (double → long 캐스트)
            long minTicks = (long)XAxisMin.GetValueOrDefault();
            long maxTicks = (long)XAxisMax.GetValueOrDefault();
            Debug.WriteLine($"[DrawRangeData] XAxisMin={new DateTime(minTicks):yyyy-MM-dd HH:mm:ss.fff}, " +
                            $"XAxisMax={new DateTime(maxTicks):yyyy-MM-dd HH:mm:ss.fff}");

            // 2) 원본 리스트 카운트 
            Debug.WriteLine($"[Raw Counts] RFfwd={_rawRfFwd.Count}, RFref={_rawRfRef.Count}, " +
                            $"G1={_rawGas[0].Count}, G2={_rawGas[1].Count}, " +
                            $"G3={_rawGas[2].Count}, G4={_rawGas[3].Count}, Vac={_rawVac.Count}");

            // 3) RF forward
            var fwdPts = _rawRfFwd
                .Where(p => p.DateTime.Ticks >= minTicks && p.DateTime.Ticks <= maxTicks)
                .ToList();
            Debug.WriteLine($"[Filtered] RFfwd count = {fwdPts.Count}");
            Series[0].Values = new ChartValues<DateTimePoint>(fwdPts);

            // 4) RF reflect
            var refPts = _rawRfRef
                .Where(p => p.DateTime.Ticks >= minTicks && p.DateTime.Ticks <= maxTicks)
                .ToList();
            Debug.WriteLine($"[Filtered] RFref count = {refPts.Count}");
            Series[1].Values = new ChartValues<DateTimePoint>(refPts);

            // 5) GAS1~4
            int si = 2;
            var gasUseFlags = new[] { Gas1Use, Gas2Use, Gas3Use, Gas4Use };
            for (int i = 0; i < 4; i++)
            {
                if (!gasUseFlags[i])
                {
                    Debug.WriteLine($"[Skipped] GAS{i + 1} (disabled)");
                    continue;
                }

                var gasPts = _rawGas[i]
                    .Where(p => p.DateTime.Ticks >= minTicks && p.DateTime.Ticks <= maxTicks)
                    .ToList();
                Debug.WriteLine($"[Filtered] GAS{i + 1} count = {gasPts.Count}");
                Series[si++].Values = new ChartValues<DateTimePoint>(gasPts);
            }

            // 6) VACUUM (마지막 시리즈)
            var vacPts = _rawVac
                .Where(p => p.DateTime.Ticks >= minTicks && p.DateTime.Ticks <= maxTicks)
                .ToList();
            Debug.WriteLine($"[Filtered] VAC count = {vacPts.Count}");
            Series[^1].Values = new ChartValues<DateTimePoint>(vacPts);
        }
        #endregion

        #region 차트 refresh 관련
        // 클래스 멤버
        private bool _isPanning = false;
        private long _prevMinTicks, _prevMaxTicks;

        // 전체 데이터 로드 후, 최초 그릴 때
        private void InitializeRange()
        {
            _prevMinTicks = (long)XAxisMin.GetValueOrDefault();
            _prevMaxTicks = (long)XAxisMax.GetValueOrDefault();
            DrawRangeData();  // 한 번만 풀 리필
        }

        // 증분 업데이트
        private void UpdateRangeIncrementally()
        {
            long newMin = (long)XAxisMin.GetValueOrDefault();
            long newMax = (long)XAxisMax.GetValueOrDefault();

            void DoSeriesIncremental(IList<DateTimePoint> raw, ChartValues<DateTimePoint> vis)
            {
                if (newMin > _prevMinTicks)
                {
                    while (vis.Count > 0 && vis[0].DateTime.Ticks < newMin)
                        vis.RemoveAt(0);
                    var toAdd = raw.Where(p => p.DateTime.Ticks > _prevMaxTicks && p.DateTime.Ticks <= newMax);
                    foreach (var p in toAdd) vis.Add(p);
                }
                else if (newMin < _prevMinTicks)
                {
                    while (vis.Count > 0 && vis[^1].DateTime.Ticks > newMax)
                        vis.RemoveAt(vis.Count - 1);
                    var toAddLeft = raw.Where(p => p.DateTime.Ticks >= newMin && p.DateTime.Ticks < _prevMinTicks)
                                       .OrderBy(p => p.DateTime.Ticks);
                    foreach (var p in toAddLeft) vis.Insert(0, p);
                }
            }
            try
            {
                DoSeriesIncremental(_rawRfFwd, (ChartValues<DateTimePoint>)Series[0].Values);
                DoSeriesIncremental(_rawRfRef, (ChartValues<DateTimePoint>)Series[1].Values);
                for (int i = 0; i < 4; i++)
                    DoSeriesIncremental(_rawGas[i], (ChartValues<DateTimePoint>)Series[2 + i].Values);
                DoSeriesIncremental(_rawVac, (ChartValues<DateTimePoint>)Series[^1].Values);
            }
            catch
            {
                Debug.WriteLine("Exception Call");
            }

            _prevMinTicks = newMin;
            _prevMaxTicks = newMax;
        }
        #endregion

        #region External API

        public void MoveXAxisLeft()
        {
            var oneStep = TimeSpan.FromSeconds(XTermSeconds).Ticks;
            if (XAxisMin.HasValue)
            {
                var newMin = XAxisMin.Value - oneStep;
                XAxisMin = newMin;
                XAxisMax = newMin + oneStep * 12;
                UpdateAxisSteps();
                //ApplyAxisRangeToChart();

                if (!_drawFull)
                    UpdateRangeIncrementally();
                    //DrawRangeData();
            }
        }

        public void MoveXAxisRight()
        {
            var oneStep = TimeSpan.FromSeconds(XTermSeconds).Ticks;
            if (XAxisMin.HasValue)
            {
                var newMin = XAxisMin.Value + oneStep;
                XAxisMin = newMin;
                XAxisMax = newMin + oneStep * 12;
                UpdateAxisSteps();
                //ApplyAxisRangeToChart();

                if (!_drawFull)
                    UpdateRangeIncrementally();
                    //DrawRangeData();
            }
        }

        public void Clear()
        {
            if (Series == null) return;
            foreach (var s in Series)
            {
                if (s?.Values == null) continue;
                s.Values.Clear();
            }
        }

        public void ResetChart()
        {
            Clear();
            UpdateXAxisRangeFromNow();
            UpdateAxisSteps();
            //ApplyAxisRangeToChart();

            if (_drawFull)
                _drawFull = false;
        }

        public void PlotCsvRange(string[] csvLines, DateTime selectedDate, TimeSpan selectedTime)
        {
            if (csvLines == null || csvLines.Length == 0) return;
            Clear();

            // private LoadCsv 내부에서 _raw… 리스트에 채우고
            LoadCsv(csvLines);

            // 초기 X축 범위를 selectedDate + selectedTime 기준으로 설정
            long stepTicks = TimeSpan.FromSeconds(XTermSeconds).Ticks;
            var origin = selectedDate.Date + selectedTime;
            XAxisMin = origin.Ticks;
            XAxisMax = origin.Ticks + stepTicks * 12;
            UpdateAxisSteps();
            //ApplyAxisRangeToChart();

            if (_drawFull)
                _drawFull = false;

            // 현재 XAxisMin/Max 구간만 그리기
            InitializeRange();
            //DrawRangeData();
        }

        public void PlotCsv(string[] csvLines, DateTime selectedDate, TimeSpan selectedTime)
        {
            if (csvLines == null || csvLines.Length == 0) return;
            Clear();

            long stepTicks = TimeSpan.FromSeconds(XTermSeconds).Ticks;
            DateTime? firstTs = null;

            foreach (var line in csvLines)
            {
                var parts = line.Split(',');
                // 최소 6개: [0]=datetime, [1]=RFfwd, [2]=RFref, [3+] = gas, 마지막 = vacuum
                if (parts.Length < 6) continue;

                // 1) datetime 파싱 ("yyyy-M-d H:m:s:fff")
                var dtTimeParts = parts[0].Split(' ');
                if (dtTimeParts.Length != 2) continue;
                var dParts = dtTimeParts[0].Split('-');
                var tParts = dtTimeParts[1].Split(':');
                if (dParts.Length != 3 || tParts.Length != 4) continue;
                var ts = new DateTime(
                    int.Parse(dParts[0]), int.Parse(dParts[1]), int.Parse(dParts[2]),
                    int.Parse(tParts[0]), int.Parse(tParts[1]), int.Parse(tParts[2]), int.Parse(tParts[3])
                );
                if (!firstTs.HasValue || ts < firstTs) firstTs = ts;

                // 2) RF forward / reflect
                if (double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var vFwd))
                    ((ChartValues<DateTimePoint>)Series[0].Values)
                        .Add(new DateTimePoint(ts, vFwd));
                if (double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var vRef))
                    ((ChartValues<DateTimePoint>)Series[1].Values)
                        .Add(new DateTimePoint(ts, vRef));

                // 3) 활성화된 gas 채널 인덱스 수집
                var gasIndices = new List<int>();
                if (Gas1Use && parts.Length > 3 + 0) gasIndices.Add(0);
                if (Gas2Use && parts.Length > 3 + 1) gasIndices.Add(1);
                if (Gas3Use && parts.Length > 3 + 2) gasIndices.Add(2);
                if (Gas4Use && parts.Length > 3 + 3) gasIndices.Add(3);

                // 4) GAS1~4 추가
                foreach (var i in gasIndices)
                {
                    if (double.TryParse(parts[3 + i], NumberStyles.Float, CultureInfo.InvariantCulture, out var gv))
                        ((ChartValues<DateTimePoint>)Series[2 + i].Values)
                            .Add(new DateTimePoint(ts, gv));
                }

                // 5) VACUUM (마지막 컬럼) 
                int vacuumSeriesIndex = 2 + gasIndices.Count;
                if (vacuumSeriesIndex < Series.Count
                 && double.TryParse(parts[parts.Length - 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var vVac))
                {
                    ((ChartValues<DateTimePoint>)Series[Series.Count - 1].Values)
                        .Add(new DateTimePoint(ts, vVac));
                }
            }

            // 6) X축 범위 설정
            //var origin = firstTs ?? (selectedDate.Date + selectedTime);
            var origin = (selectedDate.Date + selectedTime);
            XAxisMin = origin.Ticks;
            XAxisMax = origin.Ticks + stepTicks * 12;
            UpdateAxisSteps();
            //ApplyAxisRangeToChart();

            if (!_drawFull)
                _drawFull = true;
        }
        #endregion

        static VsChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VsChart),
                new FrameworkPropertyMetadata(typeof(VsChart)));

            var dict = new ResourceDictionary
            {
                Source = new Uri("/VSLibrary;component/UIComponent/Styles/VsChartStyle.xaml", UriKind.RelativeOrAbsolute)
            };
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public VsChart()
        {
            Series = new SeriesCollection
            {
                new LineSeries { Title = "RF FWD", PointGeometry =  DefaultGeometries.Diamond, Fill = Brushes.Transparent, Stroke = Brushes.Transparent, PointForeground = Brushes.Blue,      ScalesYAt = 0 },
                new LineSeries { Title = "RF REF", PointGeometry =  DefaultGeometries.Diamond, Fill = Brushes.Transparent, Stroke = Brushes.Transparent, PointForeground = Brushes.Red,       ScalesYAt = 0 },

                new LineSeries { Title = "GAS1",   PointGeometry = DefaultGeometries.Diamond, Fill = Brushes.Transparent, Stroke = Brushes.Transparent, PointForeground = Brushes.Pink,        ScalesYAt = 0 },
                new LineSeries { Title = "GAS2",   PointGeometry = DefaultGeometries.Diamond, Fill = Brushes.Transparent, Stroke = Brushes.Transparent, PointForeground = Brushes.Brown,        ScalesYAt = 0 },
                new LineSeries { Title = "GAS3",   PointGeometry = DefaultGeometries.Diamond, Fill = Brushes.Transparent, Stroke = Brushes.Transparent, PointForeground = Brushes.Silver,        ScalesYAt = 0 },
                new LineSeries { Title = "GAS4",   PointGeometry =  DefaultGeometries.Diamond, Fill = Brushes.Transparent, Stroke = Brushes.Transparent, PointForeground = Brushes.Green,       ScalesYAt = 0 },

                new LineSeries { Title = "VACUUM", PointGeometry =  Geometry.Parse("M 0 6 L 6 -6 L -6 -6 Z"), Fill = Brushes.Transparent, Stroke = Brushes.Transparent, PointForeground = Brushes.Orange, ScalesYAt = 1 },
            };

            foreach (var ls in Series.OfType<LineSeries>())
            {
                ls.Values = new ChartValues<DateTimePoint>();
            }

            this.Loaded += (sender, e) =>
            {
                //ResetChart();
                //ApplyGasUsage();
            };
        }



        #region Dependency Properties

        // Left axis title
        public static readonly DependencyProperty LeftAxisTitleProperty =
            DependencyProperty.Register(nameof(LeftAxisTitle), typeof(string), typeof(VsChart),
                new PropertyMetadata("RF Power(Watt), GAS Flow(SCCM)"));
        public string LeftAxisTitle
        {
            get => (string)GetValue(LeftAxisTitleProperty);
            set => SetValue(LeftAxisTitleProperty, value);
        }

        // Right axis title
        public static readonly DependencyProperty RightAxisTitleProperty =
            DependencyProperty.Register(nameof(RightAxisTitle), typeof(string), typeof(VsChart),
                new PropertyMetadata("Vacuum(Torr)"));
        public string RightAxisTitle
        {
            get => (string)GetValue(RightAxisTitleProperty);
            set => SetValue(RightAxisTitleProperty, value);
        }

        // Series as DP
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(nameof(Series), typeof(SeriesCollection), typeof(VsChart),
                new PropertyMetadata(null));
        public SeriesCollection Series
        {
            get => (SeriesCollection)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        // Legend location
        public static readonly DependencyProperty LegendLocationProperty =
            DependencyProperty.Register(nameof(LegendLocation), typeof(LegendLocation), typeof(VsChart),
                new PropertyMetadata(LegendLocation.Bottom));
        public LegendLocation LegendLocation
        {
            get => (LegendLocation)GetValue(LegendLocationProperty);
            set => SetValue(LegendLocationProperty, value);
        }

        // Left/Right axis max
        public static readonly DependencyProperty LeftAxisMaxProperty =
            DependencyProperty.Register(nameof(LeftAxisMax), typeof(double?), typeof(VsChart),
                new PropertyMetadata(600.0, OnAxisRangeChanged));
        public double? LeftAxisMax
        {
            get => (double?)GetValue(LeftAxisMaxProperty);
            set => SetValue(LeftAxisMaxProperty, value);
        }

        public static readonly DependencyProperty RightAxisMaxProperty =
            DependencyProperty.Register(nameof(RightAxisMax), typeof(double?), typeof(VsChart),
                new PropertyMetadata(10.0, OnAxisRangeChanged));
        public double? RightAxisMax
        {
            get => (double?)GetValue(RightAxisMaxProperty);
            set => SetValue(RightAxisMaxProperty, value);
        }

        // X axis range & step (hidden)
        public static readonly DependencyProperty XAxisMinProperty =
            DependencyProperty.Register(nameof(XAxisMin), typeof(double?), typeof(VsChart),
                new PropertyMetadata(null));
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double? XAxisMin
        {
            get => (double?)GetValue(XAxisMinProperty);
            set => SetValue(XAxisMinProperty, value);
        }

        public static readonly DependencyProperty XAxisMaxProperty =
            DependencyProperty.Register(nameof(XAxisMax), typeof(double?), typeof(VsChart),
                new PropertyMetadata(null));
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double? XAxisMax
        {
            get => (double?)GetValue(XAxisMaxProperty);
            set => SetValue(XAxisMaxProperty, value);
        }

        public static readonly DependencyProperty XAxisStepProperty =
            DependencyProperty.Register(nameof(XAxisStep), typeof(double?), typeof(VsChart),
                new PropertyMetadata(null));
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double? XAxisStep
        {
            get => (double?)GetValue(XAxisStepProperty);
            set => SetValue(XAxisStepProperty, value);
        }

        // Y axis steps
        public static readonly DependencyProperty LeftAxisStepProperty =
            DependencyProperty.Register(nameof(LeftAxisStep), typeof(double?), typeof(VsChart),
                new PropertyMetadata(null));
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double? LeftAxisStep
        {
            get => (double?)GetValue(LeftAxisStepProperty);
            set => SetValue(LeftAxisStepProperty, value);
        }

        public static readonly DependencyProperty RightAxisStepProperty =
            DependencyProperty.Register(nameof(RightAxisStep), typeof(double?), typeof(VsChart),
                new PropertyMetadata(null));
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double? RightAxisStep
        {
            get => (double?)GetValue(RightAxisStepProperty);
            set => SetValue(RightAxisStepProperty, value);
        }

        // XTermSeconds
        public static readonly DependencyProperty XTermSecondsProperty =
            DependencyProperty.Register(nameof(XTermSeconds), typeof(int), typeof(VsChart),
                new PropertyMetadata(1, OnXTermSecondsChanged));
        public int XTermSeconds
        {
            get => (int)GetValue(XTermSecondsProperty);
            set => SetValue(XTermSecondsProperty, value);
        }

        private static void OnXTermSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VsChart chart)
                chart.ResetChart();
        }

        private static void OnAxisRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is VsChart chart)
                chart.ResetChart();
        }

        private void UpdateAxisSteps()
        {
            if (LeftAxisMax.HasValue)
                LeftAxisStep = (LeftAxisMax.Value) / 10.0;
            if (RightAxisMax.HasValue)
                RightAxisStep = (RightAxisMax.Value) / 10.0;
            XAxisStep = TimeSpan.FromSeconds(XTermSeconds).Ticks;
        }

        #region Gas Usage Properties

        public static readonly DependencyProperty Gas1UseProperty =
     DependencyProperty.Register(nameof(Gas1Use), typeof(bool), typeof(VsChart),
         new PropertyMetadata(true));

        public bool Gas1Use
        {
            get => (bool)GetValue(Gas1UseProperty);
            set => SetValue(Gas1UseProperty, value);
        }

        public static readonly DependencyProperty Gas2UseProperty =
            DependencyProperty.Register(nameof(Gas2Use), typeof(bool), typeof(VsChart),
                new PropertyMetadata(true));
        public bool Gas2Use
        {
            get => (bool)GetValue(Gas2UseProperty);
            set => SetValue(Gas2UseProperty, value);
        }

        public static readonly DependencyProperty Gas3UseProperty =
            DependencyProperty.Register(nameof(Gas3Use), typeof(bool), typeof(VsChart),
                new PropertyMetadata(true));
        public bool Gas3Use
        {
            get => (bool)GetValue(Gas3UseProperty);
            set => SetValue(Gas3UseProperty, value);
        }

        public static readonly DependencyProperty Gas4UseProperty =
            DependencyProperty.Register(nameof(Gas4Use), typeof(bool), typeof(VsChart),
                new PropertyMetadata(true));
        public bool Gas4Use
        {
            get => (bool)GetValue(Gas4UseProperty);
            set => SetValue(Gas4UseProperty, value);
        }

        private void ApplyGasUsage()
        {
            // Series 자체가 없으면 아무 것도 안 함
            if (Series == null) return;

            // 각 인덱스가 유효하고 LineSeries일 때만 처리
            SetSeriesVisibility(2, Gas1Use);
            SetSeriesVisibility(3, Gas2Use);
            SetSeriesVisibility(4, Gas3Use);
            SetSeriesVisibility(5, Gas4Use);
        }

        private void SetSeriesVisibility(int index, bool isVisible)
        {
            // 컬렉션 크기 체크
            if (Series.Count <= index) return;
            // 해당 항목이 LineSeries인지 검사
            if (!(Series[index] is LineSeries ls)) return;
            // Visibility 토글
            ls.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion


        #endregion

        #region Apply Template & Axis Range

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _chart = GetTemplateChild("PART_Chart") as CartesianChart;
            if (_chart == null) return;

            _chart.DisableAnimations = true;
            _chart.AnimationsSpeed = TimeSpan.Zero;


            ApplyGasUsage();
            ResetChart();
        }

        private void ApplyAxisRangeToChart()
        {
            if (_chart == null || _chart.AxisX.Count == 0) return;

            var xAxis = (Axis)_chart.AxisX[0];
            xAxis.MinValue = XAxisMin.GetValueOrDefault();
            xAxis.MaxValue = XAxisMax.GetValueOrDefault();
            xAxis.Separator = new LiveCharts.Wpf.Separator
            {
                Step = XAxisStep.GetValueOrDefault(),
                Stroke = xAxis.Separator.Stroke,
                StrokeThickness = xAxis.Separator.StrokeThickness
            };

            _chart.Update(true, true);
        }

        public void UpdateXAxisRangeFromNow()
        {
            var totalRange = TimeSpan.FromSeconds(XTermSeconds * 12).Ticks;
            var now = DateTime.Now.Ticks;
            XAxisMin = now;
            XAxisMax = now + totalRange;
        }

        #endregion

        #region Mouse Drag Pan

        private Point? _dragStart;
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            // 이미 패닝 중이면 다시 구독하지 않음
            if (_isPanning) 
                return;
            else 
                _isPanning = true;
            _dragStart = e.GetPosition(this);

            // 현재 축 범위 저장
            _prevMinTicks = (long)XAxisMin.GetValueOrDefault();
            _prevMaxTicks = (long)XAxisMax.GetValueOrDefault();

            CompositionTarget.Rendering += OnRendering;
            CaptureMouse();
        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            //if (_dragStart.HasValue && e.LeftButton == MouseButtonState.Pressed)
            if (_dragStart.HasValue && _isPanning && e.LeftButton == MouseButtonState.Pressed)
            {
                var current = e.GetPosition(this);
                var deltaX = current.X - _dragStart.Value.X;
                if (XAxisMin.HasValue && XAxisMax.HasValue && ActualWidth > 0)
                {
                    var range = XAxisMax.Value - XAxisMin.Value;
                    double tickPerPixel = range / ActualWidth;
                    long offset = (long)(-deltaX * tickPerPixel);

                    XAxisMin += offset;
                    XAxisMax += offset;

                    //ApplyAxisRangeToChart();
                    _dragStart = current;
                }
            }
        }
        private void OnRendering(object sender, EventArgs e)
        {
            if (_isPanning && !_drawFull)
            {
                try
                {
                    UpdateRangeIncrementally();
                }
                catch
                {
                    DrawRangeData();
                }
            }
        }
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            ReleaseMouseCapture();
            _dragStart = null;

            if (_isPanning)
            {
                CompositionTarget.Rendering -= OnRendering;
                _isPanning = false;

                _prevMinTicks = (long)XAxisMin.GetValueOrDefault();
                _prevMaxTicks = (long)XAxisMax.GetValueOrDefault();
                UpdateRangeIncrementally();

            }
        }
        #endregion
    }
}