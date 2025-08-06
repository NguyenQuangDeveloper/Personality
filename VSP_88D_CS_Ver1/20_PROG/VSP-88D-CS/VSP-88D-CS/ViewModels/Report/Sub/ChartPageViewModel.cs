using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Report;

namespace VSP_88D_CS.ViewModels.Report.Sub
{

    public class ChartPageViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }
        #region COMMAND
        public ICommand OpenBtnCommand { get; }
        public ICommand ResetBtnCommand { get; }
        public ICommand SetAxisCommand { get; }
        #endregion COMMAND

        #region PROPERTY
        private ISeries[] _seriesCollection;
        public ISeries[] SeriesCollection
        {
            get => _seriesCollection;
            set
            {
                SetProperty(ref _seriesCollection, value);
            }
        }

        private Axis[] _YAxes;
        public Axis[] YAxes
        {
            get => _YAxes;
            set
            {
                SetProperty(ref _YAxes, value);
            }
        }
        private Axis[] _XAxes;
        public Axis[] XAxes
        {
            get => _XAxes;
            set
            {
                SetProperty(ref _XAxes, value);
            }
        }

        private int _limitTime;
        public int LimitTime
        {
            get => _limitTime;
            set
            {
                SetProperty(ref _limitTime, value);
            }
        }
        private int _limitRF;
        public int LimitRF
        {
            get => _limitRF;
            set
            {
                SetProperty(ref _limitRF, value);
            }
        }
        private int _limitGas;
        public int LimitGas
        {
            get => _limitGas;
            set
            {
                SetProperty(ref _limitGas, value);
            }
        }
        private int _limitVacuum;
        public int LimitVacuum
        {
            get => _limitVacuum;
            set
            {
                SetProperty(ref _limitVacuum, value);
            }
        }
        #endregion PROPERTY
        public DrawMarginFrame Frame { get; set; } =
        new() { Stroke = new SolidColorPaint(new SKColor(200, 200, 200), 2) };
        public ChartPageViewModel(IRegionManager regionManager)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            OpenBtnCommand = new RelayCommand(OnOpenBtn);
            ResetBtnCommand = new RelayCommand(OnResetBtn);
            SetAxisCommand = new RelayCommand(OnSetAxis);
            SelectedDate = DateTime.Now;
            InitChart();
            LimitRF = 500;
            LimitGas = 100;
            LimitVacuum = 10;
            LimitTime = 600;

        }
        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                SetProperty(ref _selectedDate, value);
            }
        }
        private void OnSetAxis()
        {
            SetAxisChart();
        }
        private void OnResetBtn()
        {
            ClearChart();
        }
        private void OnOpenBtn()
        {
            string strPath = "", strHeader = "", strPMIdx = "";
            string strDate = _selectedDate.ToString("yyyy_MM_dd");
            string strDrawFile = string.Format("{0}{1}_PM_{2}_{3}", strPath, strHeader, strPMIdx, strDate);
            PlotChart();
        }

        #region function
        private void InitChart()
        {
            var gridPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 };
            XAxes = new Axis[]
            {
                new Axis
                {                   
                    UnitWidth = TimeSpan.FromSeconds(1).Ticks,   
                    Labeler = value =>
                    {
                        if (value >= DateTime.MinValue.Ticks && value <= DateTime.MaxValue.Ticks)
                            return new DateTime((long)value).ToString("HH:mm:ss");
                        return "";
                    },
                    MinLimit = DateTime.Now.Ticks, 
                   
                } 
            };


            YAxes = new Axis[]
            {
                new Axis  //0-1000 //RF
                {  
                   // TicksPaint = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 }, 
                    //SubticksPaint = new SolidColorPaint(SKColors.Yellow.WithAlpha(80)) { StrokeThickness = 1 }, 
                    SubseparatorsCount = 1,
                    Name = "RF Power(Watt)",
                    NameTextSize=16,
                    SeparatorsPaint = gridPaint,
                    ShowSeparatorLines = false,
                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    MinLimit=0,
                    MaxLimit=500,
                    CustomSeparators = new double[]
                    {
                       0,50, 100,150,200,250,300,350,400,450,500
                    },
                    ZeroPaint = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
                },
                new Axis //0-100
                {
                    SeparatorsPaint = gridPaint,
                    Name = "GAS Flow(SCCM)",
                    NameTextSize=16,
                    ShowSeparatorLines = false,
                    LabelsPaint = new SolidColorPaint(SKColors.Red),
                    CustomSeparators = new double[]
                    {
                       0,10,20,30,40,50,60,70,80,90,100
                    },
                    MinLimit=0,
                    MaxLimit=100
                },
                new Axis  //0-10 //Va
                {   SeparatorsPaint = gridPaint,
                    ShowSeparatorLines = false,
                    Name = "Vacuum(Torr)",
                    NameTextSize=16,
                    Position = LiveChartsCore.Measure.AxisPosition.End,
                    LabelsPaint = new SolidColorPaint(SKColors.Red),
                    CustomSeparators = new double[]
                    {
                       0,2,4,6,8,10
                    },
                     MinLimit=0,
                    MaxLimit=10,
                    ZeroPaint = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },

                }
            };
        }
        private void ClearChart()
        {
            InitChart();
            SeriesCollection = null;
        }
        private void PlotChart()
        {
            var fakeData = GenerateFakeData(DateTime.Now, 10, TimeSpan.FromMinutes(1));
            UpdateChart(fakeData);
        }
        private void UpdateChart(List<ChartModel> data)
        {

            SeriesCollection = new ISeries[]
            {
                 new LineSeries<DateTimePoint> { Name = "RfFwd",  Values = data.Select(d => new DateTimePoint(d.Time, d.RfFwd)).ToArray() ,ScalesYAt = 0 },
                 new LineSeries<DateTimePoint> { Name = "RfRef",  Values = data.Select(d => new DateTimePoint(d.Time, d.RfRef)).ToArray() ,ScalesYAt = 0},
                 new LineSeries<DateTimePoint> { Name = "Vacuum", Values = data.Select(d => new DateTimePoint(d.Time, d.Vacuum)).ToArray(),ScalesYAt = 2},
                 new LineSeries<DateTimePoint> { Name = "Gas1",   Values = data.Select(d => new DateTimePoint(d.Time, d.Gas1)).ToArray() ,ScalesYAt = 1},
                 new LineSeries<DateTimePoint> { Name = "Gas2",   Values = data.Select(d => new DateTimePoint(d.Time, d.Gas2)).ToArray() ,ScalesYAt = 1},
                 new LineSeries<DateTimePoint> { Name = "Gas3",   Values = data.Select(d => new DateTimePoint(d.Time, d.Gas3)).ToArray() ,ScalesYAt = 1},
                 new LineSeries<DateTimePoint> { Name = "Gas4",   Values = data.Select(d => new DateTimePoint(d.Time, d.Gas4)).ToArray() ,ScalesYAt = 1}
            };
            XAxes = new Axis[]
            {
                  new Axis
                  {
                       Labeler = value =>
                       {
                           if (value >= DateTime.MinValue.Ticks && value <= DateTime.MaxValue.Ticks)
                           {
                               var time = new DateTime((long)value);
                               return time.ToString("HH:mm:ss");
                           }
                           return "";
                       },

                       UnitWidth = TimeSpan.FromMinutes(1).Ticks
                      
                  }
            };



        }
        #endregion

        #region button
        private void SetAxisChart()
        {
            SetLimitAxis(YAxes[0], _limitRF, _limitRF / 10);
            SetLimitAxis(YAxes[1], _limitGas, _limitGas / 10);
            SetLimitAxis(YAxes[2], _limitVacuum, LimitVacuum / 10);

            long unitWithFollowSeconds = TimeSpan.FromSeconds(_limitTime).Ticks;
            SetMinAxis(XAxes[0], _limitTime, unitWithFollowSeconds);
            OnPropertyChanged(nameof(YAxes));
            OnPropertyChanged(nameof(XAxes));
        }
        #endregion
        private void SetLimitAxis(Axis axis, long maxLimit, long unitWidth)
        {
            axis.MaxLimit = maxLimit;
            axis.UnitWidth = unitWidth;
            axis.CustomSeparators = GenerateRange(0, maxLimit, unitWidth);

        }
        private void SetMinAxis(Axis axis, long minLimit, long unitWidth)
        {
            double min, max;
            min= DateTime.Now.Ticks;
            max= DateTime.Now.Ticks +5*unitWidth;
            axis.MinLimit = min;
            axis.UnitWidth = unitWidth;         
            axis.CustomSeparators = GenerateRange(min, max, unitWidth);

        }
        public static double[] GenerateRange(double min, double max, double delta)
        {
            if (delta <= 0) throw new ArgumentException("Delta must be > 0");
            if (min > max) throw new ArgumentException("Min must be <= Max");

            int count = (int)Math.Floor((max - min) / delta) + 1;
            double[] result = new double[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = min + i * delta;
            }

            return result;
        }
        public List<ChartModel> GenerateFakeData(DateTime startTime, int count, TimeSpan interval)
        {
            var rand = new Random();
            var dataList = new List<ChartModel>();

            for (int i = 0; i < count; i++)
            {
                var time = startTime.AddTicks(interval.Ticks * i);
                var rfFwd = 60 + rand.NextDouble() * 0.5;
                var rfRef = 85 + rand.NextDouble() * 2;
                var vacuum = 1 + 1 / 1f;
                var gas1 = 1 + i * 0.5;
                //var gas2 = 1 + rand.NextDouble();
                //var gas3 = 1500 + rand.Next(0, 100); ;
                //var gas4 = 0.045 + rand.NextDouble() * 0.01; ;

                dataList.Add(new ChartModel
                {
                    Time = time,
                    RfFwd = Math.Round(rfFwd, 3),
                    RfRef = Math.Round(rfRef, 2),
                    Vacuum = Math.Round(vacuum, 2),
                    Gas1 = Math.Round(gas1, 3),
                    //Gas2 = Math.Round(gas2, 3),
                    //Gas3 = gas3,
                    //Gas4 = Math.Round(gas4, 4)
                });
            }

            return dataList;
        }
    }


}
