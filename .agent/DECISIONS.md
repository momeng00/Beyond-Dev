프로젝트 결정 기록

## D-014 — 업로드 감지 영역의 부분 이탈 시 감지 목록 정리

날짜: 2026-10-07 (Asia/Seoul)

상태: Accepted — 사용자가 OnTriggerStay2D 수정안 적용을 승인했다.

- [FACT] UploadStation.OnTriggerStay2D는 레이어 조건과 기존 IsFullyContained 검사를 모두 만족한 대상만 detectedList에 유지한다. 조건을 벗어나면 목록에서 제거하고 남은 개수로 Animator의 IsDetected를 갱신한다. GameObject를 파괴하지 않는다.
- 이유: [FACT] 기존 코드는 완전히 포함될 때 등록하지만 부분 이탈 시 제거하는 분기가 없어, 완전히 분리되는 Exit 이벤트 전까지 감지 대상이 남았다.
- 영향: [FACT] 감지 대상이 하나도 남지 않으면 기존 SwitchOn(true)의 빈 목록 검사로 업로드를 거절한다. 다른 대상이 남아 있으면 감지 표시를 유지한다. isUploading 보호 조건과 OnTriggerExit2D, 업로드·회수 흐름은 유지한다.
- 대안: Exit 이벤트만 유지하면 부분 이탈을 처리하지 못한다. 물리 영역 재조회나 Collider별 감지 자료구조 도입은 승인된 최소 변경 범위를 넘어 적용하지 않았다.
- 검증: [FACT] 변경 분기와 기존 호출 흐름을 정적으로 검토하고 코드의 git diff --check를 통과했다. Unity 컴파일·Play Mode 검증은 수행하지 않았다.
- [UNCERTAIN] 완전히 분리된 후에도 남는 별도 현상, 비활성화·텔레포트 및 복수 Collider 구성의 실제 동작은 실행 확인이 필요하다. 이 변경으로 모두 해결되었다고 판단하지 않는다.
- 재검토 조건: 위 별도 현상이 재현되거나 회전된 감지 영역의 정확한 형태 판정이 필요하면 감지 방식과 기존 Bounds 검사를 재검토한다.

## D-013 — 대상 승인 후 스위치 상태·이벤트·연출 확정

날짜: 2026-10-07 (Asia/Seoul)

상태: Accepted — 사용자가 첨부한 비교 수정안 적용을 승인했다.

- [FACT] BlockSwitch.Interact는 nextState를 계산하여 기존 ApplyToTargets로 요청한 뒤 하나 이상의 대상이 성공했을 때만 SwitchState, OnSwitchAction, 효과음, 카메라 회전 및 IsDetected를 실행한다. 대상이 없거나 모두 거절하면 상태·연출을 변경하지 않는다. 여러 대상 중 일부만 성공하는 경우 되돌리지는 않는다.
- [FACT] UploadStation.SwitchOn(false)는 stationState가 false이면 거절한다. 실제 업로드 상태라면 감지 목록이 비어 있어도 회수를 허용한다. ResetAction의 PoolingReturn 직접 호출은 유지한다.
- 이유: [FACT] 기존 BlockSwitch는 대상의 반환값을 받기 전에 상태·이벤트·소리·회전 연출을 실행했다. D-009에서 감지 목록 없이 회수하도록 바꾼 분기가 업로드하지 않은 상태도 성공 처리하여 상태 불일치를 키웠다.
- 영향: [FACT] 실제 대상 처리 이후 OnSwitchAction이 발생하도록 이벤트 순서가 변경된다. 성공 표시도 대상별 반복 호출에서 입력당 한 번으로 바뀐다. Switch.cs의 공통 함수와 기존 회전 코루틴 본문은 변경하지 않았다.
- 대안: [FACT] 모든 요청에 감지 개수 검사를 적용하면 정상 회수까지 막으므로 채택하지 않았다. 전체 대상의 사전 승인·일괄 적용은 사용자 승인안의 범위가 아니므로 도입하지 않았다.
- 검증: [FACT] 실제 BlockSwitch의 Interact·SwitchState 및 Switch.ApplyToTargets를 추출하고 전체 UploadStation/DownloadStation 소스와 함께 대체 API로 컴파일했다. 기존 스테이션 회귀 검사 41개, 스위치 승인 검사 15개를 통과했다. 빈 감지·반복 입력·범위 밖·대상 없음·모두 거절·일부 성공·이벤트 순서·감지 없는 정상 회수·리셋을 확인했고 git diff --check도 통과했다.
- [UNCERTAIN] Unity 컴파일·Play Mode, 실제 TriggerAction 연결·회전 연출·물리 동작은 미검증이다. 연쇄 복제 취소 문제는 이번 수정에 포함하지 않았다.
- [FACT] 수정 전·후와 격리 검증 스크립트는 .agent/reviews/switch-approval-20261007-040926/에 보관했다.
- 재검토 조건: [PROPOSAL] 모든 대상의 원자적 성공이 필요하거나 기존 이벤트가 대상 실행보다 먼저 발생해야 하는 연결이 확인되면 승인·이벤트 정책을 재검토한다.

---

## D-012 — 순서·선택지가 아닌 설명 문단은 가운데점 사용

날짜: 2026-10-07 (Asia/Seoul)

상태: Accepted — 사용자 명시 요청으로 PROJECT_RULES.md에 반영했다.

