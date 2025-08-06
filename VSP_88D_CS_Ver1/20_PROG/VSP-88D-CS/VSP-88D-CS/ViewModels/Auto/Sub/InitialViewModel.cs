using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Device;
using VSP_88D_CS.Models.Sub;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common.Helpers;
namespace VSP_88D_CS.ViewModels.Auto.Sub
{
    public partial class InitialViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }
        #region PROPERTY
        [ObservableProperty]
        private ObservableCollection<InitializationModel> items;

        private bool _isAllChecked;
        public bool IsAllChecked
        {
            get => _isAllChecked;
            set
            {
                _isAllChecked = value;
                OnPropertyChanged(nameof(IsAllChecked));
                if (!Items.IsNullOrEmpty())
                    OnIsAllCheckedChanged(value);
            }
        }
        void OnIsAllCheckedChanged(bool value)
        {
            foreach (var item in Items)
            {
                item.IsChecked = value;
            }
        }
        #endregion PROPERTY
        
        #region COMMAND
        public ICommand InitialCommand { get; set; }
        public ICommand StopCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        private void Initial()
        {
            foreach (var item in Items.Where(i => i.IsChecked))
            {
                //_seqMain.InitializeAxis(item.EquipmentIdx);
            }
        }
        private void Stop()
        {
            foreach (var item in Items.Where(i => i.IsChecked))
            {
                //_seqMain.StopAxis(item.EquipmentIdx);
            }
        }
        private void Close()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }
        #endregion EXECUTE COMMAND

        public InitialViewModel(VSMotionListRepository repo)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            InitialCommand = new RelayCommand(Initial);
            StopCommand = new RelayCommand(Stop);
            CloseCommand = new RelayCommand(Close);

            //Items = new ObservableCollection<InitializationModel>(
            //            repo.Data.Select(x => new InitializationModel
            //            {
            //                EquipmentIdx = x.AxisNo,
            //                EquipmentName = x.StrAxisData,
            //                IsChecked = false,
            //                ProgressValue = 0
            //            }));
        }
        private void OnAxisStepChanged(int axisNo)//, SequenceStep step)
        {
            var model = Items.FirstOrDefault(x => x.EquipmentIdx == axisNo);
            if (model == null)
                return;

            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    model.StepDisplay = step.ToString();

            //    model.ProgressValue = step switch
            //    {
            //        SequenceStep.ServoPowerOn => 10,
            //        SequenceStep.InterlockCheck => 30,
            //        SequenceStep.InitializStart => 50,
            //        SequenceStep.InitializingDone => 70,
            //        SequenceStep.MoveAction => 90,
            //        SequenceStep.MoveToWorkActionDone or SequenceStep.Idle => 100,
            //        _ => 0
            //    };

            //    model.ProgressPer = $"{model.ProgressValue}%";
            //});
        }
    }
}
