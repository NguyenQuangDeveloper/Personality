using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Common.MVVM.Core;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Models.Setting;

namespace VSP_88D_CS.ViewModels.Setting.PopUp
{
    public class UserAccessMatrixViewModel : ViewModelBase
    {
        private readonly IGlobalSystemOption _globalSystemOption;
        string _dataPath;
        public ObservableCollection<FunctionPermission> FunctionPermissionItems
        {
            get => _functionPermissionItems;
            set
            {
                SetProperty(ref _functionPermissionItems, value);
            }
        }
        private ObservableCollection<FunctionPermission> _functionPermissionItems;
        public ICommand HeaderCommand { get; }
        public ICommand EditPermissionCommand { get; }
        public ICommand SavePermissionCommand { get; }
        public ICommand CancelCommand { get; }
        public UserAccessMatrixViewModel(IGlobalSystemOption globalSystemOption)
        {
            _globalSystemOption = globalSystemOption;
            HeaderCommand = new RelayCommand<string>(OnHeader);
            EditPermissionCommand = new RelayCommand<object>(OnEditPermission);
            SavePermissionCommand = new RelayCommand<object>(OnSavePermission);
            CancelCommand = new RelayCommand<object>(OnCancel);
            _dataPath =_globalSystemOption.DataPath;
         
            LoadPermissionFile();
          
           
        }
        private void LoadPermissionFile()
        {

            string fileMatrix = $"{_dataPath}/{CreateFileName("Permission")}";

            if (!File.Exists(fileMatrix))
            {
                Init();
                SupportFunctions.SaveJsonFile(fileMatrix, FunctionPermissionItems);
            }
            var  functionPermissions = new ObservableCollection<FunctionPermission>( SupportFunctions.LoadJsonFile<List<FunctionPermission>>(fileMatrix));
            if (functionPermissions.Count ==0) 
            {
               Init();
               SupportFunctions.SaveJsonFile(fileMatrix, FunctionPermissionItems);
            }
            else { FunctionPermissionItems = functionPermissions; }
           
        }
        private void OnSavePermission(object obj)
        {
            string fileMatrix = $"{_dataPath}/{CreateFileName("Permission")}";
           
            SupportFunctions.SaveJsonFile(fileMatrix, FunctionPermissionItems);
            CloseForm();
        }

        private void OnCancel(object obj)
        {
            CloseForm();
        }

        private void OnEditPermission(object obj)
        {
            if (IsEditEnable)
            {
                IsEditEnable = false;
            }
            else { IsEditEnable = true; }
          
            //throw new NotImplementedException();
        }

        private void OnHeader(string? level)
        {
            if(IsEditEnable)
            SetPermission(level);
        }

        private void SetPermission( string level)
        {
            bool level1=false,level2=false,level3=false,allowAll = false;
            switch (level)
            {
                case "Level1":
                    level1 = true;
                    break;
                case "Level2":
                    level2 = true;
                    break;
                case "Level3":
                    level3 = true;
                    break;
                case "AllowAll":
                    allowAll = true;
                    break;
                default:
                    level1 = true;
                    break;
            }
            foreach (var item in _functionPermissionItems)
            {
                item.Level1 = level1 ;
                item.Level2 = level2;
                item.Level3 = level3;
                item.AllowAll = allowAll;
            }
            FunctionPermissionItems = new ObservableCollection<FunctionPermission>(_functionPermissionItems);
        }

        private void Init()
        {
           
            InitPermission();
            // _functionPermissionItems = new ObservableCollection<FunctionPermission>();
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.PARA,
            //    Title = "PARA",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.Setup,
            //    Title = "Setup",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.IOTest,
            //    Title = "I/O Test",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false,
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.Log,
            //    Title = "Log",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.ExitFunction,
            //    Title = "Exit Function",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.RecipeSelect,
            //    Title = "Recipe Select",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.ChamberValvePumpFunction,
            //    Title = "Chamber Valve Pump Function",
            //    Level1 = true,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false,
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.OfflineLocalRemote,
            //    Title = "Offline/Local/Remote",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.CIMSetFunction,
            //    Title = "CIM Set Function",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.AlarmReset,
            //    Title = "Alarm Reset",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.ParameterFunction,
            //    Title = "Parameter Function",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //_functionPermissionItems.Add(new FunctionPermission
            //{
            //    KeyItem = eFunctionItem.RegisterUser,
            //    Title = "Register User",
            //    Level1 = false,
            //    Level2 = false,
            //    Level3 = false,
            //    AllowAll = false
            //});
            //FunctionPermissionItems=_functionPermissionItems;
        }

        private void InitPermission()
        {
            _functionPermissionItems = new ObservableCollection<FunctionPermission>();
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.ResetFunction,
                Title = "Reset Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.InitialFunction,
                Title = "Initial Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.ReportFunction,
                Title = "Report Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false,
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.ManualFunction,
                Title = "Manual Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.ExitFunction,
                Title = "Exit Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.SettingFunction,
                Title = "Setting Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.RecipeFunction,
                Title = "Recipe Function",
                Level1 = true,
                Level2 = false,
                Level3 = false,
                AllowAll = false,
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.CleaningFunction,
                Title = "Cleaning Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.DeviceFunction,
                Title = "Device Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.ParameterFunction,
                Title = "Parameter Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.RegisterUsers,
                Title = "Register Users",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.LidOpenFunction,
                Title = "Lid Open Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.LidCloseFunction,
                Title = "Lid Close Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });



            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.ManualCleanFunction,
                Title = "Manual Clean Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.RecipeSelect,
                Title = "Recipe Select",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.CIMSetFunction,
                Title = "CIM Set Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.OfflineLocalRemote,
                Title = "Offline/Local/Remote",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });

            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.SequenceMonitor,
                Title = "Sequence Monitor(F7)",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.ChamberValvePumpFunction,
                Title = "Chamber Valve Pump Function",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });

            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.LeakageTestSet,
                Title = "Leakage Test Set",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.GeneratorPumpTimerReset,
                Title = "Generator, Pump Timer Reset",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });
            _functionPermissionItems.Add(new FunctionPermission
            {
                KeyItem = eFunctionItem.CounterReset,
                Title = "Counter Reset",
                Level1 = false,
                Level2 = false,
                Level3 = false,
                AllowAll = false
            });

            FunctionPermissionItems = _functionPermissionItems;
        }
        private bool _isEditEnable =false;
        public bool IsEditEnable
        {
            get { return _isEditEnable; }
            set
            {
                SetProperty(ref _isEditEnable, value);
            }
        }

        private string CreateFileName(string fileName)
        {
            return $"{fileName}.mtx";
        }

        private void CloseForm()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }
    }
}
