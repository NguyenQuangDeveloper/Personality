using AlarmConfig.Models;
using AlarmConfig.ViewModels.Common;
using System.Collections.ObjectModel;

namespace AlarmConfig.ViewModels.Bottoms
{
    public class AlarmInfoViewModel : BaseViewModel
    {
        private string _solutionMessage;
        public string SolutionMessage
        {
            get => _solutionMessage;
            set => SetProperty(ref _solutionMessage, value);
        }

        private string _alarmMessage;
        public string AlarmMessage
        {
            get => _alarmMessage;
            set => SetProperty(ref _alarmMessage, value);
        }

        private ObservableCollection<string> _alarmNames = new();
        public ObservableCollection<string> AlarmNames
        {
            get => _alarmNames;
            set => SetProperty(ref _alarmNames, value);
        }

        private string _selectedAlarmName;
        public string SelectedAlarmName
        {
            get => _selectedAlarmName;
            set
            {
                if(value == SelectedAlarmName) return;

                SetProperty(ref _selectedAlarmName, value);

                if (_isOutSideSet)
                {
                    _isOutSideSet = false;
                    return;
                }

                SetAlarmExecute();
            }
        }

        private AlarmViewModel _alarmViewModel;
        private bool _isOutSideSet = false;

        public AlarmInfoViewModel(AlarmViewModel alarmViewModel)
        {
            _alarmViewModel = alarmViewModel;

            GetAlarmNames();
        }

        public void SetAlarmMessage(Button button, bool isOutSide)
        {
            AlarmMessage = button.Message;
            SolutionMessage = button.Solution;
            _isOutSideSet = isOutSide;
            SelectedAlarmName = button.Content + "-" + button.Name;
        }

        public void GetAlarmNames()
        {
            if (_alarmViewModel.AlarmCodeVM?.AlarmCodes == null) return;

            AlarmNames = new ObservableCollection<string>();

            foreach (var item in _alarmViewModel.AlarmCodeVM.AlarmCodes)
            {
                AlarmNames.Add(item.Content + "-" + item.Name);
            }    
        }

        private void SetAlarmExecute()
        {
            if (AlarmNames.Count <= 0) return;

            var alarmItem = _alarmViewModel.AlarmCodeVM.AlarmCodes.FirstOrDefault(x => x.Name == _selectedAlarmName.Split("-")[1]);

            if (alarmItem == null) return;

            _isOutSideSet = false;

            _alarmViewModel.AlarmCodeVM?.AlarmExecute(alarmItem, true);

            AlarmMessage = alarmItem.Message;
            SolutionMessage = alarmItem.Solution;
        }
    }
}
