# PROJECT_STATE

조사 기준일: 2026-10-02 (Asia/Seoul)
부분 갱신일: 2026-10-04 — 카드 제거 오류, 입력 등록·해제 API, 입력 구독자 7개의 파괴 시 해제 및 UI 선택 이동 공통화 반영. 아래 기존 조사 기록은 전체 재검증 결과가 아니다.

## 순차 개선 진행

### 포커스 종료 알파 페이드 (2026-10-08)

- [FACT] 최종 축소 후 Scale=0을 유지한 채 현재 알파를 0으로 보간하고, 완료 후 마스크 초기화와 기존 리셋 처리를 실행한다. focusFadeDuration 기본값은 0.36초이며 Inspector에서 조절한다. D-018 참조.
- [FACT] 정적 검토와 변경 코드의 git diff --check를 통과했다. Unity 컴파일·Play Mode의 시각 결과는 미검증이다.

### 사망 상태와 사운드 순서 (2026-10-08)

- [FACT] Hazard에서 Die 전환 성공 후 포커스를 시작하고 Die.EnterState에서 사망 사운드를 재생한다. 리셋의 사운드는 제거했으며 리스폰 위치 복원 시 Die에서 Idle로 복귀한다. D-017 참조.
- [FACT] 대체 타입 기반 컴파일·동작 검사 7개를 통과했다. Unity 실행과 실제 오디오 출력은 미검증이다. 복수 캐릭터 및 진행 중 포커스와의 경합은 별도 확인이 필요하다.

### 이름별 사운드 그룹 (2026-10-08, 코드·기존 유효 참조 이전 완료)

- [FACT] AudioManager.audioGroups에 이름과 클립 목록을 등록하고 같은 이름으로 랜덤 재생한다. AudioName은 숫자 지정 없이 Jump/Walk/Die/Switch/CameraSwitch/ItemSound를 사용한다. D-016 참조.
- [FACT] 7개 씬의 10개 인스턴스, 39개 클립 참조를 이전했다. 대체 타입을 이용한 C# 컴파일·동작 검증 10개 및 참조 보존 검사를 통과했다. Unity 실행 검증은 미완료다.
- [UNCERTAIN] AudioMixer.prefab과 SampleScene.unity의 기존 클립 GUID는 대응 에셋을 찾지 못해 그룹을 지정하지 않았다. 해당 audioGroups는 비어 있으며 수동 등록이 필요하다. _Recovery 백업은 이전하지 않았다. Jump용 신규 클립은 임의로 추가하지 않았다.

### 포커스 두 단계 축소 (2026-10-08, 코드 수정 완료·실행 검증 미완료)

- [FACT] EffectManager의 첫 축소는 0.72초, 중간 대기는 0.18초, 추가한 0까지의 축소는 0.36초이다. 마지막 크기 적용 후 한 프레임을 기다리고 기존 초기화·재개·리셋을 실행한다. D-015 참조.
- [FACT] 축소는 unscaled, 대기는 기존 scaled 시간 기준을 유지한다. 정적 검토를 수행했으며 Unity 컴파일·Play Mode에서의 실제 연출은 미검증이다.

### 업로드 감지 영역 부분 이탈 정리 (2026-10-07, 코드 수정 완료·실행 검증 미완료)

- [FACT] UploadStation.OnTriggerStay2D에서 레이어 또는 완전 포함 조건을 벗어난 대상을 detectedList에서 제거하고 남은 개수로 IsDetected를 갱신한다. 오브젝트 자체를 삭제하지 않는다. D-014 참조.
- [FACT] 기존 업로드 보호 조건과 완전 이탈 시 OnTriggerExit2D 처리는 유지했다. 정적 검토와 코드의 git diff --check를 통과했으며 Unity 컴파일·Play Mode는 실행하지 않았다.
- [UNCERTAIN] 부분 이탈·재진입, 복수 대상 중 하나의 이탈, 마지막 대상 이탈 후 업로드 거절은 Unity에서 확인해야 한다. 완전 이탈 후 목록 잔류라는 별도 현상은 해결 여부가 미확인이다.

