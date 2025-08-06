using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.UIComponent.MessageBox;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Common.Helpers;
using VSP_88D_CS.Models.Auth;
using VSP_88D_CS.Models.Common;
using VSP_88D_CS.Models.Recipe;
using VSP_88D_CS.Styles.Controls;
using VSP_88D_CS.Views.Setting.PopUp;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;


namespace VSP_88D_CS.ViewModels.Setting.Sub
{
    public class RegisterPageViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }

        private readonly IRegionManager _regionManager;
        private readonly UserRepository _userRepository;
        private ObservableCollection<User> _users;
        string _dataPath;
        bool _isEditMode, _isAddMode;
        User _oldUser;
        private readonly IGlobalSystemOption _globalSystemOption;

        #region COMMAND
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand SetAccessMatrixCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand EditAliasCommand { get; }
        #endregion COMMAND

        #region PROPERTY

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
        private ButtonInfo _btnSetMatrixInfo;
        public ButtonInfo BtnSetMatrixInfo
        {
            get { return _btnSetMatrixInfo; }
            set { SetProperty(ref _btnSetMatrixInfo, value); }
        }
        private ButtonInfo _btnEditAliasInfo;
        public ButtonInfo BtnEditAliasInfo
        {
            get { return _btnEditAliasInfo; }
            set { SetProperty(ref _btnEditAliasInfo, value); }
        }


        private RadioOption _level1Option;
        public RadioOption Level1Option
        {
            get => _level1Option;
            set
            {
                SetProperty(ref _level1Option, value);
            }
        }
        private RadioOption _level2Option;
        public RadioOption Level2Option
        {
            get => _level2Option;
            set
            {
                SetProperty(ref _level2Option, value);
            }
        }
        private RadioOption _level3Option;
        public RadioOption Level3Option
        {
            get => _level3Option;
            set
            {
                SetProperty(ref _level3Option, value);
            }
        }
        private string _editingPassword;
        public string EditingPassword
        {
            get => _editingPassword;
            set
            {
                SetProperty(ref _editingPassword, value);
            }
        }



        private bool _isEnableUserListRegion;
        public bool IsEnableUserListRegion
        {
            get { return _isEnableUserListRegion; }
            set { SetProperty(ref _isEnableUserListRegion, value); }
        }
        private bool _isAddEnable;
        public bool IsAddEnable
        {
            get => _isAddEnable;
            set { SetProperty(ref _isAddEnable, value); }
        }

        private bool _isEditEnable;
        public bool IsEditEnable
        {
            get => _isEditEnable;
            set { SetProperty(ref _isEditEnable, value); }
        }

        private User _editingUser;
        public User EditingUser
        {
            get => _editingUser;
            set
            {
                SetProperty(ref _editingUser, value);
                UpdateUserLevel(_editingUser);
            }

        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                if (_selectedUser != null)
                    LoadForEdit(_selectedUser);
            }

        }
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }
        #endregion PROPERTY
        private void UpdateUserLevel(User user)
        {
            switch (user.Role)
            {
                case UserAccessLib.Common.Enum.UserRole.Admin:
                    Level1Option.IsChecked = false;
                    Level2Option.IsChecked = false;
                    Level3Option.IsChecked = true;
                    break;
                case UserAccessLib.Common.Enum.UserRole.Manager:
                    Level1Option.IsChecked = false;
                    Level2Option.IsChecked = true;
                    Level3Option.IsChecked = false;

                    break;
                case UserAccessLib.Common.Enum.UserRole.Operator:
                    Level1Option.IsChecked = true;
                    Level2Option.IsChecked = false;
                    Level3Option.IsChecked = false;
                    break;
                default:
                    Level1Option.IsChecked = true;
                    Level2Option.IsChecked = false;
                    Level3Option.IsChecked = false;
                    break;
            }
        }
        public RegisterPageViewModel(IRegionManager regionManager, UserRepository userRepository, IGlobalSystemOption globalSystemOption)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();
            _globalSystemOption = globalSystemOption;
            _regionManager = regionManager;
            _userRepository = userRepository;
            AddUserCommand = new RelayCommand<object>(OnAddUser);
            EditUserCommand = new RelayCommand<object>(OnEditUser);
            DeleteUserCommand = new RelayCommand<object>(OnDeleteUser);
            SetAccessMatrixCommand = new RelayCommand<object>(OnSetAccessMatrix);
            ConfirmCommand = new RelayCommand<object>(OnConfirm);
            CancelCommand = new RelayCommand<object>(OnCancel);
            EditAliasCommand = new RelayCommand<object>(OnEditAlias);

            _dataPath = _globalSystemOption.DataPath;
            CreateButton();
            GetAllUser();
            CreateRadioOption();
            Init();
        }
        private void Init()
        {
            _btnAddInfo.IsEnable = true;
            _btnEditInfo.IsEnable = true;
            _btnDeleteInfo.IsEnable = true;
            UpdateControlEnable();
        }
        private void CreateButton()
        {
            BtnAddInfo = new ButtonInfo { Key = "Add", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/AddFile.png" };
            BtnEditInfo = new ButtonInfo { Key = "Edit", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/EditFile.png" };
            BtnDeleteInfo = new ButtonInfo { Key = "Delete", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/DeleteFile.png" };
            BtnConfirmInfo = new ButtonInfo { Key = "OK", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/OK.png" };
            BtnCancelInfo = new ButtonInfo { Key = "Cancel", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/Cancel.png" };
            BtnSetMatrixInfo = new ButtonInfo { Key = "SetMatrix", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/SetMatrix.png" };
            BtnEditAliasInfo = new ButtonInfo { Key = "Edit", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/Edit.png" };
        }
        private void CreateRadioOption()
        {
            List<RadioOption> radioOptions;
            string fileLevel = $"{_dataPath}/{CreateFileName("LevelOptions")}";
            if (!File.Exists(fileLevel))
            {
                radioOptions = new List<RadioOption>();
                radioOptions.Add(new RadioOption { IsChecked = true, OptionId = 1, OptionName = "Level1" });
                radioOptions.Add(new RadioOption { IsChecked = true, OptionId = 2, OptionName = "Level2" });
                radioOptions.Add(new RadioOption { IsChecked = true, OptionId = 3, OptionName = "Level3" });
                SupportFunctions.SaveJsonFile(fileLevel, radioOptions);
            }
            radioOptions = SupportFunctions.LoadJsonFile<List<RadioOption>>(fileLevel);

            Level1Option = radioOptions[0];
            Level2Option = radioOptions[1];
            Level3Option = radioOptions[2];
        }      

        #region ProcessCommand
        private void OnEditAlias(object obj)
        {
            VSContainer.Instance.ClearCache(typeof(LevelAliasSetting));
            if (VSContainer.Instance.Resolve(typeof(LevelAliasSetting)) is LevelAliasSetting levelAliasSetting)
            {

                levelAliasSetting.ShowDialog();
                CreateRadioOption();
            }
            //    MessageBox.Show("edit");
        }

        private void OnCancel(object? obj)
        {
            if (_isEditMode || _isAddMode)
            {
                _isEditMode = false;
                _isAddMode = false;
                UpdateControlEnable();
                if (_users.Count> 0)
                {
                    SelectedUser = _users[0];
                }
            }
            IsAddEnable = false;
            IsEditEnable = false;
        }

        private void OnConfirm(object? obj)
        {
            if (_isAddMode || _isEditMode)
            {
                if (Level1Option.IsChecked)
                {
                    EditingUser.Role = UserAccessLib.Common.Enum.UserRole.Operator;
                }
                else
                {
                    if (Level2Option.IsChecked)
                    {
                        EditingUser.Role = UserAccessLib.Common.Enum.UserRole.Manager;

                    }
                    else
                    {
                        if (Level3Option.IsChecked)
                        {
                            EditingUser.Role = UserAccessLib.Common.Enum.UserRole.Admin;
                        }
                    }
                }
            }
            if (_isAddMode)
            {


                CreateUser();
                UpdateControlEnable();
            }
            else if (_isEditMode)
            {
                UpdateUser();
                UpdateControlEnable();
            }
        }

        private void OnSetAccessMatrix(object? obj)
        {
            VSContainer.Instance.ClearCache(typeof(UserAccessMatrix));
            if (VSContainer.Instance.Resolve(typeof(UserAccessMatrix)) is UserAccessMatrix userAccessMatrix)
            {
                userAccessMatrix.ShowDialog();
            }
        }

        private async void OnDeleteUser(object? obj)
        {
            if (_users.Count <= 0)
            {
                return;
            }
            if (!IsAddEditEnable())
            {
                return;
            }
            if (SelectedUser == null) return;
            BtnDeleteInfo.IsSelected = true;


            string nameMsg = "User";
            string confirmMsg = "{0} Id:{1}, Name:{2} \nDo you want to delete it? ";
            var result = MessageUtils.ShowQuestion(string.Format(confirmMsg, nameMsg, _selectedUser.Id, _selectedUser.UserName), nameMsg);
            if (result == MessageBoxResult.OK)
            {
               
                await _userRepository.DeleteAsync(SelectedUser.Id);
                GetAllUser();
                SelectedUser = new User();
            }
            BtnDeleteInfo.IsSelected = false;

        }

        private void OnEditUser(object? obj)
        {
            if (_users.Count <= 0)
            {
                return;
            }
            if(_selectedUser == null) return;
            if (IsAddEditEnable())
            {
                IsAddEnable = true;
                IsEditEnable = true;
                _isEditMode = true;
                UpdateControlEnable();
            }
        }

        private void OnAddUser(object? obj)
        {
            if (IsAddEditEnable())
            {
                IsAddEnable = true;
                _isAddMode = true;
                EditingUser = new User();
                EditingPassword = "";
                UpdateControlEnable();
            }
        }
        #endregion
        private async void GetAllUser()
        {
            var users = await _userRepository.GetAllAsync();
            Users = new ObservableCollection<User>(users.Where(x => x.Role != UserAccessLib.Common.Enum.UserRole.Maker));
        }

        public void LoadForEdit(User user)
        {
            EditingUser = new User
            {
                UserName = user.UserName,
                Role = user.Role,
                Id = user.Id,
                Password = user.Password
            };
            EditingPassword = "*@#";
            _oldUser = new User
            {
                UserName = user.UserName,
                Role = user.Role,
                Id = user.Id,
                Password = user.Password
            };
            //  IsEditMode = true;
        }
       
        private async void UpdateUser()
        {

            User user = _editingUser;
            string oldName = _oldUser.UserName;
            string newName = _editingUser.UserName;
            if (newName != oldName)
            {
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show($"UserName is not null or empty.");
                    return;
                }
                if (Users.Any(x => x.UserName == newName))
                {
                    MessageBox.Show($"{newName} already exists.");
                    return;
                }
            }
            if (EditingPassword != "*@#")
            {
                user.Password = PasswordHelper.Hash(EditingPassword);
            }
            else
            {

                user.Password = _oldUser.Password;
            }

            await _userRepository.UpdateAsync(user);
            GetAllUser();
            _isEditMode = false;

        }
        private async void CreateUser()
        {
            bool resultInsert = await _userRepository.InsertAsync(new User
            {

                UserName = EditingUser.UserName,
                Role = EditingUser.Role,
                Password = PasswordHelper.Hash(EditingPassword)
            });

            if (!resultInsert)
            {
                throw new Exception("Insert user unsuccessfully!");
            }

            GetAllUser();
            _isAddMode = false;

        }

        private string CreateFileName(string fileName)
        {
            return $"{fileName}.lvl";
        }
        private bool IsAddEditEnable()
        {
            if (_isEditMode || _isAddMode)
            {
                return false;
            }
            return true;
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

            IsEnableUserListRegion = !_isEditMode && !_isAddMode;
        }
   
    }
}
