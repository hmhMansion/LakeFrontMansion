# LakeFrontMansion 게임 오브젝트 가이드

**Room / Player / Door / Furniture 설정 및 사용법**

---

## 📦 게임 오브젝트 개요

### Room (방)
**역할:** 배경 이미지 + 이동 가능한 경계 영역
**구성:** SpriteRenderer(배경) + EdgeCollider2D(테두리) + RoomBoundary

### Player (플레이어)
**역할:** 방 안에서 WASD로 8방향 이동, 경계/가구 충돌
**구성:** SpriteRenderer + Rigidbody2D(Dynamic) + BoxCollider2D + PlayerMovement

### Furniture (가구)
**역할:** 플레이어가 관통할 수 없는 장애물
**구성:** SpriteRenderer + Rigidbody2D(Static) + BoxCollider2D + Furniture

### Door (문)
**역할:** 다른 씬(방)으로 전환하는 트리거
**구성:** SpriteRenderer + BoxCollider2D(Trigger) + SceneTransitionTrigger

---

## 🎯 핵심 개념

**하나의 Room 오브젝트에 배경 이미지 + 테두리 경계**

- Room 오브젝트 하나 생성
- 배경 Sprite 설정 (임시 회색 사각형 → 실제 방 이미지로 교체)
- EdgeCollider2D로 테두리만 경계 생성
- **배경 내부는 비어있어서 플레이어가 자유롭게 이동**
- **테두리만 막혀서 못 나감**

## 빠른 시작

```
1. GameObject > LakeFrontMansion > Create Room
2. GameObject > LakeFrontMansion > Create Player
3. GameObject > LakeFrontMansion > Create Furniture (가구 여러 개)
4. GameObject > LakeFrontMansion > Create Door
5. Play 버튼 (▶) 테스트
```

---

## 1️⃣ Room (방) - 상세 설명

### Room이란?

**Room = 배경 이미지 + 경계선이 합쳐진 하나의 GameObject**

```
Room GameObject
├── SpriteRenderer      → 방 배경 이미지 (플레이어가 보는 그림)
├── Rigidbody2D        → Static (움직이지 않는 물리 오브젝트)
├── EdgeCollider2D     → 테두리 경계선 (플레이어가 밖으로 못 나감)
└── RoomBoundary       → 경계 관리 스크립트
```

### Room 생성 방법

```
Unity 에디터 상단 메뉴:
GameObject > LakeFrontMansion > Create Room
```

### 생성 시 기본 설정

| 컴포넌트 | 기본값 | 설명 |
|---------|--------|------|
| **SpriteRenderer** | 회색 사각형 512x512 | 임시 배경 (실제 방 이미지로 교체 필요) |
| **Order in Layer** | -10 | 플레이어/가구보다 뒤에 렌더링 |
| **Rigidbody2D** | Static | 절대 움직이지 않는 물리 오브젝트 |
| **EdgeCollider2D** | 사각형 테두리 | 점 5개로 닫힌 경로 (편집 가능) |
| **RoomBoundary** | - | 경계선 시각화 (Scene 뷰에서 빨간 선) |

### Room은 경계가 아니라 "방 자체"

**중요:**
- Room은 단순한 경계선이 아닙니다
- **배경 + 경계를 모두 포함한 완전한 방**입니다
- 임시로 회색 사각형이 표시되지만, **실제 방 이미지로 교체**해서 사용합니다

---

## 2️⃣ Player (플레이어) - 상세 설명

### Player 생성 방법

```
GameObject > LakeFrontMansion > Create Player
```

### Player 구성

```
Player GameObject
├── SpriteRenderer      → 초록색 사각형 32x32 (임시 - 캐릭터 스프라이트로 교체)
├── Rigidbody2D        → Dynamic (WASD 입력으로 이동)
│   ├── Gravity Scale: 0 (탑다운 게임 - 중력 없음)
│   ├── Constraints: Freeze Rotation (회전 방지)
│   ├── Collision Detection: Continuous (빠른 이동에도 충돌 정확)
│   └── Interpolation: Interpolate (부드러운 움직임)
├── BoxCollider2D      → 0.8 x 0.8 (플레이어 충돌 영역)
├── PlayerMovement     → WASD 이동 스크립트
└── CameraFollow       → 카메라가 플레이어 따라다님
```

