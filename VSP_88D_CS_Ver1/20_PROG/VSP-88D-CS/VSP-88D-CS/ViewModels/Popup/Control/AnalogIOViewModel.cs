using System.Windows.Threading;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.ViewModels.Popup.Control
{
    public class AnalogIOViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;
        //private readonly AnalogIOCtrlManager _analogIOManager;
        private readonly DispatcherTimer _timer; // 타이머 추가
        private readonly Dispatcher _dispatcher;

        //public ObservableCollection<AnalogData> DataGridValues { get; set; }

        public AnalogIOViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _dispatcher = Dispatcher.CurrentDispatcher;

            //DataGridValues = new ObservableCollection<AnalogData>();

            // 500ms마다 실행되는 타이머 설정
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300) // 300ms 간격
            };
            _timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// 500ms마다 호출되는 타이머 이벤트 핸들러
        /// </summary>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            GenerateTestData();
        }

        /// <summary>
        /// AnalogIOManager 데이터를 기반으로 DataGridValues 업데이트
        /// </summary>
        private void GenerateTestData()
        {
            //if (_analogIOManager == null) return;

            //var startTime = DateTime.Now;
            //var newValues = new ObservableCollection<AnalogData>();

            //for (int i = 0; i < 20; i++)
            //{
            //    var gas1 = _analogIOManager.DbItems[10].ContainsKey("AI000") ? _analogIOManager.DbItems[10]["AI000"].AValue : 0;
            //    var gas2 = _analogIOManager.DbItems[10].ContainsKey("AI006") ? _analogIOManager.DbItems[10]["AI006"].AValue : 0;
            //    var vacuum = _analogIOManager.DbItems[10].ContainsKey("AI002") ? _analogIOManager.DbItems[10]["AI002"].AValue : 0;

            //    newValues.Add(new AnalogData
            //    {
            //        Time = startTime.AddMilliseconds(i * 50).ToString("HH:mm:ss.fff"),
            //        Gas1 = gas1,
            //        Gas2 = gas2,
            //        Vacuum = vacuum,
            //        RFFwd = new Random().Next(0, 6),
            //        RFRe = new Random().Next(0, 6)
            //    });
            //}

            // UI 업데이트를 위해 Dispatcher.Invoke 사용
            //_dispatcher.Invoke(() =>
            //{
            //    DataGridValues.Clear();
            //    foreach (var item in newValues)
            //    {
            //        DataGridValues.Add(item);
            //    }
            //});
        }

        /// <summary>
        /// ViewModel 활성화 시 호출됩니다.
        /// </summary>
        public override void Activate()
        {
            _timer.Start();
        }

        /// <summary>
        /// ViewModel 비활성화 시 호출됩니다.
        /// </summary>
        public override void Deactivate()
        {
            _timer.Stop();
        }

    }
}
