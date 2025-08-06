using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using VSLibrary.Database;

namespace VSP_88D_CS.Models.Recipe;

/// <summary>
/// A data model class that represents each record in the Recipe table.
/// </summary>
[Table(nameof(RecipeItem))] // 선택 사항: 테이블 이름 지정
public class RecipeItem
{
    /// <summary>
    /// Unique ID for the recipe record.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// The recipe name.
    /// </summary>
    public string Recipe { get; set; }

    /// <summary>
    /// Here are the cleaning instructions for the recipe.
    /// </summary>
    public string Cleaning { get; set; }

    /// <summary>
    /// Represents device information associated with the recipe.
    /// </summary>
    public string Device { get; set; }
}
