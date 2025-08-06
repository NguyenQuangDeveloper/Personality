using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Common.Helpers;
using VSP_88D_CS.Models.Recipe;

namespace VSP_88D_CS.ViewModels.Setting.Sub;

public class RecipePageViewModel : ViewModelBase
{
    public LanguageService LanguageResources { get; }
    private readonly RecipeRepository _recipeRepository;
    private readonly DeviceRepository _deviceRepository;
    private readonly CleaningRepository _cleaningRepository;

    private bool _isAdd;
    public bool IsAdd
    {
        get => _isAdd;
        set => SetProperty(ref _isAdd, value);
    }

    private bool _isEdit;
    public bool IsEdit
    {
        get => _isEdit;
        set => SetProperty(ref _isEdit, value);
    }

    private string? _recipeName;
    public string? RecipeName
    {
        get => _recipeName;
        set => SetProperty(ref _recipeName, value);
    }

    private RecipeData? _selectedRecipe;
    public RecipeData? SelectedRecipe
    {
        get => _selectedRecipe;
        set
        {
            SetProperty(ref _selectedRecipe, value);

            RecipeDatas.ToList().ForEach(x => x.IsSelected = false);
            CleaningDatas.ToList().ForEach(x => x.IsSelected = false);
            if (value == null)
            {
                RecipeName = string.Empty;
                return;
            }

            RecipeName = value.Recipe;
            value.IsSelected = true;
            SelectedCleaning = CleaningDatas.FirstOrDefault(x => x.CleaningName == value.Cleaning.CleaningName)!;
            SelectedDevice = DeviceDatas.FirstOrDefault(x => x.DeviceName == value.Device.DeviceName)!;
        }
    }


    private ObservableCollection<RecipeData> _selectedRecipes;
    public ObservableCollection<RecipeData> SelectedRecipes
    {
        get => _selectedRecipes;
        set => SetProperty(ref _selectedRecipes, value);
    }

    private CleaningData? _selectedCleaning;
    public CleaningData? SelectedCleaning
    {
        get => _selectedCleaning;
        set
        {
            SetProperty(ref _selectedCleaning, value);
            CleaningDatas.ToList().ForEach(x => x.IsSelected = false);
            if (value == null)
                return;
            value.IsSelected = true;
        }
    }

    private DeviceData? _selectedDevice;
    public DeviceData? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            SetProperty(ref _selectedDevice, value);

