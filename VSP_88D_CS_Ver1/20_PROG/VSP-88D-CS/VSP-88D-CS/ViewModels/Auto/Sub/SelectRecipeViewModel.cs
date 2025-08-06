using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Models.Recipe;

namespace VSP_88D_CS.ViewModels.Auto.Sub
{
    public class SelectRecipeViewModel : ViewModelBase
    {
        private readonly RecipeRepository _recipeRepository;
        public LanguageService LanguageResources { get; }

        #region PROPERTY
        public ObservableCollection<RecipeData> Recipes { get; set; } = new();
        private RecipeData _selectedRecipe;
        public RecipeData SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (_selectedRecipe != null) _selectedRecipe.IsSelected = false;
                _selectedRecipe = value;
                if (_selectedRecipe != null)
                {
                    _selectedRecipe.IsSelected = true;
                    OnPropertyChanged(nameof(SelectedRecipe));
                    SelectedRecipeName = _selectedRecipe?.Recipe ?? "No Recipe Selected";
                }
            }
        }
        private string _selectedRecipeName;
        public string SelectedRecipeName
        {
            get => _selectedRecipeName;
            set
            {
                if (_selectedRecipeName != value)
                {
                    _selectedRecipeName = value;
                    OnPropertyChanged(nameof(SelectedRecipeName));
                }
            }
        }
        private string _recipeName;
        public string RecipeName
        {
            get => _recipeName;
            set => SetProperty(ref _recipeName, value);
        }
        #endregion PROPERTY

        #region COMMAND
        public ICommand SelectRecipeCommand { get; set; }
        public ICommand CancelSelectCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        void SelectRecipe()
        {
            //TODO: update to SelectJobView
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }
        void CancelSelect()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }
        #endregion EXECUTE COMMAND

        public SelectRecipeViewModel(RecipeRepository recipeRepository)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _recipeRepository = recipeRepository;
            SelectRecipeCommand = new RelayCommand(SelectRecipe);
            CancelSelectCommand = new RelayCommand(CancelSelect);
            UpdateListRecipe();
        }
        void UpdateListRecipe()
        {
            var recipes = _recipeRepository.GetAllRecipesFromCache();
            foreach (var recipe in recipes)
                Recipes.Add(new(recipe));

            SelectedRecipe = Recipes.FirstOrDefault(r => r.IsSelected)!;
        }
    }
}
