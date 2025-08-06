using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Views.Auto.Sub;

namespace VSP_88D_CS.ViewModels.Auto.Sub
{
    public class SelectJobViewModel : ViewModelBase
    {
        private SelectRecipe _selectRecipe;
        public LanguageService LanguageResources { get; }

        #region PROPERTY
        private string _nameSelectedRecipe;
        public string NameSelectedRecipe
        {
            get => _nameSelectedRecipe;
            set => SetProperty(ref _nameSelectedRecipe, value);
        }
        private string _nameSelectDevice;
        public string NameSelectDevice
        {
            get => _nameSelectDevice;
            set => SetProperty(ref _nameSelectDevice, value);
        }
        #endregion PROPERTY

        #region COMMAND
        public ICommand SelectRecipeShowCommand { get; set; }
        public ICommand StartJobCommand { get; set; }
        public ICommand StopJobCommand { get; set; }
        #endregion COMMAND

        #region EXECUTE COMMAND
        void SelectRecipeShow()
        {
            _selectRecipe.ShowDialog();
        }
        void StartJob()
        {

        }
        void StopJob()
        {

        }
        #endregion EXECUTE COMMAND

        public SelectJobViewModel(SelectRecipe selectRecipeView)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _selectRecipe = selectRecipeView;
            SelectRecipeShowCommand = new RelayCommand(SelectRecipeShow);
            StartJobCommand = new RelayCommand(StartJob);
            StopJobCommand = new RelayCommand(StopJob);
        }
    }
}
