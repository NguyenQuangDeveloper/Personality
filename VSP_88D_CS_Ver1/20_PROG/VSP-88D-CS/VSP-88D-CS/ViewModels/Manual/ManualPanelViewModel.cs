using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;

namespace VSP_88D_CS.ViewModels.Manual
{
    public class ManualPanelViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }

        #region PROPERTY
        private string _stripID1;
        public string StripID1
        {
            get => _stripID1;
            set => SetProperty(ref _stripID1, value);
        }

        private string _stripID2;
        public string StripID2
        {
            get => _stripID2;
            set => SetProperty(ref _stripID2, value);
        }

        private string _stripID3;
        public string StripID3
        {
            get => _stripID3;
            set => SetProperty(ref _stripID3, value);
        }

        private string _stripID4;
        public string StripID4
        {
            get => _stripID4;
            set => SetProperty(ref _stripID4, value);
        }

        private string _stripID5;
        public string StripID5
        {
            get => _stripID5;
            set => SetProperty(ref _stripID5, value);
        }
        #endregion PROPERTY

        #region COMMAND
        //LOADING PUSHER
        public ICommand LoadingPusherLeftCommand { get; set; }
        public ICommand LoadingPusherRightCommand { get; set; }
        public ICommand LoadingPusherUpCommand { get; set; }
        public ICommand LoadingPusherDownCommand { get; set; }
        //INDEX PUSHER
        public ICommand IndexPusherLeftCommand { get; set; }
        public ICommand IndexPusherRightCommand { get; set; }
        public ICommand IndexPusherUpCommand { get; set; }
        public ICommand IndexPusherDownCommand { get; set; }
        //CHAMBER 
        public ICommand ChamberUpCommand { get; set; }
        public ICommand ChamberDownCommand { get; set; }
        //LOADING ELEVATOR
        public ICommand LoadingElevatorUpCommand { get; set; }
        public ICommand LoadingElevatorDownCommand { get; set; }
        //UNLOADING ELEVATOR
        public ICommand UnloadingElevatorUpCommand { get; set; }
        public ICommand UnloadingElevatorDownCommand { get; set; }
        //LOADING BUFFER
        public ICommand LoadingBufferFWDCommand { get; set; }
        public ICommand LoadingBufferBWDCommand { get; set; }
        public ICommand LoadingBufferRollerRunCommand { get; set; }
        public ICommand LoadingBufferRollerStopCommand { get; set; }
        public ICommand LoadingBufferStopperUpCommand { get; set; }
        public ICommand LoadingBufferStopperDownCommand { get; set; }
        //UNLOADING BUFFER
        public ICommand UnloadingBufferFWDCommand { get; set; }
        public ICommand UnloadingBufferBWDCommand { get; set; }
        public ICommand UnloadingBufferRollerRunCommand { get; set; }
        public ICommand UnloadingBufferRollerStopCommand { get; set; }
        public ICommand UnloadingBufferStopperUpCommand { get; set; }
        public ICommand UnloadingBufferStopperDownCommand { get; set; }
        //STRIP ID
        public ICommand ReadStripIDCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        // LOADING PUSHER
        private void LoadingPusherLeft() { }
        private void LoadingPusherRight() { }
        private void LoadingPusherUp() { }
        private void LoadingPusherDown() { }

        // INDEX PUSHER
        private void IndexPusherLeft() { }
        private void IndexPusherRight() { }
        private void IndexPusherUp() { }
        private void IndexPusherDown() { }

        // CHAMBER
        private void ChamberUp() { }
        private void ChamberDown() { }

        // LOADING ELEVATOR
        private void LoadingElevatorUp() { }
        private void LoadingElevatorDown() { }

        // UNLOADING ELEVATOR
        private void UnloadingElevatorUp() { }
        private void UnloadingElevatorDown() { }

        // LOADING BUFFER
        private void LoadingBufferFWD() { }
        private void LoadingBufferBWD() { }
        private void LoadingBufferRollerRun() { }
        private void LoadingBufferRollerStop() { }
        private void LoadingBufferStopperUp() { }
        private void LoadingBufferStopperDown() { }

        // UNLOADING BUFFER
        private void UnloadingBufferFWD() { }
        private void UnloadingBufferBWD() { }
        private void UnloadingBufferRollerRun() { }
        private void UnloadingBufferRollerStop() { }
        private void UnloadingBufferStopperUp() { }
        private void UnloadingBufferStopperDown() { }
        //STRIP ID
        public void ReadStripID() { }
        #endregion EXECUTE COMMAND
        public ManualPanelViewModel() 
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            // LOADING PUSHER
            LoadingPusherLeftCommand = new RelayCommand(LoadingPusherLeft);
            LoadingPusherRightCommand = new RelayCommand(LoadingPusherRight);
            LoadingPusherUpCommand = new RelayCommand(LoadingPusherUp);
            LoadingPusherDownCommand = new RelayCommand(LoadingPusherDown);

            // INDEX PUSHER
            IndexPusherLeftCommand = new RelayCommand(IndexPusherLeft);
            IndexPusherRightCommand = new RelayCommand(IndexPusherRight);
            IndexPusherUpCommand = new RelayCommand(IndexPusherUp);
            IndexPusherDownCommand = new RelayCommand(IndexPusherDown);

            // CHAMBER
            ChamberUpCommand = new RelayCommand(ChamberUp);
            ChamberDownCommand = new RelayCommand(ChamberDown);

            // LOADING ELEVATOR
            LoadingElevatorUpCommand = new RelayCommand(LoadingElevatorUp);
            LoadingElevatorDownCommand = new RelayCommand(LoadingElevatorDown);

            // UNLOADING ELEVATOR
            UnloadingElevatorUpCommand = new RelayCommand(UnloadingElevatorUp);
            UnloadingElevatorDownCommand = new RelayCommand(UnloadingElevatorDown);

            // LOADING BUFFER
            LoadingBufferFWDCommand = new RelayCommand(LoadingBufferFWD);
            LoadingBufferBWDCommand = new RelayCommand(LoadingBufferBWD);
            LoadingBufferRollerRunCommand = new RelayCommand(LoadingBufferRollerRun);
            LoadingBufferRollerStopCommand = new RelayCommand(LoadingBufferRollerStop);
            LoadingBufferStopperUpCommand = new RelayCommand(LoadingBufferStopperUp);
            LoadingBufferStopperDownCommand = new RelayCommand(LoadingBufferStopperDown);

            // UNLOADING BUFFER
            UnloadingBufferFWDCommand = new RelayCommand(UnloadingBufferFWD);
            UnloadingBufferBWDCommand = new RelayCommand(UnloadingBufferBWD);
            UnloadingBufferRollerRunCommand = new RelayCommand(UnloadingBufferRollerRun);
            UnloadingBufferRollerStopCommand = new RelayCommand(UnloadingBufferRollerStop);
            UnloadingBufferStopperUpCommand = new RelayCommand(UnloadingBufferStopperUp);
            UnloadingBufferStopperDownCommand = new RelayCommand(UnloadingBufferStopperDown);

            //STRIP ID
            ReadStripIDCommand = new RelayCommand(ReadStripID);
        }
    }
}
