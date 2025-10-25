# Scene Management 사용 가이드

LakeFrontMansion 게임의 Scene 전환 시스템 사용법입니다.

## Scene 구조

```
Scenes/
├── Core/
│   ├── Boot.unity              # 게임 초기화
│   └── MainMenu.unity          # 메인 메뉴
├── Tutorial.unity              # 튜토리얼
├── Floor3/
│   ├── F3_Corridor.unity       # 3층 복도
│   ├── F3_GuestRoom01.unity    # 3층 손님방 1
│   ├── F3_GuestRoom02.unity    # 3층 손님방 2
│   └── F3_GuestRoom03.unity    # 3층 손님방 3
├── Floor2/
│   ├── F2_Corridor.unity       # 2층 복도
│   ├── F2_Room01.unity         # 2층 방 1
│   ├── F2_Room02.unity         # 2층 방 2
│   └── F2_Room03.unity         # 2층 방 3
└── Floor1/
    ├── F1_Corridor.unity       # 1층 복도
    ├── F1_Room01.unity         # 1층 방 1
    ├── F1_Room02.unity         # 1층 방 2
    └── F1_Room03.unity         # 1층 방 3
```

## 스크립트 설명

### 1. SceneLoader.cs
Scene 로딩을 관리하는 싱글톤 매니저입니다.

**주요 기능:**
- `LoadScene(string sceneName)`: Scene 이름으로 로드
- `LoadSceneAsync(string sceneName)`: 비동기 로딩 (로딩 바 구현 가능)
- `QuitGame()`: 게임 종료

**사용 예시:**
```csharp
SceneLoader.Instance.LoadScene("F1_Corridor");
```

### 2. SceneTransitionTrigger.cs
문이나 특정 오브젝트에 붙여서 Scene 전환을 트리거합니다.

**설정 방법:**

1. **문 오브젝트 생성**
   - GameObject 생성 (Sprite Renderer로 문 이미지 설정)
   - Box Collider 2D 추가
   - `Is Trigger` 체크

2. **SceneTransitionTrigger 컴포넌트 추가**
   - Target Scene Name: 이동할 Scene 이름 입력 (예: "F1_Room01")
   - Use Click: true (E키나 클릭으로 전환)
   - Use Trigger: false (자동 전환은 false)
   - Player Tag: "Player"

3. **상호작용 UI (선택사항)**
   - Canvas 생성
   - Text/Image 추가 (예: "E키를 눌러 입장")
   - Interaction UI 필드에 할당

**전환 방식:**

- **클릭 방식** (Use Click = true)
  - 플레이어가 Trigger 영역에 들어감
  - E키 또는 마우스 클릭으로 Scene 전환

- **자동 방식** (Use Trigger = true, Use Click = false)
  - 플레이어가 Trigger 영역에 들어가면 즉시 전환

### 3. FadeTransition.cs (선택사항)
Scene 전환 시 페이드 효과를 제공합니다.

**설정 방법:**

1. **Canvas 생성**
   - Hierarchy 우클릭 > UI > Canvas
   - Canvas Scaler: Scale With Screen Size
   - Reference Resolution: 1920x1080

2. **Fade Image 생성**
   - Canvas 하위에 Image 생성
   - 이름: "FadePanel"
   - Anchor: Stretch (전체 화면)
   - Color: 검은색 (R:0, G:0, B:0, A:0)

3. **FadeTransition 스크립트 추가**
   - FadePanel에 FadeTransition.cs 추가
   - Fade Duration: 1.0 (초)

**사용 예시:**
```csharp
// 기본 전환
SceneLoader.Instance.LoadScene("F2_Corridor");

// 페이드 효과와 함께 전환
FadeTransition.Instance.LoadSceneWithFade("F2_Corridor");
```

## Build Settings에 Scene 추가

1. File > Build Settings
2. "Add Open Scenes" 또는 폴더에서 드래그
3. 모든 Scene을 Build에 포함시켜야 로드 가능

**필수 Scene 리스트:**
- Core/Boot
- Core/MainMenu
- Tutorial
- Floor3/F3_Corridor
- Floor3/F3_GuestRoom01
- Floor3/F3_GuestRoom02
- Floor3/F3_GuestRoom03
- Floor2/F2_Corridor
- Floor2/F2_Room01
- Floor2/F2_Room02
- Floor2/F2_Room03
- Floor1/F1_Corridor
- Floor1/F1_Room01
- Floor1/F1_Room02
- Floor1/F1_Room03

## 예제: 복도에서 방으로 이동

### F1_Corridor Scene 설정:

1. **방 1 입구 문**
   - Target Scene Name: "F1_Room01"
   - 위치: 복도 왼쪽

2. **방 2 입구 문**
   - Target Scene Name: "F1_Room02"
   - 위치: 복도 중간

3. **방 3 입구 문**
   - Target Scene Name: "F1_Room03"
   - 위치: 복도 오른쪽

4. **계단 (2층으로)**
   - Target Scene Name: "F2_Corridor"
   - 위치: 복도 끝

### F1_Room01 Scene 설정:

1. **나가기 문**
   - Target Scene Name: "F1_Corridor"
   - 위치: 방 입구

## 협업 팁

1. **Scene 이름 규칙 지키기**
   - 복도: `F{층수}_Corridor` (예: F1_Corridor)
   - 방: `F{층수}_Room{번호}` (예: F2_Room01)

2. **각자 다른 Scene 작업하기**
   - 개발자 A: Floor1 전체
   - 개발자 B: Floor2 전체
   - 개발자 C: Floor3 전체

3. **Prefab 활용**
   - 문, UI 등 공통 요소는 Prefab으로 만들기
   - 변경사항이 모든 Scene에 자동 적용

4. **Git 충돌 방지**
   - 작업 전 git pull
   - 각자 다른 Scene 파일 수정
   - 자주 commit & push

## 문제 해결

### Scene을 찾을 수 없다는 에러
- Build Settings에 Scene이 추가되어 있는지 확인
- Scene 이름 오타 확인

### Scene이 로드되지 않음
- SceneLoader가 DontDestroyOnLoad로 설정되어 있는지 확인
- Console에서 에러 메시지 확인

### Trigger가 작동하지 않음
- Collider2D의 "Is Trigger" 체크 확인
- Player 오브젝트의 Tag가 "Player"인지 확인
- Player에 Rigidbody2D가 있는지 확인