### 리뷰 1 — 카드 제거와 빈 목록 접근 (2026-10-03, 코드 수정 완료·실행 검증 미완료)

- [FACT] 사용자 선택에 따라 `Card.RemoveCard()`가 인자를 생략하거나 null을 전달받으면 자기 카드를 대상으로 처리하도록 수정했다. 기존 `Action<Card>` 콜백과 NextStep 애니메이션 이벤트는 유지했다.
- [FACT] `CardManager.RemoveCard()`의 null 검사 순서를 바로잡고, `ShowTopCard()`에 빈 목록 검사를 추가했다.
- [FACT] 호출 경로를 정적으로 검토하고 대상 코드의 `git diff --check`를 통과했다. Unity 컴파일·Play Mode 테스트는 실행하지 않았다.
- [UNCERTAIN] 카드 개수 제한 초과 시 제거, NextStep 이벤트, 완료 콜백 및 카드가 없는 상태에서의 조작은 Unity에서 실행 확인이 필요하다.
- [FACT] 구조·의존관계는 변경하지 않아 ARCHITECTURE.md의 카드 책임 설명은 유지한다. 선택 근거와 대안은 DECISIONS.md의 D-002에 기록했다.
- [FACT] 착수 당시 Landing·Stage·Switch 관련 소스에는 기존 작업 트리 변경이 있었으나 아래 조사 기록과 일부 결정 기록은 이전 상태를 기술한다. 이번 작업은 해당 변경의 완료 여부를 확정하지 않으며, 후속 항목 착수 시 코드와 기록을 다시 대조해야 한다.

## 현재 기준점과 작업

- [FACT] 현재 브랜치: `Test-main`. 조사 시 HEAD: `965172b` (merge commit).
- [FACT] 최근 커밋 제목에 `Focus` (`30af7b2`), `Build_Scenes` (`2db66fc`), `Card 정리` (`c88f50f`), `null오류` (`c697229`)가 있다. 제목은 완료 검증의 근거가 아니다.
- [FACT] 이번 작업은 저장소 조사와 `.agent/PROJECT_RULES.md`, `.agent/ARCHITECTURE.md`, `.agent/PROJECT_STATE.md` 작성이다. 코드·씬·설정은 수정 대상이 아니다.
- [UNCERTAIN] 팀이 현재 집중하는 개발 기능, 담당자, 일정 및 마일스톤. 최근 커밋 제목만으로 현재 집중 작업을 확정하지 않는다.

### 조사 시작 전 작업 트리

[FACT] 아래 변경은 이번 문서 작성 이전부터 존재했다.

| Git 상태 | 경로 |
| --- | --- |
| 수정됨 | `Assets/01.Scene/Sample_Stage_TestSH.unity` |
| 수정됨 | `Assets/10. Rendering/FullscreenFocusFeature/FocusMask.mat` |
| 수정됨 | `ProjectSettings/QualitySettings.asset` |
| 미추적 | `AGENTS.md` |
| 미추적 | `.agent/` |

- [FACT] `.agent/`의 세 대상 문서와 DECISIONS.md는 조사 시작 시 모두 0바이트였다. DECISIONS.md는 이번에 수정하지 않는다.
- [FACT] `.agent/PLANS.md`는 조사 시 없었다. [UNCERTAIN] AGENTS.md가 참조하는 ExecPlan 운영 세부사항.

## 구현 현황

[FACT] 다음은 코드가 존재하는 기능 목록이다. 플레이·빌드 검증을 거친 완료 목록은 아니다. 책임과 연결은 ARCHITECTURE.md를 참조한다.

