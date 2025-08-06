using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;

namespace VSP_88D_CS.Models.Setting
{
    public class TowerLampItem 
    {

        //LanguageRepository _languageRepository;
        private string _item;
        private string _red;
        private string _yellow;
        private string _green;
        private string _buzzer;

        public string Item 
        {
            get { return _item; }
        }
        public string Red 
        {
            get { return _red; } 
            set { _red = value; }
        }
        public string Yellow
        {
            get { return _yellow; }
            set { _yellow = value; }
        }
        public string Green
        {
            get { return _green; }
            set { _green = value; }
        }
        public string Buzzer
        {
            get { return _buzzer; }
            set { _buzzer = value; }
        }

        public TowerLampItem(string item, string red, string yellow, string green, string buzzer)
        {
            _item = item;
            _red = red; 
            _yellow = yellow;   
            _green = green;
            _buzzer = buzzer;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        //public string Item
        //{
        //    get => _item;
        //    set => SetProperty(ref _item, value);
        //}

        //public string Red
        //{
        //    get => _red;
        //    set => SetProperty(ref _red, value);    
        //}

        //public string Yellow
        //{
        //    get => _yellow;
        //    set => SetProperty(ref _yellow, value);
        //}

        //public string Green
        //{
        //    get => _green; 
        //    set => SetProperty(ref _green, value);
        //}

        //public string Buzzer
        //{
        //    get => _buzzer; 
        //    set => SetProperty(ref _buzzer, value);
        //}
    }
}
