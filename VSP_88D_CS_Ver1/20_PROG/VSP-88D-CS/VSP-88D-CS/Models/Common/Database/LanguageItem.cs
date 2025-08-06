using VSLibrary.Database;

namespace VSP_88D_CS.Models.Common.Database;

public class LanguageItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }
    public string Kor { get; set; }
    public string Eng { get; set; }
    public string Use1 { get; set; }
    public string Use2 { get; set; }
}
