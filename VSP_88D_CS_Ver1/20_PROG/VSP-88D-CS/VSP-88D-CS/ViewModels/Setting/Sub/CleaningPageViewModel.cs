using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UserAccessLib.Common.Interfaces;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.UIComponent.MessageBox;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Common.Helpers;
using VSP_88D_CS.Models.Recipe;
using VSP_88D_CS.Styles.Controls;

namespace VSP_88D_CS.ViewModels.Setting.Sub
{
    public class CleaningPageViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IGlobalSystemOption _globalSystemOption;
        private readonly RecipeRepository _recipeRepository;
        //private readonly DeviceRepository _deviceRepository;
        private readonly CleaningRepository _cleaningRepository;
        public readonly LanguageService LanguageResources;
        public ICommand AddCleaningParaCommand { get; }
        public ICommand EditCleaningParaCommand { get; }
        public ICommand DeleteCleaningParaCommand { get; }
        public ICommand ConfirmCleaningParaCommand { get; }
        public ICommand CancelCleaningParaCommand { get; }
        public ICommand AddStepCleaningParaCommand { get; }
        public ICommand DeleteStepCleaningParaCommand { get; }

        bool _isEditMode;
        bool _isAddMode;
        int _minStep, _maxStep;
        string _dataPath;

        #region PROPERTY
        private ObservableCollection<CleaningStepView> _cleaningStepViews;
        public ObservableCollection<CleaningStepView> CleaningStepViews
        {
            get { return _cleaningStepViews; }
            set { SetProperty(ref _cleaningStepViews, value); }
        }

        private ObservableCollection<CleaningData> _cleaningDataList;
        public ObservableCollection<CleaningData> CleaningDataList
        {
            get { return _cleaningDataList; }
            set { SetProperty(ref _cleaningDataList, value); }
        }
        private CleaningData _selectedCleaningData;
        public CleaningData SelectedCleaningData
        {
            get => _selectedCleaningData;
            set
            {
                _selectedCleaningData = value;
                if (_selectedCleaningData != null)
                {

                    UpdateCleaningStepView();
                }
                OnPropertyChanged();

            }
        }
        private string _cleaningName;
        public string CleaningName
        {
            get { return _cleaningName; }
            set
            {
                SetProperty(ref _cleaningName, value);
            }
        }