### 플레이어 태그 설정

**중요: 자동으로 Tag = "Player" 설정됨**
- Door가 플레이어를 인식하는데 필요
- 수동으로 삭제하지 말 것

---

## 3️⃣ Furniture (가구) - 상세 설명

### Furniture 생성 방법

```
GameObject > LakeFrontMansion > Create Furniture
```

### Furniture 구성

```
Furniture GameObject
├── SpriteRenderer      → 갈색 사각형 64x64 (임시 - 가구 스프라이트로 교체)
├── Rigidbody2D        → Static (플레이어가 밀어도 안 움직임)
├── BoxCollider2D      → Is Trigger = FALSE (물리적으로 막음)
└── Furniture          → 가구 정보 스크립트
    ├── furnitureType: "Furniture"
    └── isInteractable: false
```

### 가구의 역할

**플레이어가 관통할 수 없는 장애물**
- Room 경계와 동일한 물리 충돌
- `Is Trigger = false` → 플레이어가 **물리적으로 막힘**
- `Rigidbody2D = Static` → 플레이어가 밀어도 **절대 안 움직임**

### 가구 배치 예시

```
침실:
- Furniture_Bed (침대)
- Furniture_Desk (책상)
- Furniture_Chair (의자)

복도:
- Furniture_Plant (화분)
- Furniture_Painting (그림)
```

---

## 4️⃣ Door (문) - 상세 설명

### Door 생성 방법

```
GameObject > LakeFrontMansion > Create Door
```

### Door 구성

```
Door GameObject
├── SpriteRenderer      → 노란색 사각형 64x64 (임시 - 문 스프라이트로 교체)
├── BoxCollider2D      → Is Trigger = TRUE (플레이어 통과 가능)
│   └── Size: 1.2 x 1.5
├── SceneTransitionTrigger
│   ├── targetSceneName: "" (Inspector에서 설정 필요!)
│   ├── useClick: true (E키 또는 클릭으로 전환)
│   ├── useTrigger: false
│   └── playerTag: "Player"
└── InteractionUI (자식 오브젝트)
    └── TextMesh: "[E] 입장"
```

### Door 설정 방법

**필수: Target Scene Name 설정**

```
1. Hierarchy에서 Door 선택
2. Inspector > Scene Transition Trigger
3. Target Scene Name: "F1_Corridor" (이동할 씬 이름 입력)
```

### Door 작동 방식

1. 플레이어가 문에 가까이 가면 → "[E] 입장" UI 표시
2. E키 또는 마우스 클릭 → 다음 씬으로 전환
3. `Is Trigger = true` → 플레이어가 Door를 **통과 가능**

### 씬 이름 확인 방법

```
Project 창 > Assets/Scenes 폴더 확인

예시:
- Tutorial.unity → "Tutorial"
- F1_Corridor.unity → "F1_Corridor"
- F1_Room01.unity → "F1_Room01"
```

---

## 5️⃣ 방 배경 이미지 교체하기

**Room은 기본적으로 회색 사각형입니다. 실제 방 이미지로 교체하세요!**

### 방법 1: Sprite 가져오기 (이미지 파일 있는 경우)

```
1. 방 배경 이미지를 Assets/Sprites 폴더로 드래그

2. 이미지 선택 > Inspector:
   - Texture Type: Sprite (2D and UI)
   - Pixels Per Unit: 100
   - Filter Mode: Point (no filter) ← 픽셀 아트
   - Compression: None
   - Apply
```

### Room에 적용

```
Room 선택 > Inspector

Sprite Renderer > Sprite:
→ 방금 가져온 배경 이미지 선택
```

### 방법 2: Unity 기본 스프라이트 사용 (임시 테스트)

