using ChamberControl;
using ConfigurationLib.Implementations;
using ConfigurationLib.Interfaces;
using ConfigurationLib.Services;
using LoggerLib.Executes;
using LoggerLib.Interfaces;
using LoggerLib.Types;
using SequenceEngine.Bases;
using SequenceEngine.Manager;
using System.IO;
using System.Reflection;
using System.Windows;
using UserAccessLib.Common.Interfaces;
using VSLibrary.Common.MVVM.Core;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Controller;
using VSLibrary.Database;
using VSLibrary.Threading;
using VSLibrary.UIComponent.Localization;
using VSP_88D_CS.Common;
using VSP_88D_CS.Common.Auth;
using VSP_88D_CS.Common.Database;
using VSP_88D_CS.Common.Device;
using VSP_88D_CS.Common.Helpers;
using VSP_88D_CS.CONTROLLER.DigitalIO;
using VSP_88D_CS.Sequence.Profiles.Buffers;
using VSP_88D_CS.Sequence.Profiles.Elevators;
using VSP_88D_CS.Sequence.Profiles.IndexPushers;
using VSP_88D_CS.Sequence.Profiles.Inspect;
using VSP_88D_CS.Sequence.Profiles.LoadPusher;
using VSP_88D_CS.Sequence.Profiles.Plasma;
using VSP_88D_CS.Views;
namespace VSP_88D_CS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILoggingService _logger;
        protected VSContainer _vsContainer => VSContainer.Instance;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var lstStr = AlarmHelper.GenerateAlarmCodeFromExcel("alarm_list.xlsx");

            RegisterServices();
            //var seeder = vsContainer.Resolve<DataSeeder>();
            // seeder.Seed();

            RegisterModule();
            //LoadDevice();
            ShowMainView();
            LoggerEngine.Application.LoggerEngine.Start("VSP_88D_CS", "D:\\VSP-88D-CS\\LOG");
        }

        /// <summary>
        /// New method using VSLibrary
        /// </summary>
        private void RegisterServices()
        {
            var configProvider = new ConfigProviderFactory();
            _vsContainer.RegisterInstance<IConfigProviderFactory>(configProvider);

            var configService = new ConfigService(configProvider);
            _vsContainer.RegisterInstance<IConfigService>(configService);

            ////Initialize global variables
            IGlobalSystemOption globalSystemOption = new GlobalSystemOption();
            _vsContainer.RegisterInstance<IGlobalSystemOption>(globalSystemOption);

            var globalData = new VS_GLOBAL_DATA();
            _vsContainer.RegisterInstance<VS_GLOBAL_DATA>(globalData);

            RegisterDatabase(globalSystemOption);

            _vsContainer.RegisterInstance<VSContainer>(_vsContainer);
            _vsContainer.RegisterInstance<IRegionManager>(_vsContainer.RegionManager);
            _vsContainer.Register<IAuthService, AuthService>();
            //vsContainer.Register<DataSeeder, DataSeeder>();

            RegisterLogger();
            ThreadManager.SetContainer(_vsContainer);
            RegisterDeviceControllers();
            RegisterSequences();

            _vsContainer.AutoInitialize(Assembly.GetExecutingAssembly());
            RegisterLanguage();
        }

        private void RegisterSequences()
        {
            var container = VSContainer.Instance;

            SequenceManager sequenceManager = new SequenceManager();
            sequenceManager.AddModule(new List<ISequenceModule> { 
                new SeqLoadBuffer(), 
                new SeqUnLoadBuffer(),
                new SeqLoadElevator(),
                new SeqUnloadElevator(),
                new SeqLoadIndexPusher(),
                new SeqUnloadIndexPusher(),
                new SeqInspect(),
                new SeqLoadingPusher(),
                new SeqPlasma() });

            container.RegisterInstance(sequenceManager);

            container.AutoInitialize(Assembly.GetExecutingAssembly());
            UIInitializer.RegisterServices(container);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Release Resources
            UserRepository.Release();
            RecipeRepository.Release();
            DeviceRepository.Release();
            CleaningRepository.Release();
            ReportRepository.Release();

            // Close devices

            // Terminate running threads
            _logger.LogWarning("Program Exit!!!");
            LoggerEngine.Application.LoggerEngine.Stop("VSP_88D_CS");

            var sequence = VSContainer.Instance.Resolve<SequenceManager>();

            if (sequence != null)
            {
                sequence.Stop();
                sequence.Disposable();
            }
        }

        private void RegisterLanguage()
        {
            var languageService = LanguageService.GetInstance();
            _vsContainer.RegisterInstance<LanguageService>(languageService);
            var proxy = StaticLocalizationProxy.Instance;
            Current.Resources["LangProxy"] = proxy;
            VsLocalizationManager.Load(LanguageType.English);
        }

        private void RegisterLogger()
        {
            var loggingServiceFactory = new LoggingServiceFactory();
            _vsContainer.RegisterInstance<ILoggingService>(loggingServiceFactory.Create(LoggingType.NLog));
            _logger = _vsContainer.Resolve<ILoggingService>();
            _logger.Prefix = "App";
            _logger.LogInfo("Program start...");
        }

        /// <summary>
        /// TODO: REGISTER DEVICE CONTROLLER
        /// </summary>
        private void RegisterDeviceControllers()
        {
            var container = VSContainer.Instance;

            List<ControllerType> controllerList = new List<ControllerType>
            {
                ControllerType.AIO_AjinAXT,
                ControllerType.AIO_Adlink,
                ControllerType.DIO_Comizoa,
                ControllerType.DIO_AjinAXT,
                ControllerType.Motion_AjinAXT,
            };

            var deviceList = new DeviceList();
            container.RegisterInstance(deviceList);

            var controllerManager = new ControllerManager(container, controllerList);
            container.RegisterInstance<ControllerManager>(controllerManager);
            controllerManager.SetIOlist(deviceList.IOSettings);
            controllerManager.SetAxislist(deviceList.AxisSettings);
            DefinedDio.Initialize(controllerManager);
        }

        private void RegisterDatabase(IGlobalSystemOption globalSystemOption)
        {
            string databasePath = Path.Combine(globalSystemOption.DataPath, "VSP_88D.db");
            string connectionString = $"Data Source={databasePath}";
            var dbManager = new DBManager(DatabaseProvider.SQLite, connectionString);
            UserRepository.AutoLoad(dbManager);
            RecipeRepository.AutoLoad(dbManager);
            DeviceRepository.AutoLoad(dbManager);
            CleaningRepository.AutoLoad(dbManager);
            ReportRepository.AutoLoad(dbManager);
            _vsContainer.RegisterInstance<DBManager>(dbManager);
            _vsContainer.RegisterInstance<UserRepository>(UserRepository.Instance);
            _vsContainer.RegisterInstance<RecipeRepository>(RecipeRepository.Instance);
            _vsContainer.RegisterInstance<DeviceRepository>(DeviceRepository.Instance);
            _vsContainer.RegisterInstance<CleaningRepository>(CleaningRepository.Instance);
            _vsContainer.RegisterInstance<ReportRepository>(ReportRepository.Instance);
        }
        private void ShowMainView()
        {
            if (_vsContainer.Resolve(typeof(Main)) is Main mainView)
            {
                mainView.Show();
            }
        }

        private void RegisterModule()
        {
            ChamberModule module = new ChamberModule();
            module.RegisterServices(VSContainer.Instance);
        }
    }

}
