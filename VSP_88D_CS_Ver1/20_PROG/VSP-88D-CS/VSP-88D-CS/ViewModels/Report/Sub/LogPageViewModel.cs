using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Common.MVVM.ViewModels;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Common.Export;
using VSP_88D_CS.Models.Report;
using VSP_88D_CS.Views.Report.PopUp;
using VSP_88D_CS.Views.Setting.Sub;

namespace VSP_88D_CS.ViewModels.Report.Sub
{
    public enum eLOG_KIND { LOG_ALARM = 0, LOG_RUNTIME, LOG_MAINT, LOG_PRODUCT, LOG_CLEANING, SHIFT_PRODUCT, LOG_GUI, LOG_SEQ, MAX_LOG_KIND };
    public class LogPageViewModel : ViewModelBase
    {
        public LanguageService LanguageResources { get; }

        private string _tableName = "ReportLog";

        private readonly IRegionManager _regionManager;
        private readonly ReportRepository _reportRepository;

        #region COMMAND
        public ICommand SearchBtnCommand { get; }
        public ICommand SaveBtnCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand HowToCalcCommand { get; }
        #endregion COMMAND

        #region PROPERTY
        private ObservableCollection<ReportLog> _logDataModel;
        public ObservableCollection<ReportLog> LogDataModel
        {
            get => _logDataModel;
            set => SetProperty(ref _logDataModel, value);
        }
        private ObservableCollection<StatisticsModel> _viewStatistics;
        public ObservableCollection<StatisticsModel> ViewStatistics
        {
            get => _viewStatistics;
            set => SetProperty(ref _viewStatistics, value);
        }

        private int _typeSearch = -1;

        private bool _isSelectAllLog;
        public bool IsSelectAllLog
        {
            get => _isSelectAllLog;
            set
            {
                SetProperty(ref _isSelectAllLog, value);
                if (_isSelectAllLog)
                {
                    _typeSearch = 0;
                }

            }
        }
        private bool _isSelectAlarm;
        public bool IsSelectAlarm
        {
            get => _isSelectAlarm;
            set
            {
                SetProperty(ref _isSelectAlarm, value);
                if (_isSelectAlarm)
                {
                    _typeSearch = 1;
                }

            }
        }
        private bool _isSelectProduct;
        public bool IsSelectProduct
        {
            get => _isSelectProduct;
            set
            {
                SetProperty(ref _isSelectProduct, value);
                if (_isSelectProduct)
                {
                    _typeSearch = 2;
                }
            }
        }
        private bool _isSelectRunAlarm;
        public bool IsSelectRunAlarm
        {
            get => _isSelectRunAlarm;
            set
            {

                SetProperty(ref _isSelectRunAlarm, value);
                if (_isSelectRunAlarm)
                {
                    _typeSearch = 3;
                }
            }
        }

        private int _searchingAlarm;
        public int SearchingAlarm
        {
            get => _searchingAlarm;
            set
            {
                SetProperty(ref _searchingAlarm, value);
            }
        }

        private DateTime _fromDate;
        public DateTime FromDate
        {
            get => _fromDate;
            set
            {

                SetProperty(ref _fromDate, value);

            }
        }

