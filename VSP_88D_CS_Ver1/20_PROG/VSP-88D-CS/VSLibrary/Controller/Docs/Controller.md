
# 🧭 Controller 라이브러리 구성도

FA 제어 장비에 사용되는 Motion, DIO, AIO 라이브러리를 모듈화하여 관리하기 위한 구조입니다. 로컬 코드 베이스를 직접 분석하여 모든 클래스·인터페이스·열거형을 포함했습니다.

---

## 📁 디렉터리 구조

```
VSLibrary/
└── Controller/
    ├── AnalogIO/
    │   ├── ADLinkAIO.cs       # Adlink DASK 기반 아날로그 I/O 구현
    │   └── AjinAxtAIO.cs      # Ajin Axt 보드용 아날로그 I/O 구현
    ├── DigitalIO/
    │   ├── AjinAxtDIO.cs      # Ajin Axt 보드용 디지털 I/O 구현
    │   └── ComizoaDIO.cs      # Comizoa 보드용 디지털 I/O 구현
    ├── Motion/
    │   ├── Ajin/
    │   │   ├── AxlMotion.cs   # Adlink Dask Axl 모션 컨트롤러 래퍼
    │   │   └── AxtMotion.cs   # Ajin CAxtCAMCFS20 모션 컨트롤러 래퍼
    │   └── ComizoaMotion.cs  # Comizoa 모션 컨트롤러 구현
    ├── ControllerBase.cs      # 공통 추상 베이스 클래스 정의
    ├── ControllerData.cs      # 컨트롤러용 데이터 모델 정의
    ├── ControllerENUM.cs      # 공통 열거형 정의
    ├── ControllerInterface.cs # 공통 인터페이스 정의
    └── ControllerManager.cs   # DI 및 라이브러리 초기화 매니저

```

---

## 🧩 구성 요소 상세

### 1. 구현 클래스 (Concrete Controllers)

#### AnalogIO
- **ADLinkAIO** (`AnalogIO/ADLinkAIO.cs`)
- **AjinAxtAIO** (`AnalogIO/AjinAxtAIO.cs`)

#### DigitalIO
- **AjinAxtDIO** (`DigitalIO/AjinAxtDIO.cs`)
- **ComizoaDIO** (`DigitalIO/ComizoaDIO.cs`)

#### Motion
- **AxlMotion** (`Motion/Ajin/AxlMotion.cs`)
- **AxtMotion** (`Motion/Ajin/AxtMotion.cs`)
- **ComizoaMotion** (`Motion/ComizoaMotion.cs`)

---

### 2. 추상 베이스 클래스 (`ControllerBase.cs`)
- `public abstract class AIOBase : IAIOBase`
- `public abstract class AIODataBase : ViewModelBase, IAnalogIOData`
- `public abstract class DIOBase : IDIOBase`
- `public abstract class DIODataBase : ViewModelBase, IDigitalIOData`
- `public abstract class MotionBase : IMotionBase`
- `public abstract class AxisDataBase : ViewModelBase, IAxisData`

---

### 3. 데이터 모델 (`ControllerData.cs`)
- `public class AIOData`            
- `public class DIOData`
- `public class AxtAxisData`

---

### 4. 공통 인터페이스 (`ControllerInterface.cs`)
- `public interface IAIOBase`
- `public interface IIOSettinglist`
- `public interface IAnalogIOData`
- `public interface IDIOBase`
- `public interface IDigitalIOData`
- `public interface IMotionBase`
- `public interface IAxisSettinglist`
- `public interface IAxisData`

---

### 5. 열거형 (`ControllerENUM.cs`)
- `public enum IOType { ... }`
- `public enum ControllerType { ... }`

---

### 6. 매니저 및 유틸리티 (`ControllerManager.cs`)
- `public class ControllerManager`  
- `public static class LibraryInitializer`

---


## 🔄 모듈 분리 기준

- **Ajin 계열**: `AnalogIO/AjinAxtAIO.cs`, `DigitalIO/AjinAxtDIO.cs`, `Motion/Ajin/` 내부
- **Adlink 계열**: `AnalogIO/ADLinkAIO.cs`, `Motion/Ajin/AxlMotion.cs`
- **Comizoa 계열**: `DigitalIO/ComizoaDIO.cs`, `Motion/ComizoaMotion.cs`
- **공통 계층**: 루트의 `ControllerInterface.cs`, `ControllerBase.cs`, `ControllerENUM.cs`, `ControllerData.cs`, `ControllerManager.cs`

---


## 🔄 향후 확장 계획

- Comizoa, AXL 계열 라이브러리추가

---

📁 문서 분류: 외부 제어 라이브러리 구조  
📅 문서 작성일: 2025-06-30  
🖋️ 기록자 서명: 서상덕 드림