| 상태 | 확인된 구현 | 근거 (`Assets/02.Script/` 기준) |
| --- | --- | --- |
| [FACT] | 2D 이동·점프·추가 속도·리셋 및 캐릭터 상태 전환 | `Character/CharacterControl.cs`, `CharacterAnimation.cs`, `State/*` |
| [FACT] | Stage 진입·퇴장, 조건 검사 및 다음 Stage 전환 | `GameManager.cs`, `StageManager/Stage.cs` |
| [FACT] | 상태별 키 입력, UI 창 스택·키 탐색·열기/닫기 애니메이션 | `InputSystem/*`, `UI/UIManager.cs`, `UIWindow.cs`, `UIBase.cs` |
| [FACT] | 스위치·블록·팝업 토글, 조건 아이템과 문 연결 | `Object/Switch/BlockSwitch.cs`, `Object/Block/Block.cs`, `Object/Spot/Door.cs` |
| [FACT] | Cinemachine 선택·카메라 리셋, blur/focus 연출 | `Camera(Cinemachine)/MainCameraController.cs`, `EffectManager.cs`, `Assets/10. Rendering` |
| [FACT] | 카드 추가·제거·배치 및 최상단 카드 확인 | `UI/CarouselUI/CardManager.cs` |
| [FACT] | 효과음·믹서, CSV 언어 데이터, JSON 팝업 데이터, 영상 리소스 로드 | `Audio/AudioManager.cs`, `LanguageSystem.cs`, `Object/PopUp/PopUpDataManager.cs`, `Video/VideoManager.cs` |
| [FACT] | 로컬 JSON 저장·로드 및 설정/진행 데이터 모델 | `SaveSystem/SaveManager.cs`, `SaveData.cs` |
| [FACT] | Timeline 이벤트 브리지 | `TimelineEvent/*` |

- [UNCERTAIN] 구현 완료로 승인된 기능 목록과 완료 조건. 테스트 결과나 승인 기록을 확인하지 못했다.
- [UNCERTAIN] 공식적으로 구현 중인 작업 목록. 빈 메서드·부분 구현은 확인되지만 현재 담당자가 개발 중이라는 근거는 없다.
- [UNCERTAIN] 아래 사용자가 지정한 예정 작업 외의 구현 예정 기능·우선순위·로드맵. 빈 메서드나 주석을 확정 계획으로 취급하지 않는다.

## 구현 예정

### 입력·이벤트 등록 및 해제 수명 정비 (2026-10-04, 입력 사용처 해제 반영·기타 이벤트 정비 미완료)

- [FACT] 최초에는 계획만 기록했다. 2026-10-03 사용자가 제시된 수정안을 승인하여 InputSystem의 키·축 등록 해제, null 등록 차단, 키 목록 복사 기반 순회와 입력 맵 전환 시 다음 항목 중단을 구현했다. 중복 등록 허용은 유지한다. DECISIONS.md D-004 참조.
- [FACT] 실제 소스를 Unity API 대체 구현과 함께 C# 컴파일한 격리 테스트 7개 시나리오 그룹 및 git diff --check를 통과했다. Unity 컴파일·Play Mode는 미실행이다.
- [FACT] 2026-10-04 사용자 요청으로 CharacterControl, GameManager, UIManager 및 입력을 등록하는 Switch 4종에 등록 당시 InputSystem 참조와 OnDestroy 해제를 추가했다. 기존 Start 경로 등록과 비활성 상태의 동작은 유지한다. 총 7개 구독자·8개 바인딩이며 리셋 등 기타 이벤트 해제는 이번 범위에 포함하지 않았다. DECISIONS.md D-005 참조.
- [FACT] 실제 입력 API와 사용처에서 추출한 등록·해제 코드를 Unity API 대체 구현으로 검증하여 28개 격리 시나리오를 통과했다. 다른 구독자 보존, 반복·미등록 해제, 입력 시스템 선행 파괴, 싱글턴 교체 후 원래 대상 해제를 확인했다. 전체 Unity 컴파일·Play Mode 검증은 미실행이다.
- [PROPOSAL] 진행 순서:
  1. 입력·이벤트 등록/해제 메서드를 정비한다. 키·축 해제와 중복 등록 처리, 콜백 실행 중 등록 변경의 안전성을 함께 검토한다.
  2. 프로젝트 전체 코드의 입력 등록, `Action`·이벤트 구독, 직접 콜백 목록을 조사하고 소유자·구독자의 수명과 해제 경로를 확인한다.
  3. 각 객체의 역할에 맞게 등록·해제를 연결한다. 활성 상태에서만 반응할 객체와 비활성 상태에서도 리셋 등을 받아야 하는 객체를 구분하며, 모든 구독을 일괄적으로 `OnDisable`에서 해제하지 않는다.
  4. 반복 활성화·비활성화, 파괴·재생성, 씬 전환, 언어 변경·리셋 후 중복 호출과 파괴된 객체 접근 여부를 검증한다. 필요하면 Profiler로 구독 누적과 관리 메모리·프레임 영향을 확인한다.
