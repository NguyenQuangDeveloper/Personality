using System.Collections.ObjectModel;
using System.Transactions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Recipe;
using VSP_88D_CS.Styles.Controls;
using VSP_88D_CS.Views.Auto.Sub;
using VSP_88D_CS.Views.Setting.PopUp;

namespace VSP_88D_CS.ViewModels.Setting.Sub
{
    public class MotionParaViewModel : ViewModelBase
    {
        public readonly LanguageService LanguageResources;
        public ICommand JobCommand { get; }
        public ICommand SetParaCommand { get; }

        public ICommand BtnServoCommand { get; }

        private bool _isSetParaEnable = false;
        public bool IsSetParaEnable
        {
            get { return _isSetParaEnable; }
            set { SetProperty(ref _isSetParaEnable, value); }
        }
        private bool _isDataGridReadOnly;
        public bool IsDataGridReadOnly
        {
            get { return _isDataGridReadOnly; }
            set { SetProperty(ref _isDataGridReadOnly, value); }
        }

        public MotionParaViewModel(LanguageService languageService, IRegionManager regionManager)
        {
            LanguageResources = languageService;
            JobCommand = new RelayCommand<object>(OnJob);
            SetParaCommand = new RelayCommand<object>(OnSetPara);
            BtnServoCommand = new RelayCommand<object>(OnBtnServo);
            CreateButton();

        }


