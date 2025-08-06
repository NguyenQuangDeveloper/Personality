namespace VSLibrary.Common.MVVM.Interfaces;

/// <summary>
/// 모션 파라미터 항목 인터페이스입니다.
/// 축 번호, 이름, 위치, 속도, 가속도 등의 정보를 제공합니다.
/// </summary>
public interface IMotionParametersItem
{
    /// <summary>
    /// 고유 인덱스 값 (DB 저장용)
    /// </summary>
    int idx { get; set; }

    /// <summary>
    /// 축 번호
    /// </summary>
    int AxisNo { get; set; }

    /// <summary>
    /// 축 이름
    /// </summary>
    string AxisName { get; set; }

    /// <summary>
    /// 축 동작 설명
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// 목표 위치
    /// </summary>
    double Pos { get; set; }

    /// <summary>
    /// 이동 속도
    /// </summary>
    double Vel { get; set; }

    /// <summary>
    /// 가속도
    /// </summary>
    double Acc { get; set; }
}
