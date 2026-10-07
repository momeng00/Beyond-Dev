# PROJECT_RULES

조사 기준일: 2026-10-02 (Asia/Seoul)

## 문서 기준

- [FACT] 이 문서는 `AGENTS.md`에 명시된 준수 규칙과 저장소에서 확인한 기술 기준을 기록한다. 현재 구현 관찰을 새로운 의무 규칙이나 영구적인 설계 결정으로 승격하지 않는다.
- [FACT] 조사 시 이 문서와 `ARCHITECTURE.md`, `PROJECT_STATE.md`, `DECISIONS.md`는 비어 있었다. 기존 설계 결정과 충돌하는 문서 내용은 확인되지 않았다.
- [UNCERTAIN] 기술 버전의 영구 고정 여부, 업그레이드 승인 절차, 별도의 팀 코딩 표준은 확인되지 않았다.

## 반드시 지킬 개발 원칙

다음은 `AGENTS.md`에 명시된 규칙이다.

- [FACT] 정확성 → 사실성 및 환각 방지 → 기존 프로젝트와의 일관성 → 안정성 → 유지보수성 → 확장성 → 구현 편의성 → 토큰 효율성 순으로 판단한다.
- [FACT] 사실은 `[FACT]`, 추론은 `[INFERENCE]`, 제안은 `[PROPOSAL]`, 미확인 사항은 `[UNCERTAIN]`으로 구분한다. 존재하지 않는 API·클래스·프로젝트 결정사항을 만들지 않는다.
- [FACT] 저장소를 주요 Source of Truth로 삼고, 변경 전에 구현·책임·의존관계·데이터 및 이벤트 흐름·생명주기를 확인한다. 코드와 문서의 충돌은 명시한다.
- [FACT] 요구사항을 충족하는 최소 변경을 우선한다. 단순히 더 깔끔해 보인다는 이유로 기존 구조를 변경하지 않는다.
- [FACT] 기능 변경은 관련 코드 조사와 영향 검증을 거친다. 장기 설계는 대안 비교, Unity 생명주기, 책임 중복, 현실적인 실패 사례 최소 3개를 검토한다.
- [FACT] 대규모 기능·구조 변경에는 `.agent/PLANS.md`에 따른 ExecPlan을 사용하도록 규정되어 있다. 다만 현재 해당 파일은 없다. [UNCERTAIN] 프로젝트 고유 ExecPlan 형식과 절차.
- [FACT] 프로젝트 문서 우선순위는 `PROJECT_RULES` → `ARCHITECTURE` → `DECISIONS` → `PROJECT_STATE` → 현재 작업 명세 → 현재 대화 내용이다.
- [FACT] 완료 전 요구사항, 컴파일 오류 가능성, 책임·의존관계, 생명주기, 실패 사례 및 부작용을 검토한다. 실행하지 않은 검증을 성공으로 보고하지 않는다.

## 수정 전 설명과 사용자의 최종 결정권

2026-10-03 사용자 명시 지시: **"최종 결정권은 나에게 준다."** 여기서 '나'는 프로젝트 사용자이며 AI가 아니다.

- [FACT] AI는 수정 전에 현재 문제, 근거 코드와 발생 조건, 수정이 필요한 이유, 구체적인 변경안과 범위, 예상 영향 및 대안을 사용자에게 설명한다. 확인된 사실과 미검증 사항을 구분한다.
- [FACT] 수정 여부와 적용할 방식의 최종 결정권은 사용자에게 있다. 사용자가 해당 변경안과 범위를 선택하거나 적용을 명시적으로 지시한 후에 수정한다. 이미 승인한 동일 범위는 반복 확인하지 않는다.
- [FACT] 리뷰 요청, 예정 작업으로의 기록, 다음 항목 제시 요청만으로 코드 수정이 승인되었다고 해석하지 않는다. 여러 항목을 순차 검토할 때 한 항목의 승인을 다음 항목의 승인으로 확대하지 않는다.
- [FACT] 승인 전에는 읽기 전용 조사와 검토 가능한 수정안 제시를 수행한다. 승인 범위를 벗어나는 추가 동작·구조 변경이 필요하면 그 이유와 변경안을 먼저 설명하고 사용자의 결정을 받는다.
- [FACT] 사용자가 명시적으로 요청한 문서 작성·갱신은 요청 범위에서 수행할 수 있다. 코드 변경이 승인되지 않았다면 제안을 구현 완료나 확정 설계로 기록하지 않는다.

## 설명 문단 구분 방식

