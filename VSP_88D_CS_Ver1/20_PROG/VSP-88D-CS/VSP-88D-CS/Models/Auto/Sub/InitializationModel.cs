using CommunityToolkit.Mvvm.ComponentModel;

namespace VSP_88D_CS.Models.Sub
{
    public partial class InitializationModel : ObservableObject
	{
		[ObservableProperty]
		private int equipmentIdx;

        [ObservableProperty]
        private string equipmentName;

        [ObservableProperty]
        private bool isChecked;

        [ObservableProperty]
        private double progressValue;

        [ObservableProperty]
        private string? stepDisplay;

        [ObservableProperty]
        private string progressPer;

        partial void OnProgressValueChanged(double value)
        {
            ProgressPer = $"{value}%";
        }
    }
}