```
Room 선택 > Inspector
Sprite Renderer > Sprite > 🔍 클릭
→ 검색: "Square" 또는 "Circle"
→ Unity 내장 스프라이트 선택
```

---

## 6️⃣ 방 경계 조정 (EdgeCollider2D)

배경 이미지 크기에 맞게 EdgeCollider2D 조정:

```
Room 선택 > Inspector

Edge Collider 2D > Edit Collider 클릭

Scene 뷰에서:
- 점 이동: 드래그
- 점 추가: Shift + 클릭
- 점 삭제: Ctrl + Shift + 클릭 (Mac: Cmd + Shift + 클릭)
```

### 사각형 방

```
배경 크기: 640x480 픽셀 (PPU=100) = 6.4 x 4.8 units

EdgeCollider2D 점 위치:
- (-3.2, -2.4)  왼쪽 아래
- (3.2, -2.4)   오른쪽 아래
- (3.2, 2.4)    오른쪽 위
- (-3.2, 2.4)   왼쪽 위
- (-3.2, -2.4)  왼쪽 아래 (닫힌 경로)
```

### 원형 방

```
원형으로 점 배치:


중심 (0, 0), 반지름 3

여러 개의 점을 원 모양으로 배치:
(3, 0) → (2.1, 2.1) → (0, 3) → (-2.1, 2.1) → (-3, 0)
→ (-2.1, -2.1) → (0, -3) → (2.1, -2.1) → (3, 0)
```

### L자 방

```
   ┌─────────┐
   │         │
   │    ┌────┘
   │    │
   └────┘

점 위치 (예시):
(-5, 5) → (5, 5) → (5, 2) → (2, 2)
→ (2, -5) → (-5, -5) → (-5, 5)
```

---

## 7️⃣ 실제 사용 예시 (Step-by-Step)

### 예시 1: 침실 (사각형)

```
1. GameObject > Create Room

2. Room > Sprite Renderer > Sprite
   → bedroom.png 선택

3. Room > Edge Collider 2D > Edit Collider
   → 배경 끝에 맞게 점 조정

4. GameObject > Create Furniture (침대)
   → Position: (2, 1)
   → Sprite: bed.png

5. GameObject > Create Furniture (책상)
   → Position: (-2, -1)
   → Sprite: desk.png

6. GameObject > Create Door
   → Position: (0, -2.4) (방 아래쪽)
   → Target Scene Name: "F1_Corridor"

7. GameObject > Create Player
   → Position: (0, 0)

8. Play 테스트
   → WASD로 이동
   → 테두리에 막힘 ✅
   → 가구에 막힘 ✅
   → 방 안에서만 이동 ✅
```

### 예시 2: L자 손님방

```
1. GameObject > Create Room

2. Room > Sprite
   → guestroom_L.png

3. Room > Edge Collider 2D > Edit Collider
   → L자 모양으로 점 배치

   ┌─────────┐
   │         │
   │    ┌────┘
   │    │
   └────┘

4. 가구/문/플레이어 배치

5. Play 테스트
   → L자 안에서만 이동 ✅
```

---

## 8️⃣ Rigidbody2D 설정 (물리 엔진)

### 플레이어

```
Player > Rigidbody 2D:
- Body Type: Dynamic
- Gravity Scale: 0
- Linear Damping: 0
- Angular Damping: 0.05
- Collision Detection: Continuous
- Sleeping Mode: Never Sleep
- Interpolation: Interpolate
- Constraints: Freeze Rotation
```

### Room (방)

```
Room > Rigidbody 2D:
- Body Type: Static
```

### Furniture (가구)

```
Furniture > Rigidbody 2D:
- Body Type: Static
```

**Static이 중요한 이유:**
- 절대 움직이지 않는 오브젝트
- 물리 엔진 최적화
- 플레이어가 밀어도 안 움직임

---

## 9️⃣ EdgeCollider2D 사용법 (경계선 편집)

### 기본 조작

