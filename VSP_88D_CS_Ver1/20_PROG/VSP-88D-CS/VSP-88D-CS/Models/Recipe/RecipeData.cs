using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Common.Helpers;

namespace VSP_88D_CS.Models.Recipe;

public class RecipeData : ViewModelBase
{
    private int _entityID;
    public int EntityID
    {
        get => _entityID;
        set => SetProperty(ref _entityID, value);
    }

    private string _recipe;
    public string Recipe
    {
        get => _recipe;
        set => SetProperty(ref _recipe, value);
    }

    private CleaningData _cleaning;
    public CleaningData Cleaning
    {
        get => _cleaning;
        set => SetProperty(ref _cleaning, value);
    }

    private DeviceData _device;
    public DeviceData Device
    {
        get => _device;
        set => SetProperty(ref _device, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public RecipeData()
    { }

    public RecipeData(RecipeItem recipeItem)
    {
        this.EntityID = recipeItem.Id;
        this.Recipe = recipeItem.Recipe;
        this.Cleaning = new(JsonHelper.SafeDeserializeJSON<CleaningItem>(recipeItem.Cleaning));
        this.Device = new(JsonHelper.SafeDeserializeJSON<DeviceItem>(recipeItem.Device));
    }

    public RecipeItem ToEntity()
    {
        return new()
        {
            Id = this.EntityID,
            Recipe = this.Recipe,
            Cleaning = JsonHelper.SafeSerializeJSON(this.Cleaning.ToEntity()),
            Device = JsonHelper.SafeSerializeJSON(this.Device.ToEntity())
        };
    }
}
