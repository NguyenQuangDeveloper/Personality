using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Main.Control
{
    public class RadioButtonItem : ViewModelBase
    {
        public string Name { get; }
        public int Value { get; }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        public RadioButtonItem(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
