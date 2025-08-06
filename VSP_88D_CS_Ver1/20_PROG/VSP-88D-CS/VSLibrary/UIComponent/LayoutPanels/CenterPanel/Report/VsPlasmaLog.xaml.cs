using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Win32;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using VSLibrary.UIComponent.LayoutPanels.CenterPanel.Parameter;
using VSLibrary.UIComponent.LayoutPanels.TopPanel;
using VSLibrary.UIComponent.Localization;
using VSLibrary.UIComponent.MessageBox;
using VSLibrary.UIComponent.VSCustomControls;
using static VSLibrary.UIComponent.MessageBox.VsMessageBoxWindow;

namespace VSLibrary.UIComponent.LayoutPanels.CenterPanel.Report;

/// <summary>
/// 플라즈마 로그 Chart UI + CSV 로딩/Zoom/Pan을 담당하는 컨트롤입니다.
/// 각 시리즈(라인)의 축 최대값은 VsCheckSelector AxisMax로 관리합니다.
/// </summary>
/// <remarks>
/// [주의]
/// GroupBox를 동적으로(템플릿, 반복, 여러 팝업, 동적 화면) 사용할 경우,
/// 반드시 Name, Key, 또는 ViewModel의 고유값을 XAML에서 바인딩 해주세요!
/// (Header만으로 라디오 그룹 충돌이 발생할 수 있습니다.)
/// 
/// 예시:
///   <GroupBox Header="Chart Set" x:Name="ChartSetGroupBox">
///   <GroupBox Header="Chamber Selection" x:Name="{Binding ChamberKey}">
/// </remarks>
[INotifyPropertyChanged]
public partial class VsPlasmaLog : UserControl
{

    /// <summary>
    /// Open 버튼 활성화 여부
    /// </summary>
    [ObservableProperty]
    private bool _isOpenEnabled = true;

    // === 전체 데이터 원본 ===
    private readonly List<string> _allTimeLabels = new();
    private readonly List<double> _allRfFwd = new();
    private readonly List<double> _allRfRef = new();
    private readonly List<double> _allVacuum = new();
    private readonly List<double> _allGas1 = new();
    private readonly List<double> _allGas2 = new();

    private int _viewportStart = 0;      ///< 현재 뷰포트 시작 인덱스
    private int _viewportSize = 10000;   ///< 한 번에 보이는 데이터 개수 (화면 폭 기준)
    private int _zoomInCount = 10;       ///< 현재까지 ZoomIn 횟수
    private const int MaxZoomInCount = 10;

    private string _axisXName = "Time";

    /// <summary>
    /// VsPlasmaLog 뷰의 생성자입니다.  
    /// 언어 리소스 바인딩 및 초기 상태를 설정합니다.
    /// </summary>
    public VsPlasmaLog()
    {
        InitializeComponent();
        InitSample();

        // 달력 다국어 반영
        ApplyCultureToCalendar(calStartDay, VsLocalizationManager.CurrentLanguage);

        // 체크박스 등 UI 다국어 적용
        ApplyLocalizedText(VsLocalizationManager.CurrentLanguage);

        // Viewport 갱신 (Dispatcher로 지연 호출)
        Application.Current.Dispatcher.Invoke(UpdateChartViewport);

        // 언어 변경 시 다국어 UI 재반영
        VsParameterData.LanguageChanged += (_, args) =>
        {
            if (Enum.TryParse(args.ToString(), out LanguageType newLanguage))
            {
                ApplyCultureToCalendar(calStartDay, newLanguage);
                ApplyLocalizedText(newLanguage);

                Application.Current.Dispatcher.Invoke(UpdateChartViewport);
            }
        };
    }


