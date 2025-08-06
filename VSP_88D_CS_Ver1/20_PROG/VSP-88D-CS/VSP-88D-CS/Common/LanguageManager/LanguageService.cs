using System.ComponentModel;
using System.IO;
using System.Text.Json;

namespace VSP_88D_CS.Common
{
    public class LanguageService : INotifyPropertyChanged
    {
        private static LanguageService _instances = new();
        private Dictionary<string, string> _languageResource = new();
        private EnumLanguage _currentLanguage;
        private string LanguageFile;
        public string this[string key] => _languageResource.ContainsKey(key) ? _languageResource[key] : key;
        public event PropertyChangedEventHandler PropertyChanged;
        public EnumLanguage CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    LoadLanguage(_currentLanguage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentLanguage)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
                }
            }
        }

        public static LanguageService GetInstance()
        {
            if (_instances == null)
            {
                _instances = new LanguageService();
            }
            return _instances;
        }

        private LanguageService()
        {
            _currentLanguage = EnumLanguage.English;
            LoadLanguage(_currentLanguage);
        }

        private void LoadLanguage(EnumLanguage SelectedLanguage)
        {
            string fileLanguage = $"{SelectedLanguage}.json";
            string languagePath = Path.Combine("D:", "VSP-88D-CS", "DATA", "Language");
            LanguageFile = Path.Combine(languagePath, fileLanguage);
            _languageResource.Clear();

            if (!File.Exists(LanguageFile))
            {
                //MessageBox.Show("LANGUAGE CONFIGURATION FILE HAS BEEN DELETED OR DOES NOT EXIST!", "NORTIFICATIONS", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string jsonContent = File.ReadAllText(LanguageFile);
            _languageResource = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }
}
