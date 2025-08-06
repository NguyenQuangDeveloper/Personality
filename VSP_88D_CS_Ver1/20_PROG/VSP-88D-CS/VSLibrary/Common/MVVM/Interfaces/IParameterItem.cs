namespace VSLibrary.Common.MVVM.Interfaces;

/// <summary>
/// 파라미터 모델이 가져야 할 최소 속성 정의
/// </summary>
public interface IParameterItem
{
    string Section { get; }
    string Key { get; }
    object Value { get; }
}