    /// <summary>
    /// 현재 언어에 따라 컨트롤들의 다국어 텍스트를 설정합니다.
    /// </summary>
    /// <param name="lang">적용할 언어 타입</param>
    private void ApplyLocalizedText(LanguageType lang)
    {
        string radioLeftText = VsLocalizationManager.Get(lang, "GUI_Parameter_SubView", "VsCheckSelector_RadioLeft");
        string radioRightText = VsLocalizationManager.Get(lang, "GUI_Parameter_SubView", "VsCheckSelector_RadioRight");
        _axisXName = VsLocalizationManager.Get(lang, "GUI_Parameter_SubView", "VsCheckSelector_AxisXName");

        chkRfFwd.CheckText = VsLocalizationManager.Get(lang, "GUI_Parameter_SubView", "VsCheckSelector_CheckBoxText_RfFwd");
        chkRfFwd.LeftRadioText = radioLeftText;
        chkRfFwd.RightRadioText = radioRightText;

        chkRfRef.CheckText = VsLocalizationManager.Get(lang, "GUI_Parameter_SubView", "VsCheckSelector_CheckBoxText_RfRef");
        chkRfRef.LeftRadioText = radioLeftText;
        chkRfRef.RightRadioText = radioRightText;

        chkVacuum.CheckText = VsLocalizationManager.Get(lang, "GUI_Parameter_SubView", "VsCheckSelector_CheckBoxText_Vacuum");
        chkVacuum.LeftRadioText = radioLeftText;
        chkVacuum.RightRadioText = radioRightText;

        chkGas1.CheckText = VsLocalizationManager.Get(lang, "GUI_Parameter_SubView", "VsCheckSelector_CheckBoxText_Gas1");
        chkGas1.LeftRadioText = radioLeftText;
        chkGas1.RightRadioText = radioRightText;

        chkGas2.CheckText = VsLocalizationManager.Get(lang, "GUI_Parameter_SubView", "VsCheckSelector_CheckBoxText_Gas2");
        chkGas2.LeftRadioText = radioLeftText;
        chkGas2.RightRadioText = radioRightText;
    }

    public void ApplyCultureToCalendar(FrameworkElement control, LanguageType langType)
    {
        var lang = langType switch
        {
            LanguageType.Korean => "ko-KR",
            LanguageType.English => "en-US",
            LanguageType.Chinese => "zh-CN",
            LanguageType.Vietnamese => "vi-VN",
            _ => "en-US"
        };

        var culture = new CultureInfo(lang);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        if (control != null)
            control.Language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
    }
    /// <summary>
    /// (샘플) 더미 데이터로 초기화
    /// </summary>
    private void InitSample()
    {
        calStartDay.SelectedDate = DateTime.Today;

        // 샘플값 (AxisMax에 직접 바인딩하면 tbLeftAxis, tbRightAxis 필요 없음)
        chkRfFwd.AxisMax = "600";
        chkRfRef.AxisMax = "600";
        chkVacuum.AxisMax = "45";
        chkGas1.AxisMax = "45";
        chkGas2.AxisMax = "45";

        var start = DateTime.Now.Date.AddHours(9);
        _allTimeLabels.Clear();
        _allRfFwd.Clear();
        _allRfRef.Clear();
        _allVacuum.Clear();
        _allGas1.Clear();
        _allGas2.Clear();

        //for (int i = 0; i < 120; i++)
        //{
        //    _allTimeLabels.Add(start.AddMinutes(i).ToString("HH:mm"));
        //    _allRfFwd.Add(100 + 50 * Math.Sin(i / 8.0));
        //    _allRfRef.Add(30 + 12 * Math.Sin(i / 6.0));
        //    _allVacuum.Add(1.2 + 0.4 * Math.Sin(i / 20.0));
        //    _allGas1.Add(10 + 2 * Math.Sin(i / 9.0));
        //    _allGas2.Add(2 + 1.5 * Math.Cos(i / 13.0));
        //}
        _viewportStart = 0;
        _viewportSize = Math.Min(60, _allTimeLabels.Count);
        UpdateChartViewport();
    }

    /// <summary>
    /// 차트에 표시될 시리즈 목록
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ISeries> _chartSeries = null!;

    /// <summary>
    /// X축 정의 리스트
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Axis> _xAxes = null!;

    /// <summary>
    /// Y축 정의 리스트
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Axis> _yAxes = null!;

