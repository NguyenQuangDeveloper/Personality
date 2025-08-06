/*!
 * \mainpage VSLibrary.Controller
 *
 * \section intro 소개
 * FA 제어 장비에 사용되는 Motion, DIO, AIO 라이브러리를 모듈화하여 관리하기 위한 구조입니다.
 * 로컬 코드 베이스를 직접 분석하여 모든 클래스·인터페이스·열거형을 포함했습니다.
 *
 * \section namespace 네임스페이스
 *
 * ```
 * VSLibrary.Controller
 * ```
 *
 * \section classes 구성 클래스
 *
 * - **AnalogIO**
 *   - `ADLinkAIO`           : Adlink DASK 기반 아날로그 I/O 구현 (`AnalogIO/ADLinkAIO.cs`)
 *   - `AjinAxtAIO`          : Ajin Axt 보드용 아날로그 I/O 구현 (`AnalogIO/AjinAxtAIO.cs`)
 * - **DigitalIO**
 *   - `AjinAxtDIO`          : Ajin Axt 보드용 디지털 I/O 구현 (`DigitalIO/AjinAxtDIO.cs`)
 *   - `ComizoaDIO`          : Comizoa 보드용 디지털 I/O 구현 (`DigitalIO/ComizoaDIO.cs`)
 * - **Motion**
 *   - `AxlMotion`           : Adlink Dask Axl 모션 컨트롤러 래퍼 (`Motion/Ajin/AxlMotion.cs`)
 *   - `AxtMotion`           : Ajin CAxtCAMCFS20 모션 컨트롤러 래퍼 (`Motion/Ajin/AxtMotion.cs`)
 *   - `ComizoaMotion`       : Comizoa 모션 컨트롤러 구현 (`Motion/ComizoaMotion.cs`)
 * - **매니저 및 유틸리티**
 *   - `ControllerManager`   : DI 및 라이브러리 초기화 매니저 (`ControllerManager.cs`)
 *   - `LibraryInitializer`  : 정적 라이브러리 초기화 헬퍼 (`ControllerManager.cs`)
 *
 * \section base 추상 베이스 클래스
 *
 * - `AIOBase`               : `IAIOBase` 기반 공통 아날로그 I/O 추상 클래스
 * - `AIODataBase`           : `ViewModelBase, IAnalogIOData` 구현용 추상 데이터 클래스
 * - `DIOBase`               : `IDIOBase` 기반 공통 디지털 I/O 추상 클래스
 * - `DIODataBase`           : `ViewModelBase, IDigitalIOData` 구현용 추상 데이터 클래스
 * - `MotionBase`            : `IMotionBase` 기반 공통 모션 추상 클래스
 * - `AxisDataBase`          : `ViewModelBase, IAxisData` 구현용 축 데이터 추상 클래스
 *
 * \section data 데이터 모델
 *
 * - `AIOData`               : 아날로그 I/O 단일 채널 데이터 모델
 * - `DIOData`               : 디지털 I/O 단일 채널 데이터 모델
 * - `AxtAxisData`           : Ajin 축 데이터 모델
 *
 * \section interface 공통 인터페이스
 *
 * - `IAIOBase`              : 아날로그 I/O 기본 인터페이스
 * - `IIOSettinglist`        : I/O 설정 리스트 인터페이스
 * - `IAnalogIOData`         : 아날로그 I/O 데이터 읽기/쓰기 인터페이스
 * - `IDIOBase`              : 디지털 I/O 기본 인터페이스
 * - `IDigitalIOData`        : 디지털 I/O 데이터 읽기/쓰기 인터페이스
 * - `IMotionBase`           : 모션 컨트롤러 기본 인터페이스
 * - `IAxisSettinglist`      : 축 설정 리스트 인터페이스
 * - `IAxisData`             : 축 데이터 읽기/쓰기 인터페이스
 *
 * \section enum 열거형
 *
 * - `IOType`                : I/O 타입 구분 (`INPut`, `OUTPut`)
 * - `ControllerType`        : 컨트롤러 장치 종류 구분
 *
 * \section usage 사용 예시
 *
 * \code{.cs}
 * public partial class DioPageViewModel
 * {
 *     public ObservableCollection<IDigitalIOData> DIOList { get; } = new();
 *
 *     private ControllerManager _controllerManager;
 *     public DioPageViewModel(ControllerManager controllerManager)
 *     {
 *         _controllerManager = controllerManager;
 *
 *         foreach (var dio in _controllerManager.DIOData.Values)
 *             DIOList.Add(dio);
 *
 *         // 출력 채널 Y001을 On
 *         _controllerManager.DIOData["Y001"].On();
 *     }
 * }
 *
 * // App.xaml.cs 초기화 예시
 * List<ControllerType> controllerList = new List<ControllerType>
 * {
 *     ControllerType.AIO_AjinAXT,
 *     ControllerType.AIO_Adlink,
 *     ControllerType.DIO_AjinAXT,
 *     ControllerType.DIO_Comizoa,
 *     ControllerType.Motion_AjinAXT,
 * };
 *
 * var controllerManager = new ControllerManager(_vsContainer, controllerList);
 * _vsContainer.RegisterInstance<ControllerManager>(controllerManager);
 * \endcode
 *
 * \section module 모듈 분리 기준
 *
 * - **Ajin 계열**   : `AnalogIO/AjinAxtAIO.cs`, `DigitalIO/AjinAxtDIO.cs`, `Motion/Ajin/` 내부
 * - **Adlink 계열** : `AnalogIO/ADLinkAIO.cs`, `Motion/Ajin/AxlMotion.cs`
 * - **Comizoa 계열**: `DigitalIO/ComizoaDIO.cs`, `Motion/ComizoaMotion.cs`
 * - **공통 계층**   : `ControllerInterface.cs`, `ControllerBase.cs`, `ControllerENUM.cs`, `ControllerData.cs`, `ControllerManager.cs`
 *
 * \section version 버전 관리
 *
 * \subsection v2025_06_30 2025-06-30 Ver 1.0.0 (By. 서상덕)
 * - 초기 릴리즈: ControllerManager, 인터페이스, 데이터 모델, Concrete 구현 클래스 포함
 *
 * \section license 라이선스
 *
 * 이 프로젝트는 내부 기업용으로만 사용되며, 외부 배포를 금지합니다.
 *
 * \section contact 문의
 *
 * Email: sdseo@visionsemicon.co.kr
 */
