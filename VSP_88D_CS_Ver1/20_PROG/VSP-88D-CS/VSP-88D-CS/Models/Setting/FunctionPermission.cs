namespace VSP_88D_CS.Models.Setting
{
    public enum eFunctionItem1
    {
        PARA,
        Setup,
        IOTest,
        Log,
        ExitFunction,
        RecipeSelect,
        ChamberValvePumpFunction,
        OfflineLocalRemote,
        CIMSetFunction,
        AlarmReset,
        ParameterFunction,
        RegisterUser
    }
    public enum eFunctionItem
    {
        ResetFunction,
        InitialFunction,
        ReportFunction,
        ManualFunction,
        ExitFunction,
        SettingFunction,
        RecipeFunction,
        CleaningFunction,
        DeviceFunction,
        ParameterFunction,      
        RegisterUsers,
        LidOpenFunction,
        LidCloseFunction,
        ManualCleanFunction,
        RecipeSelect,
        CIMSetFunction,
        OfflineLocalRemote,
        SequenceMonitor,
        ChamberValvePumpFunction,
        LeakageTestSet,
        GeneratorPumpTimerReset,
        CounterReset

    }
    public class FunctionPermission
    {
        public eFunctionItem KeyItem { get; set; }
        public string Title { get; set; }
        public bool Level1 { get; set; } = false;
        public bool Level2 { get; set; } = false;
        public bool Level3 { get; set; } = false;
        public bool AllowAll { get; set; } = false;
    }
}