2026-10-07 사용자 명시 지시: 순서나 선택지로 제공하는 것이 아니면 문단을 숫자가 아니라 `·`로 구분한다.

- [FACT] 파일별·기능별 설명, 원인 분석, 변경 내용 등 단순히 설명을 나누는 문단에는 숫자 번호 대신 `·`를 사용한다. 하나의 수정안을 여러 부분으로 설명할 때도 같은 규칙을 적용한다.
- [FACT] 숫자 번호는 실제 진행 순서·절차 또는 사용자가 고를 선택지를 제시할 때 사용한다. 단순 설명을 단계나 별도 선택지처럼 보이게 번호로 구분하지 않는다.

## 상세 설명 요청 시 코드 비교와 설명 깊이

2026-10-04 사용자 명시 지시: 자세한 설명을 요청하면 이전 코드와 변경된 코드를 함께 보여주고, 추가 토큰을 사용하더라도 아주 세부적으로 설명한다.

- [FACT] 사용자가 코드 또는 변경 사항에 대해 "자세하게 설명해줘", "상세하게 설명해줘" 등으로 요청하면 이 규칙을 적용한다. 이 경우 간결한 요약이나 비교 파일 링크만으로 답변을 대신하지 않는다.
- [FACT] 설명 대상의 실제 수정 전 코드와 수정 후 코드를 답변에 각각 코드 블록으로 제시한다. 파일·메서드와 비교 기준을 명시하고, 여러 변경은 기능 또는 메서드 단위로 나누어 대응시킨다. 이해에 필요한 주변 코드도 포함하며 핵심 로직을 생략하지 않는다.
- [FACT] 각 변경에 대해 기존 코드의 역할과 실행 순서, 문제의 발생 조건과 원인, 수정 이유, 바뀐 줄·조건·자료구조의 의미, 변경 후 실행 순서와 결과를 아주 세부적으로 설명한다. 필요한 경우 구체적인 입력·상태 예시로 전후 동작을 비교한다.
- [FACT] 관련 호출 관계, 데이터 소유권, 상태 변화, 이벤트·Unity 생명주기, 예외 처리, 유지되는 동작과 달라지는 동작, 영향 범위 및 대안을 설명에 필요한 만큼 포함한다. 검증한 내용과 아직 확인하지 못한 내용도 구분한다.
- [FACT] 이 요청에서는 토큰 절약이나 답변 길이보다 설명의 충분성·정확성·이해 가능성을 우선한다. 추가 토큰을 사용해도 되며, 길이를 줄이기 위해 필요한 코드 비교나 세부 설명을 생략하지 않는다. 다만 관련 없는 배경과 의미 없는 반복은 추가하지 않는다.
- [FACT] 이전 코드는 작업 직전 스냅샷, Git 이력 등 확인 가능한 근거에서 가져온다. 이전 버전을 확인할 수 없으면 [UNCERTAIN]으로 명시하며 추측한 코드를 실제 이전 코드처럼 제시하지 않는다. 미적용 수정안은 [PROPOSAL]로 표시한다.
- [FACT] 자세한 설명 요청 자체는 추가 코드 변경 승인이 아니다. 기존 사용자 최종 결정권 규칙을 유지한다. 사용자가 간략한 설명을 요청하면 그 요청에 맞춰 답한다.

## 기술 기준

- [FACT] Unity Editor: **6000.3.10f1**, revision `e35f0c77bd8e`. 근거: `ProjectSettings/ProjectVersion.txt`.
- [FACT] 프로젝트 식별: README는 `Beyond(가칭) 개발`, Player Settings는 `companyName: DefaultCompany`, `productName: ProjectB`이다. [UNCERTAIN] 확정 제품명과 출시 대상 플랫폼.
- [FACT] 주 게임 코드는 `Assets/02.Script`의 C#이며, `MonoBehaviour`, `ScriptableObject`, `Rigidbody2D`, `Collider2D`, `Animator`를 사용한다. 근거: `Character/CharacterControl.cs`, `Character/Stat.cs` 등.
- [FACT] URP와 VFX Graph는 17.3.0이다. `GraphicsSettings.asset`의 기본 렌더 파이프라인 참조는 GUID 기준 `Assets/Settings/2DRendererAsset.asset`으로 연결된다. 별도 blur/focus 렌더링 코드는 `Assets/10. Rendering`에 있다.

### 직접 지정 패키지

[FACT] 아래 버전은 `Packages/manifest.json`과 `Packages/packages-lock.json`에서 일치한다. 설치 사실이 모든 기능의 실제 사용을 뜻하지는 않는다. 전체 전이 의존성은 lock 파일을 기준으로 확인한다.

