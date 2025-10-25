# LakeFrontMansion 게임 셋업 가이드

## 중복 코드 재사용 - Prefab 시스템 ✅

**결론: 중복 코드 작성 필요 없음! Prefab으로 재사용 가능**

Unity의 **Prefab 시스템**을 사용하면:
- ✅ **한 번만 만들면 모든 Scene에서 재사용**
- ✅ **Prefab 수정 시 모든 인스턴스 자동 업데이트**
- ✅ **스크립트도 Prefab에 포함되어 자동 적용**

### 재사용 가능한 요소들

| 요소 | 재사용 방법 | 중복 작업 필요? |
|------|-----------|--------------|
| 플레이어 캐릭터 | Prefab 1개 → 모든 Scene에 배치 | ❌ 없음 |
| 문 오브젝트 | Prefab 1개 → 복사 후 Scene 이름만 변경 | ❌ 없음 |
| UI 요소 | Prefab로 생성 | ❌ 없음 |
| 카메라 | 각 Scene에 기본 포함 | ❌ 없음 |
| 매니저 스크립트 | SceneLoader는 싱글톤 (자동 생성) | ❌ 없음 |

## 빠른 시작 (5분 안에 테스트)

### 1단계: Unity Editor 열기
```
프로젝트 열기 → Assets/Scenes/Tutorial.unity 더블클릭
```

### 2단계: Build Settings 설정
```
Unity Editor 메뉴:
GameObject > LakeFrontMansion > Create Build Settings Helper

→ "모든 Scene을 Build Settings에 추가" 버튼 클릭
```

### 3단계: 플레이어 생성
```
Unity Editor 메뉴:
GameObject > LakeFrontMansion > Create Player

→ 초록색 사각형이 Scene에 생성됨 (임시 플레이어)
```

### 4단계: 문 생성
```
Unity Editor 메뉴:
GameObject > LakeFrontMansion > Create Door (Scene Transition)

→ 노란색 사각형이 Scene에 생성됨 (임시 문)

Inspector에서 설정:
- Target Scene Name: "F1_Corridor" 입력
- 전환방식 설명 :  
(1) Use Click = true, Use Trigger = false
  플레이어 근처 → UI 표시 → E키 입력 → 전환
  대부분의 문에 사용
(2) 자동 전환: Use Click = false, Use Trigger = true
  플레이어 접촉 → 즉시 전환
  계단, 자동문에 사용
(3) 둘 다 true?
  Use Click이 우선되어 클릭 방식으로 작동
```

### 5단계: 테스트
```
Play 버튼 (▶) 클릭

WASD 또는 방향키로 이동
문 근처로 가면 "[E] 입장" 텍스트 표시
E키 누르면 F1_Corridor Scene으로 전환!
```

## Prefab 생성 방법 (한 번만 하면 됨)

### 플레이어 Prefab 만들기

1. **플레이어 생성** (위 3단계)
2. **Hierarchy에서 Player 선택**
3. **Project 창의 Assets/Prefabs 폴더로 드래그**
4. **Prefab 생성 완료!**

이제 다른 Scene에서:
- Assets/Prefabs/Player를 Hierarchy로 드래그
- 끝! 스크립트 자동 적용됨

### 문 Prefab 만들기

1. **문 생성** (위 4단계)
2. **Inspector에서 Target Scene Name 설정**
3. **Hierarchy에서 Door 선택**
4. **Project 창의 Assets/Prefabs 폴더로 드래그**
5. **이름 변경**: "Door_To_F1_Room01"

다른 Scene에서 재사용:
1. Prefab을 Hierarchy로 드래그
2. Inspector에서 Target Scene Name만 변경
3. 끝!

## Scene별 작업 예시

### Tutorial Scene (튜토리얼)

```
필요한 오브젝트:
- Player (Prefab에서 드래그)
- Door_To_F1_Corridor (Target: "F1_Corridor")
```

### F1_Corridor Scene (1층 복도)

```
필요한 오브젝트:
- Player (Prefab에서 드래그)
- Door_To_F1_Room01 (Target: "F1_Room01") ← 방1 입구
- Door_To_F1_Room02 (Target: "F1_Room02") ← 방2 입구
- Door_To_F1_Room03 (Target: "F1_Room03") ← 방3 입구
- Door_To_F2_Corridor (Target: "F2_Corridor") ← 2층 계단
```

### F1_Room01 Scene (1층 방1)

```
필요한 오브젝트:
- Player (Prefab에서 드래그)
- Door_To_F1_Corridor (Target: "F1_Corridor") ← 나가기
```

## 스크립트 재사용 정리

### ✅ 자동으로 재사용되는 것들 (중복 작성 불필요)

1. **SceneLoader** (싱글톤)
   - 게임 시작 시 자동 생성
   - 모든 Scene에서 `SceneLoader.Instance` 사용 가능