- [PROPOSAL] 완료 조건: 조사한 사용처마다 등록과 해제 수명이 대응하고, 반복 사용 시 콜백이 누적되지 않으며, 비활성 객체가 받아야 할 이벤트가 유지된다. 실제 변경 시 `DECISIONS.md`에 이유와 영향을 기록하고 관련 구조·상태 문서를 갱신한다.
- [UNCERTAIN] 현재 누락으로 인한 실제 메모리 증가·프레임 저하는 측정하지 않았다. 기타 이벤트의 객체별 해제 시점과 후속 구현 범위는 사용처 조사 후 결정한다.

### 다운로드 지점별 복제 블록 관리 정비 (2026-10-07, 빈 업로드 스위치 동작 보완·Unity 실행 검증 미완료)

- [FACT] 사용자는 여러 지점 연결을 유지하되 한 곳에서 다운로드하면 회수·리셋 전까지 다른 지점을 차단하는 정책을 선택했다. UploadStation이 선택을 기록하고 DownloadStation의 실제 활성화 요청을 승인한다.
- [FACT] 미리보기는 (원본, 지점)별 별도 복제본을 사용한다. 선택한 지점만 물리를 활성화하고 다른 지점은 미리보기로 남는다. 입력 스위치와 직접 호출 모두 재다운로드를 차단한다.
- [FACT] 회수는 감지 목록이 비어 있어도 원본·복제본 상태와 선택을 정리한다. 새 업로드에서 다른 지점을 선택할 수 있다. 리셋은 전체 캐시의 복제본을 파괴하고, 파괴된 캐시 객체는 다음 생성 시 재생성한다.
- [FACT] 두 실제 클래스 전체와 Unity 대체 구현을 사용한 격리 검증 41개 조건 및 git diff --check를 통과했다. 수정 전·후와 검증 스크립트는 .agent/reviews/single-download-20261004-163356/에 보관했다. DECISIONS.md D-009 참조.
- [FACT] 후속 수정으로 BlockSwitch는 하나 이상의 대상이 승인한 뒤에만 상태·이벤트·소리·회전 연출을 실행한다. UploadStation의 회수 요청은 실제 업로드 상태일 때만 승인한다. 기존 회귀 검사 41개와 추가 승인 검사 15개를 통과했다. D-013 및 .agent/reviews/switch-approval-20261007-040926/ 참조.
- [FACT] 상위 업로드 취소를 하위 복제의 업로드 지점에 전파하는 구현은 없다. 사용자가 연쇄 복제 문제는 일단 보류했으며 이번 수정에 포함하지 않았다.
- [UNCERTAIN] Unity 컴파일·Play Mode의 물리·트리거·회수 애니메이션 및 전체 씬의 partnerStation 연결은 미검증이다.
- 근거: `Assets/02.Script/Object/Spot/UploadStation.cs`, `Assets/02.Script/Object/Spot/DownloadStation.cs`.

### UI 애니메이션 시간 보간 공통화 (2026-10-04, 카드 재배치 및 UI·팝업·효과 일부 적용)