- [FACT] 순서나 선택지가 아닌 설명 문단은 숫자 대신 `·`로 구분한다. 실제 절차·순서와 선택지를 제시할 때는 숫자를 사용할 수 있다.
- 이유: [FACT] 하나의 수정안을 파일별로 나눈 설명이 숫자 때문에 별도 순서나 선택지로 오해될 수 있어 사용자가 구분 방식 변경을 요청했다.
- 영향: [FACT] 프로젝트 응답 형식 규칙만 추가했다. 게임 코드·구조·동작은 변경하지 않았다.

---

## D-011 — 언어 로딩 최소 정리, Awake 유지

날짜: 2026-10-06 (Asia/Seoul)

상태: Accepted — 사용자가 Awake 재작성 제안을 제외하고 나머지 최소 변경을 승인했다.

- [FACT] LanguageSystem.Instance의 AddComponent 뒤 명시적 LoadLocalizationData 호출을 제거했다. 기존 Awake의 로딩 호출과 Instance 비교를 포함한 본문은 그대로 유지했다.
- 이유: [FACT] 자동 생성 경로와 Awake 양쪽에 있던 로딩 호출을 Awake 쪽에 남긴다. Assets 전체 검색에서 선언 외의 참조가 확인되지 않은 LoadDataWithWebRequest·LoadDataForAndroid와 관련 using System.Collections·UnityEngine.Networking도 제거했다.
- 영향: [FACT] 기존 동기 파일 읽기, CSV 파싱, GetText, 언어 변경 이벤트 및 WebGL 조건부 흐름은 유지한다. 비동기 로딩이나 완료 알림은 추가하지 않았다.
- 대안: [FACT] Awake에서 인스턴스를 먼저 확정하도록 재작성하는 안은 사용자가 제외했다. 플랫폼별 로딩 재설계·코루틴 통합은 이번 최소 변경 범위에 포함하지 않았다.
- 검증: [FACT] 수정 전 스냅샷과 비교하여 Awake·LoadLocalizationData·CSV 파싱 부분이 동일함을 확인했다. 제거 대상 이름은 Assets 전체에서 선언 외 참조가 없었으며, 수정 후 로딩 호출은 Awake에 남아 있다. 대상 파일 git diff --check를 통과했다.
- [UNCERTAIN] Unity 컴파일·Play Mode·실제 플랫폼별 파일 로딩은 미검증이다. 런타임에서 문자열을 조합하는 동적 호출까지 텍스트 검색으로 검증하지는 않았다.
- [FACT] 수정 전·후 원본은 .agent/reviews/language-minimal-20261006-031500/에 보관했다.
- 재검토 조건: [PROPOSAL] 지원 플랫폼 변경, 비동기 로딩 또는 데이터 준비 후 UI 갱신이 필요할 때 별도 요구사항으로 검토한다.

---

## D-010 — 상세 설명 요청 시 전후 코드와 세부 설명 제공

날짜: 2026-10-04 (Asia/Seoul)

상태: Accepted — 사용자 명시 요청으로 PROJECT_RULES.md에 반영했다.

- [FACT] 사용자가 자세한 설명을 요청하면 실제 이전 코드와 변경 코드를 답변에 함께 제시하고, 동작·문제 원인·수정 이유·변경 후 흐름·영향·검증 범위를 아주 세부적으로 설명한다. 비교 링크나 짧은 요약만으로 대체하지 않는다.
- 이유: [FACT] 사용자는 변경 내용을 충분히 이해할 수 있도록 전후 코드 비교와 세부 설명을 요구했으며 이때 추가 토큰 사용을 허용했다.
- [FACT] 해당 요청에서는 토큰 효율보다 설명의 충분성과 정확성을 우선한다. 이전 코드의 근거를 확인하며, 미확인 내용과 미적용 제안을 사실과 구분한다. 상세 설명 요청을 추가 코드 수정 승인으로 확대하지 않는다.
- 영향: [FACT] 프로젝트 응답 규칙만 변경했다. 게임 코드·구조·동작은 변경하지 않아 ARCHITECTURE.md와 PROJECT_STATE.md의 구현 현황은 갱신하지 않는다.
- 대안과 재검토: [FACT] 간결한 요약만 제공하는 방식은 이번 요청에 맞지 않아 채택하지 않았다. 이후 사용자가 설명 깊이나 형식을 지정하면 해당 요청을 따른다.

---

## D-009 — 여러 다운로드 지점 중 한 곳만 사용

날짜: 2026-10-04 (Asia/Seoul)

상태: Accepted — 사용자가 연결 목록을 유지하고 다운로드한 지점을 기록하여 회수·리셋 전까지 다른 지점을 차단하는 방식을 선택했다.

