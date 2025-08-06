
# 🧭 Controller 라이브러리 구성도

FA 제어 장비에 사용되는 Motion, DIO, AIO 라이브러리를 정리한 폴더 및 클래스 구조입니다.  
외부 벤더 SDK는 Ajin(AxtLib), Adlink(Dask 등)으로 구분되며, 각 벤더별로 모듈화하여 관리합니다.

---

## 📁 디렉터리 구조

```
VSLibrary/
└── Controller/
    ├── Ajin/
    │   └── AxtLib/
    │       ├── AxtAIO.cs
    │       ├── AxtDIO.cs
    │       ├── AxtTrigger.cs
    │       ├── ...
    │       ├── AXL.dll           # Ajin 라이브러리 DLL
    │       └── EzBasicAxl.dll
    │
    └── AdlinkLib/
        └── Dask/
            ├── Dask64.cs
            ├── PCI-Dask64.dll
            └── PCI-Dask64.lib
```

---

## 🧩 주요 클래스 목록

| 클래스 이름 | 설명 |
|-------------|------|
| `CAxtAIO` | 아날로그 입력/출력 제어 |
| `CAxtDIO` | 디지털 IO 제어 |
| `CAxtTrigger` | 트리거 기능 제어 |
| `CAxtLib` | AXT 공통 기능 래퍼 |
| `DASK` | Adlink DASK 초기화 및 기본 기능 |
| `CCAMCIPdef` | 고정 파라미터 정의 |
| `CAxtCAMCFS` | 커스텀 모션 컨트롤 정의 |
| `CAxtKeCAMCFS` | CAMCFS 커널 확장 정의 |
| `CAxtKeLib` | Ajin의 KeLib 인터페이스 |
| … | 기타 Ajin 전용 커스텀 제어 유닛 |

---

## 🧠 모듈 분리 기준

- Ajin 계열은 `AxtLib`로 통합하여, 각 모듈을 **기능 단위(AIO, DIO, Trigger 등)** 로 파일 분리함.
- Adlink 계열은 제품별 라이브러리 기준으로 `Dask`, `APS` 등 하위 폴더로 정리
- 클래스는 각 DLL 함수 래핑 및 고유 기능 단위로 정의
- 외부 라이브러리는 DLL 파일만 포함하며, NuGet 패키지 또는 `.lib`는 별도 링크 제공 가능

---

## 🔄 향후 확장 계획

- `Ajin.AxtLib`는 내부적으로 공통 인터페이스로 래핑 예정 (예: `IAjinMotionController`, `IDigitalOutput`)
- `AdlinkLib`의 `APS` SDK 및 `ACL` SDK도 폴더 단위로 추가 예정
- 내부 장비 추상화를 위한 공통 컨트롤러 인터페이스 설계 추진

---

📁 문서 분류: 외부 제어 라이브러리 구조  
📅 문서 작성일: 2025-06-18  
🖋️ 기록자 서명: 장민수 드림
