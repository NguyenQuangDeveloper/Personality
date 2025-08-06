using System.ComponentModel.DataAnnotations.Schema;
using VSLibrary.Common.MVVM.ViewModels;

namespace VSP_88D_CS.Models.Device
{
    /// <summary>
    /// IOSetting 테이블의 데이터를 표현하는 클래스입니다.
    /// </summary>
    public class VSIOSettingItem : ViewModelBase/*, IDigitalIOData*/
    {
        //인터페이스 공통 사용
        //#####################################
        //[PrimaryKey]
        //[AutoIncrement]
        //public int Id { get; set; }

        //public ControllerDIOType _controller = ControllerDIOType.Ajin;
        //[IgnoreColumn]
        //[NotMapped]
        //public ControllerDIOType IOController
        //{
        //    get => _controller;
        //    set => _controller = value;
        //}

        //public ControllerMotionType _motionController = ControllerMotionType.Ajin;
        //[IgnoreColumn]
        //[NotMapped]
        //public ControllerMotionType MotionController
        //{
        //    get => _motionController;
        //    set => _motionController = value;
        //}

        //public IOType _iOType;
        //public IOType IOType
        //{
        //    get => _iOType;
        //    set => _iOType = value;
        //}

        //[IgnoreColumn][NotMapped] public short AxisNo { get; set; }
        //public string WireName { get; set; }    //와이어번호
        //public string EmName { get; set; }    //IO 이름

        //private string _strdataName;
        //public string StrdataName
        //{
        //    get => _strdataName;
        //    set => SetProperty(ref _strdataName, value);
        //}
        //[IgnoreColumn][NotMapped] public string ModuleName { get; set; } //모듈의 이름 (예: SIO_DO32P)
        //[IgnoreColumn][NotMapped] public int IOIndex { get; set; } //모듈의 ID (특정 모듈을 식별하기 위한 ID)
        ////public bool Value { get; set; } = false;   //IO상태
        //[IgnoreColumn][NotMapped] public bool PollingState { get; set; }    //폴링 상태의 IO : 하이나로우 엣지와 감지시간이 걸렸을때 폴링 이후 데이터 
        //public bool StateReversal { get; set; } = false;
        //public int Offset { get; set; } //입력 또는 출력의 오프셋 (특정 비트를 지정하는 값)
        //[IgnoreColumn][NotMapped] public bool Edge { get; set; } = false;//하이엣지 로우엣지감지
        //[IgnoreColumn][NotMapped] public int DetectionTime { get; set; } = 0;//감지시간

        //private bool _value = false;
        ///// <summary>
        ///// 화면에 표시될 데이터
        ///// </summary>
        //[IgnoreColumn]
        //[NotMapped] // Entity Framework에서 제외
        //public bool Value
        //{
        //    get => _value;
        //    set => SetProperty(ref _value, value);
        //}
    }
}
