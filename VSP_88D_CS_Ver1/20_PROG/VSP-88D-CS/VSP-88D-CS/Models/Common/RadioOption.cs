using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Common
{
    public class RadioOption : ViewModelBase
    {
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;

                OnPropertyChanged(nameof(IsChecked));

            }
        }
        private bool isChecked;
        public string OptionName { get; set; }
        public int OptionId { get; set; }
    }
}