- [FACT] UploadStation이 업로드 상태와 선택된 DownloadStation을 소유한다. DownloadStation.SwitchOn(true)는 기존 partnerStation 참조를 통해 TrySelectDownload 승인을 받은 뒤 자기 블록의 물리를 활성화한다. false 요청, 업로드 전 요청, 미연결 지점, 누락·파괴된 미리보기, 이미 선택된 상태의 반복·다른 지점 요청은 거절한다.
- 이유: [FACT] 기존에는 원본만 키로 사용하여 여러 지점이 동일 복제본을 공유했고, 위치가 마지막 지점으로 덮어써졌다. 각 DownloadStation은 독립적으로 물리를 활성화하여 단일 다운로드 정책을 보장하지 못했다.
- [FACT] 미리보기 캐시는 (원본, 다운로드 지점) 조합으로 관리한다. 각 지점의 미리보기는 별도 객체이며 Kinematic·Collider 비활성 상태다. 선택된 지점만 Dynamic·Collider 활성으로 전환한다. 다른 미리보기는 표시를 유지하되 물리는 비활성이다.
- [FACT] 첫 승인 직후 연결된 다운로드 스위치들의 isUpload와 activate 표시를 해제한다. 중앙 선택 상태도 별도로 검사하므로 직접 호출로 우회하지 못한다. 선택된 Unity 객체가 먼저 파괴되어도 참조 자체가 남아 있으면 잠금을 유지한다.
- [FACT] PoolingReturn은 원본 복원·모든 현재 복제본의 물리 비활성화·다운로드 목록 정리·선택 해제를 함께 수행한다. PushBlock은 기존 회수 애니메이션을 호출하고 일반 블록은 즉시 비활성화한다. SwitchOn(false)는 감지 목록이 비어 있어도 회수를 수행한다. 회수 후 새 업로드에서 다시 선택할 수 있다.
- [FACT] 같은 업로드 중 SwitchOn(true) 재호출은 거절한다. 캐시 재사용 시 선형·각속도를 초기화하며 파괴된 복제본은 다시 생성한다. ResetAction은 현재 지점 목록이 아니라 전체 캐시의 복제본을 파괴한다. 공개 GetDetectedObject는 DownloadStation.SwitchOn을 거쳐 동일 선택 검사를 받는다.
- 대안: [FACT] 연결 자체를 하나로 제한하는 방식과 여러 지점의 동시 실제 다운로드는 사용자의 선택과 달라 채택하지 않았다. 단일 복제본을 모든 지점의 미리보기로 공유하는 방식은 원래 위치 충돌을 유지하므로 사용하지 않았다.
- 영향: [FACT] UploadStation.cs와 DownloadStation.cs만 수정했다. 씬·프리팹의 연결 목록과 기존 partnerStation 필드는 유지한다. DownloadStation의 partnerStation이 없으면 요청을 거절한다. 리셋 이벤트 구독 해제 등 다른 예정 작업은 포함하지 않았다.
- 검증: [FACT] 두 실제 클래스 전체 소스를 Unity·주변 컴포넌트 대체 구현과 함께 C# 컴파일했다. 별도 미리보기·상대 위치·물리 상태, 첫 선택·중복·다른 지점·우회 진입 차단, 감지 목록이 빈 회수, 재사용·속도 초기화, 회수 후 다른 지점 선택, 여러 원본, 선택 지점 파괴, 리셋·반복 리셋·캐시 파괴·재생성 등을 포함한 41개 조건과 git diff --check를 통과했다.
- [UNCERTAIN] Unity 전체 컴파일·Play Mode의 실제 물리, 트리거 순서, PushBlock 애니메이션 및 모든 씬의 양방향 연결 무결성은 미검증이다. 격리 테스트는 이를 대신하지 않는다.
- [FACT] 수정 전·후 원본, 비교 화면 및 재실행 가능한 격리 검증 스크립트는 .agent/reviews/single-download-20261004-163356/에 보관했다.
- 재검토 조건: [PROPOSAL] 선택하지 않은 미리보기도 숨겨야 하거나, 업로드 한 번으로 회수 없이 다른 지점으로 이동해야 하거나, 하나의 다운로드 지점을 여러 업로드 지점이 공유해야 하면 정책을 별도로 검토한다.

---

## D-008 — UI·팝업·효과 코루틴 순차 공통화

날짜: 2026-10-04 (Asia/Seoul)

상태: Accepted — 사용자가 제시된 후보를 차례대로 수정하도록 요청했다.

- [FACT] UIBase 열기·닫기 2개, UIBaseUpgrade 열기 2개·닫기, OriginalPopUp 열기·닫기, Card 닫기, EffectManager 포커스·블러, TestBlur 블러 등 6개 파일의 11개 코루틴에 기존 UIAnimationRoutine.Run을 적용했다. 블러는 시작·목표 값을 선택하여 켜기·끄기 반복문을 하나로 합쳤다.
- 이유: [FACT] 각 코루틴은 기존에도 unscaled 시간 기반 보간을 수행했다. 위치·크기·alpha·효과 값 적용은 각 클래스에 유지하고 시간 진행과 진행률 처리를 공통 함수에 위임한다.
- [FACT] 초기 설정·첫 프레임 대기·후속 Todo 닫기·위치 복원·포커스 대기와 캐릭터 재개·리셋 순서를 유지한다. UIBaseUpgrade.CloseCoroutine의 현재 위치를 매번 읽는 보간도 유지한다. 기존 코루틴 시작·중단 책임과 공통 함수 자체는 변경하지 않았다.
- [FACT] 시작값을 먼저 적용하고 대기하는 순서, 진행률 제한 및 끝값 강제 적용은 공통 함수 정책을 따른다. OriginalPopUp 닫기·TestBlur·포커스는 명시적인 끝값 적용이 추가된다. 0 이하 지속 시간에도 공통 함수가 최종값을 적용하므로 기존 반복문 생략과 적용 호출 횟수가 다르다.
- 대안: [PROPOSAL] 모든 코루틴을 일괄 이전하는 대신 현재 API와 시간 기준이 맞는 후보부터 적용했다. scaled 시간 지원이 필요한 머티리얼·스위치·일부 UI 및 중간 Animator 실행을 포함한 Carousel 등은 후속 검토 대상으로 남긴다.
- 검증: [FACT] 실제 수정 코루틴 11개를 추출하여 실제 UIAnimationRoutine과 함께 C# 컴파일했다. Unity·주변 컴포넌트 대체 구현에서 일반·0·음수 지속 시간, 두 블러 방향을 포함한 39개 실행 시나리오를 완료했다. 블러 끝값 및 포커스 종료 상태를 검사했고 대상 파일 git diff --check를 통과했다.
- [UNCERTAIN] 전체 Unity 컴파일·Play Mode, 실제 프레임 스케줄링·UI 연출·중단·동시 실행·씬 참조는 미검증이다. 추출 테스트는 Unity 실행 검증을 대체하지 않는다.
- [FACT] 작업 직전·직후 원본과 diff 비교는 .agent/reviews/animation-batch-20261004-161620/에 임시 저장했다. 기존 사용자 변경을 포함하는 작업 직전 상태를 기준으로 비교한다.
- 재검토 조건: [PROPOSAL] 프레임별 적용 시점 차이가 연출에 영향을 주거나 scaled 시간·중간 시점 이벤트 지원이 필요하면 공통 함수 계약을 별도로 검토한다.