        private DateTime _toDate;
        public DateTime ToDate
        {
            get => _toDate;
            set
            {

                SetProperty(ref _toDate, value);
            }
        }
        #endregion PROPERTY
        private DateTime? UpdateCombinedDateTime(DateTime? date, TimeSpan? time, ref DateTime? outDateTime)
        {
            DateTime? combine = null;
            if (date != null && time != null)
            {

                combine = date.Value.Date + time.Value;

            }
            else
            {
                combine = null;
            }
            outDateTime = combine;
            return combine;
        }
        public LogPageViewModel(IRegionManager regionManager, ReportRepository reportRepository)
        {
            //Load Language
            LanguageResources = LanguageService.GetInstance();

            _regionManager = regionManager;
            _reportRepository = reportRepository;
            FilterCommand = new RelayCommand<object>(OnFilter);
            SearchBtnCommand = new RelayCommand<object>(OnSearchBtn);
            SaveBtnCommand = new RelayCommand<object>(OnSaveBtn);
            HowToCalcCommand = new RelayCommand(OnHowToCalc);
            ViewStatistics = new ObservableCollection<StatisticsModel>()
            {
                new StatisticsModel() { LeftHeader = "RUN TIME",LeftValue="", RightHeader="MBTA\n(Mean Time Between Assists)",RightValue="" },
                new StatisticsModel() { LeftHeader = "MAINT TIME",LeftValue="", RightHeader="MTBF\n(Mean Time Between Failures)",RightValue="" },
                new StatisticsModel() { LeftHeader = "WAIT TIME",LeftValue="", RightHeader="MCBA\n(Mean Cycle Between Assists)",RightValue="" },
                new StatisticsModel() { LeftHeader = "ASSIST TIME",LeftValue="", RightHeader="MCBF\n(Mean Cycle Between Failures)",RightValue="" },
                new StatisticsModel() { LeftHeader = "FAILURE TIME",LeftValue="", RightHeader="SPH",RightValue="" },
                new StatisticsModel() { LeftHeader = "SHOT #",LeftValue="", RightHeader="",RightValue="-999" }
            };
            IsSelectAllLog = true;
            FromDate = DateTime.Now.AddDays(-1);
            ToDate = DateTime.Now;

        }

        private void OnHowToCalc()
        {
            if (VSContainer.Instance.Resolve(typeof(HowToStatistics)) is HowToStatistics howToStatistics)
            {
                howToStatistics.ShowDialog();
            }
        }

