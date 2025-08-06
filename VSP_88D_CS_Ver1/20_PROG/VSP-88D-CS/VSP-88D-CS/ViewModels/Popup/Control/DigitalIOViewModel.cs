using System.Collections.ObjectModel;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.ViewModels;
using VSLibrary.Controller;
using VSP_88D_CS.Models.Main.Control;
using VSP_88D_CS.ViewModels.Setting.Sub;

namespace VSP_88D_CS.ViewModels.Popup.Control
{
    public class DigitalIOViewModel : ViewModelBase
    {
        public string InputText { get; set; } = "Input";
        public string OutputText { get; set; } = "Output";
        public string SelectChannelText { get; set; } = "Selected Channel";
        public string LegendONText { get; set; } = "ON";
        public string LegendOFFText { get; set; } = "OFF";
        public string MotionText { get; set; } = "MOTION";

        public ObservableCollection<RadioButtonItem> InputRadioButtons { get; set; }
        public ObservableCollection<RadioButtonItem> OutputRadioButtons { get; set; }
        public ICommand InputSelectedCommand { get; }
        public ICommand OutputSelectedCommand { get; }
        public ObservableCollection<IDigitalIOData> InputItems { get; set; } = new ObservableCollection<IDigitalIOData>();
        public ObservableCollection<IDigitalIOData> OutputItems { get; set; } = new ObservableCollection<IDigitalIOData>();

        public ICommand ToggleItemSelectedCommand { get; }

        //private readonly DigitalIOCtrlManager _digitalIOManager;

        private string _selectLanguage = "";

        private bool _isMXMode;
        public bool IsMXMode
        {
            get => _isMXMode;
            set
            {
                if (SetProperty(ref _isMXMode, value))
                {
                    GenerateRadioButtons(); // 체크 상태가 변경될 때 라디오 버튼 목록을 갱신
                    OnInputSelected(_inputSelect);
                    OnOutputSelected(_outputSelect);
                }
            }
        }

        private int _inputSelect;

        public int InputSelect
        {
            get => _inputSelect;
            set
            {
                SetProperty(ref _inputSelect, value);
                OnInputSelected(value.ToString());
                if (!_selectLanguage.Equals(""))
                {
                    OnLanguageChanged(_selectLanguage);
                }
            }
        }

        public void OnInputSelected(string select)
        {
            if (!int.TryParse(select, out int selectedPage) || selectedPage < 0)
                return;

            InputItems.Clear();

            int skipCount = selectedPage * 16;
            IEnumerable<IDigitalIOData> source;

            //TODO: APPLY VSLIBRARY FRAMEWORK
            //if (IsMXMode)
            //{
            //    // MX 모드: WireName이 "MX"로 시작하는 InPut만
            //    source = _digitalIOManager.DigitalIODataDictionary.Values
            //        .Where(item => item.IOType == IOType.InPut && item.WireName.StartsWith("MX"));
            //}
            //else
            //{
            //    // 일반 모드: WireName이 "X"로 시작하는 InPut만
            //    source = _digitalIOManager.DigitalIODataDictionary.Values
            //        .Where(item => item.IOType == IOType.InPut && item.WireName.StartsWith("X"));
            //}

            //var pagedItems = source.Skip(skipCount).Take(16);

            //foreach (var item in pagedItems)
            //{
            //    InputItems.Add(item);
            //}
        }

        private int _outputSelect;
        public int OutputSelect
        {
            get => _outputSelect;
            set
            {
                SetProperty(ref _outputSelect, value);
                OnOutputSelected(value.ToString()); // 출력 선택 시 처리 로직 실행
                if (!_selectLanguage.Equals(""))
                {
                    OnLanguageChanged(_selectLanguage);
                }
            }
        }

        public void OnOutputSelected(string select)
        {
            if (!int.TryParse(select, out int selectedPage) || selectedPage < 0)
                return;

            OutputItems.Clear();

            int skipCount = selectedPage * 16;
            IEnumerable<IDigitalIOData> source;

            //TODO: APPLY VSLIBRARY FRAMEWORK
            //if (IsMXMode)
            //{
            //    // MX 모드: WireName이 "MY"로 시작하는 OUTPut만
            //    //source = _digitalIOManager.DigitalIODataDictionary.Values
            //    //    .Where(item => item.IOType == IOType.OUTPut && item.WireName.StartsWith("MY"));

            //    source = _digitalIOManager.DigitalIODataDictionary.Values
            //      .Where(item => item.IOType == IOType.OUTPut && item.WireName.StartsWith("Y"));
            //}
            //else
            //{
            //    // 일반 모드: WireName이 "Y"로 시작하는 OUTPut만
            //    source = _digitalIOManager.DigitalIODataDictionary.Values
            //        .Where(item => item.IOType == IOType.OUTPut && item.WireName.StartsWith("Y"));
            //}

            //var pagedItems = source.Skip(skipCount).Take(16);

            //foreach (var item in pagedItems)
            //{
            //    OutputItems.Add(item);
            //}
        }