| 조작 | 방법 |
|------|------|
| 점 이동 | 드래그 |
| 점 추가 | Shift + 클릭 (선 위) |
| 점 삭제 | Ctrl + Shift + 클릭 |
| 편집 끝내기 | Edit Collider 버튼 다시 클릭 |

### 닫힌 경로 만들기

**중요: 마지막 점을 첫 점과 같은 위치에**

```
사각형 예시:
point[0] = (-3, -3)
point[1] = (3, -3)
point[2] = (3, 3)
point[3] = (-3, 3)
point[4] = (-3, -3) ← 첫 점과 동일!
```

---

## 🔧 문제 해결 (Troubleshooting)

### Q: 플레이어가 방 밖으로 튕겨나갑니다

**해결:**
```
1. Room > Rigidbody 2D
   → Body Type = Static 확인

2. Player > Rigidbody 2D
   → Body Type = Dynamic
   → Sleeping Mode = Never Sleep

3. Scene 뷰에서 플레이어가 Room 안에 있는지 확인
```

### Q: 플레이어가 경계를 통과합니다

**해결:**
```
1. Room > Edge Collider 2D 확인
   → 점들이 닫힌 경로인지 확인
   → 마지막 점 = 첫 점

2. Player > Box Collider 2D
   → Is Trigger = false

3. Player > Rigidbody 2D
   → Collision Detection = Continuous
```

### Q: EdgeCollider2D가 제대로 안 보입니다

**해결:**
```
Scene 뷰 > Gizmos 버튼 활성화

Room 선택 > Edge Collider 2D
→ 초록색 선이 보여야 함
```

### Q: 배경 크기와 경계가 안 맞습니다

**해결:**
```
Room > Edge Collider 2D > Edit Collider

Scene 뷰에서:
→ 점들을 배경 끝에 맞게 드래그
→ Gizmos를 켜서 배경과 경계 동시에 확인
```

### Q: 가구를 플레이어가 밀어버립니다

**해결:**
```
Furniture > Rigidbody 2D
→ Body Type = Static
```

### Q: 문이 작동하지 않습니다

**해결:**
```
Door > Box Collider 2D
→ Is Trigger = true

Door > SceneTransitionTrigger
→ Target Scene Name 오타 확인

File > Build Settings
→ Scene이 추가되어 있는지 확인
```

---

## 📁 Scene 구조 예시

### Tutorial Scene

```
Hierarchy:
├── Main Camera
├── Room
│   └── (Sprite: tutorial_room.png)
│   └── (EdgeCollider2D: 사각형)
├── Player
├── Furniture_Table
└── Door_To_F1_Corridor
```

### F1_Corridor Scene

```
Hierarchy:
├── Main Camera
├── Room
│   └── (Sprite: corridor_1f.png)
│   └── (EdgeCollider2D: 긴 복도)
├── Player
├── Door_To_F1_Room01 (왼쪽)
├── Door_To_F1_Room02 (중간)
├── Door_To_F1_Room03 (오른쪽)
└── Door_To_F2_Corridor (계단)
```

### F3_GuestRoom01 Scene (L자)

```
Hierarchy:
├── Main Camera
├── Room
│   └── (Sprite: guestroom_L.png)
│   └── (EdgeCollider2D: L자 모양)
├── Player
├── Furniture_Bed
├── Furniture_Sofa
└── Door_To_F3_Corridor
```

---

## 💾 Prefab으로 재사용

```
1. Room 설정 완료 (배경 + 경계)
2. Hierarchy > Room 선택
3. Project > Prefabs 폴더로 드래그
4. 이름: "Room_Bedroom"

다른 Scene에서:
→ Prefabs/Room_Bedroom을 드래그
→ Sprite만 바꾸면 됨
→ EdgeCollider2D는 그대로 재사용
```

---

## ✅ 체크리스트

### 생성 후 확인

- [ ] Room에 Sprite 설정됨
- [ ] Room > Edge Collider 2D가 배경 테두리에 맞음
- [ ] Scene 뷰 Gizmos 활성화 → 초록색 선 보임
- [ ] Player가 Room 안에 있음

### Rigidbody2D 확인