        private ButtonInfo _btnAddStepInfo;
        public ButtonInfo BtnAddStepInfo
        {
            get { return _btnAddStepInfo; }
            set { SetProperty(ref _btnAddStepInfo, value); }
        }
        private ButtonInfo _btnDeleteStepInfo;
        public ButtonInfo BtnDeleteStepInfo
        {
            get { return _btnDeleteStepInfo; }
            set { SetProperty(ref _btnDeleteStepInfo, value); }
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
        private bool _isEnableCleaningListRegion;
        public bool IsEnableCleaningListRegion
        {
            get { return _isEnableCleaningListRegion; }
            set { SetProperty(ref _isEnableCleaningListRegion, value); }
        }
        private bool _isEnableCleaningNameRegion;
        public bool IsEnableCleaningNameRegion
        {
            get { return _isEnableCleaningNameRegion; }
            set { SetProperty(ref _isEnableCleaningNameRegion, value); }
        }
        private bool _isEnableCleaningStepRegion;
        public bool IsEnableCleaningStepRegion
        {
            get { return _isEnableCleaningStepRegion; }
            set { SetProperty(ref _isEnableCleaningStepRegion, value); }
        }
        #endregion

        public CleaningPageViewModel(LanguageService languageService, IRegionManager regionManager, RecipeRepository recipeRepository, CleaningRepository cleaningRepository, IGlobalSystemOption globalSystemOption, IAuthService authService)
        {
            _authService = authService;
            _globalSystemOption = globalSystemOption;
            _cleaningRepository = cleaningRepository;
            _recipeRepository = recipeRepository;
            LanguageResources = languageService;
            AddCleaningParaCommand = new RelayCommand<object>(OnAddCleaningPara);
            EditCleaningParaCommand = new RelayCommand<object>(OnEditCleaningPara);
            DeleteCleaningParaCommand = new RelayCommand<object>(OnDeleteCleaningPara);
            AddStepCleaningParaCommand = new RelayCommand<object>(OnAddStepCleaningPara);
            DeleteStepCleaningParaCommand = new RelayCommand<object>(OnDeleteStepCleaningPara);
            ConfirmCleaningParaCommand = new RelayCommand<object>(OnConfirmCleaningPara);
            CancelCleaningParaCommand = new RelayCommand<object>(OnCancelCleaningPara);
            _cleaningStepViews = new ObservableCollection<CleaningStepView>();

            _cleaningDataList = new ObservableCollection<CleaningData>();
            _minStep = 1;
            _maxStep = 5;
            _dataPath = _globalSystemOption.DataPath;
            CreateButton();
            Init();
        }
        private void CreateButton()
        {
            BtnAddInfo = new ButtonInfo { Key = "Add", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/AddFile.png" };
            BtnEditInfo = new ButtonInfo { Key = "Edit", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/EditFile.png" };
            BtnDeleteInfo = new ButtonInfo { Key = "Delete", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/DeleteFile.png" };
            BtnAddStepInfo = new ButtonInfo { Key = "AddStep", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/AddStep.png" };
            BtnDeleteStepInfo = new ButtonInfo { Key = "DeleteStep", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/DeleteStep.png" };
            BtnConfirmInfo = new ButtonInfo { Key = "OK", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/OK.png" };
            BtnCancelInfo = new ButtonInfo { Key = "Cancel", IsSelected = false, ImagePath = "pack://application:,,,/Resources/Icons/Cancel.png" };
        }
        private void Init()
        {
            //var init = new CleaningData();
            //var init1 = new CleaningData();
            //init.CleaningName = "clean1";
            //init1.CleaningName = "clean2";
            //CleaningDataList.Add(init);
            //CleaningDataList.Add(init1);
            _btnAddInfo.IsEnable = true;
            BtnEditInfo.IsEnable = true;
            BtnDeleteInfo.IsEnable = true;
            _btnAddStepInfo.IsEnable = true;
            _btnDeleteStepInfo.IsEnable = true;
            UpdateControlEnable();
            CleaningListInit();
        }
        private async void CleaningListInit()
        {
            var cleaningList = _cleaningRepository.GetAllItemsFromCache();
            var cleaningDataList = new ObservableCollection<CleaningData>();
            foreach (var item in cleaningList)
            {
                var cleaningData = SupportFunctions.LoadJsonFile<CleaningData>(CreateFileName(item.Name), _dataPath);
                if (cleaningData != null)
                {
                    cleaningData.EntityID = item.Id;
                    cleaningDataList.Add(cleaningData);
                }
                else
                {
                    await _cleaningRepository.DeleteAsync(item.Id);
                }
            }
            _cleaningDataList = cleaningDataList;
            SelectedCleaningDataInit();
        }
        private void SelectedCleaningDataInit()
        {
            if (_cleaningDataList.Count > 0)
            {
                SelectedCleaningData = _cleaningDataList[0];
            }
            else
            {
                var tempCleaningData = new CleaningData();
                tempCleaningData.Steps.Add(new CleaningStep($"Step 1"));
                SelectedCleaningData = tempCleaningData;
            }
        }

        private void UpdateCleaningStepView()
        {
            var cleaningStepViews = new List<CleaningStepView>();
            foreach (var item in _selectedCleaningData.Steps)
            {
                var cleaningStepView = new CleaningStepView();
                cleaningStepViews.Add(cleaningStepView.FromModel(item));
            }
            CleaningStepViews = new ObservableCollection<CleaningStepView>(cleaningStepViews);
            CleaningName = _selectedCleaningData.CleaningName;
        }


        #region ProcessCommand
        private void OnCancelCleaningPara(object? obj)
        {
            if (_isEditMode || _isAddMode)
            {
                _isAddMode = false;
                _isEditMode = false;
                UpdateControlEnable();
                SelectedCleaningDataInit();
            }
        }

        private void OnConfirmCleaningPara(object? obj)
        {
            if (!_isAddMode && !_isEditMode)
            {
                return;
            }
            if (_isEditMode)
            {
                _isEditMode = false;
                UpdateCleaningItem();
            }
            else if (_isAddMode)
            {
                _isAddMode = false;
                AddCleaningItem(_cleaningName);
            }
            SelectedCleaningDataInit();
            UpdateControlEnable();
        }

        private void OnDeleteStepCleaningPara(object? obj)
        {
            BtnDeleteStepInfo.IsSelected = true;

            if (CleaningStepViews.Count > _minStep)
            {
                CleaningStepViews.RemoveAt
                    (CleaningStepViews.Count - 1);
            }
            else
            {
                MessageUtils.ShowInfo($"Min Step {_minStep}");
            }

            BtnDeleteStepInfo.IsSelected = false;
        }

        private void OnAddStepCleaningPara(object? obj)
        {
            BtnAddStepInfo.IsSelected = true;

            if (CleaningStepViews.Count < _maxStep)
            {
                CleaningStepViews.Add(new CleaningStepView($"Step {CleaningStepViews.Count + 1}"));
            }
            else
            {
                MessageUtils.ShowInfo($"Max Step{_maxStep}");
            }

            BtnAddStepInfo.IsSelected = false;
        }

        private void OnDeleteCleaningPara(object? obj)
        {
            if (_cleaningDataList.Count <= 0)
            {
                return;
            }
            if (!IsAddEditEnable())
            {
                return;
            }
            string nameMsg="Cleaning";
            string confirmMsg = "{0} Id:{1}, Name:{2} \nDo you want to delete it? ";
            var result = MessageUtils.ShowQuestion(string.Format(confirmMsg,nameMsg, _selectedCleaningData.EntityID,_selectedCleaningData.CleaningName));
            if (result == MessageBoxResult.OK)
            {
                DeleteCleaningItem(_selectedCleaningData.EntityID);
                SelectedCleaningDataInit();
            }
        }

        private void OnEditCleaningPara(object? obj)
        {
            if (_cleaningDataList.Count <= 0)
            {
                return;
            }
            if (IsAddEditEnable())
            {
                _isEditMode = true;
                var cleaningData = _selectedCleaningData;
                SelectedCleaningData = cleaningData.DeepCloneObject();
                UpdateControlEnable();

            }
        }

        private void OnAddCleaningPara(object? obj)
        {
            if (IsAddEditEnable())
            {
                _isAddMode = true;
                var cleaningData = _selectedCleaningData;
                SelectedCleaningData = cleaningData.DeepCloneObject();
                UpdateControlEnable();
            }
        }

        #endregion
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

            IsEnableCleaningListRegion = !_isEditMode && !_isAddMode;
            IsEnableCleaningNameRegion = _isEditMode || _isAddMode;
            IsEnableCleaningStepRegion = _isEditMode || _isAddMode;

        }
        private bool IsAddEditEnable()
        {
            if (_isEditMode || _isAddMode)
            {
                return false;
            }
            return true;
        }

        private async void AddCleaningItem(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show($"Cleaning Name must be not null or empty.");
                return;
            }
            if (_cleaningDataList.Any(x => x.CleaningName == name))
            {
                MessageBox.Show($"{name} already exists.");
                return;
            }
            var cleaningItem = new CleaningItem()
            {
                Name = name
            };

            bool resultQuery = await _cleaningRepository.UpsertAsync(cleaningItem, true);

            if (!resultQuery)
            {
                throw new Exception("Add cleaning item unsuccessfully!");
            }

            CleaningData cleaningData = new CleaningData();
            cleaningData.EntityID = cleaningItem.Id;
            cleaningData.CleaningName = name;
            cleaningData.Steps = new List<CleaningStep>();
            foreach (var item in CleaningStepViews)
            {
                cleaningData.Steps.Add(item.ToModel());
            }
            cleaningData.OverPressure = _selectedCleaningData.OverPressure;
            cleaningData.Time = _selectedCleaningData.Time;
            SupportFunctions.SaveJsonFile(CreateFileName(name), _dataPath, cleaningData);
            _cleaningDataList.Add(cleaningData);
        }
        private void DeleteCleaningItem(object id)
        {
            CleaningData cleaningData = _selectedCleaningData;
            _cleaningDataList.Remove(cleaningData);
            _ = _cleaningRepository.DeleteAsync(id);
            SupportFunctions.DeleteFile(CreateFileName(cleaningData.CleaningName), _dataPath);
        }
        private async void UpdateCleaningItem()
        {
            CleaningData cleaningData = SelectedCleaningData;
            string oldName = cleaningData.CleaningName;
            string newName = _cleaningName;
            if (newName != oldName)
            {
                if (string.IsNullOrEmpty(newName))
                {
                    MessageBox.Show($"Cleaning Name is not null or empty.");
                    return;
                }
                if (_cleaningDataList.Any(x => x.CleaningName == newName))
                {
                    MessageBox.Show($"{newName} already exists.");
                    return;
                }
            }

            cleaningData.CleaningName = newName;
            cleaningData.Steps = new List<CleaningStep>();
            foreach (var item in CleaningStepViews)
            {
                cleaningData.Steps.Add(item.ToModel());
            }
            cleaningData.OverPressure = _selectedCleaningData.OverPressure;
            cleaningData.Time = _selectedCleaningData.Time;

            var cleaningItem = await _cleaningRepository.FindItem(oldName);
            cleaningItem!.Name = cleaningData.CleaningName;
            cleaningItem.RecipeName = "rcp1";
            cleaningItem.Operator = "test";
            var tempData = _cleaningDataList.FirstOrDefault(x => x.EntityID == cleaningData.EntityID);
            if (tempData != null)
            {
                tempData.CleaningName = cleaningData.CleaningName;
                tempData.Steps = cleaningData.Steps;
                tempData.OverPressure = cleaningData.OverPressure;
                tempData.Time = cleaningData.Time;
            }

            _ = _cleaningRepository.UpdateAsync(cleaningItem);
            SupportFunctions.DeleteFile(CreateFileName(oldName), _dataPath);
            SupportFunctions.SaveJsonFile(CreateFileName(cleaningData.CleaningName), _dataPath, cleaningData);
            CleaningDataList = new ObservableCollection<CleaningData>(_cleaningDataList);
        }
        private string CreateFileName(string fileName)
        {
            return $"{fileName}.pls";
        }
    }
}
