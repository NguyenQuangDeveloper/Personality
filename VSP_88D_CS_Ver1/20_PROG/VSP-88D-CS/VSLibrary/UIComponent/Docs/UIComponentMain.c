/*!
 * \mainpage VSLibrary.Common.Ini
 *
 * \section intro 소개
 *
 * INI 설정 파일을 로드하고, Key-Value 방식으로 접근 가능한 구성 관리 시스템입니다.
 * 단독 사용 또는 DI 방식 모두 지원되며, 다양한 시스템 설정을 구조화하여 관리할 수 있습니다.
 * 내부적으로 Dictionary 기반 저장소를 통해 빠른 조회 및 수정이 가능하며,
 * VsIniManager 정적 클래스를 통해 간편하게 접근할 수 있습니다.
 *
 * \section namespace 네임스페이스
 *
 * ```
 * VSLibrary.Common.Ini
 * ```
 *
 * \section classes 구성 클래스
 *
 * - `IIniProvider`         : 설정 제공자 인터페이스
 * - `IniBase`              : 설정 공급자 베이스 클래스 (공통 Dictionary 로직 제공)
 * - `IIniManager`          : 설정 관리 인터페이스
 * - `VsIniManagerProxy`    : 실제 INI 파일 처리 로직 (Load/Save/Get/Set 등)
 * - `VsIniManager`         : 설정 시스템의 정적 진입점 (외부 호출용 Wrapper)
 * - `IniEnum`              : 섹션/키/파일 경로 등을 Enum으로 구성하기 위한 보조 파일
 *
 * \section usage 기본 사용법
 *
 * \code{.cs}
 * // 1. 초기화
 * VsIniManager.Initialize(@"D:\\Config\\app.ini");
 *
 * // 2. 문자열 값 가져오기
 * string timeout = VsIniManager.Get("SYSTEM", "TIMEOUT");
 *
 * // 3. 리스트 값 가져오기
 * var langs = VsIniManager.GetList("SYSTEM", "LANGUAGE");
 *
 * // 4. 값 설정
 * VsIniManager.Set("SYSTEM", "MODE", "AUTO");
 *
 * // 5. 저장
 * VsIniManager.Save();
 * \endcode
 *
 * \section format INI 포맷 예시
 *
 * ```ini
 * [SYSTEM]
 * TIMEOUT = 30
 * LANGUAGE = ko,en,vi
 * MODE = AUTO
 * ```
 *
 * \section di DI 확장 방식
 *
 * DI 사용 시 `IIniManager` 인터페이스를 통해 주입 받을 수 있으며,
 * 내부적으로 `VsIniManagerProxy`가 실제 구현체로 작동합니다.
 * DI 컨테이너 초기화 예시는 다음과 같습니다.
 *
 * \code{.cs}
 * container.Register<IIniManager, VsIniManagerProxy>();
 * \endcode
 *
 * \section version 버전 관리
 *
 * \subsection v2025_06_23 2025-06-23 Ver1.0.0 (By.장민수 프로)
 * - INI 파서 및 설정 관리자 모듈 최초 구현
 * - VsIniManager 정적 진입점 제공
 * - DI 확장 가능 구조 제공
 *
 * \section license 라이선스
 *
 * 이 프로젝트는 VisionSemicon 내부 전용 라이브러리입니다.
 * 외부 배포 및 무단 복제를 금지합니다.
 *
 * \section contact 문의
 *
 * Email: msjang@visionsemicon.co.kr
 */
