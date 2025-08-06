namespace VSP_88D_CS.Models.Recipe
{
    public class ServoState
    {
        public bool IsServoOn {  get; set; }
        public bool IsHomeDone { get; set; }
        public bool IsAlarmOn { get; set; }
        public bool IsPositiveLimitOn { get; set; }
        public bool IsNegativeLimitOn { get; set; }
        public bool IsOrigin { get; set; }
        public ServoState()
        {
            IsServoOn = false;
            IsHomeDone = false;
            IsAlarmOn = false;
            IsPositiveLimitOn = false;
            IsNegativeLimitOn = false;
            IsOrigin = false;
        }
    }
}
