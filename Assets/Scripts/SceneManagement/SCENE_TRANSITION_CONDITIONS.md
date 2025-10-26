# Scene Transition Conditions (씬 전환 조건 시스템)

Door 오브젝트에 조건부 씬 전환 기능을 추가하는 시스템입니다.

## 기본 사용법

### 1. 조건 없이 사용 (기본)

Door 오브젝트에 `SceneTransitionTrigger` 컴포넌트를 추가하면:
- `alwaysAllowTransition`이 기본적으로 `true`로 설정됨
- 플레이어가 가까이 가서 E키를 누르면 즉시 씬 전환

### 2. 조건 추가하기

1. Door 오브젝트 또는 다른 오브젝트에 조건 컴포넌트 추가
2. `SceneTransitionTrigger`의 `alwaysAllowTransition`을 `false`로 변경
3. `Transition Conditions` 배열에 조건 컴포넌트 드래그

```
Door GameObject
├─ SceneTransitionTrigger
│  ├─ Target Scene Name: "NextScene"
│  ├─ Always Allow Transition: false  ← 중요!
│  └─ Transition Conditions:
│     ├─ [0] SimpleBoolCondition (같은 오브젝트 또는 다른 오브젝트)
│     ├─ [1] ItemCollectionCondition
│     └─ [2] ObjectCountCondition
```

**모든 조건이 만족되어야** 씬 전환이 가능합니다.

## 제공되는 조건 컴포넌트

### SimpleBoolCondition
단순 bool 값으로 조건 체크
- 다른 스크립트에서 `conditionMet` 값을 변경
- 또는 `SetConditionMet()` 메서드 호출

```csharp
// 예시: 퍼즐을 풀었을 때
SimpleBoolCondition condition = door.GetComponent<SimpleBoolCondition>();
condition.SetConditionMet();
```

### ItemCollectionCondition
특정 아이템들을 모두 수집했는지 체크
- `requiredItemCount`: 수집해야 할 개수
- `itemTag`: 아이템 태그 (선택사항)

```csharp
// 예시: 아이템 수집 시
ItemCollectionCondition condition = FindObjectOfType<ItemCollectionCondition>();
condition.CollectItem(itemGameObject);
```

### ObjectCountCondition
특정 태그를 가진 오브젝트 개수 체크
- 예: 모든 적 처치 (Enemy 태그 개수 == 0)
- `targetTag`: 체크할 태그
- `requiredCount`: 조건 개수
- `comparisonType`: 비교 방식 (같음/이상/이하 등)

### TimeCondition
일정 시간 경과 체크
- `requiredTime`: 필요한 대기 시간 (초)
- `startOnAwake`: 시작 시 자동 시작

## 사용 예시

### 예시 1: 아이템 3개 수집 후 문 열기

```
Door GameObject
├─ SceneTransitionTrigger
│  ├─ Always Allow Transition: false
│  └─ Transition Conditions: [ItemCollectionCondition]
│
└─ ItemCollectionCondition
   ├─ Required Item Count: 3
   └─ Item Tag: "Key"
```

아이템을 수집할 때:
```csharp
void OnItemCollected(GameObject item) {
    ItemCollectionCondition condition = door.GetComponent<ItemCollectionCondition>();
    condition.CollectItem(item);
}
```

### 예시 2: 모든 적 처치 후 문 열기

```
Door GameObject
├─ SceneTransitionTrigger
│  ├─ Always Allow Transition: false
│  └─ Transition Conditions: [ObjectCountCondition]
│
└─ ObjectCountCondition
   ├─ Target Tag: "Enemy"
   ├─ Required Count: 0
   └─ Comparison Type: Equal
```

### 예시 3: 여러 조건 조합

```
Door GameObject
├─ SceneTransitionTrigger
│  ├─ Always Allow Transition: false
│  └─ Transition Conditions:
│     ├─ [0] ItemCollectionCondition (아이템 3개 수집)
│     ├─ [1] ObjectCountCondition (모든 적 처치)
│     └─ [2] SimpleBoolCondition (보스 처치)
```

## 커스텀 조건 만들기

`ISceneTransitionCondition` 인터페이스를 구현하여 커스텀 조건 생성 가능:

```csharp
using UnityEngine;

public class MyCustomCondition : MonoBehaviour, ISceneTransitionCondition
{
    public bool IsSatisfied()
    {
        // 조건 체크 로직
        return true;
    }

    public string GetDescription()
    {
        // 조건 설명 반환
        return "내 커스텀 조건";
    }
}
```

## 디버깅

- 조건이 만족되지 않았을 때 콘솔에 로그 출력
- `GetDescription()`으로 어떤 조건이 만족되지 않았는지 확인 가능

## 주의사항

1. `alwaysAllowTransition`이 `true`면 모든 조건 무시
2. 조건이 하나라도 만족되지 않으면 씬 전환 불가
3. 조건 컴포넌트는 Door와 같은 오브젝트에 있어도 되고, 다른 오브젝트에 있어도 됨
