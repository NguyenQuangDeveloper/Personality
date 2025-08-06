# 📘 UIComponent Module

`VSLibrary.UIComponent`  
WPF/WinForms 등 다양한 UI 컴포넌트를 제공하는 핵심 모듈입니다.  
버튼, 그리드, 네비게이션, 차트 등 실무 UI 요소와  
공통 스타일, 커맨드 바인딩, 템플릿 등을 포함합니다.

---

## 📦 네임스페이스

```
- `VSLibrary.UIComponent.VSControls` : 커스텀 버튼, 체크박스 등  
- `VSLibrary.UIComponent.VsGrids` : 데이터 그리드/리스트  
- `VSLibrary.UIComponent.VsNavigations` : 네비게이션 바, 페이지 이동  
- `VSLibrary.UIComponent.VSCharts` : 실시간/통계 차트  
- 기타: Styles, MessageBox, LayoutPanels, KeyPads 등
```

---

## 🧱 주요 클래스

| 클래스명 | 설명 |
|----------|------|
| `IniBase` | 기본 제공자 클래스. Dictionary 기반 저장소 관리 |
| `VsIniManager` | 정적 진입점. 외부에서 접근 가능한 API 제공 |
| `VsIniManagerProxy` | 실제 데이터 로직 수행. Load/Save/Set/Get 구현 |
| `IIniManager` | 설정 인터페이스. DI 또는 내부 proxy에 의존 |
| `IIniProvider` | 설정 제공자 인터페이스 |
| `IniEnum.cs` | 섹션, 키 등의 Enum 구성 보조 |

---

## 🧩 클래스 구조

```

```

---

## ⚙️ 기본 사용법

### 1. 초기화


---




## 📌 요약 기능

| 메서드 | 설명 |
|--------|------|
| `Initialize(path)` | INI 파일 로딩 |
| `Get(section, key)` | 문자열 값 반환 |
| `GetList(section, key)` | 쉼표로 분리된 리스트 반환 |
| `Set(section, key, value)` | 설정값 지정 |
| `Save(path)` | 현재 설정 저장 |

---

📅 문서 작성일: 2025-06-23  
🖋️ 기록자 서명: 장민수 드림
