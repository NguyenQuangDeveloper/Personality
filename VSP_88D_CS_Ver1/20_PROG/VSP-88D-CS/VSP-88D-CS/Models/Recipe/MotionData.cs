namespace VSP_88D_CS.Models.Recipe
{
    public class MotionData
    {
        public bool IsUpDown { get; set; }
        public bool IsLeftRight { get; set; }
        public string MotionName {  get; set; }
        public ServoState ServoState { get; set; }
        public List<MotionParameter> MotionParameters { get; set; }
    }
}
