using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Models.Report;

namespace VSP_88D_CS.ViewModels.Report.PopUp
{
    public class HowToStatisticsViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }

        private readonly IRegionManager _regionManager;
        public ICommand ClosingCommand { get; }

        public HowToStatisticsViewModel(IRegionManager regionManager)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _regionManager = regionManager;
            ClosingCommand = new RelayCommand(OnClosing);
            InitData();
        }

        private void OnClosing()
        {
            Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(w => w.IsActive)?.Hide();
        }

        public ObservableCollection<StatisticDefinition> DataStatisticDefinitions { get; set; }
        public ObservableCollection<StatisticDefinition> DataCalcDefinitions { get; set; }
        private void InitData()
        {
            List<StatisticDefinition> statisticDefinitions = new List<StatisticDefinition>
            {
                new StatisticDefinition()
                {
                  DataName="RUNNING TIME",
                  Definition= "Time from line auto start to stop"
                },
                new StatisticDefinition()
                {
                  DataName="ERROR TIME",
                  Definition= "Time from error occurs to the system becomes statnd-by Error in maintenance mode is invalid.\n " +
                  "If the mode is invalid. If the mode is switched to maintenance mode, the time when switching \n" +
                  "happened is considered as the error ending time."                
                },
                new StatisticDefinition()
                {
                  DataName="WAITING TIME",
                  Definition= "Time from errors to key input by operator"
                },
                new StatisticDefinition()
                {
                  DataName="ASSIST TIME",
                  Definition= "The value less than 6 minutes obtained by deducting WAITING TIME from ERROR TIME."
                },
                new StatisticDefinition()
                {
                  DataName="FAILURE TIME",
                  Definition= "The value more than 6 minutes obtained by deducting WAITING TIME from ERROR TIME."
                },
                new StatisticDefinition()
                {
                  DataName="SHOT NUMBER",
                  Definition= "Shot count."
                },
                new StatisticDefinition()
                {
                  DataName="RUN TIME",
                  Definition= "RUNNING TIME + FAILURE TIME + ASSIST TIME + MAINTENANCE TIME"
                }
           };
            DataStatisticDefinitions= new ObservableCollection<StatisticDefinition>(statisticDefinitions);

            List<StatisticDefinition> dataCalcDefinitions = new List<StatisticDefinition>
            {
                new StatisticDefinition()
                {
                  Title="MTBA(Mean Time Between Assists)",
                  DataName="Average value of mean time(productive time) between assists.",
                  Definition= "MTBA"
                },
                new StatisticDefinition()
                {
                  Title="MTBF(Mean Time Between Failures)",
                  DataName="Average value of mean time(productive time) between assists.",
                  Definition= "MTBF"
                },
                new StatisticDefinition()
                {
                  Title="MCBA(Mean Cycle Between Assists)",
                  DataName="Mean value of cycle count (shot count) between assists.",
                  Definition= "MCBA"
                },
                new StatisticDefinition()
                {
                  Title="MCBF(Mean Cycle Between Failures)",
                  DataName="Mean value of cycle count (shot count) between Failures.",
                  Definition= "MCBF"
                },
                new StatisticDefinition()
                {
                  Title="SPH(Strips Per Hour)",
                  DataName="Total strip count for run-time.",
                  Definition= "SPH"
                }
              
           };
            DataCalcDefinitions= new ObservableCollection<StatisticDefinition>(dataCalcDefinitions);
        }
    }
}