| 패키지 | 버전 |
| --- | --- |
| com.unity.ai.navigation | 2.0.10 |
| com.unity.cinemachine | 3.1.4 |
| com.unity.collab-proxy | 2.11.3 |
| com.unity.feature.2d | 2.0.2 |
| com.unity.ide.visualstudio | 2.0.26 |
| com.unity.inputsystem | 1.18.0 |
| com.unity.multiplayer.center | 1.0.1 |
| com.unity.recorder | 5.1.5 |
| com.unity.render-pipelines.universal | 17.3.0 |
| com.unity.test-framework | 1.6.0 |
| com.unity.timeline | 1.8.10 |
| com.unity.ugui | 2.0.0 |
| com.unity.visualeffectgraph | 17.3.0 |
| com.unity.visualscripting | 1.9.9 |

- [FACT] manifest에 나열된 `com.unity.modules.*` 직접 의존성의 버전은 모두 `1.0.0`이다.
- [FACT] `Assets/Samples`에는 Cinemachine·Timeline 등의 샘플 소스가 있다. `MainCameraController`는 Cinemachine 샘플의 `PlatformerCamera2D`를 직접 참조한다. 샘플 폴더가 전부 독립적인 예제인 것은 아니다.

### 입력 및 네트워크

- [FACT] `ProjectSettings/ProjectSettings.asset`의 `activeInputHandler: 2`는 Both 설정이다. 값의 의미는 로컬 Input System 패키지의 `InputSystem/Editor/Settings/EditorPlayerSettingHelpers.cs`에서 `InputBoth = 2`로 확인했다.
- [FACT] 게임의 자체 `InputSystem` 클래스는 Unity 패키지의 동명 네임스페이스와 별개이며, `UnityEngine.Input.GetAxisRaw/GetKeyDown`으로 등록된 콜백을 실행한다. 상태는 `Play_Key`, `Play_Pad`, `Pause`이다. 근거: `Assets/02.Script/InputSystem`.
- [FACT] `CharacterControl`은 Horizontal·Space, `BlockSwitch`는 E, `GameManager`는 Escape를 등록한다. R 리셋과 일부 UI 입력은 직접 `UnityEngine.Input`을 읽는다.
- [FACT] `Assets/InputSystem_Actions.inputactions`가 있고 Build Settings에 입력 액션 참조가 있다. [UNCERTAIN] 새 Input System 액션의 전체 실행 연결 범위와 입력 시스템 통일 계획.
- [FACT] `com.unity.multiplayer.center` 설치는 확인되지만, `Assets`의 C# 검색에서 `Unity.Netcode`, `NetworkBehaviour`, Photon, `Mirror.Network` 기반 게임 네트워크 구현은 확인되지 않았다.
- [UNCERTAIN] 네트워크 채택 방식, 서버 권한 구조, 동기화 방식, 싱글플레이 전용 여부. `LanguageSystem`의 파일 읽기용 `UnityWebRequest` 코드만으로 게임 네트워크 방식을 판단하지 않는다.

## 코딩 규칙과 설계 철학의 확인 범위

- [FACT] 명시된 철학은 기존 책임·의존관계를 존중하고, 검증 가능한 최소 변경을 우선하는 것이다. 근거: `AGENTS.md` 1·4·7·10절.
- [FACT] 현재 구현에는 정적 접근자 기반 매니저, Inspector 참조, 인터페이스, `Action` 및 `UnityEvent`, 캐릭터 상태 클래스가 함께 사용된다. 이는 관찰된 구현이며 필수 아키텍처 규칙은 아니다.
- [FACT] 정적 접근자 이름도 `Instance`와 `instance`가 혼재한다. 근거: `GameManager.cs`, `UI/UIManager.cs`, `EffectManager.cs`.
- [UNCERTAIN] 강제 네이밍·namespace·접근 제한자·주석·비동기·이벤트 구독 해제 규칙. 조사한 저장소 파일 목록에서 `.editorconfig`는 확인되지 않았다.
- [FACT] `.gitignore`는 Library, Temp, Obj, Build, Logs, UserSettings 및 생성된 솔루션·프로젝트 파일 등을 제외한다.
- [UNCERTAIN] 별도의 금지 패키지/API 목록, 성능 예산, 테스트 커버리지 기준, 플랫폼별 빌드 기준. 명시된 근거 없이 추가 규칙으로 만들지 않는다.