        private ObservableCollection<MotionParameter> _motionItems;
        public ObservableCollection<MotionParameter> MotionItems
        {
            get { return _motionItems; }
            set
            {
                SetProperty(ref _motionItems, value);
            }

        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private double _currentPosition;
        public double CurrentPosition
        {
            get { return _currentPosition; }
            set { SetProperty(ref _currentPosition, value); }
        }

        private ButtonInfo _btnMoveJobLeft;
        public ButtonInfo BtnMoveJobLeft
        {
            get { return _btnMoveJobLeft; }
            set { SetProperty(ref _btnMoveJobLeft, value); }
        }
        private ButtonInfo _btnMoveJobRight;
        public ButtonInfo BtnMoveJobRight
        {
            get { return _btnMoveJobRight; }
            set { SetProperty(ref _btnMoveJobRight, value); }
        }
        private ButtonInfo _btnMoveJobUp;
        public ButtonInfo BtnMoveJobUp
        {
            get { return _btnMoveJobUp; }
            set { SetProperty(ref _btnMoveJobUp, value); }
        }
        private ButtonInfo _btnMoveJobDown;
        public ButtonInfo BtnMoveJobDown
        {
            get { return _btnMoveJobDown; }
            set { SetProperty(ref _btnMoveJobDown, value); }
        }

        #region IO color
        private Brush _servoOnColor = Brushes.Gray;
        public Brush ServoOnColor
        {
            get { return _servoOnColor; }
            set { SetProperty(ref _servoOnColor, value); }
        }

        private Brush _homeColor = Brushes.White;
        public Brush HomeColor
        {
            get { return _homeColor; }
            set { SetProperty(ref _homeColor, value); }
        }

        private Brush _alarmColor = Brushes.White;
        public Brush AlarmColor
        {
            get { return _alarmColor; }
            set { SetProperty(ref _alarmColor, value); }
        }

        private Brush _positiveLimitOnColor = Brushes.White;
        public Brush PositiveLimitOnColor
        {
            get { return _positiveLimitOnColor; }
            set { SetProperty(ref _positiveLimitOnColor, value); }
        }
        private Brush _negativeLimitOnColor = Brushes.White;
        public Brush NegativeLimitOnColor
        {
            get { return _negativeLimitOnColor; }
            set { SetProperty(ref _negativeLimitOnColor, value); }
        }
        private Brush _originColor = Brushes.White;
        public Brush OriginColor
        {
            get { return _originColor; }
            set { SetProperty(ref _originColor, value); }
        }

        #endregion
        private void OnJob(object obj)
        {
            var btn = obj as Button;
            var btnTag = btn.Tag as ButtonInfo;
            switch (btnTag.Key)
            {
                case "MoveJobLeft":
                    CurrentPosition = _currentPosition - 1*_distance;
                    break;
                case "MoveJobRight":
                    CurrentPosition = _currentPosition + 1* _distance;
                    break;
                case "MoveJobUp":
                    CurrentPosition = _currentPosition + 1* _distance;
                    break;
                case "MoveJobDown":
                    CurrentPosition = _currentPosition - 1* _distance;
                    break;
            }
        }
        private void OnBtnServo(object obj)
        {
            var btn = obj as Button;
            switch (btn.Tag)
            {
                case "ServoOn":
                    break;
                case "AlarmClear":
                    break;
                case "Initialize":
                    VSContainer.Instance.ClearCache(typeof(Initial));
                    if (VSContainer.Instance.Resolve(typeof(Initial)) is Initial initialView)
                    {
                        initialView.ShowDialog();
                    }
                    break;
            }
        }
        private void OnSetPara(object obj)
        {
            var para = obj as MotionParameter;
            if (!string.IsNullOrEmpty(para!.Description))
            {
                para.Position = _currentPosition;
            }

        }
        private void CreateButton()
        {
            BtnMoveJobLeft = new ButtonInfo { Key = "MoveJobLeft", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/arrow_left.png" };
            BtnMoveJobRight = new ButtonInfo { Key = "MoveJobRight", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/arrow_right.png" };
            BtnMoveJobUp = new ButtonInfo { Key = "MoveJobUp", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/arrow_up.png" };
            BtnMoveJobDown = new ButtonInfo { Key = "MoveJobDown", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/arrow_down.png" };
        }
        public void LoadData(string title, MotionData motionData)
        {
            Title = title;
            var motionItems = motionData.MotionParameters;
            bool bUpDown = motionData.IsUpDown;
            bool bLeftRight = motionData.IsLeftRight;
            var servoState= motionData.ServoState;
            MotionItems = new ObservableCollection<MotionParameter>(motionItems);
            BtnMoveJobUp.IsEnable = bUpDown;
            BtnMoveJobDown.IsEnable = bUpDown;
            BtnMoveJobLeft.IsEnable = bLeftRight;
            BtnMoveJobRight.IsEnable = bLeftRight;
            if(servoState!=null)
            UpdateColor(servoState);
        }

        double _distance, _vel, _acc;
        public void LoadDistacnce(double distance, double vel, double acc)
        {
            _distance = distance;
            _vel = vel;
            _acc = acc;
        }
        public List<MotionParameter> GetData()
        {
            return new List<MotionParameter>(MotionItems);
        }

        private void UpdateColor(ServoState servoState)
        {

            if (servoState.IsServoOn)
            {
                ServoOnColor = Brushes.Red;
            }
            else
            {
                ServoOnColor = Brushes.Gray;
            }
            if (servoState.IsHomeDone)
            {
                HomeColor = Brushes.Lime;
            }
            else
            {
                HomeColor = Brushes.White;
            }
            if (servoState.IsAlarmOn)
            {
                AlarmColor = Brushes.Red;
            }
            else { AlarmColor = Brushes.White; }

            if (servoState.IsPositiveLimitOn)
            {

                PositiveLimitOnColor = Brushes.Red;
            }
            else
            {
                PositiveLimitOnColor = Brushes.White;
            }
            if (servoState.IsNegativeLimitOn)
            {

                NegativeLimitOnColor = Brushes.Red;
            }
            else 
            {
                NegativeLimitOnColor = Brushes.White; 
            }
            if (servoState.IsOrigin)
            {
                OriginColor = Brushes.Lime;
            }
            else 
            { 
                OriginColor = Brushes.White; 
            }
        }
    }
}
