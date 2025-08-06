using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Auth;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Models.Recipe;
using VSP_88D_CS.Models.Setting;
using VSP_88D_CS.Styles.Controls;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.UIComponent.MessageBox;
using VSP_88D_CS.Common.Helpers;

namespace VSP_88D_CS.ViewModels.Setting.Sub
{
    public class DevicePageViewModel : ViewModelBase
    {
        private enum eTypeMotor
        {
            LoadingElevator,
            LoadingPusher,
            IndexPusher,
            UnloadingElevator

        }
        private readonly IGlobalSystemOption _globalSystemOption;
        private readonly DeviceRepository _deviceRepository;
        public readonly LanguageService LanguageResources;
        private readonly IRegionManager _regionManager;

        bool _isEditMode;
        bool _isAddMode;
        string _dataPath;

        public ICommand AddDeviceCommand { get; }
        public ICommand EditDeviceCommand { get; }
        public ICommand DeleteDeviceCommand { get; }
        public ICommand ConfirmDeviceCommand { get; }
        public ICommand CancelDeviceCommand { get; }

       
        #region PROPERTY
        private ObservableCollection<WorkPair> _selectMotors;
        public ObservableCollection<WorkPair> SelectMotors
        {
            get { return _selectMotors; }
            set { SetProperty(ref _selectMotors, value); }
        }
        private WorkPair _selectedMotor;
        public WorkPair SelectedMotor
        {
            get { return _selectedMotor; }
            set
            {
                SetProperty(ref _selectedMotor, value);
                UpdateMotion();
            }
        }

        private double _selectedDistance =0.001;
        public double SelectedDistance
        {
            get { return _selectedDistance; }
            set
            {
                SetProperty(ref _selectedDistance, value);
                UpdateDistance();
            }
        }

