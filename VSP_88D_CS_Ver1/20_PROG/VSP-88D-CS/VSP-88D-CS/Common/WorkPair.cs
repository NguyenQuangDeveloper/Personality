namespace VSP_88D_CS.Common
{
    public class WorkPair
    {
        public int Id1 {  get; set; }
        public int Id2 { get; set; }
        public string Alias1 {  get; set; }=string.Empty;
        public string Alias2 { get; set; } = string.Empty;
        public string Display => Id2 == -1 ? $"{Alias1}" : $"{Alias1}, {Alias2}";
    }
}
