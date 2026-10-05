# ARCHITECTURE

조사 기준일: 2026-10-02 (Asia/Seoul)

## 범위

- [FACT] 현재 작업 트리의 코드·설정·일부 씬 직렬화 내용을 정적으로 조사한 문서다. 핵심 게임 코드는 `Assets/02.Script`에 있다.
- [UNCERTAIN] 모든 씬·프리팹의 참조 무결성, 실행 순서 및 실제 플레이 결과. Unity 실행·컴파일·빌드는 수행하지 않았다.
- [FACT] 아래 경로는 별도 표기가 없으면 `Assets/02.Script/` 기준이다. 현재 구현만 기술하며 미래 설계를 포함하지 않는다.

## 시스템별 책임과 의존관계

| 상태 | 요소 / 근거 파일 | 현재 책임 및 직접 연결 |
| --- | --- | --- |
| [FACT] | `GameManager.cs` | 현재·시작 Stage, 게임 상태 필드, 씬 전환, 초기화·리셋 콜백을 소유한다. `InputSystem`, `UIManager`, `UIWindow`, `Stage`, `SceneManager`를 참조한다. |
| [FACT] | `StageManager/Stage.cs` | `ClearCondition` 목록과 다음 Stage를 소유한다. 조건 확인 콜백을 구독하고 모두 만족하면 다음 Stage로 전환한다. 진입·퇴장 `UnityEvent`를 제공한다. |
| [FACT] | `Character/CharacterControl.cs` | 플레이어 역할의 이동·점프·접지 검사·방향·추가 속도·리셋을 처리한다. `Rigidbody2D`, `Stat`, 자체 입력, GameManager, 카메라에 의존한다. |
| [FACT] | `Character/CharacterAnimation.cs`, `Character/State/*` | 상태 ID→상태 객체 사전을 소유하며 `OnUpdateState` 결과에 따라 Exit/Enter를 호출한다. 상태는 이동·점프·낙하 등 캐릭터 값을 읽고 Animator와 카메라를 제어한다. |
| [FACT] | `InputSystem/InputSystem.cs`, `KeyBinding.cs` | 입력 상태별 KeyCode→Action, 축 이름→Action<float> 매핑을 보관하고 Update에서 현재 맵만 실행한다. |
| [FACT] | `UI/UIManager.cs` | 타입별 IUI 등록 사전과 열린 UI의 LinkedList를 관리한다. Push/Pop으로 입력 권한을 전환하고 마지막 창 닫기 및 Play_Key 복귀를 처리한다. |
| [FACT] | `UI/UIBase.cs`, `UI/UIWindow.cs` | UIBase는 CanvasGroup·RectTransform 열기/닫기 애니메이션을 담당한다. UIWindow는 IUI 구현, 매니저 Push/Pop, 키 탐색, 표시 이벤트와 시퀀스 실행을 추가한다. |
| [FACT] | `UI/CarouselUI/CardManager.cs` | Card 목록·개수 제한·배치 애니메이션을 관리하고 최상단 카드의 완료 확인을 요청한다. RefreshCardCoroutine의 시간 진행·보간은 UIAnimationRoutine.Run에 위임하며 위치 계산·적용은 CardManager가 담당한다. |
| [FACT] | `Object/Switch/Switch.cs`, `BlockSwitch.cs` | 스위치 공통 기반과 E 입력 상호작용을 제공한다. BlockSwitch는 트리거 범위 조건을 검사하고 등록된 ISwitchable 대상의 SwitchOn을 호출한다. |
| [FACT] | `Object/Block/Block.cs` | 블록 공통 리셋 구독, 머티리얼 애니메이션, 연결된 IEventListener 목록 및 우선순위별 토글 실행을 담당한다. |
| [FACT] | `Object/Spot/UploadStation.cs`, `DownloadStation.cs` | UploadStation이 원본·지점별 미리보기 캐시와 업로드별 단일 다운로드 선택을 소유한다. DownloadStation은 partnerStation의 TrySelectDownload 승인 후 자기 블록의 물리를 활성화한다. 회수는 선택과 현재 목록을 정리하고, 리셋은 전체 캐시를 파괴한다. 선택되지 않은 지점의 미리보기는 물리 비활성으로 유지한다. D-009 참조. |
| [FACT] | `Interface/ClearCondition.cs`, `Object/Spot/Door.cs` | ClearCondition은 조건 확인 콜백의 기반이다. Door는 OpenDoorItem 목록의 만족 여부에 따라 문 Collider를 활성화한다. |
| [FACT] | `Camera(Cinemachine)/MainCameraController.cs` | 캐릭터 상태가 요청하는 카메라 선택을 모아 LateUpdate에서 PlatformerCamera2D에 전달한다. 리셋 시 활성 가상 카메라의 이전 상태를 무효화한다. |
| [FACT] | `EffectManager.cs` | Volume별 blur 설정과 FocusMaskController를 관리한다. 포커스 연출에서 CharacterControl 정지·재개 후 GameManager.ResetGame을 호출한다. |
| [FACT] | `Audio/AudioManager.cs` | AudioClip·AudioMixerSnapshot 이름 사전, Music/SFX 볼륨 및 효과음 재생을 관리한다. enum의 문자열과 에셋 이름으로 조회한다. |
| [FACT] | `LanguageSystem.cs` | StreamingAssets/textSetting.csv를 언어별 사전으로 파싱하고 키 조회·언어 변경 이벤트를 제공한다. WebGL 조건부 코드에 한국어 생성 데이터 시트 로딩이 있다. |
| [FACT] | `Object/PopUp/PopUpDataManager.cs` | StreamingAssets/PopupData.json을 읽어 키별 PopUpData 사전을 생성한다. |
| [FACT] | `SaveSystem/SaveManager.cs`, `SaveData.cs` | 순수 C# 싱글턴이 설정·진행 데이터 모델을 소유하며 persistentDataPath/save_data.json에 JsonUtility와 File API로 저장·로드한다. |
| [FACT] | `Video/VideoManager.cs` | 순수 C# 싱글턴이 VideoDataSheet의 리소스 경로 사전을 참조하여 Resources.Load<VideoClip>을 호출한다. |
| [FACT] | `TimelineEvent/*` | EventTrack→EventClip→EventBehaviour→TimelineEventBridge를 통해 Timeline 실행을 UnityEvent로 연결한다. 클래스 TimelineEventBridge의 실제 파일명은 `TimelineEventBrigde.cs`이다. |

