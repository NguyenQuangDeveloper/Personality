using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Models.Main.Control;

namespace VSP_88D_CS.ViewModels.Auto.Sub
{
    public class StateViewModel : ViewModelBase
    {
        bool use5Lane;
        public LanguageService LanguageResources { get; }
        public ObservableCollection<DataItem> Items { get; set; } = new();

        public ICommand SharedCommand { get; }
        public ObservableCollection<object> DataGridValues { get; set; } = new();

        public StateViewModel()
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            DataItem Lane1 = new DataItem()
            {
                Loading = "Empty",
                Chamber = "Empty",
                Unloading = "Empty"
            };
            DataItem Lane2 = new DataItem()
            {
                Loading = "Empty",
                Chamber = "Empty",
                Unloading = "Empty"
            };
            DataItem Lane3 = new DataItem()
            {
                Loading = "Empty",
                Chamber = "Empty",
                Unloading = "Empty"
            };
            DataItem Lane4 = new DataItem()
            {
                Loading = "Empty",
                Chamber = "Empty",
                Unloading = "Empty"
            };
            Items.Add(Lane1);
            Items.Add(Lane2);
            Items.Add(Lane3);
            Items.Add(Lane4);
            //TEST option
            use5Lane = true;
            if (use5Lane)
            {
                DataItem Lane5 = new DataItem()
                {
                    Loading = "Empty",
                    Chamber = "Empty",
                    Unloading = "Empty"
                };
                Items.Add(Lane5);
            }    
            UpdateCell(2, "Loading", "Empty", Brushes.White, Brushes.Black);
            UpdateCell(0, "Chamber", "Working", Brushes.Yellow, Brushes.Black);
            UpdateCell(1, "Unloading", "Done", Brushes.Green, Brushes.White);
        }
        public void UpdateCell(int rowIndex, string columnName, string newText, Brush background, Brush foreground)
        {
            if (rowIndex >= 0 && rowIndex < Items.Count)
            {
                var item = Items[rowIndex];
                switch (columnName)
                {
                    case "Loading":
                        item.Loading = newText;
                        item.LoadingBackground = background;
                        item.LoadingForeground = foreground;
                        break;
                    case "Chamber":
                        item.Chamber = newText;
                        item.ChamberBackground = background;
                        item.ChamberForeground = foreground;
                        break;
                    case "Unloading":
                        item.Unloading = newText;
                        item.UnloadingBackground = background;
                        item.UnloadingForeground = foreground;
                        break;
                }
                OnPropertyChanged(nameof(Items));
            }
        }
    }
}
