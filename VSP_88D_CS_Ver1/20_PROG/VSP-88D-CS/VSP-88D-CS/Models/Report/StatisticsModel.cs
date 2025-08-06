using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Report
{
    public class StatisticsModel :ViewModelBase
    {
        private string _leftHeader;
        public string LeftHeader
        {

            get { return _leftHeader; }
            set
            {
                _leftHeader = value;
                OnPropertyChanged();
            }
        }
        private string _leftValue;
        public string LeftValue
        {

            get { return _leftValue; }
            set
            {
                _leftValue = value;
                OnPropertyChanged();
            }
        }
        private string _rightHeader;
        public string RightHeader
        {

            get { return _rightHeader; }
            set
            {
                _rightHeader = value;
                OnPropertyChanged();
            }
        }
        private string _rightValue;
        public string RightValue
        {

            get { return _rightValue; }
            set
            {
                _rightValue = value;
                OnPropertyChanged();
            }
        }
    }
}
