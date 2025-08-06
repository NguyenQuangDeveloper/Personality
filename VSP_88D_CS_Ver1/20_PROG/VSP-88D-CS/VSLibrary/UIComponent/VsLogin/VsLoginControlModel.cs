using System.ComponentModel.DataAnnotations.Schema;
using VSLibrary.Database;

namespace VSLibrary.UIComponent.VsLogin;

/// <summary>
/// 사용자 로그인용 데이터 모델 클래스입니다.
/// </summary>
[Table("UserTbl")] // 선택 사항: 테이블 이름 지정
public class UserItem
{
    /// <summary>
    /// 사용자 ID (Primary Key)
    /// </summary>
    [PrimaryKey]
    public string UserID { get; set; } = string.Empty;

    /// <summary>
    /// 이름
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 권한 등급
    /// </summary>
    public int Grade { get; set; } = 0;

    /// <summary>
    /// 비밀번호
    /// </summary>
    public string Password { get; set; } = string.Empty;   

    /// <summary>
    /// 사번 또는 구분번호
    /// </summary>
    public string Department { get; set; } = "";   
}