- [FACT] UIAnimationRoutine.Run은 이미 작성되어 있었지만 사용처는 없었다. 2026-10-04 수정 요청으로 CardManager.RefreshCardCoroutine의 반복문 한 곳에 적용했다. 시작 설정·목표 위치 계산·최종 위치 보정은 유지하고 공통 함수는 변경하지 않았다. DECISIONS.md D-007 참조.
- [FACT] 수정 전·후 전체 소스와 비교 화면은 .agent/reviews/card-animation-20261004-154819/에 임시 저장했다. 실제 공통 함수와 추출 코루틴의 격리 검증에서 지속 시간 4개 시나리오·22개 조건 및 이동 범위 검사, git diff --check를 통과했다. Unity 실행·취소·동시 실행 검증은 미완료다.
- [PROPOSAL] `UIBase`, `UIBaseUpgrade`, `CarouselElement` 등의 반복되는 시간 진행·보간 곡선 처리를 작은 공통 함수로 분리한다. 시작·종료 위치, 크기·alpha 적용, Animator 호출, 완료 이벤트 등 UI별 연출과 책임은 각 컴포넌트에 유지한다.
- [PROPOSAL] 기존 클래스 전체를 하나의 상속 구조로 통합하지 않고 UI 한 종류부터 적용해 기존 연출과 비교한 뒤 확대한다. 진행 중 코루틴의 취소·재시작 책임도 해당 UI가 유지한다.
- [PROPOSAL] 검증 항목: 열기·닫기 정상 완료, 도중 반대 동작 요청, 반복 실행, 비활성화, 지속 시간 0, 첫 프레임 대기 및 완료 이벤트 시점. 시간 배율 변경 시 동작도 확인하며 기존 scaled/unscaled 시간 선택을 일괄 변경하지 않는다.
- [FACT] 후속 승인으로 UIBase 2개, UIBaseUpgrade 3개, OriginalPopUp 2개, Card 1개, EffectManager 2개, TestBlur 1개 등 11개 코루틴에 추가 적용했다. 변경 이유와 정책 차이는 DECISIONS.md D-008, 수정 전·후는 .agent/reviews/animation-batch-20261004-161620/ 참조.
- [FACT] 실제 코루틴 추출 코드와 공통 함수의 격리 C# 검증 39개 실행 시나리오 및 git diff --check를 통과했다. Unity 컴파일·실제 연출·중단·동시 실행은 미검증이다.
- [UNCERTAIN] 후속 이전 범위와 일정, scaled 시간 지원 및 중간 Animator 실행을 위한 API 확장 여부는 미확정이다. CarouselElement·TodoList·InGameTodo 및 scaled 시간 기반 후보 등은 아직 이전하지 않았다.
- 관련 경로: `Assets/02.Script/UI/UIBase.cs`, `Assets/02.Script/UI/UIBaseUpgrade.cs`, `Assets/02.Script/UI/CarouselUI/CarouselElement.cs`. 실제 구현·설계 확정 시 `DECISIONS.md`에 선택 이유와 영향을 기록하고 관련 구조·상태 문서를 갱신한다.

### F — UI 선택 이동 공통화 (2026-10-04, 코드 수정 완료·Unity 실행 검증 미완료)

- [FACT] 사용자 승인으로 UIKeyNavigator의 목록 이동을 MoveSelection(int direction)으로 공통화했다. 공개 이동 메서드 4개와 Carousel 값 변경 경로는 유지한다.
- [FACT] null·빈 목록은 이동하지 않고 null 항목은 최대 한 바퀴 탐색하며 건너뛴다. 현재 선택이 없거나 목록에 없으면 다음은 첫 유효 항목, 이전은 마지막 유효 항목을 선택한다. Initialize도 첫 유효 항목을 선택한다. 유효 항목이 없으면 기존 선택을 유지하며, 단일 항목 재선택 콜백은 기존처럼 실행한다.
- [FACT] 실제 UIKeyNavigator 소스와 의존성 대체 구현을 사용한 격리 C# 검증 19개 조건 및 git diff --check를 통과했다.
- [UNCERTAIN] Unity Editor 컴파일·Play Mode의 실제 UI 연출과 파괴된 객체 처리는 미검증이다.
- 관련 경로: `Assets/02.Script/UI/UIKeyNavigator.cs`. 선택 이유와 검증 범위는 DECISIONS.md D-006 참조. 클래스 책임·외부 의존관계는 유지한다.