---

## D-007 — 카드 재배치에 UIAnimationRoutine 최초 적용

날짜: 2026-10-04 (Asia/Seoul)

상태: Accepted — 수정 및 수정 전·후 임시 보관 요청에 따라 앞서 추천한 첫 적용 대상에 반영했다.

- [FACT] CardManager.RefreshCardCoroutine의 시간 누적·Sin 보간 반복문을 기존 UIAnimationRoutine.Run 호출로 교체했다. 공통 함수 자체는 수정하지 않았다.
- 이유: [FACT] 공통 함수는 이미 존재했지만 Assets C# 검색에서 사용처가 없었고, 카드 재배치는 unscaled 시간으로 위치만 변경하므로 첫 적용 대상으로 선정했다.
- [FACT] 기존 첫 프레임 대기, 카드 간격·목표 위치 계산, Sin 곡선, 종료 위치 보정은 유지한다. 시작 시 apply(0), 대기 후 시간 증가, 진행률 제한과 끝값 적용은 공통 함수의 정책을 따른다. 기존 반복문과 프레임별 적용 순서가 완전히 같지는 않다.
- 대안: [PROPOSAL] 모든 UI에 일괄 적용할 수 있지만 scaled 시간, Animator 중간 실행, 현재값 기반 보간 등 개별 차이가 있어 이번에는 한 곳만 적용했다. 기존 동시 재배치 코루틴의 중복 실행·취소 정책은 변경하지 않았다.
- 영향: [FACT] CardManager가 UIAnimationRoutine에 의존한다. 다른 애니메이션 사용처 이전은 남아 있다.
- [FACT] 이번 작업 직전·직후 전체 소스를 원본 바이트로 .agent/reviews/card-animation-20261004-154819/에 .cs.txt 파일로 저장했다. comparison.html은 변경 구간을 나란히 표시한다. Assets 밖에 보관해 복제 스크립트가 Unity 컴파일 대상에 들어가지 않도록 했다.
- 검증: [FACT] 실제 공통 함수와 추출한 수정 코루틴을 Unity API 대체 구현으로 C# 컴파일했다. 일반·0·음수·프레임보다 짧은 지속 시간 4개 시나리오에서 시작 대기, 공통 함수 연결, 시작값, 종료 및 최종 위치 등 22개 검증 조건과 이동 범위 검사를 통과했다. git diff --check 통과.
- [UNCERTAIN] Unity 컴파일·Play Mode의 실제 프레임 스케줄링, 연출, 중단·재시작·동시 실행은 미검증이다.
- 재검토 조건: [PROPOSAL] 시작 프레임 차이가 연출에 영향을 주거나 scaled 시간·경과 시간 전달이 필요하면 공통 함수 계약을 별도로 검토한다.

---

## D-006 — UI 선택 이동 공통화와 유효 항목 탐색

날짜: 2026-10-04 (Asia/Seoul)

상태: Accepted — 사용자가 제시된 수정 코드 적용을 승인했다.

- [FACT] UIKeyNavigator의 공개 이동 메서드 4개는 유지하고 목록 이동을 private MoveSelection(int direction)으로 모았다. NextElement/PreElement의 Carousel 값 변경 경로는 유지한다.
- 이유: [FACT] 기존 목록 순환과 선택 변경 코드가 반복되었으며 빈 목록·null 선택 대상에 대한 방어가 없었다. 중복을 줄이면서 예외 조건의 처리를 일치시켰다.
- [FACT] null 또는 빈 목록은 이동하지 않는다. null 항목은 최대 한 바퀴 탐색하며 건너뛴다. 현재 선택이 없거나 목록에 없으면 다음은 첫 유효 항목, 이전은 마지막 유효 항목을 선택한다. Initialize도 첫 유효 항목을 선택한다. 유효한 대상이 없으면 기존 선택을 유지한다.
- [FACT] 유효한 대상이 결정된 뒤 기존 선택을 해제한다. 유효 항목이 하나이면 기존처럼 동일 항목의 UnSelected/Selected를 다시 호출한다. SelectElement와 UpdateUIElement는 변경하지 않았다.
- 대안: [PROPOSAL] 네 메서드에 개별 방어 코드를 추가할 수도 있지만 같은 수정이 반복된다. 같은 항목 재선택을 생략하는 방식은 선택 콜백 횟수가 바뀌므로 채택하지 않았다.
- 영향: [FACT] 변경은 UIKeyNavigator 내부 이동·초기 선택 처리에 한정된다. 공개 호출 이름과 UIElement/CarouselUIElement 의존관계는 유지한다.
- 검증: [FACT] 수정된 UIKeyNavigator 전체 소스를 Unity 및 UI 의존성 대체 구현과 함께 메모리에서 C# 컴파일했다. 정상 양방향 순환, null·빈 목록, null 항목 건너뛰기, 선택 누락, 단일 항목 재선택, Carousel 값 변경과 항목 이동 구분, 선택 실행 등 19개 검증 조건을 통과했다. 코드의 git diff --check도 통과했다.
- [UNCERTAIN] Unity Editor 컴파일·Play Mode 및 실제 UI 연출은 미검증이다. 대체 구현은 Unity의 파괴된 객체 동작이나 실제 Carousel 내부 구현을 검증하지 않는다.
- 재검토 조건: [PROPOSAL] 비활성 UI 건너뛰기, 선택 콜백 중 목록 변경, 유효 항목이 없을 때 선택 해제가 필요하면 별도 정책을 검토한다.