        private void OnLanguageChanged(string languageKey)
        {
            _selectLanguage = languageKey;

            //ChangeLanguage(languageKey);

            try
            {
                foreach (IDigitalIOData item in OutputItems)
                {
                 //   var langItem = _languageRepository.Data
                 //.FirstOrDefault(x =>
                 //    x.Kor == item.StrdataName ||
                 //    x.Eng == item.StrdataName ||
                 //    x.Use1 == item.StrdataName ||
                 //    x.Use2 == item.StrdataName);

                 //   if (langItem != null)
                 //   {
                 //       item.StrdataName = languageKey switch
                 //       {
                 //           "Kor" => langItem.Kor,
                 //           "Eng" => langItem.Eng,
                 //           "Use1" => langItem.Use1,
                 //           "Use2" => langItem.Use2,
                 //           _ => item.StrdataName
                 //       };
                 //   }
                }
                foreach (IDigitalIOData item in InputItems)
                {
                //    var langItem = _languageRepository.Data
                //.FirstOrDefault(x =>
                //    x.Kor == item.StrdataName ||
                //    x.Eng == item.StrdataName ||
                //    x.Use1 == item.StrdataName ||
                //    x.Use2 == item.StrdataName);

                //    if (langItem != null)
                //    {
                //        item.StrdataName = languageKey switch
                //        {
                //            "Kor" => langItem.Kor,
                //            "Eng" => langItem.Eng,
                //            "Use1" => langItem.Use1,
                //            "Use2" => langItem.Use2,
                //            _ => item.StrdataName
                //        };
                //    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public DigitalIOViewModel(/*DigitalIOCtrlManager dioController, */ParameterPageViewModel para) 
        {
            //_digitalIOManager = dioController;

            //para.LanguageChanged += OnLanguageChanged;

            // 라디오 버튼 동적 생성
            InputRadioButtons = new ObservableCollection<RadioButtonItem>();
            OutputRadioButtons = new ObservableCollection<RadioButtonItem>();

            GenerateRadioButtons();

            //TODO: APPLY VS LIBRARY FRAMEWORK
            //ParameterItem lang = para.ParameterItems.FirstOrDefault(item => item.Section == "SYSTEM" && item.Key == "LANG");
            //long index = Convert.ToInt32(lang.Value);
            //string languageKey = index switch
            //{
            //    0 => "Kor",
            //    1 => "Eng",
            //    2 => "Use1",
            //    3 => "Use2",
            //    _ => "Eng"
            //};
            //ChangeLanguage(languageKey);

            ToggleItemSelectedCommand = new RelayCommand<IDigitalIOData>(OnToggleItemSelected);
            InputSelectedCommand = new RelayCommand<int>(OnInputSelected);
            OutputSelectedCommand = new RelayCommand<int>(OnOutputSelected);

            //TODO: APPLY VS LIBRARY FRAMEWORK
            //foreach (IDigitalIOData item in _digitalIOManager.DigitalIODataDictionary.Values)
            //{
            //    if (item.IOType == IOType.InPut)
            //        InputItems.Add(item);
            //    else if (item.IOType == IOType.OUTPut)
            //        OutputItems.Add(item);
            //}

            InputSelect = 0;
            OutputSelect = 0;
        }

        private void OnToggleItemSelected(IDigitalIOData item)
        {
            if (item == null)
                return;

            //if (item.IOType == IOType.OUTPut)
            //    _digitalIOManager.WriteBit(item, !item.Value);
        }

        /// <summary>
        /// 동적으로 Input 및 Output Radio 버튼을 생성
        /// </summary>
        private void GenerateRadioButtons()
        {
            string inputPrefix, outputPrefix;
            int totalInputCount, totalOutputCount;

            //TODO: APPLY VS LIBRARY FRAMEWORK
            //if (IsMXMode)
            //{
            //    inputPrefix = "MX";
            //    outputPrefix = "Y";
            //    totalInputCount = _digitalIOManager.DigitalIODataDictionary.Values
            //        .Count(x => x.IOType == IOType.InPut && x.WireName.StartsWith("MX"));
            //    //totalOutputCount = _digitalIOManager.DigitalIODataDictionary.Values
            //    //    .Count(x => x.IOType == IOType.OUTPut && x.WireName.StartsWith("MY"));
            //    totalOutputCount = _digitalIOManager.DigitalIODataDictionary.Values
            //       .Count(x => x.IOType == IOType.OUTPut && x.WireName.StartsWith("Y"));
            //}
            //else
            //{
            //    inputPrefix = "X";
            //    outputPrefix = "Y";
            //    totalInputCount = _digitalIOManager.DigitalIODataDictionary.Values
            //        .Count(x => x.IOType == IOType.InPut && x.WireName.StartsWith("X"));
            //    totalOutputCount = _digitalIOManager.DigitalIODataDictionary.Values
            //        .Count(x => x.IOType == IOType.OUTPut && x.WireName.StartsWith("Y"));
            //}

            //int inputPages = (int)Math.Ceiling(totalInputCount / 16.0);
            //int outputPages = (int)Math.Ceiling(totalOutputCount / 16.0);

            //InputRadioButtons.Clear();
            //OutputRadioButtons.Clear();

            //for (int i = 0; i < inputPages; i++)
            //{
            //    InputRadioButtons.Add(new RadioButtonItem(
            //        $"{inputPrefix}{i * 16:X3} ~ {inputPrefix}{(i * 16 + 15):X3}", i)
            //    {
            //        IsChecked = (i == InputSelect)
            //    });
            //}

            //for (int i = 0; i < outputPages; i++)
            //{
            //    OutputRadioButtons.Add(new RadioButtonItem(
            //        $"{outputPrefix}{i * 16:X3} ~ {outputPrefix}{(i * 16 + 15):X3}", i)
            //    {
            //        IsChecked = (i == OutputSelect)
            //    });
            //}
        }

        private void OnInputSelected(int select)
        {
            InputSelect = select;
            foreach (var btn in InputRadioButtons)
                btn.IsChecked = (btn.Value == select);
        }

        private void OnOutputSelected(int select)
        {
            OutputSelect = select;
            foreach (var btn in OutputRadioButtons)
                btn.IsChecked = (btn.Value == select);
        }
    }
}