    public static readonly DependencyProperty TimeProperty =
     DependencyProperty.Register(
         nameof(Time), typeof(TimeSpan), typeof(VsPlasmaLog),
         new FrameworkPropertyMetadata(default(TimeSpan), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public string DefaultFolder { get; set; } = @"C:\Vision\Report\PM_DATA_PM_";

    /// <summary>
    /// Open 명령
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsOpenEnabled))]
    private void Open()
    {
        IsOpenEnabled = false; // 중복 클릭 방지
        // 날짜(yyyy_MM_dd) 추출 (달력 null 안전 처리)
        var selectedDate = calStartDay.SelectedDate ?? DateTime.Today;
        string dateStr = selectedDate.ToString("yyyy_MM_dd");

        // 기본 경로 + 챔버명(0/1) + 날짜
        string fileName = DefaultFolder + (rbTop.IsChecked == true ? "0_" : "1_") + dateStr + ".csv";

        if (File.Exists(fileName))
        {
            LoadAndShowCsv(fileName);
            IsOpenEnabled = true;
            return;
        }

        // 없으면 직접 찾기 물어보기
        if (MessageBoxResult.Yes == VsMessageBox.Show(
            "지정된 파일이 없습니다.\n직접 선택하시겠습니까?",
            "알림",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question))
        {
            var dlg = new OpenFileDialog
            {
                Title = "CSV 파일 선택",
                Filter = "CSV 파일 (*.csv)|*.csv|모든 파일 (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                LoadAndShowCsv(dlg.FileName);
            }
        }

        IsOpenEnabled = true ;
    }

    /// <summary>
    /// CSV 로딩 및 차트 갱신 일괄 처리
    /// </summary>
    private void LoadAndShowCsv(string filePath)
    {
        LoadCsvAllData(filePath);     
        UpdateChartViewport();
    }

    /// <summary>
    /// 전체 CSV 데이터 메모리로 로드 (대용량 지원)
    /// </summary>
    /// <summary>
    /// CSV 전체 데이터 로딩 (Time 값에 따라 필터링)
    /// </summary>
    /// <summary>
    /// CSV 전체 데이터 로딩 (날짜+시간 파싱, Time 필터 지원)
    /// </summary>
    private void LoadCsvAllData(string filePath)
    {
        _zoomInCount = 10;
        _allTimeLabels.Clear();
        _allRfFwd.Clear();
        _allRfRef.Clear();
        _allVacuum.Clear();
        _allGas1.Clear();
        _allGas2.Clear();

        bool filterByTime = (Time != TimeSpan.Zero);

        using (var reader = new StreamReader(filePath))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var tokens = line.Trim().Split(',');
                if (tokens.Length < 6) continue;

                string label = tokens[0].Trim();

                // "2024-8-1 0:0:19:414" → 시간만 파싱
                TimeSpan labelTime = TimeSpan.Zero;
                bool validTime = false;
                try
                {
                    // 공백 이후(시각부분)만 분리
                    string[] parts = label.Split(' ');
                    if (parts.Length > 1)
                    {
                        // "0:0:19:414" → "H:m:s:fff"
                        var tstr = parts[1];
                        var tarr = tstr.Split(':');
                        int h = 0, m = 0, s = 0, ms = 0;
                        if (tarr.Length == 4) // h:m:s:ms
                        {
                            h = int.Parse(tarr[0]);
                            m = int.Parse(tarr[1]);
                            s = int.Parse(tarr[2]);
                            ms = int.Parse(tarr[3]);
                            // 범위 벗어나면 유효하지 않은 데이터로 간주
                            if (m > 59 || s > 59 || ms > 999) throw new Exception();
                            labelTime = new TimeSpan(0, h, m, s, ms);
                        }
                        else if (tarr.Length == 3) // h:m:s
                        {
                            h = int.Parse(tarr[0]);
                            m = int.Parse(tarr[1]);
                            s = int.Parse(tarr[2]);
                            if (m > 59 || s > 59) throw new Exception();
                            labelTime = new TimeSpan(h, m, s);
                        }
                        else
                        {
                            throw new Exception();
                        }
                        validTime = true;
                    }
                }
                catch { validTime = false; }

                // Time > 0 일 때만 필터
                if (filterByTime && validTime && labelTime < Time)
                    continue;

                _allTimeLabels.Add(label);
                _allRfFwd.Add(double.TryParse(tokens[1], out var v1) ? v1 : 0);
                _allRfRef.Add(double.TryParse(tokens[2], out var v2) ? v2 : 0);
                _allVacuum.Add(double.TryParse(tokens[3], out var v3) ? v3 : 0);
                _allGas1.Add(double.TryParse(tokens[4], out var v4) ? v4 : 0);
                _allGas2.Add(double.TryParse(tokens[5], out var v5) ? v5 : 0);
            }

            int dataCount = _allTimeLabels.Count;
          
            _viewportStart = 0;
            _viewportSize = Math.Min( 10000 , dataCount );
        }
    }

    /// <summary>
    /// LanguageType enum 기반으로 SKTypeface 반환
    /// </summary>
    /// <param name="language">LanguageType enum</param>
    /// <returns>SKTypeface</returns>
    private static SKTypeface GetFontByLanguage(LanguageType language)
    {
        return language switch
        {
            LanguageType.Korean => SKTypeface.FromFamilyName("Malgun Gothic"),
            LanguageType.English => SKTypeface.FromFamilyName("Segoe UI"),
            LanguageType.Chinese => SKTypeface.FromFamilyName("Microsoft YaHei"), // 중국어
            LanguageType.Vietnamese => SKTypeface.FromFamilyName("Arial"),
            _ => SKTypeface.FromFamilyName("Segoe UI")
        };
    }

    /// <summary>
    /// 메인 차트 뷰포트 갱신 (데이터 슬라이스, 축/시리즈/이벤트 완전 동적 관리)
    /// </summary>
    private void UpdateChartViewport()
    {
        SKTypeface fontName = GetFontByLanguage(VsLocalizationManager.CurrentLanguage);
        // 1. 데이터 슬라이싱
        var (labels, rfFwd, rfRef, vacuum, gas1, gas2) = SliceData(_viewportStart, _viewportSize);

        // 2. 각 시리즈/셀렉터/색상 정의
        var dataList = new[]
        {
            (chkRfFwd, chkRfFwd.CheckText, rfFwd, SKColors.Red, SKColors.DarkRed),
            (chkRfRef, chkRfRef.CheckText, rfRef, SKColors.Orange, SKColors.DarkOrange),
            (chkVacuum, chkVacuum.CheckText, vacuum, SKColors.Green, SKColors.DarkGreen),
            (chkGas1, chkGas1.CheckText, gas1, SKColors.Blue, SKColors.DarkBlue),
            (chkGas2, chkGas2.CheckText, gas2, SKColors.Purple, SKColors.DarkViolet),
        };

        // 3. Y축/시리즈/축명 Pool 조립
        BuildChartAxesAndSeries(
            out var yAxisList,
            out var seriesList,
            out var axisSeriesNames,
            dataList,
            fontName
        );

        // 4. 축별 이름 병합 (RF FWD, RF RED, ...)
        MergeAxisNames(yAxisList, axisSeriesNames);

        // 5. X/Y/Series UI 적용
        XAxes = new ObservableCollection<Axis>
        {
            new Axis
            {
                Name = _axisXName,
                NamePaint = new SolidColorPaint(SKColors.Black, 22) { SKTypeface = fontName},
                Labels = labels,
                LabelsPaint = new SolidColorPaint(SKColors.Black, 16) { SKTypeface = fontName },
                MinLimit = 0,
                MaxLimit = labels.Length - 1,                
            }
        };
        YAxes = new ObservableCollection<Axis>(yAxisList);
        ChartSeries = new ObservableCollection<ISeries>(seriesList);

        chart.Series = ChartSeries;
        chart.XAxes = XAxes;
        chart.YAxes = YAxes;
           
        chart.LegendTextPaint = new SolidColorPaint
        {
            Color = SKColors.Black,
            SKTypeface = fontName, // 사용자가 전달한 한글 지원 
        };
        chart.TooltipTextPaint = new SolidColorPaint
        {
            Color = SKColors.Black,
            SKTypeface = fontName, // 사용자가 전달한 한글 지원 
        };
        chart.LegendTextSize = 10;
        // 6. 마우스 이벤트(최초 1회만 등록해도 됨, 중복 방지 필요시 분리)
        chart.PreviewMouseWheel -= Chart_PreviewMouseWheel;
        chart.PreviewMouseWheel += Chart_PreviewMouseWheel;
        chart.MouseDown -= Chart_MouseDown;
        chart.MouseDown += Chart_MouseDown;
        chart.MouseMove -= Chart_MouseMove;
        chart.MouseMove += Chart_MouseMove;
        chart.MouseUp -= Chart_MouseUp;
        chart.MouseUp += Chart_MouseUp;
    }

    /// <summary>
    /// 원하는 범위의 차트 데이터와 라벨을 슬라이스합니다.
    /// </summary>
    private (string[] Labels, double[] RfFwd, double[] RfRef, double[] Vacuum, double[] Gas1, double[] Gas2)
    SliceData(int start, int size)
    {
        int end = Math.Min(start + size, _allTimeLabels.Count);
        if (end <= start) end = start + 1;
        int count = end - start;

        var labels = _allTimeLabels.Skip(start).Take(count).ToArray();
        var rfFwd = _allRfFwd.Skip(start).Take(count).ToArray();
        var rfRef = _allRfRef.Skip(start).Take(count).ToArray();
        var vacuum = _allVacuum.Skip(start).Take(count).ToArray();
        var gas1 = _allGas1.Skip(start).Take(count).ToArray();
        var gas2 = _allGas2.Skip(start).Take(count).ToArray();

        // 라벨 일정 간격만 출력
        int labelStep = Math.Max(1, count / 15);
        var smartLabels = labels.Select((x, i) => (i % labelStep == 0) ? x : "").ToArray();

        return (smartLabels, rfFwd, rfRef, vacuum, gas1, gas2);
    }

    /// <summary>
    /// 시리즈와 축을 동적으로 빌드합니다. (중복 축 병합 및 Pool 관리)
    /// </summary>
    private void BuildChartAxesAndSeries(
        out List<Axis> yAxisList,
        out List<ISeries> seriesList,
        out Dictionary<int, List<string>> axisSeriesNames,
        (VsCheckSelector sel, string name, double[] values, SKColor color, SKColor stroke)[] dataList,
        SKTypeface font)
    {
        yAxisList = new List<Axis>();
        seriesList = new List<ISeries>();
        axisSeriesNames = new Dictionary<int, List<string>>();
        var axisMap = new Dictionary<string, int>();

        foreach (var (sel, name, values, color, stroke) in dataList)
        {
            if (!sel.IsChecked) continue;
            string dir = sel.IsLeftChecked ? "L" : "R";
            string axisMaxStr = sel.AxisMax ?? "100";
            double axisMax = double.TryParse(axisMaxStr, out var v) ? v : 100;
            string axisKey = $"{dir}:{axisMaxStr}";

            if (!axisMap.TryGetValue(axisKey, out int axisIdx))
            {
                axisIdx = yAxisList.Count;
                yAxisList.Add(new Axis
                {
                    Name = "", // 이름 병합용
                    NamePaint = new SolidColorPaint(SKColors.Black, 18) { SKTypeface = font },
                    MinLimit = 0,
                    MaxLimit = axisMax,
                    Position = sel.IsLeftChecked ? LiveChartsCore.Measure.AxisPosition.Start : LiveChartsCore.Measure.AxisPosition.End
                });
                axisMap[axisKey] = axisIdx;
            }

            if (!axisSeriesNames.TryGetValue(axisIdx, out var names))
                axisSeriesNames[axisIdx] = names = new List<string>();
            names.Add(name);

            seriesList.Add(new LineSeries<double, VariableSVGPathGeometry>
            {
                Name = name,
                Values = values,
                Fill = null,
                LineSmoothness = 0,
                GeometrySvg = SVGPoints.Pin,
                GeometryFill = new SolidColorPaint(color),
                GeometryStroke = new SolidColorPaint(stroke, 2),
                ScalesYAt = axisIdx,
            });
        }
    }

    /// <summary>
    /// 같은 축(왼쪽/오른쪽, 최대값 동일)인 경우 축 이름을 자동 병합합니다.
    /// </summary>
    private void MergeAxisNames(List<Axis> yAxisList, Dictionary<int, List<string>> axisSeriesNames)
    {
        for (int i = 0; i < yAxisList.Count; i++)
        {
            if (axisSeriesNames.TryGetValue(i, out var names))
                yAxisList[i].Name = string.Join(", ", names);
        }
    }


    // === 이하 Pan/Zoom/Mouse 이벤트 동일 ===

    private Point? _lastMousePos = null;

    private void Chart_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
        {
            if (e.Delta > 0)
                PanLeft();
            else
                PanRight();
        }
        else
        {
            if (e.Delta > 0)
                ZoomIn();
            else
                ZoomOut();
        }
        e.Handled = true;
    }

    private void Chart_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _lastMousePos = e.GetPosition(chart);
        chart.CaptureMouse();
    }

    private void Chart_MouseMove(object sender, MouseEventArgs e)
    {
        if (_lastMousePos == null || e.LeftButton != MouseButtonState.Pressed) return;

        var pos = e.GetPosition(chart);
        double dx = pos.X - _lastMousePos.Value.X;
        if (Math.Abs(dx) > 20)
        {
            if (dx > 0)
                PanLeft();
            else
                PanRight();
            _lastMousePos = pos;
        }
    }

    private void Chart_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _lastMousePos = null;
        chart.ReleaseMouseCapture();
    }

    /// <summary>
    /// [이벤트] 팬(왼쪽)
    /// </summary>
    public void PanLeft()
    {
        _viewportStart = Math.Max(0, _viewportStart - _viewportSize / 5);
        UpdateChartViewport();
    }

    /// <summary>
    /// [이벤트] 팬(오른쪽)
    /// </summary>
    public void PanRight()
    {
        _viewportStart = Math.Min(
            Math.Max(0, _allTimeLabels.Count - _viewportSize),
            _viewportStart + _viewportSize / 5);
        UpdateChartViewport();
    }

    /// <summary>
    /// [이벤트] 줌인
    /// </summary>
    public void ZoomIn()
    {
        if (0 >= _zoomInCount)
            return;

        _zoomInCount--;

        if (_viewportSize > 10)
            _viewportSize = Math.Max(10, _viewportSize / 2);
        UpdateChartViewport();
    }

    /// <summary>
    /// [이벤트] 줌아웃
    /// </summary>
    public void ZoomOut()
    {
        if (_zoomInCount + 1 >= MaxZoomInCount)
            return;

        _zoomInCount++;

        _viewportSize = Math.Min(_allTimeLabels.Count, _viewportSize * 2);
        if (_viewportStart + _viewportSize > _allTimeLabels.Count)
            _viewportStart = Math.Max(0, _allTimeLabels.Count - _viewportSize);
        UpdateChartViewport();
    }

    private void Left_Click(object sender, RoutedEventArgs e) => PanLeft();
    private void ZoomIn_Click(object sender, RoutedEventArgs e) => ZoomIn();
    private void ZoomOut_Click(object sender, RoutedEventArgs e) => ZoomOut();
    private void Right_Click(object sender, RoutedEventArgs e) => PanRight();

    private void CheckSelector_ValueChanged(object sender, RoutedEventArgs e)
    {
        UpdateChartViewport();
    }

    private void ResetData_Click(object sender, RoutedEventArgs e)
    {
        IsOpenEnabled = true;
        _zoomInCount = 10;
        _allTimeLabels.Clear();
        _allRfFwd.Clear();
        _allRfRef.Clear();
        _allVacuum.Clear();
        _allGas1.Clear();
        _allGas2.Clear();
        UpdateChartViewport();

        calStartDay.SelectedDate = DateTime.Today;
    }

    private void TimeClear_Click(object sender, RoutedEventArgs e)
    {
        Time = TimeSpan.Zero;
    }
}