### G — 언어 데이터 로딩 정리 (2026-10-06, 승인된 최소 변경 완료·Unity 실행 검증 미완료)

- [FACT] 사용자 요청으로 Awake 재작성은 제외했다. Instance 자동 생성 경로의 추가 LoadLocalizationData 호출과 미사용 WebRequest 코루틴 2개·관련 using을 제거했다.
- [FACT] Awake·LoadLocalizationData·CSV 파싱이 수정 전과 동일함을 확인했다. 기존 동기 로딩, GetText, 언어 변경 이벤트, WebGL 분기는 유지한다. Assets 전체 참조 검색과 git diff --check를 수행했다. DECISIONS.md D-011 참조.
- [UNCERTAIN] Unity 컴파일·Play Mode 및 실제 플랫폼별 로딩은 미검증이다. 비동기 전환·플랫폼별 정책 재설계는 이번 승인 범위에 포함하지 않았으며 확정된 후속 구현으로 취급하지 않는다.
- 관련 경로: `Assets/02.Script/LanguageSystem.cs`. 수정 전·후는 .agent/reviews/language-minimal-20261006-031500/에 보관했다.

## 코드에서 확인된 문제·미완성 지점

[FACT] 아래 항목은 정적 코드 상태다. [UNCERTAIN] 실제 씬에서의 호출 여부, 발생 빈도 및 사용자 영향은 실행 검증 전에는 확정할 수 없다.

| 상태 | 확인 사항 | 근거 (`Assets/02.Script/` 기준) |
| --- | --- | --- |
| [FACT] | 키·축 DeregisterAction은 2026-10-03 구현했으며 2026-10-04 입력 구독자 7개의 OnDestroy 해제를 연결했다. initialize는 여전히 비어 있고 PadDefaultSet은 빈 사전을 반환한다. | `InputSystem/InputSystem.cs`, `InputSystem/ActionID.cs` 및 위 입력 수명 정비 기록 |
| [FACT] | UIWindow.sortingOrder getter는 항상 0, setter는 test 필드만 바꾼다. Canvas 정렬값 연결은 주석 처리되어 있다. | `UI/UIWindow.cs` |
| [FACT] | EventBehaviour.ProcessFrame은 target이 null이면 먼저 반환하므로, 뒤쪽 else의 onPlay 호출에 도달하지 않는다. | `TimelineEvent/EventBehaviour.cs` |
| [FACT] | CharacterStateSheet는 Landing을 `new Landing()`으로 생성한다. Landing의 machine을 설정하는 코드가 이 생성 경로에 없지만 EnterState에서 machine을 사용한다. | `Character/State/CharacterStateSheet.cs`, `Landing.cs` |
| [FACT] | ConditionItem.Satisfied setter는 satisfied 대입 전에 OnCheck를 호출한다. ConditionItem은 기본 IsSatisfied를 재정의하지 않으며 기반 구현은 예외를 던진다. | `Object/Token/ConditionItem.cs`, `Interface/ClearCondition.cs` |
| [FACT] | CharacterControl.OnDetected(), 일부 Switch 프로퍼티 및 토큰·스위치 메서드에 NotImplementedException이 남아 있다. | 예: `Character/CharacterControl.cs`, `Object/Block/MessageBlock.cs`, `Object/Block/RetweetBlock.cs`, `Object/Token/OpenDoorItem.cs` |
| [FACT] | CharacterControl의 입력 해제는 구현했으나 리셋 관련 구독 해제는 아직 없다. | `Character/CharacterControl.cs` |
| [FACT] | SaveManager 외부의 저장 시스템 호출을 Assets C# 검색에서 확인하지 못했다. 파일 I/O·JSON 파싱 예외를 처리하는 try/catch도 없다. | `SaveSystem/SaveManager.cs` 및 Assets C# 검색 |
| [FACT] | InteractActionDrawer.cs는 Editor 폴더 밖에 있으며 UnityEditor/PropertyDrawer를 사용하고 UNITY_EDITOR 조건부 보호가 없다. | `CameraBound/InteractActionDrawer.cs` |
| [UNCERTAIN] | 위 Editor 의존성이 Player 빌드에 미치는 실제 결과. 이번 조사에서 빌드는 실행하지 않았다. | 정적 소스 조사만 수행 |