2. **PlayerMovement** (Prefab에 포함)
   - Player Prefab 사용 시 자동 적용

3. **SceneTransitionTrigger** (Prefab에 포함)
   - Door Prefab 사용 시 자동 적용
   - Scene Name만 변경하면 됨

4. **CameraFollow** (Prefab에 포함)
   - Player Prefab 사용 시 자동 적용

### ⚠️ Scene마다 수동 설정 필요한 것

1. **문의 Target Scene Name**
   - 각 문마다 이동할 Scene 이름 다름
   - Inspector에서 변경 (1초 소요)

2. **오브젝트 위치**
   - 각 Scene마다 레이아웃 다름
   - Transform으로 위치 조정

## 협업 시 작업 분담 예시

### 개발자 A - Floor1 담당
```
작업 Scene:
- F1_Corridor.unity
- F1_Room01.unity
- F1_Room02.unity
- F1_Room03.unity

사용 Prefab:
- Player (공통)
- Door (공통, Scene 이름만 변경)

추가 작업:
- 1층 컨셉에 맞는 배경 추가
- 퍼즐/아이템 배치
```

### 개발자 B - Floor2 담당
```
작업 Scene:
- F2_Corridor.unity
- F2_Room01.unity
- F2_Room02.unity
- F2_Room03.unity

사용 Prefab:
- Player (공통)
- Door (공통, Scene 이름만 변경)

추가 작업:
- 2층 컨셉에 맞는 배경 추가
- 퍼즐/아이템 배치
```

### 개발자 C - Floor3 담당
```
작업 Scene:
- F3_Corridor.unity
- F3_GuestRoom01.unity
- F3_GuestRoom02.unity
- F3_GuestRoom03.unity

사용 Prefab:
- Player (공통)
- Door (공통, Scene 이름만 변경)

추가 작업:
- 3층 손님방 컨셉 배경 추가
- 퍼즐/아이템 배치
```

## Prefab의 장점

### 1. 일관성 유지
```
플레이어 이동 속도 변경:
- Prefab 수정 1번
- 모든 Scene 자동 업데이트 ✅

중복 작성 방식:
- 15개 Scene 전부 수정 필요 ❌
```

### 2. 버그 수정
```
플레이어 충돌 버그 발견:
- Prefab 수정 1번
- 모든 Scene 자동 수정 ✅

중복 작성 방식:
- 15개 Scene 전부 수정 필요 ❌
```

### 3. 기능 추가
```
플레이어에 점프 기능 추가:
- Prefab에 스크립트 추가 1번
- 모든 Scene 자동 적용 ✅

중복 작성 방식:
- 15개 Scene 전부 추가 필요 ❌
```

## FAQ

**Q: Scene마다 플레이어 스크립트를 다시 작성해야 하나요?**
A: 아니요! Prefab을 사용하면 한 번만 작성하고 모든 Scene에서 재사용합니다.

**Q: 각 Scene마다 문을 새로 만들어야 하나요?**
A: 기본 구조는 Prefab 재사용, Target Scene Name만 변경하면 됩니다. (5초 소요)

**Q: Prefab을 수정하면 기존 Scene들은 어떻게 되나요?**
A: 자동으로 모두 업데이트됩니다! 이게 Prefab의 핵심 기능입니다.

**Q: Scene마다 플레이어 시작 위치를 다르게 할 수 있나요?**
A: 네! Prefab 인스턴스마다 Transform은 독립적으로 설정 가능합니다.

**Q: 협업 시 Prefab 충돌이 발생하나요?**
A: Prefab은 별도 파일이므로, Scene 충돌보다 훨씬 적습니다.
   작업 전 git pull만 하면 대부분 문제없습니다.

## 실제 픽셀 아트 적용하기

현재는 테스트용 사각형이지만, 나중에 실제 에셋 적용 시:

### 플레이어 교체
1. Player Prefab 열기
2. Sprite Renderer의 Sprite를 실제 캐릭터 이미지로 변경
3. 저장
4. 모든 Scene의 플레이어가 자동 업데이트!

### 문 교체
1. Door Prefab 열기
2. Sprite Renderer의 Sprite를 실제 문 이미지로 변경
3. 저장
4. 모든 Scene의 문이 자동 업데이트!

## 다음 단계

1. ✅ 기본 셋업 완료 (이 가이드)
2. 🎨 픽셀 아트 에셋 적용
3. 🎵 사운드 추가
4. 🧩 각 방별 퍼즐 구현
5. 📖 스토리/대화 시스템
6. 🎮 UI/메뉴 시스템

---

**요약:**
- ❌ **중복 코드 작성 필요 없음**
- ✅ **Prefab으로 한 번만 만들면 모든 Scene 재사용**
- ✅ **Scene 이름만 바꾸면 문 전환 완료**
- ✅ **협업 충돌 최소화**