---

## D-005 — 입력 구독자의 파괴 시 등록 해제

날짜: 2026-10-04 (Asia/Seoul)

상태: Accepted — 사용자 요청에 따라 입력 등록 해제를 추가했다.

- [FACT] CharacterControl, GameManager, UIManager, BlockSwitch, RetweetSwitch, TagRetweetSwitch, DownloadStationSwtich의 기존 Start 경로 등록을 유지하고 OnDestroy에서 해제한다. CharacterControl의 이동·점프를 포함하여 총 8개 바인딩이다.
- [FACT] 등록 당시 InputSystem을 필드에 저장하고 그 인스턴스에서 자기 콜백만 해제한다. 미등록 또는 입력 시스템이 먼저 파괴된 경우 반환한다. 해제 과정에서 InputSystem.Instance에 접근하지 않는다.
- 이유: [FACT] 기존 사용처에는 입력 해제 연결이 없었으며, InputSystem.Instance는 객체가 없으면 새로 생성한다. 파괴된 구독자의 콜백을 정리하면서 종료 중 입력 시스템을 새로 생성하지 않도록 한다.
- 영향과 대안: [FACT] OnEnable/OnDisable 쌍으로 옮기면 비활성 상태의 입력 정책과 재등록 경로가 바뀌므로 이번에는 기존 Start 등록과 OnDestroy 해제를 짝지었다. 리셋 및 기타 이벤트 해제, 입력 등록 메서드 공통화는 포함하지 않았다.
- 검증: [FACT] 실제 InputSystem/KeyBinding 소스와 7개 사용처에서 추출한 등록·해제 코드를 Unity API 대체 구현과 함께 메모리에서 C# 컴파일했다. 구독자별로 자기 콜백 해제와 타 구독자 보존·반복 해제, 미등록 해제, 입력 시스템 선행 파괴, 싱글턴 교체 후 원래 등록 대상 해제를 확인하여 총 28개 격리 시나리오를 통과했다. 대상 코드의 git diff --check도 통과했다.
- [UNCERTAIN] 전체 Unity 컴파일 및 Play Mode의 실제 입력·씬 전환은 미검증이다. 추출 코드와 대체 API 테스트는 Unity 실행 검증을 대신하지 않는다.
- 재검토 조건: [PROPOSAL] 풀링·비활성 상태의 입력 정책을 바꾸거나 실행 중 InputSystem 교체 후 자동 재등록이 필요하면 등록 수명을 재검토한다.

---

## D-004 — 입력 등록 해제와 실행 중 맵 변경 처리

날짜: 2026-10-03 (Asia/Seoul)

상태: Accepted — 사용자가 비교·설명된 수정안의 적용을 승인하여 InputSystem.cs에 반영.

- [FACT] 키·축 등록은 null 콜백을 무시한다. 동일 콜백 중복 등록은 기존처럼 허용하며, 키·축 해제는 지정된 콜백을 한 번 제거한다. 마지막 콜백이 제거되면 해당 사전 항목을 삭제한다. 기존 매개변수 이름 act를 유지했다.
- [FACT] DoAction은 시작 시 키·축 이름을 재사용 목록에 복사하고, 각 항목을 처리할 때 현재 콜백을 다시 조회한다. 순회 중 사전 항목의 추가·제거에 대응하기 위한 변경이다. 새 키·축 이름은 다음 실행부터 처리하며 기존 이름의 콜백 변경은 아직 처리하지 않은 항목의 조회에 반영될 수 있다.
- [FACT] currentMap이 시작 시 맵과 다르면 다음 항목을 처리하지 않고 반환한다. 이미 시작한 한 Action.Invoke의 나머지 콜백은 즉시 취소하지 않는다. 맵이 다른 맵으로 갔다가 검사 전에 원래 맵으로 돌아오는 경우까지 추적하는 방식은 아니다.
- 이유: [FACT] 기존 키 해제는 비어 있었고 축 해제는 없었다. 해제 구현과 함께 사전 직접 순회를 변경해야 콜백에서 등록 항목을 변경하는 경로를 지원할 수 있다.
- 대안: [FACT] 기존 등록 표현을 유지하는 최소안, 입력 맵 전환 후에도 남은 입력을 실행하는 정책을 비교했다. 사용자는 null 차단·등록 정리·맵 전환 중단을 포함한 제시안을 승인했다. 콜백별 즉시 취소와 중복 등록 금지는 이번 범위에 포함하지 않았다.
- 영향: [FACT] KeyBinding 구조와 입력 호출 방식은 유지한다. CharacterControl·Switch 등의 실제 구독 해제 연결은 아직 수정하지 않았다. 전체 구독 수명 정비 완료를 의미하지 않는다.
- 검증: [FACT] 실제 InputSystem/KeyBinding 소스를 Unity API 대체 구현과 함께 메모리에서 C# 컴파일하고 7개 시나리오 그룹을 통과했다: 다른 콜백 보존, 마지막/미등록/null 키 처리, 축 값 전달·해제, 실행 중 대기 키 해제, 새 키의 다음 실행 반영, 맵 전환 중단과 현재 multicast 완료, 중복 등록 정책 유지. git diff --check 통과.
- [UNCERTAIN] Unity Editor 컴파일·Play Mode·실제 키 입력 및 씬 전환은 미검증이다. 대체 API 테스트는 Unity 실행 검증을 대신하지 않는다.
- 재검토 조건: [PROPOSAL] 콜백별 즉시 취소, 중복 등록 차단, DoAction 재진입 또는 멀티스레드 입력 처리가 필요하면 실행·등록 정책을 재검토한다.

