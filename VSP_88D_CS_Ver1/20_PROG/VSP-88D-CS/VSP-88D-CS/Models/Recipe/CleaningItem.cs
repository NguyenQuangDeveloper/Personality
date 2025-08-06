using System.ComponentModel.DataAnnotations.Schema;
using VSLibrary.Database;

namespace VSP_88D_CS.Models.Recipe;

/// <summary>
/// A data model class representing the data in the CleaningItem table.
/// </summary>
[Table(nameof(CleaningItem))] // 선택 사항: 테이블 이름 지정
public class CleaningItem
{
    /// <summary>
    /// Unique ID for the CleaningItem record.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }

    public string CleaningSteps { get; set; }

    /// <summary>
    /// The date the CleaningItem record was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The date the CleaningItem record was modified.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// The name of the Recipe associated with Cleaning.
    /// </summary>
    public string RecipeName { get; set; }

    /// <summary>
    /// The name of the worker who created the cleaning.
    /// </summary>
    public string Operator { get; set; }
}