- [ ] Room > Rigidbody2D > Body Type = Static
- [ ] Player > Rigidbody2D > Body Type = Dynamic
- [ ] Player > Rigidbody2D > Sleeping Mode = Never Sleep
- [ ] Furniture > Rigidbody2D > Body Type = Static

### Collider 확인

- [ ] Room > Edge Collider 2D 닫힌 경로 (첫 점 = 마지막 점)
- [ ] Player > Collider > Is Trigger = false
- [ ] Furniture > Collider > Is Trigger = false
- [ ] Door > Collider > Is Trigger = true

### 테스트

- [ ] Play 모드에서 플레이어 제자리 (튕겨나가지 않음)
- [ ] WASD로 이동 가능
- [ ] 경계(테두리)에 막힘
- [ ] 가구에 막힘
- [ ] 방 안에서만 이동
- [ ] 문으로 Scene 전환 (E키)

---

## 🎮 Unity 메뉴 요약

```
GameObject > LakeFrontMansion >

1. Create Player                   ← 플레이어 생성 (초록색 사각형)
2. Create Door (Scene Transition)  ← 문 생성 (노란색 사각형)
3. Create Room                     ← 방 생성 (회색 배경 + 경계)
4. Create Furniture                ← 가구 생성 (갈색 사각형)
5. Create Build Settings Helper    ← Scene 빌드 설정 도구
```

---

## 📝 핵심 정리

### 구조
- **Room 하나에 모두 포함**
  - Sprite Renderer (배경)
  - Rigidbody2D (Static)
  - EdgeCollider2D (테두리만, 내부는 비어있음)

### 왜 EdgeCollider2D?
- **테두리만 선으로 그음**
- **내부는 비어있어서 플레이어가 자유롭게 이동**
- 사각형, 원형, L자 등 어떤 모양이든 가능

### Rigidbody2D
- **Player**: Dynamic (움직임)
- **Room/Furniture**: Static (고정)

### Collider
- **Room/Furniture**: Is Trigger = false (막음)
- **Door**: Is Trigger = true (통과, Scene 전환)

### 순서
1. 방 생성 → 배경 Sprite 설정 → EdgeCollider 조정
2. 가구 배치
3. 문 배치 (Target Scene Name 설정)
4. 플레이어 배치
5. Build Settings에 Scene 추가
6. Play 테스트

---

## 🚀 빠른 참조표

### GameObject 색상 코드 (임시 스프라이트)

| GameObject | 색상 | 크기 | 용도 |
|-----------|------|------|------|
| **Room** | 회색 (0.9, 0.9, 0.9) | 512x512 | 방 배경 |
| **Player** | 초록색 (0, 1, 0) | 32x32 | 플레이어 캐릭터 |
| **Door** | 노란색 (1, 1, 0) | 64x64 | 씬 전환 문 |
| **Furniture** | 갈색 (0.6, 0.4, 0.2) | 64x64 | 가구/장애물 |

### Collider 설정 요약

| GameObject | Collider | Is Trigger | Rigidbody2D |
|-----------|----------|------------|-------------|
| **Room** | EdgeCollider2D | ❌ false | Static |
| **Player** | BoxCollider2D | ❌ false | Dynamic |
| **Door** | BoxCollider2D | ✅ true | - |
| **Furniture** | BoxCollider2D | ❌ false | Static |

### 스크립트 위치

```
Assets/Scripts/
├── Player/
│   ├── PlayerMovement.cs
│   └── CameraFollow.cs
├── Environment/
│   ├── RoomBoundary.cs
│   └── Furniture.cs
└── SceneManagement/
    ├── SceneLoader.cs
    ├── SceneTransitionTrigger.cs
    └── FadeTransition.cs
```

---

**이제 플레이어가 방 안에서만 자유롭게 움직이고, 테두리를 넘지 못합니다!**

---

> 💡 **참고:** GameObjectCreator.cs (Assets/Editor/GameObjectCreator.cs)에서 각 GameObject의 생성 로직을 확인할 수 있습니다.