## 주요 실행 흐름

### 시작·스테이지·리셋

1. [FACT] `GameManager.Start()`는 현재 상태를 `OnGameStateChanged`로 알리고 Escape를 등록한 뒤 `sceneStart`를 호출한다.
2. [FACT] `StartGameNow()`는 상태 필드를 Playing으로 설정하고 `NextStage(startStage)`를 호출한다. 이 메서드에서는 `OnGameStateChanged`를 다시 호출하지 않는다.
3. [FACT] `NextStage`는 기존 Stage가 있으면 Exit 후 새 Stage의 Enter를 호출한다. StageEnter는 EnterEvent 후 GameManager.initAction을 호출한다.
4. [FACT] Stage는 Start에서 각 조건의 OnCheck에 StageSatisfied를 등록한다. 모든 IsSatisfied가 참이면 nextStage로 진행하고, 다음 Stage가 없으면 ExitEvent를 호출한다. 자동으로 Ending을 로드하는 코드는 이 분기에 없다.
5. [FACT] `GameManager.Update()`에서 R을 holdTime 동안 누르면 OnReset이 실행된다. CharacterControl은 속도 초기화·시작 위치 복귀 후 한 프레임 뒤 카메라 리셋을 호출한다.

- [FACT] `Assets/01.Scene/SampleScene_Flow.unity`에는 GameManager의 startStage 참조와 `StartGameNow` 직렬화 호출, Stage의 conditionItems가 존재한다.
- [UNCERTAIN] 모든 씬에서의 시작 이벤트 배선과 컴포넌트 Start 실행 순서. 단일 중앙 부트스트랩이 보장된다고 판단할 근거는 없다.

### 캐릭터 입력·상태·카메라

- [FACT] CharacterControl.Awake가 물리·애니메이터 참조를 얻고, Start가 입력·초기화·리셋 콜백을 등록한다. FixedUpdate에서 수평 속도와 추가 속도를 적용한다.
- [FACT] CharacterAnimation.Start는 CharacterStateSheet로 상태 객체들을 생성한다. Update는 현재 상태를 갱신하고 전환 가능한 경우 Exit→Enter를 호출한다.
- [FACT] 예를 들어 Move 상태는 접지·벽 감지·속도를 확인하고 MainCameraController에 좌우 카메라 선택을 전달한다.
- [FACT] 카메라는 `Assets/Samples/Cinemachine/3.1.4/2D Samples/2D Platformer/Platformer Camera 2D.cs`에 정의된 PlatformerCamera2D에 직접 의존한다.
- [FACT] CharacterControl의 OnEnable/OnDisable에 있는 카메라 Rigidbody 등록·해제 호출은 주석 처리되어 있다. 등록 기반 평균 속도 집계가 플레이어에 자동 연결된다고 볼 수 없다.

### UI 및 일시정지

