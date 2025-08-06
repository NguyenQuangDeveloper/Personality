using System.Windows.Controls;
using VSP_88D_CS.Models.Recipe;
using VSP_88D_CS.ViewModels.Setting.Sub;

namespace VSP_88D_CS.Views.Setting.Sub;

/// <summary>
/// Interaction logic for RecipePage.xaml
/// </summary>
public partial class RecipePage : UserControl
{
    public RecipePage()
    {
        InitializeComponent();
    }

    private void VsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (this.DataContext is RecipePageViewModel viewModel && sender is DataGrid dataGrid)
        {
            try
            {
                viewModel.SelectedRecipes = new(dataGrid.SelectedItems.Cast<RecipeData>().ToList());
            }
            catch (Exception)
            {
                viewModel.SelectedRecipes = new();
            }
        }
    }
}
