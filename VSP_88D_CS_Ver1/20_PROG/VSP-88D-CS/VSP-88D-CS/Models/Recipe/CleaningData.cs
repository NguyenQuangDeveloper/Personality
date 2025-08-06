using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common.Helpers;

namespace VSP_88D_CS.Models.Recipe;

public class CleaningData : ViewModelBase
{
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public int EntityID { get; set; }
    public string CleaningName {  get; set; }
    public double OverPressure { get; set; }
    public int Time { get; set; }
    public List<CleaningStep> Steps { get; set; }
    public CleaningData()
    {
        CleaningName = string.Empty;
        OverPressure = 0.2;
        Time = 2;
        Steps = new List<CleaningStep>();
        //Steps.Add(new CleaningStep());
    }

    public CleaningData(CleaningItem cleaningItem)
    {
        this.EntityID = cleaningItem.Id;
        this.CleaningName = cleaningItem.Name;
    }

    public CleaningItem ToEntity()
    {
        return new()
        {
            Id = this.EntityID,
            Name = CleaningName,
            CleaningSteps = Steps.SafeSerializeJSON()
        };
    }
}