            DeviceDatas.ToList().ForEach(x => x.IsSelected = false);
            if (value == null)
                return;
            value.IsSelected = true;
        }
    }

    public ObservableCollection<RecipeData> RecipeDatas { get; set; } = new();
    public ObservableCollection<CleaningData> CleaningDatas { get; set; } = new();
    public ObservableCollection<DeviceData> DeviceDatas { get; set; } = new();
    public ObservableCollection<RecipeData> UndoRecipes { get; set; } = new();

    public bool IsElementEnabled => RecipeDatas.Count > 0;

    public ICommand OnButtonClickCommand { get; }

    public RecipePageViewModel(RecipeRepository recipeRepository, DeviceRepository deviceRepository, CleaningRepository cleaningRepository)
    {
        LanguageResources = LanguageService.GetInstance();
        _recipeRepository = recipeRepository;
        _deviceRepository = deviceRepository;
        _cleaningRepository = cleaningRepository;

        LoadData();

        RecipeDatas.CollectionChanged += RecipeDatas_CollectionChanged;

        OnButtonClickCommand = new RelayCommand<object>(OnButtonClick);
    }

    private void RecipeDatas_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(IsElementEnabled));
        //OnPropertyChanged(nameof(SelectedRecipe));
    }

    private void OnButtonClick(object? obj)
    {
        switch (obj)
        {
            case "Add":
                {
                    UndoRecipes = RecipeDatas.DeepCloneObject();
                    IsAdd = true;
                    break;
                }
            case "Edit":
                {
                    UndoRecipes = RecipeDatas.DeepCloneObject();
                    IsEdit = true;
                    break;
                }
            case "Delete":
                {
                    DeleteRecipe();
                    break;
                }
            case "Save":
                {
                    SaveRecipe();
                    break;
                }
            case "Cancel":
                {
                    CancelEdit();
                    break;
                }

            default:
                break;
        }
    }

    private void CancelEdit()
    {
        RecipeDatas = UndoRecipes.DeepCloneObject();
        OnPropertyChanged(nameof(RecipeDatas));

        SelectedRecipe = RecipeDatas.FirstOrDefault();
        IsAdd = IsEdit = false;
    }

    private void SaveRecipe()
    {
        if (IsAdd)
        {
            InsertRecipe();
        }
        else if (IsEdit)
        {
            UpdateRecipe();
        }
    }

    private async void InsertRecipe()
    {
        if (!ValidatedAdd())
        {
            return;
        }

        var recipe = new RecipeItem();
        recipe.Recipe = RecipeName!;

        var cleaningData = (CleaningDatas.FirstOrDefault(x => x.IsSelected == true) ?? new());
        recipe.Cleaning = cleaningData.ToEntity().SafeSerializeJSON();

        var device = (DeviceDatas.FirstOrDefault(x => x.IsSelected == true) ?? new());
        recipe.Device = device.ToEntity().SafeSerializeJSON();

        var entityID = await _recipeRepository.InsertGetIdAsync(recipe);
        recipe.Id = entityID;
        _recipeRepository.RecipeCache[recipe.Id.ToString()] = recipe;

        RecipeDatas.Add(new(recipe));
        SelectedRecipe = RecipeDatas.Last();

        IsAdd = false;
    }

    private bool ValidatedAdd()
    {
        if (string.IsNullOrWhiteSpace(RecipeName))
        {
            MessageUtils.ShowWarning($"Please enter a recipe name!");
            return false;
        }

        if (RecipeDatas.Any(x => x.Recipe == RecipeName))
        {
            MessageUtils.ShowWarning($"The reciple [{RecipeName}] already exists!");
            return false;
        }

        return true;
    }

    private void UpdateRecipe()
    {
        if (!ValidatedUpdate())
            return;

        SelectedRecipe!.Recipe = RecipeName!;
        var cleaningData = (CleaningDatas.FirstOrDefault(x => x.IsSelected == true) ?? new());
        SelectedRecipe.Cleaning = cleaningData;

        var device = (DeviceDatas.FirstOrDefault(x => x.IsSelected == true) ?? new());
        SelectedRecipe.Device = device;

        _ = _recipeRepository.UpdateAsync(SelectedRecipe.ToEntity());
        _recipeRepository.RecipeCache[SelectedRecipe.EntityID.ToString()] = SelectedRecipe.ToEntity();

        IsEdit = false;
    }

    private bool ValidatedUpdate()
    {
        if (SelectedRecipe == null)
        {
            MessageUtils.ShowWarning($"Please select a recipe!");
            return false;
        }

        if (string.IsNullOrWhiteSpace(RecipeName))
        {
            MessageUtils.ShowWarning($"Please enter a recipe name!");
            return false;
        }

        if (SelectedRecipe.Recipe != RecipeName && RecipeDatas.Any(x => x.Recipe == RecipeName))
        {
            MessageUtils.ShowWarning($"The reciple [{RecipeName}] already exists!");
            return false;
        }

        return true;
    }

    private void DeleteRecipe()
    {
        if (!SelectedRecipes.Any())
            return;

        foreach (var item in SelectedRecipes)
        {
            _ = _recipeRepository.DeleteRecipeAsync(item.ToEntity());
            RecipeDatas = new(RecipeDatas.Where(x => x.EntityID != item.EntityID));
            OnPropertyChanged(nameof(RecipeDatas));
            SelectedRecipe = RecipeDatas.FirstOrDefault();
        }
    }

    private void LoadData()
    {
        var recipes = _recipeRepository.GetAllRecipesFromCache();
        foreach (var recipe in recipes)
        {
            RecipeDatas.Add(new(recipe));
        }

        var devices = _deviceRepository.GetAllDevicesFromCache();
        foreach (var device in devices)
        {
            DeviceDatas.Add(new(device));
        }

        var cleanings = _cleaningRepository.GetAllItemsFromCache();
        foreach (var item in cleanings)
        {
            CleaningDatas.Add(new(item));
        }

        SelectedRecipe = RecipeDatas.FirstOrDefault();
    }
}