---

## D-003 — 수정 전 설명 및 사용자 최종 결정권 명시

날짜: 2026-10-03 (Asia/Seoul)

상태: Accepted — 사용자 명시 요청으로 PROJECT_RULES.md에 반영.

- [FACT] 수정 전 문제·근거·필요성·변경안·영향·대안을 제시하고, 수정 여부와 방식은 사용자가 최종 결정한다. 사용자가 승인한 범위에 한해 적용하며 같은 승인에 대한 반복 확인은 하지 않는다.
- [FACT] 예정 기록이나 다음 항목 제시 요청을 코드 수정 승인으로 확대하지 않는다. 한 항목의 승인 역시 다음 항목에 자동 적용하지 않는다.
- 이유: [FACT] 사용자가 수정 전에 최종 결정권을 자신에게 주도록 프로젝트 규칙에 명시해 달라고 요청했다.
- 영향: [FACT] 이후 리뷰·수정 작업 절차에 적용된다. 이번 변경은 문서 규칙과 결정 기록에 한정되며 게임 코드는 수정하지 않았다.
- 대안과 재검토: [FACT] AI가 예정 목록 전체를 자동 적용하는 방식은 이번 지시와 맞지 않아 채택하지 않는다. 사용자가 이후 명시적으로 일괄 적용 범위나 절차를 지정하면 해당 지시를 반영한다.

---

## D-002 — 카드 제거의 기본 대상과 빈 목록 처리

날짜: 2026-10-03 (Asia/Seoul)

상태: Accepted — [FACT] 사용자가 리뷰 1번부터 진행하도록 선택했으며 아래 코드 변경을 적용했다.

- 변경 대상: `Assets/02.Script/UI/Card/Card.cs`, `Assets/02.Script/UI/CarouselUI/CardManager.cs`.
- [FACT] `Card.RemoveCard(Card card = null)`은 대상이 null이면 `this`를 사용한다. 명시적으로 전달된 유효한 Card는 기존처럼 사용한다. 공개 메서드 이름·시그니처와 `Action<Card>` 연결은 유지한다.
- [FACT] `CardManager.RemoveCard()`는 목록의 null 여부를 Count보다 먼저 검사한다. `ShowTopCard()`는 null 또는 빈 목록이면 반환하여 마지막 요소 인덱스 접근을 막는다.
- 이유: [FACT] 기존 CardManager와 NextStep은 인자 없이 RemoveCard를 호출하지만, 기존 구현은 null 인자를 즉시 역참조했다. `Assets/05. Animation/Card/In_Start_ForSys.anim`에도 NextStep 이벤트가 있다.
- 영향: [FACT] 카드 수 제한에 따른 제거, NextStep에 의한 제거, 시작 시 자기 카드 제거와 완료 콜백 경로의 기존 연결을 보존한다. 빈 목록에서 최상단 카드 표시 요청은 아무 작업도 하지 않는다. 카드 목록 소유권이나 애니메이션 흐름은 재설계하지 않았다.
- 고려한 대안: [PROPOSAL] 매개변수 없는 제거와 Action<Card>용 메서드를 분리하는 방식. 이번에는 시그니처와 호출부 변경을 최소화하기 위해 기존 선택적 매개변수를 유지했다.
- 재검토 조건: [PROPOSAL] null을 잘못된 호출로 취급해야 하거나, 자기 카드 외의 카드 제거 책임을 분리할 때 메서드 계약을 다시 검토한다.
- 검증: [FACT] 호출부 및 Animation Clip 연결을 정적으로 확인했고 대상 두 파일의 `git diff --check`가 통과했다. [UNCERTAIN] Unity 컴파일·Play Mode 실행은 수행하지 않았으므로 카드 초과 제거·완료 애니메이션·빈 목록 조작의 실제 실행 결과는 미검증이다.

---



이 문서는 프로젝트의 중요한 설계 결정과 의미 있는 변경 이유를 기록한다.



단순한 작업 로그가 아니라, 이후 작업자가 다음 내용을 이해할 수 있도록 작성한다.



무엇이 변경되었는가



왜 변경했는가



어떤 시스템에 영향을 주는가



어떤 대안을 검토했는가



언제 다시 검토해야 하는가



기록 형식



D-001 — 제목



날짜:



YYYY-MM-DD



상태:



Accepted / Temporary / Deprecated / Reconsider



변경 대상:



관련 시스템 또는 파일



변경 내용:



무엇을 변경했는지 작성한다.



변경 이유:



왜 이 결정을 내렸는지 작성한다.



영향:



영향을 받는 시스템이나 동작을 작성한다.



고려한 대안:



검토했던 다른 방법이 있다면 작성한다.



선택하지 않은 이유:



다른 방법을 선택하지 않은 이유를 작성한다.



재검토 조건:



어떤 상황이 발생하면 이 결정을 다시 검토해야 하는지 작성한다.

---

## 검토안 — 현재 Stage의 조건 검사와 중복 완료 방지

날짜: 2026-10-02 (Asia/Seoul)

상태: Reconsider — [PROPOSAL] 미승인·미적용 수정안. 문서 기록 요청만 받았으며 코드 변경 또는 설계 확정으로 간주하지 않는다.

### 변경 대상

- `Assets/02.Script/StageManager/Stage.cs`
- 관련 호출자: `Assets/02.Script/GameManager.cs`
- 관련 이벤트 기반: `Assets/02.Script/Interface/ClearCondition.cs`

### 현재 확인한 사실과 변경 이유

- [FACT] Stage는 Start에서 각 조건의 OnCheck에 StageSatisfied를 등록한다. StageSatisfied에는 현재 Stage 확인, 초기화 중 검사 차단, 완료 여부 검사가 없다.
- [FACT] GameManager.NextStage는 기존 Stage의 StageExit를 호출하고 currentStage를 변경한 뒤 새 Stage의 StageEnter를 호출한다.
- [FACT] StageEnter는 EnterEvent 다음에 GameManager.initAction을 실행한다. 다음 Stage가 없는 경우에는 NextStage가 ExitEvent를 직접 실행한다.
- [INFERENCE] 이전 Stage의 조건이 다시 만족되거나 같은 조건 알림이 반복되면 다음 Stage 재진입 및 초기화가 반복될 수 있다. 진입·퇴장 이벤트에서 조건이 변경되는 경우에도 중첩된 완료 요청이 발생할 수 있다.

### 제안하는 수정 내용

- [PROPOSAL] Stage에 `_canCheckConditions`, `_isCompleted` 두 상태를 추가한다. 이는 현재 코드에 존재하는 필드가 아니라 추가 제안이다.
- [PROPOSAL] StageEnter 시작 시 검사를 차단하고 완료 표시를 해제한다. 기존 EnterEvent → initAction 순서를 유지한 후 검사를 허용한다.
- [PROPOSAL] StageSatisfied는 검사 허용 상태이고, 아직 완료하지 않았으며, GameManager.currentStage가 자신일 때만 조건을 검사한다. 여기서 활성 여부는 GameObject 활성 상태가 아니라 게임 진행상의 현재 Stage를 의미한다.
- [PROPOSAL] 조건 목록이 null 또는 비어 있거나, 목록에 null 조건이 있으면 전환하지 않는다. 이 동작은 조건 없는 Stage의 자동 완료를 막으므로 적용 전에 의도를 확인해야 한다.
- [PROPOSAL] 모든 조건이 만족되면 NextStage 호출 전에 완료 표시를 설정하고 검사를 차단한다. 전환 중 발생하는 추가 조건 알림을 무시하기 위한 순서다.
- [PROPOSAL] StageExit는 ExitEvent 실행 전에 검사를 차단한다. 마지막 Stage 완료도 ExitEvent 직접 실행 대신 StageExit를 호출한다.
- [PROPOSAL] 이번 최소 변경안에서는 기존 Start의 구독 방식과 GameManager의 전환 책임을 유지한다. 다른 호출자가 GameManager.NextStage를 직접 반복 호출하는 문제까지 해결하는 안은 아니다.

### 영향과 미결정 사항

- [PROPOSAL] 현재 Stage의 완료 처리를 진입당 한 번으로 제한하고 이전 Stage의 조건 알림으로 인한 전환을 막는다.
- [PROPOSAL] 진입·초기화 중 발생한 조건 알림은 무시한다. 초기화 직후 자동 재검사는 최소 변경안에 포함하지 않는다.
- [UNCERTAIN] 진입 전부터 모든 조건이 만족된 Stage를 즉시 완료해야 하는지, 다음 조건 알림을 기다려야 하는지는 확정되지 않았다. 자동 완료가 필요하면 관련 컴포넌트의 초기화 순서와 검사 시점을 별도로 검토해야 한다.
- [FACT] 현재 ResetGame은 OnReset만 호출하고 StageEnter를 호출하지 않는다.
- [PROPOSAL] 완료 표시 해제는 StageEnter에서 수행한다. 사망 리셋과 완료된 Stage 재도전은 구분한다.
- [UNCERTAIN] 완료된 Stage 재도전의 실제 진입 경로와 조건 데이터 초기화 정책은 확정되지 않았다. 완료 표시만 해제해도 조건 데이터까지 초기화되는 것은 아니다.
- [UNCERTAIN] 비활성 GameObject의 조건 알림까지 차단해야 하는지, 조건 없는 Stage를 허용할지는 추가 결정이 필요하다.

### 고려한 대안과 추천 근거

- [PROPOSAL] currentStage 비교만 추가: 변경은 가장 작지만 현재 Stage의 반복 완료 및 초기화 중 중첩 호출을 별도로 막지 못한다.
- [PROPOSAL] StageEnter/StageExit에서 구독·해제를 수행: 활성 구간에만 알림을 받을 수 있지만 초기화 순서와 구독 수명까지 변경하며, 전환 도중 중복 실행 방지는 여전히 필요하다.
- [PROPOSAL] GameManager로 조건 검사를 통합: 전환 통제를 집중할 수 있지만 Stage의 책임과 이벤트 연결 변경 범위가 커진다.
- [PROPOSAL] 추천안은 기존 구조에 검사 허용·완료 상태와 currentStage 검사를 추가하는 최소 변경이다. 다른 대안의 채택 여부를 확정하거나 배제한 것은 아니다.