- [FACT] UIBase·UIBaseUpgrade·OriginalPopUp·Card·EffectManager·TestBlur의 열기·닫기 또는 효과 코루틴 11개와 CardManager의 재배치 코루틴은 UIAnimationRoutine.Run에 unscaled 시간 진행·진행률 처리를 위임한다. 보간 곡선과 값 적용, 초기·종료 처리 및 코루틴 관리 책임은 호출 클래스에 남는다. D-007·D-008 참조.
- [FACT] GameManager.GamePause는 열린 UI 수를 확인하고 입력 상태를 Pause로 바꾼 뒤 pauseMenu.Open을 호출한다.
- [FACT] UIWindow.Open/Close는 UIManager.Push/Pop과 UIBase 애니메이션을 호출한다. Pause 상태의 Escape는 UIManager.HideLast에 연결된다.
- [FACT] UIBase 애니메이션은 Time.unscaledDeltaTime을 사용한다. UIWindow.SetVisible은 현재 interactable·blocksRaycasts만 바꾸며 alpha 변경은 주석 처리되어 있다.
- [FACT] GamePause 본문에는 Time.timeScale 변경이나 모든 물리 정지 처리가 없다. [UNCERTAIN] 씬에 연결된 UnityEvent를 포함한 최종 일시정지 범위.

## 이벤트와 수명 관리

- [FACT] 조사한 Assets C# 검색에서 중앙 `EventBus` 구현은 확인되지 않았다. `EventType.cs`는 빈 클래스다.
- [FACT] GameManager의 공개 Action, Stage/UI/Timeline의 UnityEvent, Switch의 Action 및 Block의 IEventListener 호출이 각각 존재한다. 단일 메시지 버스로 통합된 구조로 기록하지 않는다.
- [FACT] GameManager와 InputSystem은 정적 접근 시 씬 객체를 찾고 없으면 생성한다. UIManager는 정적 참조가 없으면 새 GameObject를 생성한다. AudioManager와 MainCameraController는 찾지 못하면 로그를 남긴다.
- [FACT] `Assets/02.Script` 검색에서 DontDestroyOnLoad 호출은 확인되지 않았다. 매니저가 모두 씬 간 영속한다고 가정하지 않는다.
- [FACT] Block.OnDestroy는 GameManager 구독을 해제한다. CharacterControl의 리셋 관련 구독 해제는 아직 없다. 2026-10-03 InputSystem에 키·축 DeregisterAction을 구현했다. 마지막 콜백 제거 시 해당 항목을 삭제하며 null 콜백 등록은 무시한다.
- [FACT] CharacterControl, GameManager, UIManager 및 BlockSwitch·RetweetSwitch·TagRetweetSwitch·DownloadStationSwtich는 Start 경로에서 등록한 입력 8개를 OnDestroy에서 해제한다. 등록 당시 InputSystem 참조를 보관하며 해제 시 싱글턴 접근자를 호출하지 않는다. 비활성화 시 해제하지 않는 기존 동작은 유지한다. DECISIONS.md D-005 참조.
- [FACT] InputSystem.DoAction은 키·축 이름 목록을 복사한 후 각 항목의 최신 콜백을 조회한다. 입력 맵 변경을 다음 항목 앞에서 확인하면 처리를 중단한다. 이미 시작된 Action.Invoke 내부의 나머지 콜백은 중단하지 않는다. 상세 정책과 검증 범위는 DECISIONS.md D-004를 참조한다.
- [UNCERTAIN] 씬 전환·반복 생성 시 전체 구독 및 정적 참조의 안전성. 실제 실행 검증은 하지 않았다.

## 데이터 소유권과 연결 한계

- [FACT] Stat은 ScriptableObject이며 CharacterControl은 최초 currentStat 접근 시 첫 Stat을 Instantiate한다. OnDetected(CharacterStat)는 일치하는 원본 Stat 참조를 직접 대입한다.
- [FACT] SaveData에는 설정과 진행 모델이 있으나, `Assets` 전체 C# 검색에서 SaveManager 외부의 SaveManager/ RegisterStage 사용은 확인되지 않았다.
- [UNCERTAIN] 저장 데이터가 설정 UI·Stage 진행·이어하기에 실제 연결되었는지 여부. 저장 클래스 존재만으로 저장 시스템 통합 완료로 간주하지 않는다.
- [FACT] 씬 목록의 활성 순서는 SampleScene_Flow, Legacy/Build_Level (0) 1, Ending, Title, Build_Scene 1, Build_Scene 2이다. 근거: `ProjectSettings/EditorBuildSettings.asset`.
- [UNCERTAIN] 별도 Build Profile의 적용 여부 및 실제 배포 빌드의 시작 씬. Title이라는 이름만으로 진입 씬이라고 가정하지 않는다.