        private ObservableCollection<DeviceData> _devices;
        public ObservableCollection<DeviceData> Devices
        {
            get { return _devices; }
            set { SetProperty(ref _devices, value); }
        }
        private DeviceData _selectedDevice;
        public DeviceData SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged();
                if (_selectedDevice != null)
                    UpdateData();
            }
        }

        private ButtonInfo _btnEditInfo;
        public ButtonInfo BtnEditInfo
        {
            get { return _btnEditInfo; }
            set { SetProperty(ref _btnEditInfo, value); }
        }
        private ButtonInfo _btnAddInfo;
        public ButtonInfo BtnAddInfo
        {
            get { return _btnAddInfo; }
            set { SetProperty(ref _btnAddInfo, value); }
        }
        private ButtonInfo _btnDeleteInfo;
        public ButtonInfo BtnDeleteInfo
        {
            get { return _btnDeleteInfo; }
            set { SetProperty(ref _btnDeleteInfo, value); }
        }
        private ButtonInfo _btnConfirmInfo;
        public ButtonInfo BtnConfirmInfo
        {
            get { return _btnConfirmInfo; }
            set { SetProperty(ref _btnConfirmInfo, value); }
        }
        private ButtonInfo _btnCancelInfo;
        public ButtonInfo BtnCancelInfo
        {
            get { return _btnCancelInfo; }
            set { SetProperty(ref _btnCancelInfo, value); }
        }

        private MotionParaViewModel _motionVM1;
        public MotionParaViewModel MotionVM1
        {
            get { return _motionVM1; }
            set { SetProperty(ref _motionVM1, value); }
        }
        private MotionParaViewModel _motionVM2;
        public MotionParaViewModel MotionVM2
        {
            get { return _motionVM2; }
            set { SetProperty(ref _motionVM2, value); }
        }


        private bool _isEnableDeviceListRegion;
        public bool IsEnableDeviceListRegion
        {
            get { return _isEnableDeviceListRegion; }
            set { SetProperty(ref _isEnableDeviceListRegion, value); }
        }

        private bool _isEnableDeviceNameRegion;
        public bool IsEnableDeviceNameRegion
        {
            get { return _isEnableDeviceNameRegion; }
            set { SetProperty(ref _isEnableDeviceNameRegion, value); }
        }

        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                SetProperty(ref _deviceName, value);
            }
        }
     
        private bool _isVisibleMotionControl;
        public bool IsVisibleMotionControl
        {
            get { return _isVisibleMotionControl; }
            set { SetProperty(ref _isVisibleMotionControl, value); }
        }
        #endregion

        private void UpdateData()
        {
            DeviceName = _selectedDevice.DeviceName;
            var listMotions = SelectedDevice.MotionItems;
            if (listMotions == null) return;
            List<WorkPair> pairs = new List<WorkPair>();

            var items = listMotions;
            for (int i = 0; i < items.Count; i += 2)
            {
                var item1 = items[i];
                var id1 = i;

                int id2 = -1;
                string alias2 = "";

                if (i + 1 < items.Count)
                {
                    id2 = i + 1;
                    alias2 = items[i + 1].MotionName;
                }

                pairs.Add(new WorkPair
                {
                    Id1 = id1,
                    Alias1 = item1.MotionName,
                    Id2 = id2,
                    Alias2 = alias2
                });
            }
            SelectMotors = new ObservableCollection<WorkPair>(pairs);
            if (pairs.Count > 0)
                SelectedMotor = pairs[0];
        }
        private void UpdateMotion()
        {
            IsVisibleMotionControl = false;
            var selectedMotor = _selectedMotor;
            if (selectedMotor == null) return;
            if (selectedMotor.Id1 == -1) return;

            _motionVM1.LoadData(selectedMotor.Alias1, SelectedDevice.MotionItems[selectedMotor.Id1]);
            if (selectedMotor.Id2 == -1) return;
            _motionVM2.LoadData(selectedMotor.Alias2, SelectedDevice.MotionItems[selectedMotor.Id2]);
            IsVisibleMotionControl = true;
        }
        private void UpdateDistance()
        {
           
            _motionVM1.LoadDistacnce(_selectedDistance,0,0);
            
            _motionVM2.LoadDistacnce(_selectedDistance, 0, 0);

        }
       
        public DevicePageViewModel(LanguageService languageService, IRegionManager regionManager, DeviceRepository deviceRepository, IGlobalSystemOption globalSystemOption)
        {
            _globalSystemOption = globalSystemOption;
            _deviceRepository = deviceRepository;
            LanguageResources = languageService;
            _regionManager = regionManager;
            AddDeviceCommand = new RelayCommand<object>(OnAddDevice);
            EditDeviceCommand = new RelayCommand<object>(OnEditDevice);
            DeleteDeviceCommand = new RelayCommand<object>(OnDeleteDevice);
            ConfirmDeviceCommand = new RelayCommand<object>(OnConfirmDevice);
            
            CancelDeviceCommand = new RelayCommand<object>(OnCancelDevice);

            CreateButton();
            // _regionManager.RequestNavigate("TopMotionPage", typeof(MotionPara));
            // _regionManager.RequestNavigate("BottonMotionPage", typeof(MotionPara));
            // _regionManager.RequestNavigate<MotionPara>("TopMotionPage");
            //_regionManager.RequestNavigate<MotionPara>("BottonMotionPage");
            _devices = new ObservableCollection<DeviceData>();
            _dataPath = _globalSystemOption.DataPath;
            Init();
            UpdateDistance();
        }

        #region InitFunctions
        private void Init()
        {
            _motionVM1 = new MotionParaViewModel(LanguageResources, _regionManager);
            _motionVM2 = new MotionParaViewModel(LanguageResources, _regionManager);
            _btnAddInfo.IsEnable = true;
            _btnEditInfo.IsEnable = true;
            _btnDeleteInfo.IsEnable = true;
            UpdateControlEnable();
            DeviceListInit();                 
        }
        private async void DeviceListInit()
        {
            var deviceList = await _deviceRepository.GetAllAsync();
            var deviceDataList = new ObservableCollection<DeviceData>();
            foreach (var item in deviceList)
            {
                var deviceData = SupportFunctions.LoadJsonFile<DeviceData>(CreateFileName(item.Name), _dataPath);
                if (deviceData != null)
                {
                    deviceData.EntityID = item.Id;
                    deviceDataList.Add(deviceData);
                }
                else
                {
                    await _deviceRepository.DeleteAsync(item.Id);
                }
            }
            _devices = deviceDataList;
            SelectedDeviceInit();
        }
        private void SelectedDeviceInit()
        {
            if (_devices.Count > 0)
            {
                SelectedDevice = _devices[0];
            }
            else
            {
                var deviceData = new DeviceData();

                deviceData.MotionItems = FillMotion("");
                SelectedDevice = deviceData;
            }
        }
        List<MotionParameter> parameters;
        MotionData _motionData;
        private void CreateButton()
        {
            BtnAddInfo = new ButtonInfo { Key = "Add", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/AddFile.png" };
            BtnEditInfo = new ButtonInfo { Key = "Edit", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/EditFile.png" };
            BtnDeleteInfo = new ButtonInfo { Key = "Delete", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/DeleteFile.png" };
            BtnConfirmInfo = new ButtonInfo { Key = "OK", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/OK.png" };
            BtnCancelInfo = new ButtonInfo { Key = "Cancel", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/Cancel.png" };
        }
        #endregion
        #region ProcessCommand
        private void OnCancelDevice(object? obj)
        {
            
            if (_isEditMode || _isAddMode)
            {
                _isEditMode = false;
                _isAddMode = false;
                //InitDeviceParamGrid();
                // IndicateSelDeviceParam();
                //UpdateDeviceParamInfo();
                UpdateControlEnable();
                SelectedDeviceInit();
                SetParaBtnEnable(false);

            }
        }
        private void OnConfirmDevice(object? obj)
        {
            if (!_isAddMode && !_isEditMode)
            {
                return;
            }
            if (_isEditMode)
            {
                _isEditMode = false;
                UpdateDeviceItem();
            }
            else if (_isAddMode)
            {
                _isAddMode = false;
                AddDeviceItem(_deviceName);
            }
            SelectedDeviceInit();
            UpdateControlEnable();
            SetParaBtnEnable(false);
            //var a = _motionData.MotionParameters.Count + 1;
            //_motionData.MotionParameters.Add(new MotionParameter
            //{
            //    Description = "Index ",
            //    Acceleration = 1,
            //    Position = 0,
            //    Velocity = a
            //});
            //_motionVM2.LoadData($"Vm2 {DateTime.Now}", _motionData.MotionParameters);
            //_motionVM1.LoadData($"Vm1 {DateTime.Now}", _motionData.MotionParameters);
        }
        private void OnDeleteDevice(object? obj)
        {
            if (_devices.Count <= 0)
            {
                return;
            }
            if (!IsAddEditEnable())
            {
                return;
            }
            BtnDeleteInfo.IsSelected = true;
            string nameMsg = "Device";
            string confirmMsg = "{0} Id:{1}, Name:{2} \nDo you want to delete it? ";
            var result = MessageUtils.ShowQuestion(string.Format(confirmMsg, nameMsg, SelectedDevice.EntityID, SelectedDevice.DeviceName));
            if (result == MessageBoxResult.OK)
            {         
                DeleteDeviceItem(_selectedDevice.EntityID);
                SelectedDeviceInit();
            }
            BtnDeleteInfo.IsSelected = false;
        }
        private void OnEditDevice(object? obj)
        {
            if (_devices.Count <= 0)
            {
                return;
            }
            if (IsAddEditEnable())
            {
                _isEditMode = true;
                var deviceData = _selectedDevice;
                SelectedDevice = deviceData.Clone();
                SetParaBtnEnable(true);
                UpdateControlEnable();
            }
        }
        private void OnAddDevice(object? obj)
        {
            if (IsAddEditEnable())
            {
                _isAddMode = true;
                var device = _selectedDevice;
                SelectedDevice = device.Clone();
                SetParaBtnEnable(true);
                UpdateControlEnable();
            }

        }
        #endregion
        #region Functions
        private bool IsAddEditEnable()
        {
            if (_isEditMode || _isAddMode)
            {
                return false;
            }
            return true;
        }
        private List<MotionParameter> FillParamesters(eTypeMotor typeMotor)
        {
            string[] arrDescription = new string[6];
            for (int i = 0; i < arrDescription.Length; i++)
            {
                arrDescription[i] = "";
            }
            switch (typeMotor)
            {
                case eTypeMotor.LoadingElevator:
                    arrDescription[0] = "BTM. SLOT WORKING POSITION";
                    arrDescription[1] = "TOP SLOT WORKING POSITION";
                    arrDescription[2] = "MGZ. LOADING/UNLOADING POSITION";
                    arrDescription[3] = "MGZ. PITCH";
                    arrDescription[4] = "DEVICE SETTING POSITION";
                    break;
                case eTypeMotor.LoadingPusher:
                    arrDescription[0] = "LOADING PUSHER RETRACT POSITION";
                    arrDescription[1] = "LOADING PUSHER EXTEND POSITION";

                    break;
                case eTypeMotor.IndexPusher:
                    arrDescription[0] = "READY POSITION";
                    arrDescription[1] = "CHAMBER INPUT STRIP CHECK POSITION";
                    arrDescription[2] = "CHAMBER LOAD START POS";
                    arrDescription[3] = "CHAMBER LOAD END POS";
                    arrDescription[4] = "CHAMBER UNLOAD END POS";
                    arrDescription[5] = "CHAMBER UNLOAD START POS";
                    break;
                case eTypeMotor.UnloadingElevator:
                    arrDescription[0] = "BTM. SLOT WORKING POSITION";
                    arrDescription[1] = "TOP SLOT WORKING POSITION";
                    arrDescription[2] = "MGZ. LOADING/UNLOADING POSITION";
                    arrDescription[3] = "MGZ. PITCH";
                    arrDescription[4] = "DEVICE SETTING POSITION";
                    break;

            }

            parameters = new List<MotionParameter>();

            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = arrDescription[0],
                Acceleration = 1,
                Position = 0,
                Velocity = 6,
            });
            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = arrDescription[1],
                Position = 0.000,
            });
            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = arrDescription[2],
                Position = 0.000,
            });
            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = arrDescription[3],
                Position = 0.000,
            });
            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = arrDescription[4],
                Position = 0.000,
            });
            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = arrDescription[5],
                Position = 0.000,
            });
            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = "",
                Position = 0.000,
            });
            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = "(-) Limit",
                Position = 0.000,
            });
            AddParameterWithIndex(ref parameters, new MotionParameter
            {
                Description = "(+) Limit",
                Position = 0.000,
            });

            return parameters;
        }
        private void AddParameterWithIndex(ref List<MotionParameter> parameters, MotionParameter motionParameter)
        {
            int newIndex = parameters.Count + 1;
            motionParameter.Index = newIndex;
            parameters.Add(motionParameter);
        }
        private List<MotionData> FillMotion(string index)
        {
            var listMotion = new List<MotionData>();
            listMotion.Add(new MotionData
            {
                MotionName = $"(M01) Loading Elevator Z {index}",
                MotionParameters = FillParamesters(eTypeMotor.LoadingElevator),
                IsLeftRight = false,
                IsUpDown = true,
                ServoState = new ServoState()
            });
            listMotion.Add(new MotionData
            {
                MotionName = $"(M02) Loading Pusher X {index}",
                MotionParameters = FillParamesters(eTypeMotor.LoadingPusher),
                IsLeftRight = true,
                IsUpDown = false,
                  ServoState = new ServoState()
            });
            listMotion.Add(new MotionData
            {
                MotionName = $"(M03) Index Pusher X {index}",
                MotionParameters = FillParamesters(eTypeMotor.IndexPusher),
                IsLeftRight = true,
                IsUpDown = false,
                ServoState = new ServoState()
            });
            listMotion.Add(new MotionData
            {
                MotionName = $"(M04) UnLoading Elevator Z {index}",
                MotionParameters = FillParamesters(eTypeMotor.UnloadingElevator),
                IsLeftRight = false,
                IsUpDown = true,
                ServoState = new ServoState()
            });

            return listMotion;
        }

        private void UpdateControlEnable()
        {
            if (_isEditMode && BtnEditInfo.IsSelected != true)
                BtnEditInfo.IsSelected = true;
            else
                BtnEditInfo.IsSelected = false;

            if (_isAddMode && BtnAddInfo.IsSelected != true)
                BtnAddInfo.IsSelected = true;
            else
                BtnAddInfo.IsSelected = false;

            IsEnableDeviceListRegion = !_isEditMode && !_isAddMode;
            IsEnableDeviceNameRegion = _isEditMode || _isAddMode;

        }

        private async void AddDeviceItem(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show($"Device Name is not null or empty.");
                return;
            }
            if (_devices.Any(x => x.DeviceName == name))
            {
                MessageBox.Show($"{name} already exists.");
                return;
            }
            var deviceItem = new DeviceItem()
            {
                Name = name
            };
            await _deviceRepository.UpsertDeviceAsync(deviceItem);
            //var a = _deviceRepository.GetAll();

            var tempDeviceItem = _deviceRepository.GetDeviceByName(name);

            DeviceData device = _selectedDevice;
            device.DeviceName = name;
            device.EntityID = tempDeviceItem.Id;
            // device.MotionItems = FillMotion(1);

            SupportFunctions.SaveJsonFile(CreateFileName(name), _dataPath, device);
            _devices.Add(device);
        }
        private void DeleteDeviceItem(object id)
        {
            DeviceData device = _selectedDevice;
            _devices.Remove(device);
            _ = _deviceRepository.DeleteAsync(id);
            SupportFunctions.DeleteFile(CreateFileName(device.DeviceName), _dataPath);
        }
        private async void UpdateDeviceItem()
        {
            DeviceData device = _selectedDevice;
            string oldName = device.DeviceName;
            string newName = _deviceName;
            if (newName != oldName)
            {
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show($"Device Name is not null or empty.");
                    return;
                }
                if (_devices.Any(x => x.DeviceName == newName))
                {
                    MessageBox.Show($"{newName} already exists.");
                    return;
                }
            }

            device.DeviceName = newName;
            //device.MotionItems= 

            var deviceItem = await _deviceRepository.GetDeviceById(device.EntityID);
            deviceItem.Name = device.DeviceName;
            //deviceItem.RecipeName = "rcp1";
            deviceItem.Operator = "test";
            var tempData = _devices.FirstOrDefault(x => x.EntityID == device.EntityID);
            if (tempData != null)
            {
                tempData.DeviceName = device.DeviceName;
                tempData.MotionItems = device.MotionItems;
            }

            _ = _deviceRepository.UpdateAsync(deviceItem);
            SupportFunctions.DeleteFile(CreateFileName(oldName), _dataPath);
            SupportFunctions.SaveJsonFile(CreateFileName(device.DeviceName), _dataPath, device);
            Devices = new ObservableCollection<DeviceData>(_devices);
        }
        private void SetParaBtnEnable(bool isEnable)
        {
            _motionVM1.IsSetParaEnable = isEnable;
            _motionVM2.IsSetParaEnable = isEnable;
        }
        private string CreateFileName(string fileName)
        {
            return $"{fileName}.svr";
        }
        
        #endregion
    }
}