### 밀기 중 Stat 교체 후 이동속도 덮어쓰기 (2026-10-02 기록, 미해결)

- [FACT] `Push.EnterState()`는 진입 당시 `currentStat.moveSpeed`를 `speed`에 저장한다. 밀기 중 `CharacterControl.OnDetected(CharacterStat stat)`가 일치하는 Stat을 찾으면 `_currentStat` 참조를 교체한다. 이후 `Push.ExitState()`는 교체 여부를 확인하지 않고 현재 Stat의 `moveSpeed`에 저장된 `speed`를 대입한다.
- [INFERENCE] 위 순서로 실행되면 새 Stat의 이동속도가 밀기 진입 전 Stat의 이동속도로 덮어써진다. 예: 속도 10인 Stat에서 밀기 시작 → 속도 5인 Stat으로 교체 → 밀기 종료 시 새 Stat의 속도가 10으로 변경된다.
- [UNCERTAIN] 실제 씬에서의 재현 여부와 발생 빈도는 실행 검증하지 않았다.
- [FACT] 이번에는 문제만 기록했다. 코드 수정은 없으며 해결 방식은 미정이다. 이동속도 배율 분리 제안은 사용자가 폐기했으므로 적용 예정으로 취급하지 않는다.
- 근거: `Assets/02.Script/Character/State/Push.cs`의 `EnterState`·`ExitState`, `Assets/02.Script/Character/CharacterControl.cs`의 `OnDetected(CharacterStat stat)`.

## 임시·부분 구현

- [FACT] `Character/CharacterAbility.cs`의 EnterAbility/UpdateAbility, `EffectManager.cs`의 TurnOffFocus, `UI/CarouselUI/CardManager.cs`의 HideCardList/HideTopCard는 비어 있다.
- [FACT] `Character/State/Slow.cs`는 빈 파생 클래스이고 Die는 기반 구현을 호출한다. 각 상태가 완성된 게임 동작을 제공한다고 판단하지 않는다.
- [FACT] `TestHardCoding.cs`의 클래스명은 TestHardCoing이며 Start에서 게임 시작, Update에서 Q/R/T 등의 카드 조작을 직접 수행한다. 실제 적용 씬은 이번 조사에서 확정하지 않았다.
- [FACT] `GameReadyProtocol.cs`에는 F1/F2/F3 해상도·화면 모드 전환 코드가 있다.
- [FACT] `OldScript` 폴더 및 Test/Temp 이름의 스크립트가 존재한다. [UNCERTAIN] 각 파일의 폐기 예정 여부와 현재 씬 사용 여부. 폴더명만으로 미사용이라고 판단하지 않는다.

## 검증 범위와 남은 불확실성

- [FACT] AGENTS.md, README, 기존 .agent 문서 상태, Unity 버전, 패키지 manifest/lock, Build Settings, 주요 런타임 코드와 일부 씬 이벤트를 확인했다.
- [FACT] 주요 직접 패키지 버전은 manifest/lock 간 일치했다. 기존 .agent 문서가 비어 있어 기존 구조 설명과 코드 사이의 충돌은 확인되지 않았다.
- [FACT] Assets C# 검색에서 `[Test]`·`[UnityTest]` 속성은 확인되지 않았다. Test 이름의 MonoBehaviour만으로 자동화 테스트라고 판단하지 않는다.
- [FACT] Unity Editor, Play Mode, 컴파일, Player 빌드, 자동화 테스트는 실행하지 않았다. 분석 과정에서 프로젝트 코드나 에셋을 수정하지 않는다.
- [UNCERTAIN] 씬·프리팹 전체의 참조 무결성, 지원 플랫폼, 저장/이어하기 통합, 언어별 번역 완성도, WebGL 실행 호환성, 입력 모드 전환과 구독 수명 안전성, 최종 배포 가능 상태.
