using System.Windows;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Models.Common;

namespace VSP_88D_CS.ViewModels.Setting.PopUp
{
    public class LevelAliasSettingViewModel : ViewModelBase
    {
        string _dataPath;
        private readonly IGlobalSystemOption _globalSystemOption;
        List<RadioOption> _radioOptions;

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        private string _level1Option="";

        public string Level1Option
        {
            get { return _level1Option; }
            set { _level1Option = value; }
        }

        private string _level2Option="";

        public string Level2Option
        {
            get { return _level2Option; }
            set { _level2Option = value; }
        }
        private string _level3Option = "";

        public string Level3Option
        {
            get { return _level3Option; }
            set { _level3Option = value; }
        }

        public LevelAliasSettingViewModel(IGlobalSystemOption globalSystemOption)
        {
            _globalSystemOption = globalSystemOption;
            _dataPath= _globalSystemOption.DataPath;
            ConfirmCommand = new RelayCommand<object>(OnConfirm);
            CancelCommand = new RelayCommand<object>(OnCancel);
            LoadLevelOptions();
        }
        private void LoadLevelOptions()
        {
           
            string fileLevel = $"{_dataPath}/{CreateFileName("LevelOptions")}";
         
            _radioOptions = SupportFunctions.LoadJsonFile<List<RadioOption>>(fileLevel);

            Level1Option = _radioOptions[0].OptionName;
            Level2Option = _radioOptions[1].OptionName;
            Level3Option = _radioOptions[2].OptionName;
        }
        private void OnCancel(object obj)
        {
            CloseForm();
        }

        private void OnConfirm(object obj)
        {
          
            string fileLevel = $"{_dataPath}/{CreateFileName("LevelOptions")}";
            _radioOptions[0].OptionName = Level1Option;
            _radioOptions[1].OptionName = Level2Option;
            _radioOptions[2].OptionName = Level3Option;

            SupportFunctions.SaveJsonFile(fileLevel, _radioOptions);
            CloseForm();
        }

        private string CreateFileName(string fileName)
        {
            return $"{fileName}.lvl";
        }

        private void CloseForm()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }
    }
}