### 적용 후 검증할 사례

1. 이전 Stage의 조건 이벤트를 다시 발생시켜도 현재 Stage가 바뀌지 않는다.
2. 만족된 조건 알림이 연속 발생해도 완료·전환은 한 번만 실행된다.
3. EnterEvent, initAction, ExitEvent에서 조건이 변경되어도 같은 Stage가 중첩 전환하지 않는다.
4. 마지막 Stage 완료 시 ExitEvent가 해당 완료 경로에서 한 번만 실행된다.
5. 사망 리셋 후 현재 Stage의 조건을 다시 달성할 수 있고, 명시적 Stage 재진입 시 완료 표시가 초기화된다.
6. 빈 조건 목록, null 조건, 진입 시 이미 만족된 조건의 처리가 선택한 정책과 일치한다.

### 재검토 조건과 기록 범위

- [PROPOSAL] 자동 완료, Stage 재도전, 병렬 Stage 진행 또는 이벤트에 의한 직접 Stage 전환을 도입할 때 이 안을 다시 검토한다.
- [FACT] 이 기록에서는 문서만 변경했다. Stage 관련 코드는 수정하지 않았고 Unity 실행·컴파일·테스트도 수행하지 않았다.
- [FACT] 현재 실제 구조를 바꾼 것이 아니므로 ARCHITECTURE.md와 PROJECT_STATE.md를 구현 완료 상태로 갱신하지 않는다. 적용 승인 및 구현 후에는 결과와 검증 내용을 별도로 기록해야 한다.

---

## D-001 — Switch의 대상 순회와 결과 처리 공통화

날짜: 2026-10-02 (Asia/Seoul)

상태: Accepted — [FACT] 사용자가 기존 Switch에 ApplyToTargets를 추가하기로 선택했다. 문서 기록 시점에는 해당 메서드가 없으며, 구현은 예정 상태다.

### 변경 대상과 결정 내용

- 대상: `Assets/02.Script/Object/Switch/Switch.cs` 및 이 메서드를 사용할 파생 스위치.
- [FACT] 대상별 `SwitchOn(state)` 호출을 공통화하는 아래 메서드를 기존 Switch에 추가하기로 했다. 신규 구현 예정 코드이며 현재 구현을 나타내지 않는다.

```csharp
protected void ApplyToTargets(
    IEnumerable<ISwitchable> targets,
    bool state,
    Action<bool> onResult = null)
{
    foreach (var target in targets)
    {
        bool accepted = target.SwitchOn(state);
        onResult?.Invoke(accepted);
    }
}
```

### 매개변수 선택 이유

- [FACT] 메서드는 대상 목록을 순회할 뿐 추가·삭제·인덱스 접근을 하지 않으므로 `IEnumerable<ISwitchable>`을 사용한다. `List<ISwitchable>`은 이 인터페이스를 구현하므로 그대로 인자로 전달할 수 있으며, 전달 때문에 리스트가 복사되는 것은 아니다. 구현 시 `System.Collections.Generic` 네임스페이스가 필요하다.
- [FACT] `Action<bool> onResult = null`은 결과 처리가 필요한 호출자만 제공하는 선택적 콜백이다. 각 대상의 `SwitchOn(state)`가 반환한 bool을 해당 호출 직후 `onResult`에 전달한다. 콜백을 생략하면 `SwitchOn`만 실행한다.
- [FACT] `ApplyToTargets` 자체는 void이며, `Action<bool>`도 값을 반환하지 않는다. 여러 결과를 하나로 합산하는 방식이 아니라 대상별 반환값 처리를 호출자에게 맡기는 방식이다. 반환값이 false여도 다음 대상을 계속 순회한다.
- [FACT] BlockSwitch처럼 반환값에 따라 시각 처리를 하는 호출자는 콜백을 제공하고, 결과를 사용하지 않는 스위치는 생략할 수 있다. 대상마다 다른 `SwitchOn`의 구현 의미는 이 공통화로 변경하지 않는다.

### 영향과 대안

- [PROPOSAL] 적용 시 기존 입력·상태 변경·방향 알림·애니메이션 순서를 보존하고 대상 순회 부분만 교체한다. 전체 호출부 이전 범위는 구현 시 확인한다.
- [FACT] 대안은 각 스위치의 반복문을 유지하거나 매개변수를 `List<ISwitchable>`로 제한하는 것이다. 선택한 방식은 반복 순회를 공유하면서 순회에 필요한 인터페이스만 요구하고, 결과별 처리는 파생 스위치에 남긴다.
- [INFERENCE] 콜백을 통한 간접 호출이 추가되므로, 단순 반복문보다 호출 흐름을 따라가는 비용은 늘어날 수 있다.

### 재검토 조건과 기록 범위

- [PROPOSAL] 대상 추가·삭제, 대상 식별 정보를 포함한 결과 처리, 전체 성공 여부 반환 또는 실패 시 중단이 필요해지면 현재 시그니처를 재검토한다. 순회 중 컬렉션 변경과 null 대상 처리 정책은 적용 시 확인한다.
- [FACT] 이번 작업은 이 결정의 문서 기록만 수행했다. Switch 및 파생 클래스 코드는 수정하지 않았고 컴파일·실행 검증도 수행하지 않았다. 실제 적용 후 구조·상태 문서의 갱신 필요성을 확인한다.

