namespace VSP_88D_CS.Models.Report;

public class ChartModel
{
    public DateTime Time { get; set; } 
    public double RfFwd { get; set; } 
    public double RfRef { get;set; }

    public double Vacuum {  get; set; }
    public double Gas1 {  get; set; }
    public double Gas2 { get; set; }
    public double Gas3 { get; set; }
    public double Gas4 { get; set; }

}