        private void OnSaveBtn(object obj)
        {
            if (LogDataModel != null && LogDataModel.Count > 0)
            {
                ExportFunctions.SaveDataGridToAlignedText(LogDataModel, string.Format("ReportLog_{0}.txt", DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")), true);
                //ExportFunctions.ExportExcel<ReportLog>(LogDataModel.ToList(), string.Format("ReportLog_{0}", DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")));
            }
        }

        private void OnSearchBtn(object obj)
        {
            var a = _fromDate;
            UpdateMTBAView();
            GetLogFromQuery(_fromDate.ToString("yyyy-MM-dd HH:mm:ss"), _toDate.ToString("yyyy-MM-dd HH:mm:ss"), false);
            //_reportRepository.AddReport(new Models.Report.ReportLog
            //{
            //    DateTime = "2025-12-1",
            //    Contents = "test2",
            //    Kind = 1,
            //    Interval = 1,
            //    WaitTime = 3

            //});
            //var a = _reportRepository.Data;
            // LogDataModel = new ObservableCollection<ReportLog>(_reportRepository.GetAll());
            //tempReportData = _reportRepository.GetReport("SELECT * FROM ReportDataModel");
        }

        private async void OnFilter(object obj)
        {
            var a = _typeSearch;
            var _button = obj as System.Windows.Controls.Button;

            object _btnName = _button.Content;
            switch (_btnName)
            {
                case "Confirm":


                    //  UpdateCombinedDateTime(_fromDate, _fromTime, ref _fromDateTime);
                    //   UpdateCombinedDateTime(_toDate, _toTime, ref _toDateTime);

                    string strFromDateTime, strToDateTime;
                    strFromDateTime = _fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                    strToDateTime = _toDate.ToString("yyyy-MM-dd HH:mm:ss");

                    bool resultQuery = await bGetTimeCountValueFromQuery(strFromDateTime, strToDateTime);

                    if (resultQuery)
                    {
                        GetLogFromQuery(strFromDateTime, strToDateTime, true);
                    }

                    //if (bGetTimeCountValueFromQuery("2025-05-05", "2026-05-05"))
                    //{
                    //    bGetLogFromQuery("2025-01-05", "2026-05-05", true);
                    //}
                    break;
                case "Cancel":

                    break;
                default:

                    break;
            };
        }
        //Function

        private long _assistCount, _failureCount, _product, _totalAssistTime, _totalMaintTime, _totalFailureTime, _totalWaitTime, _totalRunTime;

        private string GetQueryTimeCount()
        {
            string result = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SUM(Iif(([Interval] > 360) AND ([Kind] = {0}), [Interval], 0)) AS [FailureTime],", (int)eLOG_KIND.LOG_ALARM);
            sb.AppendFormat("SUM(Iif(([Interval] <= 360) AND ([Kind] = {0}), [Interval], 0)) AS [AssistTime],", (int)eLOG_KIND.LOG_ALARM);
            sb.AppendFormat("SUM(Iif(([Interval] > 0) AND ([Kind] = {0}), [Interval], 0)) AS [MaintTime],", (int)eLOG_KIND.LOG_MAINT);
            sb.AppendFormat("SUM(Iif(([Interval] > 0) AND ([Kind] = {0}), [Interval], 0)) AS [RunTime],", (int)eLOG_KIND.LOG_RUNTIME);
            sb.AppendFormat("SUM(Iif(([Interval] > 360) AND ([Kind] = {0}), 1, 0)) AS [FailureCount],", (int)eLOG_KIND.LOG_ALARM);
            sb.AppendFormat("SUM(Iif(([Interval] <= 360) AND ([Kind] = {0}), 1, 0)) AS [AssistCount],", (int)eLOG_KIND.LOG_ALARM);
            sb.AppendFormat("SUM(Iif(([Product] > 0), [Product], {0})) AS [ProductCount],", 0);
            sb.AppendFormat("SUM([WaitTime]) AS [SumWaitTime]");
            result = sb.ToString();
            return result;
        }
        private string GetQueryWhere(string timeStart, string timeEnd)
        {
            string result = "";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Where DateTime between '{0}' and '{1}'", timeStart, timeEnd);
            result = sb.ToString();
            return result;
        }
        private string GetQueryMsgCode()
        {
            string result = "";
            StringBuilder sb = new StringBuilder();
            //  sb.AppendFormat("SWITCH(([Kind] = {0}), ([Kind] & '(E.No.:' & [ErrNo] & ')'), ([Kind] <> 1), [Kind]) AS [ECode]", LOG_KIND.LOG_ALARM);

            sb.AppendFormat("CASE  WHEN [Kind] = '{0}' THEN ([Kind] || '(E.No.:' || [ErrNo] || ')') WHEN [Kind] <> 1 THEN [Kind] END AS [ECode]", (int)eLOG_KIND.LOG_ALARM);
            result = sb.ToString();
            return result;
        }
        private async Task<bool> bGetTimeCountValueFromQuery(string timeStart, string timeEnd)
        {
            bool bResult = false;
            string queryOption = GetQueryTimeCount();

            string queryWhere = GetQueryWhere(timeStart, timeEnd);
            var reportRecord = await _reportRepository.GetReportAsync<ReportTimeCount>(queryOption, queryWhere);
            if (reportRecord != null)
            {
                bResult = true;
                CalcMTBA(reportRecord);
            }
            return bResult;
        }
        private async void GetLogFromQuery(string timeStart, string timeEnd, bool bType)
        {
            //bool bResult = false;
            int typeSearch = 4; //default
            string errNo = "";
            if (bType)
            {
                typeSearch = _typeSearch;
            }
            else
            {
                errNo = SearchingAlarm.ToString();
            }
            string query = "SELECT [DateTime], [Contents], [ErrNo], [Interval], [WaitTime], ";
            query += GetQueryMsgCode();
            query += String.Format(" FROM(SELECT * FROM {0} ", _tableName);
            query += GetQueryWhere(timeStart, timeEnd);
            query += GetQueryType(typeSearch, errNo);
            query += ");";

            query = String.Format(" SELECT * FROM {0} ", _tableName);
            query += GetQueryWhere(timeStart, timeEnd);
            query += GetQueryType(typeSearch, errNo);
            query += "";
            var reportRecord = await _reportRepository.GetReportsAsync<ReportLog>(query);
            LogDataModel = new ObservableCollection<ReportLog>(reportRecord);
            //return bResult;
        }
        long _MTBA, _MTBF, _MCBA, _MCBF, _SPH;
        private void CalcMTBA(ReportTimeCount reportTimeCount)
        {
            //check type radio button !=0 and !=3 return todo make after
            _assistCount = reportTimeCount.AssistCount;
            _failureCount = reportTimeCount.FailureCount;
            _product = reportTimeCount.ProductCount;

            _totalFailureTime = reportTimeCount.FailureTime;
            _totalMaintTime = reportTimeCount.MaintTime;
            _totalRunTime = reportTimeCount.RunTime;
            _totalWaitTime = reportTimeCount.SumWaitTime;


            long totalRunTime = _totalMaintTime + _totalAssistTime + _totalFailureTime + _totalRunTime;
            long totalErrTime = _totalFailureTime + _totalAssistTime + _totalWaitTime;

            _MTBA = _assistCount > 0 ? totalRunTime / _assistCount : 0;
            _MTBF = _failureCount > 0 ? totalRunTime / _failureCount : 0;
            _MCBA = _assistCount > 0 ? _product / _assistCount : 0;
            _MCBF = _failureCount > 0 ? _product / _failureCount : 0;
            if (totalRunTime > 0)
            {
                _SPH = (_product * 3600) / totalRunTime;
            }

            long totalRun = reportTimeCount.MaintTime + reportTimeCount.AssistCount;
            UpdateMTBAView(_MTBA.ToString(), _MTBF.ToString(), _MCBA.ToString(), _MCBF.ToString(), _SPH.ToString(),false);
        }

        private void UpdateMTBAView(string strMTBA="", string strMTBF = "", string strMCBA = "", string strMCBF = "", string strSPH = "", bool bInit=true)        
        {
            string strRunTime, strMaintTime, strWaitTime, strAssistTime, strFailureTime, strShot;
            if (bInit)
            {
                strRunTime = "";
                strMaintTime = "";
                strWaitTime = "";
                strFailureTime = "";
                strAssistTime = "";
                strShot = "";
            }
            else
            {
                strRunTime = _totalRunTime.ToString();
                strMaintTime = _totalMaintTime.ToString();
                strWaitTime = _totalWaitTime.ToString();
                strFailureTime = _totalFailureTime.ToString();
                strAssistTime = _totalAssistTime.ToString();
                strShot = _product.ToString();
            }
            var tempStatistics = new ObservableCollection<StatisticsModel>()
            {
                new StatisticsModel() { LeftHeader = "RUN TIME",LeftValue=strRunTime, RightHeader="MBTA\n(Mean Time Between Assists)",RightValue=strMTBA },
                new StatisticsModel() { LeftHeader = "MAINT TIME",LeftValue=strMaintTime, RightHeader="MTBF\n(Mean Time Between Failures)",RightValue=strMTBF },
                new StatisticsModel() { LeftHeader = "WAIT TIME",LeftValue=strWaitTime, RightHeader="MCBA\n(Mean Cycle Between Assists)",RightValue=strMCBA },
                new StatisticsModel() { LeftHeader = "ASSIST TIME",LeftValue=strAssistTime, RightHeader="MCBF\n(Mean Cycle Between Failures)",RightValue=strMCBF },
                new StatisticsModel() { LeftHeader = "FAILURE TIME",LeftValue=strFailureTime, RightHeader="SPH",RightValue= strSPH},
                new StatisticsModel() { LeftHeader = "SHOT #",LeftValue=strShot, RightHeader="",RightValue="-999" }
            };
            ViewStatistics = tempStatistics;


        }

        private string GetQueryType(int type, string errNo = "")
        {
            string strResult = "";
            string strTemp = "";
            switch (type)
            {
                case 0:
                    strTemp = " ORDER BY DateTime ASC";
                    break;
                case 1:
                    strTemp = string.Format(" and (Kind = {0}) ORDER BY DateTime ASC", (int)eLOG_KIND.LOG_ALARM);
                    break;
                case 2:
                    strTemp = string.Format(" and ((Kind = {0}) or (Kind = {1}) ) ORDER BY DateTime ASC", (int)eLOG_KIND.LOG_PRODUCT, (int)eLOG_KIND.SHIFT_PRODUCT);
                    break;

                case 3:
                    strTemp = string.Format(" and ((Kind = {0}) or (Kind = {1}) ) ORDER BY DateTime ASC", (int)eLOG_KIND.LOG_ALARM, (int)eLOG_KIND.LOG_RUNTIME);
                    break;
                case 4:

                    strTemp = string.Format(" and ((Kind = {0}) and (ErrNo = {1}) ) ORDER BY DateTime ASC", (int)eLOG_KIND.LOG_ALARM, errNo);
                    break;

                default:
                    break;
            }
            strResult = strTemp;

            return strResult;
        }

    }
}
