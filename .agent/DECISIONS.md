프로젝트 결정 기록

## D-031 — 입구 도착 경로와 지수 감속

날짜: 2026-10-11 (Asia/Seoul)

상태: Accepted — 사용자가 최종 위치, 최소 추가 블록 개수, 남은 묶음 소진과 회수·재사용, 시작 고속·도착 저속의 지수 감속을 "포함해서 적용시켜"로 승인했다.

- [FACT] BuildingScroller.RequestEntrance가 클릭 입력을 받는 외부 UI/입력 처리의 진입점이다. 직접 마우스 입력을 읽지 않는다. 중복 요청과 도착 후 재시작은 무시하며 ResetScroll 후 재실행할 수 있다.
- [FACT] 추가 일반 블록 수는 max(현재 묶음의 미사용 개수, minimumBlocksBeforeEntrance)다. 기존 배치 개수는 제외한다. 부족분은 새 묶음에서 가져오되 마지막 새 묶음까지 소진하도록 강제하지 않는다. 순서 선택은 기존 경계 중복 방지 로직을 사용한다.
- [FACT] 요청 시 종류 순서, 총 이동 거리, 목표 Y, 시간·감속 강도를 고정한다. 각 종류의 실제 부모 공간 높이를 확인하기 위해 보관 객체 하나를 꺼내 측정 후 반환하며, 없으면 종류당 한 개를 생성한다. 입구 객체도 하나 준비한다. 전체 경로 객체를 미리 생성하지 않는다.
- [FACT] BuildingBlock의 entranceStopPoint는 입구 내부의 지정 정지 기준점이다. 일반 블록에는 필요하지 않으며 이미지 연결 계산의 자식 제외 규칙은 유지한다. 입구는 일반 프리팹 목록과 별도로 지정하고 기준점은 이미지 높이 안에 있어야 한다. 최종 목표 Y는 두 경계 사이여야 한다.
- [FACT] 지수 이동률 (1-exp(-k*t))/(1-exp(-k))로 누적 이동 거리를 계산하고 프레임별 차이만 전체 블록에 적용한다. Time.deltaTime을 유지한다. 입구 연결 후 아래 추가를 종료하고, 지정 시간 완료 시 기준점을 목표 Y에 맞춰 정지한다. X/Z는 기존 연결 위치를 유지한다.
- [FACT] 이동 중 위쪽 회수·아래 연결을 계속한다. 큰 프레임 간격의 채우기 루프에서도 위쪽 객체를 반환해 재사용한다. StopScroll은 진행을 보존하며 ResetScroll은 입구와 일반 블록을 회수하고 경로·도착 상태를 초기화한다.
- 이유·대안: 회수만 반복하면 입구에 도달하지 않아 최소 개수와 유한 경로를 도입했다. 속도에 일정 비율을 곱해 무기한 접근하는 대신 유한 시간의 정규화 곡선을 사용했다. 직접 클릭 감지는 UI 입력 책임과 겹쳐 추가하지 않았다.
- 검증: Unity 프로젝트 참조 컴파일 성공(기존 경고 10개). 실제 BuildingScroller 소스에 Unity·BuildingBlock 기하/수명/난수 대체 타입을 연결한 메모리 실행에서 557개 검증 통과. 최소 0/1/7/30개, 남은 묶음, 감속 단조성, 목표 도착, 큰 프레임 간격 회수·재사용, 중복 요청, 정지·재개·리셋, 잘못된 목표 거부를 확인했다. 실제 SpriteRenderer bounds·Animator·씬 클릭 연결 및 화면은 Play Mode 미검증이다.
- 범위: BuildingBlock/BuildingScroller와 문서만 수정했다. 씬·프리팹·기존 입력은 변경하지 않았다. 실제 작업 직전/후 전체 코드는 .agent/reviews/building-entrance-20261011/comparison.md에 보관한다.
- 사용 조건·재검토: 이동 도중 연결 이미지/정지점/부모의 크기·회전·위치를 별도 연출로 바꾸지 않는다. 프리팹 설정 변경은 실행 중 지원하지 않는다. 입구 아래는 채우지 않으므로 입구 외형이 화면 하단을 덮도록 제작·배치해야 한다. 화면에서 감속 강도와 시간 조정이 필요하다.

## D-030 — 연출용 건물 블록 무한 스크롤

날짜: 2026-10-11 (Asia/Seoul)

상태: Accepted — 사용자가 연출 전용 배경, 프리팹 회수·재사용, 자식 제외 연결 계산, 섞은 묶음 순회와 묶음 경계 중복 방지를 선택한 뒤 스크립트 작성을 요청했다.

- [FACT] BuildingBlock은 직접 지정한 SpriteRenderer의 월드 bounds 위·아래 중앙만 제공한다. 다른 자식 렌더러를 수집하거나 크기를 합산하지 않는다. 자식은 블록 이동·비활성화에 함께 따른다.
- [FACT] BuildingScroller는 Inspector의 upperBoundary/lowerBoundary 사이를 채우고 Time.deltaTime 기준 위로 이동한다. 위쪽 경계보다 아래 연결점이 높아진 블록을 종류별 Stack에 반환한다. 아래가 비면 마지막 아래 연결점에 새 블록의 위 연결점을 맞춘다. 인스턴스 부족 시에만 생성한다.
- [FACT] 프리팹 목록은 첫 초기화 시 고정한다. null, 중복 프리팹, 연결 이미지 누락·유효 높이 부족을 거부한다. 종류를 Fisher–Yates로 섞고 첫 항목이 직전 묶음 마지막과 같으면 나머지 중 무작위 항목과 교환한다. 한 종류만 등록하면 반복한다.
- [FACT] StartScroll/StopScroll/ResetScroll을 제공한다. ResetScroll은 표시 블록을 회수하고 순서를 초기화한 뒤 정지한다. 다음 StartScroll에서 재배치한다. 비활성화는 Update를 멈추며 재활성화 시 기존 상태를 유지한다. 대량 채우기 오류 방지를 위해 한 번에 2048개를 넘으면 오류를 출력하고 정지한다.
- 이유·대안: 무작위 선택마다 생성·파괴하는 대신 종류별로 재사용한다. 완전 랜덤 대신 묶음마다 각 종류를 한 번 사용한다. 기존 카메라·GameManager와 연결하지 않고 명시적 경계 두 개로 생성·회수 범위를 설정한다.
- 사용 조건: 고정된 세로 스크롤, 회전하지 않은 건물 이미지, 정렬된 공통 경계 및 고정된 연결 이미지 크기를 전제로 한다. 기준 이미지 밖 자식 장식은 upperBoundary를 화면 위로 충분히 띄워 보호한다. 자식 Animator·파티클의 개별 상태 초기화는 자동 처리하지 않는다.
- 검증: Unity 6000.3.10f1 프로젝트 참조 컴파일 성공(기존 경고 10개). 실제 NextType/Swap 메서드를 추출하고 난수 공급만 대체해 1·2·4·8종류의 총 4000묶음/15000선택에서 묶음별 종류 완전성 및 여러 종류일 때 연속 중복 없음을 확인했다. Unity 난수 구현·시각 연결·풀 재활성화는 Play Mode 미검증이다. 테스트 소스 파일은 생성하지 않았다.
- 범위: 신규 스크립트 및 meta, 관련 문서만 추가·갱신했다. 씬·프리팹 배치는 하지 않았다.
- 재검토 조건: 가중 확률, 조건부 연결, 런타임 프리팹 목록 변경, 자식 연출 초기화, 카메라 자동 경계 또는 일시정지와 무관한 진행이 필요할 때 검토한다.

## D-029 — 카드 정렬의 초반 감속·후반 가속 연출

날짜: 2026-10-11 (Asia/Seoul)

상태: Accepted — 사용자가 정렬 보간을 progress * progress로 변경하는 안을 "적용해줘"로 승인했다.

- [FACT] CardManager.RefreshCardCoroutine의 위치 보간을 sin(progress * PI / 2)에서 progress²로 변경했다. 기존 카드와 새 카드가 같은 진행률로 천천히 출발해 끝으로 갈수록 빨라진다.
- 이유: 기존의 빠른 출발·느린 도착을 사용자가 요청한 느린 출발·빠른 도착으로 바꾼다. 시간 연장만으로는 가속 방향을 바꿀 수 없어 보간식만 변경했다.
- [FACT] animDuration, 목표 위치, Card의 등장·닫기 투명도, Todo 점검 및 UI 입력 흐름은 유지한다. 책임·의존관계 변경은 없어 ARCHITECTURE 갱신은 필요하지 않다.
- 검증: 작업 직전 바이트와 비교해 코드 변경이 보간식 한 곳뿐임을 확인했다. 컴파일 및 Unity Play Mode 시각 검증은 수행하지 않았다.
- 재검토 조건: 도착 시 급정지가 부자연스럽거나 전체 정렬 시간이 여전히 짧게 느껴지면 곡선 또는 Inspector 시간을 별도로 검토한다.

## D-028 — 모든 수정 후 실제 전후 비교 보고

날짜: 2026-10-11 (Asia/Seoul)

상태: Accepted — 사용자가 수정 시 수정 전·후 코드를 반드시 명시하고 설명하도록 규칙 추가를 요청했다.

- [FACT] PROJECT_RULES에 코드 수정 후 최종 응답의 전후 코드 블록 및 변경 이유·동작 차이 설명을 의무화했다. 기존의 상세 설명 요청 시 비교 규칙을 모든 코드 수정 보고로 확대한다.
- 이유: 완료 요약만으로 끝내지 않고 사용자가 실제 변경을 바로 확인할 수 있도록 한다. 비교 기준은 실제 작업 직전 코드이며 문서만 수정하면 변경 문구를 비교한다.
- 범위: 규칙과 결정 기록만 갱신했다. 기존 사용자 승인 절차는 유지한다.

## D-027 — 삼항 조건 연산자 사용 금지

날짜: 2026-10-11 (Asia/Seoul)

상태: Accepted — 사용자가 "삼항연산자는 쓰지마. 규칙에도 추가해"라고 명시했다.

- [FACT] PROJECT_RULES에 삼항 조건 연산자 대신 명시적인 if/else를 사용하도록 기록했다. 새 코드와 수정안·예시 코드에 적용한다.
- 이유: 사용자가 요청한 코드 표현 방식을 프로젝트 규칙으로 유지한다.
- 범위: 이번에는 규칙과 결정 기록만 갱신했다. 기존 코드 전체의 삼항 연산자를 일괄 변경하지 않았다. 직전 UIManager의 해당 식은 이미 같은 동작의 if/else로 변경했다.

## D-026 — 카드 표시 종료와 보관 해제 분리

날짜: 2026-10-11 (Asia/Seoul)

상태: Accepted — 아무 키로 전체 카드 표시를 닫되 보관 목록을 유지하고, 다음 Add에서 기존 카드를 배경으로 다시 보여주며 Limit 초과 시만 오래된 카드를 제외하는 변경안을 사용자가 "적용해"로 승인했다.

- [FACT] CardManager.cardList는 보관 이력이다. 일반 Closed 이벤트는 해당 목록을 지우지 않는다. Limit을 초과하면 오래된 순서로 보관 목록에서 제거하며 파괴된 참조도 정리한다. 초과 카드가 화면에 남아 있으면 outgoingCards에서 페이드·왼쪽 이동 완료까지 임시 관리한다. GameObject는 파괴하지 않는다.
- [FACT] 새 Add는 보관된 카드에 ShowAsBackground를 요청한다. 기존 위치에서 새 배치 목표로 이동하며 기존 카드의 Todo 점검과 등장 페이드를 재실행하지 않는다. 새 카드만 BeginEntrance로 표시하고 UI에 등록한다. 보관 중이거나 초과 제거 연출 중인 동일 객체의 Add 중복 요청은 무시한다.
- [FACT] Card는 Background 상태와 ShowAsBackground/HideAsBackground를 제공한다. 기존 UI 등록은 자신이 해제하며 CardManager는 UIManager를 직접 참조하지 않는다. 배경 카드에서는 뒤늦은 NextStep·SuccessClearStage·Todo 완료 통지가 입력 대기를 재시작하지 않는다.
- [FACT] 새 카드의 Closing 이벤트를 CardManager가 받아 배경 카드들을 같은 시간의 페이드로 함께 숨긴다. Closed 이후 보관은 유지한다. FinishClose는 UI 등록만 정리하고 게임 오브젝트를 파괴하지 않는다. 닫기 완료 시 Todo를 Default로 돌리던 처리는 제거하여 배경 재표시를 위해 기존 Todo 표현을 유지한다. 새 카드로 여는 경로는 기존 초기화를 유지한다.
- [FACT] CardManager.RemoveCard()는 호환 호출로 전체 표시 닫기를 요청한다. HideCardList/HideTopCard도 보관 해제가 아닌 표시 종료이며, ShowTopCard는 현재 표시 중인 새 카드에만 적용한다. 비활성화 시에는 표시·UI 등록을 정리하고 보관 이력은 유지한다.
- 이유: D-024의 닫힘 완료 시 목록 삭제는 사용자가 의도한 카드 이력 보관과 달랐다. UI 표시 수명과 카드 보관 수명을 분리한다. D-025의 닫는 카드 동시 이동은 outgoingCards에 유지한다.
- 검증: Unity 6000.3.10f1 제공 컴파일러와 프로젝트 참조로 컴파일 성공(기존 경고 10개). 실제 Card/CardManager/UIAnimationRoutine 및 대체 Unity·UIManager·Todo를 사용한 메모리 실행 검증 16개 통과: 보관 유지, 재표시, UI 단일 등록, Todo 재점검 방지, 동시 이동, Limit 초과, 전체 숨김, 닫기 도중 Add 등. 실제 UIManager와의 런타임 통합은 대체 타입 검증에 포함되지 않는다. 테스트 소스 파일은 생성하지 않았으며 검증용 DLL/PDB는 삭제했다.
- [UNCERTAIN] Unity Play Mode의 실제 Canvas·Animator 표시와 UI 입력 인계 통합은 미검증이다. 비활성 부모 아래 카드는 ShowAsBackground로 표시할 수 없으며, 누락된 Animation Event나 OriginalPopUp의 동시 연출은 별도 확인 대상이다.

## D-025 — 닫히는 카드도 신규 카드와 함께 왼쪽 이동

날짜: 2026-10-11 (Asia/Seoul)

상태: Accepted — 닫는 카드의 이동 제외 조건과 수정안을 제시한 뒤 사용자가 "변경해줘"로 승인했다.

- [FACT] CardManager의 배치 스냅샷에 IsClosing 카드도 포함한다. 개수 제한 판단은 닫기 시작 전 카드 수를 기준으로 유지하며, 실제 이동은 목록에서 제거되기 전까지 진행한다. 등장 완료 통지는 닫히지 않은 신규 카드에만 전달한다.
- 이유: D-024에서 닫기 시작 즉시 이동 대상에서 제외하여 오래된 카드가 제자리에 머무른 채 페이드되는 조건을 수정한다. 예를 들어 A(-90), B(0)에 C를 추가하면 A→-180, B→-90, C(+90)→0으로 함께 이동하며 A는 별도로 페이드할 수 있다.
- [FACT] 마지막 배치 목표를 보관한다. Closed 이벤트 후 남은 카드의 목표가 같으면 이동 코루틴을 재시작하지 않는다. 가운데 카드 제거 등으로 목표가 바뀔 때만 현재 위치에서 다시 배치한다. 제거된 카드는 진행 중 스냅샷에 있어도 좌표를 갱신하지 않는다.
- 책임: CardManager의 배치 처리만 변경했다. Card의 페이드·입력 대기 및 UIManager의 등록·입력 정책은 유지한다.
- 검증: 실제 CardManager/UIAnimationRoutine을 Unity·Card 대체 타입과 함께 메모리에서 컴파일·실행하여 닫는 카드 동시 이동, 오래된 카드 제거 시 코루틴 유지, 가운데 제거 시 재배치, 다중 제한 초과, 도착 통지 등 11개 검증을 통과했다. git diff --check 통과. 테스트 소스 파일은 새로 생성하지 않았다.
- [UNCERTAIN] 실제 Unity 화면의 Animator·LayoutGroup·Canvas와 조합한 결과는 미검증이다. 닫힘 상태가 아닌 카드도 움직이지 않는 현상은 이번 조건과 다른 원인일 수 있다.

## D-024 — 카드 UI 입력 대기와 Manager 책임 분리

날짜: 2026-10-11 (Asia/Seoul)

상태: Accepted — 역할 분리 검토안을 제시한 뒤 사용자가 "좋아 그 방향으로 수정해보자"로 적용을 승인했다. D-021의 등장·점검 흐름을 이 설계로 통합하며, Todo 완료 즉시 닫기 및 NextStep 즉시 닫기를 대체한다.

- [FACT] CardManager는 카드 목록·개수 제한·현재 위치 기반 배치와 배치 완료 통지만 담당한다. UIManager/InputSystem을 참조하지 않으며 Card.OnEntranceCompleted를 호출한다. Card.Closed를 구독하여 닫기 완료 후 해당 카드를 목록에서 제거하고 재배치한다. 제한 초과 카드는 닫기를 요청하고 완료 전까지 목록에 보관하되 배치 대상에서 제외한다.
- [FACT] Card는 IUI를 구현하고 자신의 표시 시작 시 UIManager.Push, 닫기 완료 시 Pop을 호출한다. Entering → Checking → Waiting → Closing → Hidden 상태로 연출·입력 대기·페이드를 관리한다. CardManager를 참조하지 않는다. 등장 알파는 Card가 소유하고 배치와 페이드가 모두 완료되면 Todo 점검을 시작한다.
- [FACT] Todo 완료 이벤트 또는 시작 카드 Animation Event의 NextStep은 자동 닫기 대신 입력 대기를 시작한다. 최상단 입력 권한과 대기 상태를 모두 만족하고 완료 프레임을 지난 새 입력만 닫기를 요청한다. Input.anyKeyDown은 마우스 버튼도 포함한다. Escape는 IUI.Hide를 거쳐 같은 조건을 검사한다. 명시적 RemoveCard 호출은 개수 제한 등을 위한 강제 닫기로 유지한다.
- [FACT] Todo가 없으면 등장 완료 후 입력을 기다린다. InitCard는 중복 구독을 방지하며 유효한 Todo를 중복 없이 등록한다. Todo별 완료 통지는 한 번만 집계한다. Todo 지연은 WaitForSecondsRealtime으로 처리한다. 비활성화·파괴 시 UI 등록과 카드 목록을 정리하고 파괴 시 Todo·조건 이벤트를 해제한다.
- [FACT] UIManager는 IUI만 다룬다. Push는 Pause 전환, Pop은 다음 프레임에 그 시점의 최상단 UI로 입력 인계 또는 빈 목록이면 Play_Key 복귀를 수행한다. 새 Push/Pop은 이전 인계 코루틴을 취소한다. Card의 SetVisible은 상호작용만 제어하여 뒤 카드의 시각적 누적 배치를 유지한다. 타입별 Register는 여러 Card 등록에 사용하지 않는다.
- [FACT] InputSystem은 Awake에서 현재 맵을 초기화하고 Start의 무조건 Play 전환을 제거했다. 상태 전환 프레임의 입력을 막는다. GameManager는 메뉴 열기·닫기만 요청하며 직접 입력 상태를 변경하지 않는다. UI 상태에서는 R 리셋 대기를 중단한다. CharacterControl은 UI 상태에서 이전 이동 축·점프 버퍼를 비운다. 게임 시간·중력·외부 속도를 일괄 정지하지 않는다.
- [FACT] 기존 Stage Enter의 AddCard(Card), StartGameCardOpen, CheckClearCard 및 Animation Event 메서드 이름은 유지한다. 닫기 페이드는 D-023의 제곱 보간을 유지한다. CardManager와 UIManager의 접근자는 기존 씬 인스턴스를 우선 탐색한다.
- 대안: UIWindow 상속은 UIBase의 초기 숨김·연출과 Card 연출이 중복될 수 있어 선택하지 않았다. CardManager가 UI Push/Pop을 호출하는 안도 카드 배치와 UI 입력의 책임을 섞으므로 선택하지 않았다.
- 검증: Unity 6000.3.10f1 Roslyn 컴파일러와 기존 프로젝트 응답 파일의 참조를 사용하여 별도 출력 디렉터리에 컴파일 성공했다. 응답 파일에서 누락된 UIAnimationRoutine 소스를 검증 목록에 추가했다. 기존 다른 파일의 숨김 멤버·미사용 필드 경고 10개가 남는다. 실제 Card/CardManager/UIManager/InputSystem을 대체 Unity/Todo 타입과 함께 실행해 입력 인계·이벤트·목록 정리 등 26개 검증을 통과했다. 두 Manager의 상호 참조 및 CardManager의 입력·Todo 내부 처리 참조가 없음을 확인했다.
- [UNCERTAIN] Unity Play Mode의 실제 Animator 이벤트, 서로 다른 Canvas의 표시 순서, 씬 전환 통합 결과는 미검증이다. IUI sortingOrder는 Card의 논리적 순서이며 새로운 Canvas 구성은 추가하지 않았다. 기존 UIWindow의 sortingOrder도 실제 Canvas에 적용하지 않으므로 화면의 Canvas 계층 검증은 별도로 필요하다. 직접 Input을 읽는 테스트/Timeline 스크립트 전체는 이번 범위에 포함하지 않았다.
- 재검토 조건: 완료 이벤트가 없는 Todo 애니메이션, 카드와 OriginalPopUp의 동시 연출, 외부 코드의 showns 직접 수정, Play_Pad 복귀 정책이 필요한 경우.

## D-023 — 카드 닫기 페이드를 점점 빠르게 변경

날짜: 2026-10-10 (Asia/Seoul)

상태: Accepted — 닫기 페이드에 제곱 보간을 적용하는 변경안을 제시한 뒤 사용자가 "변경해줘"로 승인했다.

- [FACT] Card.PlayCloseAnimation의 보간 함수를 사인 보간에서 `progress * progress`로 변경했다. 현재 알파에서 0으로 처음에는 천천히, 끝으로 갈수록 빠르게 감소한다.
- 이유: 사용자가 요청한 점차 빨라지는 투명화에 맞춘다. 기존 사인 보간은 처음에 빠르고 끝에서 느려졌다.
- 범위: Card의 닫기 페이드 한 줄만 변경했다. animDuration, 등장 페이드, 카드 이동, 종료 조건은 유지한다. D-021 전체 변경의 유지 승인을 뜻하지 않는다.
- 검증: 대상 보간식의 단일 교체와 git diff --check로 변경 형식을 확인했다. Unity 실행 화면의 체감 속도는 미검증이다.

## D-022 — 수정안 제시와 최종 결정권 확인 누락 방지

날짜: 2026-10-09 (Asia/Seoul)

상태: Accepted — 사용자가 최종 결정권 제시를 누락하지 않도록 규칙 추가를 명시적으로 요청했다.

- [FACT] PROJECT_RULES에 변경안·핵심 코드 차이·영향의 사전 제시, 승인 질문 명시 및 실제 응답 대기, 수정 직전 승인 근거 확인을 추가했다. 설명·참고 자료·요구사항 보완을 적용 승인으로 해석하지 않는다.
- 이유: D-021에서 사용자의 동작 설명과 이미지를 수정 승인으로 해석해 코드를 먼저 적용했다. 변경안 안내와 사용자의 적용 결정을 구분하기 위한 규칙이다.
- [FACT] 이미 승인된 동일 범위와 명시적으로 요청한 문서 갱신에는 반복 승인을 요구하지 않는다. 승인 없이 수행한 변경은 사실을 알리고 유지·되돌리기를 임의로 결정하지 않는다.
- 영향: AI의 프로젝트 수정 절차 및 승인 상태 기록. 이번 작업에서는 규칙과 결정 기록만 갱신하며 기존 코드의 유지·되돌리기는 결정하지 않았다.

## D-021 — 오른쪽 등장·페이드 완료 후 Todo 점검

날짜: 2026-10-09 (Asia/Seoul)

상태: 적용됨 / 사용자 최종 승인 미확인 — 사용자가 첨부 이미지와 동작을 설명했으나 AI가 이를 적용 승인으로 해석하여 수정했다. 기존 Accepted 표기는 잘못된 승인 기록이므로 정정한다. 현재 코드는 D-020의 신규 카드 즉시 표시 방식을 대체한 상태이며, 유지·되돌리기는 사용자 결정 전이다.

- [FACT] AddCard는 신규 카드를 OriginalPosition 오른쪽에 카드 너비 × ratio/100만큼 배치하고 alpha=0으로 시작한다. 기존 카드 이동과 같은 animDuration·사인 보간으로 원래 위치 및 alpha=1까지 이동한다. 크기는 변경하지 않는다.
- [FACT] 등장 중인 카드를 별도 집합으로 추적한다. 재배치가 중단되면 현재 위치·알파에서 이어가며, 완료된 카드만 집합에서 제거한 뒤 SuccessClearStage로 Todo 점검을 시작한다. 일반 재배치는 점검을 반복하지 않는다. 목록에서 제거된 카드는 대기 중인 점검도 취소한다.
- [FACT] 등장 중 ShowTopCard는 조기 점검을 시작하지 않는다. 도착 후에는 기존 수동 호출을 유지한다. SuccessClearStage는 이전 Card 코루틴을 중단하고 완료 횟수를 초기화하여 새 점검을 시작한다.
- 이유: 새 카드가 즉시 나타나던 D-020 구현과 사용자 이미지의 등장 동작 차이를 해소하고 이동 완료 전에 Todo 연출이 시작되지 않도록 한다.
- 대안: CheckClearCard의 In_Clear 애니메이션을 다시 재생하면 별도 연출과 0.875초 이벤트를 거치므로 이번 자동 진입에서는 기존 점검 메서드를 직접 호출한다. 애니메이션 에셋은 변경하지 않았다.
- 검증: 실제 CardManager·UIAnimationRoutine과 Card/Unity 대체 타입으로 컴파일·실행하여 진입 초기값, 중간 보간, 조기 수동 호출 방지, 연속 추가의 위치·알파 유지, 도착 후 한 번 점검, 재배치 재점검 방지, 이동 중 제거, 제한 초과 취소, 0초 재등장 등 9개 검증을 통과했다. 실제 Card의 완료 횟수 초기화는 정적 확인했다.
- [UNCERTAIN] Unity Play Mode의 Animator·팝업·레이아웃 통합 결과는 미검증이다. MKV 대신 사용자가 첨부한 이미지와 설명을 기준으로 반영했다.
- 범위: Todo.CheckClear의 미완료 항목도 연출 완료로 집계하고 전체 점검 후 카드를 닫는 기존 정책은 유지한다. 자동으로 닫힌 카드의 Manager 목록 잔류도 이번 범위에서 변경하지 않았다. ratio=0이면 수평 이동 없이 페이드만 실행된다.

## D-020 — 크기 변경 없이 새 카드 추가 시 기존 카드를 왼쪽으로 배치

날짜: 2026-10-09 (Asia/Seoul)

상태: Accepted — 사용자가 크기 조정을 제거하고 원래 위치에서 새 카드가 추가될 때 기존 카드가 왼쪽으로 밀리는 동작을 요청했다.

- [FACT] CardManager는 신규 카드를 OriginalPosition에 표시하고 마지막 형제로 배치한다. 목표 오프셋은 카드 너비 × ratio/100 × (개수-1-index)의 음수이며 최신 카드는 오프셋 0이다. 각 카드의 현재 위치에서 사인 보간한다. ratio와 animDuration은 기존 Inspector 설정을 유지한다.
- [FACT] 재배치 코루틴 하나가 카드 목록과 시작·목표 위치의 스냅샷을 처리한다. 재배치·제거 요청 시 기존 코루틴을 중단하므로 서로 다른 목표가 같은 좌표를 덮어쓰지 않는다. 개수 초과 시 가장 오래된 카드를 현재 위치에서 페이드로 제거한다. 제거 후 남은 카드도 재배치한다.
- [FACT] Card의 닫기 연출은 현재 알파에서 0으로만 보간하며 크기를 바꾸거나 복원하지 않는다. ShowForLayout은 진행 중인 카드 코루틴을 중단하고 표시한다. 최초 Start는 닫기 애니메이션 대신 즉시 숨기되, 먼저 표시 요청을 받은 카드는 숨기지 않는다. 시작·결과 카드 열기도 이 표시 경로를 사용한다.
- [FACT] SampleScene_Flow의 같은 카드에 OriginalPopUp이 붙어 있고 Awake에서 Close를 실행하며 크기를 0으로 만들 수 있다. OriginalPopUp은 같은 객체에 Card가 있을 때 크기 애니메이션만 건너뛴다. 일반 팝업의 크기 연출은 유지한다. 위치·알파의 독립 동시 재생은 이번 변경으로 통합하지 않았다.
- 이유: 최초 위치로 매번 되돌아가는 이동 시작점, 중복 이동 코루틴, 초기 닫기와 표시의 충돌을 제거하고 고정 크기의 카드 누적 배치를 구현한다. Pivot이나 Animator 클립을 수정하는 대신 코드의 크기 변경을 제거했다.
- 검증: 실제 CardManager/Card/UIAnimationRoutine을 Unity 대체 타입과 함께 컴파일하고 단일·연속 추가, 이동 도중 추가, 제한 초과, 제거, 재추가, Start 이전 표시, 닫기 취소, limit=0 등 17개 검증을 통과했다. OriginalPopUp의 크기 변경 분기는 정적으로 확인했다. Unity 실행·Animator·레이아웃의 실제 결과는 미검증이다.
- [UNCERTAIN] 사용자 참고 MKV는 브라우저 로컬 파일 보안 정책으로 열지 못해 시각적으로 비교하지 않았다. 문자로 지정한 동작을 기준으로 구현했다.
- 범위: 완료 연출 후 Card 자체가 닫힌 경우 Manager 목록에 남는 기존 정책과 Todo 완료 횟수 재초기화는 변경하지 않았다. 동일 카드가 목록에 남아 있으면 AddCard 중복 요청은 계속 무시한다. 부모 LayoutGroup 또는 OriginalPopUp.Open이 동시에 위치를 변경하는 경우 별도 검토가 필요하다.

## D-019 — 완전 축소 후 부활 완료를 기다리고 알파 페이드

날짜: 2026-10-08 (Asia/Seoul)

상태: Accepted — 사용자가 0.5 축소 → 0 축소 → 부활 → 알파 1에서 0으로 페이드 순서를 지정했다.

- [FACT] EffectManager는 Scale=0 적용 후 ResetGame을 먼저 호출하고 해당 캐릭터의 RespawnCoroutine이 끝날 때까지 기다린 다음 알파 페이드를 실행한다. 페이드 완료 후 마스크 크기 초기화와 UnPauseCharacter를 실행한다. D-018의 리셋 시점을 변경한다.
- [FACT] CharacterControl.ResetAction은 문자열 대신 RespawnRoutine()으로 코루틴을 시작하고 반환된 Coroutine을 읽기 전용 공개 프로퍼티에 보관한다. 위치 복원·Idle 전환·한 프레임 대기·CameraReset을 포함한 기존 리스폰 작업의 완료를 기다릴 수 있다. ResetAction에 페이드 연출은 넣지 않는다.
- 이유: 페이드 전에 부활 위치와 카메라를 복원하여 죽은 위치가 먼저 드러나는 순서를 방지한다. 고정 시간 대기나 별도 완료 이벤트 대신 실제 리스폰 코루틴 핸들을 사용한다.
- [FACT] 0.72초 첫 축소, 0.18초 중간 대기, 0.36초 두 번째 축소와 기본 0.36초 페이드는 유지한다. 알파는 현재 값에서 0으로 보간하며 기본 visibleAlpha=1인 경우 1→0이다. 물리 재개는 페이드 이후이다.
- 검증: ResetGame 중 RespawnCoroutine 저장 → 리스폰의 CameraReset 실행 완료 → 페이드 → 마스크 초기화 순서를 정적으로 검토했고 변경 코드의 git diff --check를 통과했다. Unity 컴파일·렌더링 결과는 미검증이며 Cinemachine의 최종 화면 반영도 Play Mode 확인이 필요하다.
- 재검토 조건: 리스폰 코루틴 취소, 캐릭터 비활성화 또는 연출 중 추가 리셋이 필요할 때 취소·중복 실행 정책을 별도 검토한다.

## D-018 — 포커스 축소 후 알파 페이드, 이후 크기 초기화

날짜: 2026-10-08 (Asia/Seoul)

상태: Accepted — 사용자가 완전히 축소한 뒤 Color.alpha를 서서히 0으로 변경하고 크기를 복원하도록 지시했다.

- [FACT] EffectManager는 기존 0.72초 축소·0.18초 대기·0.36초 축소 이후 Scale=0을 유지하며 현재 머티리얼 알파에서 0으로 선형 보간한다. FocusMaskController.ReturnAlpha로 현재 _OverlayColor.a를 읽고 기존 SetAlpha로 적용한다.
- [FACT] 페이드가 끝난 후 InitFocusMaskMaterial로 크기를 10, 알파를 0으로 초기화하고 기존 캐릭터 재개 → ResetGame 순서를 실행한다. 페이드 중에도 isFocusWorking을 유지한다.
- 이유: 축소 직후 크기·알파를 즉시 초기화하던 동작을 점진적인 투명화로 변경한다. RGB와 기존 축소 곡선은 유지한다.
- [INFERENCE] 사용자가 페이드 길이를 지정하지 않아 두 번째 축소와 같은 0.36초를 기본값으로 선택했다. EffectManager.focusFadeDuration 공개 필드로 Inspector에서 조절할 수 있다. UIAnimationRoutine의 unscaled 시간과 종료 시 apply(1) 처리를 사용한다.
- 검증: 호출 순서, 현재 알파 사용, UIAnimationRoutine의 최종값 적용 및 변경 코드의 git diff --check를 확인했다. Unity 컴파일·실제 시각 연출은 미검증이다.
- 재검토 조건: 화면이 가려진 동안 리스폰한 뒤 페이드로 새 위치를 보여주려면 리셋 시점도 별도 변경해야 한다. 이번 요청에서는 리셋 순서를 변경하지 않았다.

## D-017 — Hazard에서 사망 상태 진입, Die에서 사운드 재생

날짜: 2026-10-08 (Asia/Seoul)

상태: Accepted — 사용자가 사망 상태 전환 위치를 FocusOnPosition 대신 Hazard로 지정했다.

- [FACT] Hazard는 레이어·CharacterControl·CharacterAnimation을 확인하고 ChangeState(Die)가 성공한 경우에만 기존 FocusOnPosition을 호출한다. Die.EnterState는 사망 사운드를 재생하며 OnUpdateState는 Die를 반환하여 리스폰까지 유지한다.
- 이유: ResetAction의 사망 사운드는 포커스 연출 종료 뒤 재생되며 일반 리셋에도 발생했다. 사망 진입 이벤트로 옮겨 사운드 재생 요청 → 포커스 시작 → 리셋 순서를 만든다. 사운드 종료를 기다리지는 않는다.
- [FACT] CharacterControl.ResetAction에서 사망 사운드를 제거했다. RespawnRoutine은 위치 복원 직후 현재 상태가 Die이면 Idle로 전환하며 기존 한 프레임 대기와 카메라 리셋을 유지한다. 일반 리셋의 다른 상태를 강제로 Idle로 바꾸지는 않는다.
- [FACT] 같은 Die 상태로 ChangeState하면 false이므로 반복 접촉은 사운드와 포커스를 재요청하지 않는다. EffectManager에는 사망 전환이나 사운드를 추가하지 않았다.
- 대안: FocusOnPosition 내부에서 상태를 전환하는 안은 사용자 지시로 제외했다. Hazard에서 사운드를 직접 재생하면 상태 진입과 재생 책임이 분리되므로 채택하지 않았다.
- 검증: 실제 Hazard, Die, CharacterAnimation, CharacterStateBase를 Unity 대체 타입과 함께 컴파일하여 7개 검증을 통과했다. 소리→포커스 순서, Die 유지, 중복 접촉 차단, Idle 복귀 뒤 재사망, 레이어 제외를 확인했다. 리스폰 삽입 위치는 정적으로 검토했다. Unity 컴파일·Play Mode·실제 사운드는 미검증이다.
- [UNCERTAIN] EffectManager의 포커스 잠금은 전역이다. 여러 캐릭터가 동시에 사망하거나 다른 포커스가 진행 중인 경우에는 사망 상태 진입과 포커스 수락이 일치하지 않을 수 있다. 기존 수동 리셋 중 연출 취소 정책도 이번 범위에 포함하지 않았다. 해당 요구가 생기면 별도 검토한다.

## D-016 — 이름별 오디오 그룹과 공통 랜덤 선택

날짜: 2026-10-08 (Asia/Seoul)

상태: Accepted — 사용자가 그룹 구조 변경을 승인하고 AudioName의 숫자 명시는 제외하도록 지시했다.

- [FACT] AudioName은 Jump, Walk, Die, Switch, CameraSwitch, ItemSound를 자동 번호로 사용한다. AudioGroup은 그룹 이름과 AudioClip 목록을 저장하고 AudioManager.audioGroups에서 Inspector 등록을 받는다.
- [FACT] Awake에서 Dictionary<AudioName, List<AudioClip>>을 구성하며 null 그룹·클립은 제외하고 중복 이름은 경고 후 첫 그룹을 유지한다. 빈 그룹과 미등록 그룹의 재생 요청은 무시한다.
- [FACT] 두 SFX 재생 메서드는 GetRandomClip을 공유하고 UnityEngine.Random.Range로 목록 인덱스를 선택한다. 기존 Play/PlayOneShot 방식, 믹서·스냅샷 처리는 유지한다. 연속 반복을 허용하며 같은 클립을 여러 번 등록하면 그만큼 선택 빈도가 높아진다.
- 이유: 개별 파일명 대신 Jump 등의 용도로 요청하고, 여러 종류의 사운드에서 동일한 그룹 선택 기능을 재사용한다. 점프 전용 목록과 이름 접두사 자동 분류는 사용하지 않는다.
- [FACT] Assets/01.Scene 아래 7개 씬의 프리팹 인스턴스 10곳을 이전했다. 기존 39개 클립 참조를 보존하며 확인된 파일 이름에 따라 Die/Switch/CameraSwitch/ItemSound 그룹을 지정했다. Assets C#에서 jump1/2/3 호출이나 별도 AudioName 필드는 발견되지 않았고, 씬·프리팹의 두 재생 메서드 UnityEvent 및 애니메이션 이벤트 연결도 검색에서 발견되지 않았다.
- [UNCERTAIN] AudioMixer.prefab과 SampleScene.unity의 기존 클립 GUID 8114a8cae8416694f85dc4d267de3e6a는 Assets 메타에서 찾지 못했다. 두 파일의 예전 clips 직렬화 텍스트는 보존하고 audioGroups는 빈 목록으로 두었다. 예전 필드는 현재 코드에서 읽지 않으며 Unity 재저장 시 보존을 보장하지 않는다. 원본 에셋과 그룹을 확인한 뒤 등록해야 한다. _Recovery 백업 씬은 이전하지 않았다.
- 검증: 실제 AudioManager 코드를 Unity 대체 타입으로 컴파일하여 10개 검증을 통과했다. 후보 인덱스별 재생, null 제외, 첫 중복 그룹 유지, 단일·빈·미등록 그룹, 재초기화 및 enum 자동 번호를 확인했다. 씬 이전 전후 39개 클립 참조의 일치와 변경 코드·씬의 git diff --check를 확인했다. Unity 컴파일·Play Mode와 실제 난수 분포·오디오 출력은 검증하지 않았다.
- 재검토 조건: 런타임 그룹 편집, 무반복 재생, 가중치, 백업 씬 복구 또는 기존 enum 직렬화 데이터를 추가 발견할 때 이전 및 선택 정책을 재검토한다.

## D-015 — 포커스 마스크의 두 단계 축소

날짜: 2026-10-08 (Asia/Seoul)

상태: Accepted — 사용자가 첫 축소 0.72초, 대기 0.18초, 두 번째 축소 0.36초로 적용을 지시했다.

- [FACT] EffectManager.FocusOnPositionCoroutine은 현재 크기에서 DefaultScale(현재 0.5)까지 0.72초 동안 축소하고 0.18초 대기한 다음 0까지 0.36초 동안 축소한다. 두 축소 모두 기존 UIAnimationRoutine과 사인 보간을 사용한다.
- 이유: 기존 연출은 0.5까지 축소 후 대기하고 바로 마스크를 초기화했다. 사용자가 요청한 완전 축소 단계를 추가한다.
- [FACT] 최종 SetScale(0) 이후 한 프레임을 기다리고 기존 마스크 초기화 → 캐릭터 재개 → 게임 리셋 순서를 유지한다. FocusMaskController와 셰이더는 변경하지 않는다. 셰이더 내부 크기 하한은 기존 0.0001이다.
- 대안: 즉시 0으로 설정하면 두 번째 축소 시간을 표현하지 못하므로 연속 보간을 선택했다. SetScale에 대기 책임을 추가하지 않고 EffectManager에서 연출 순서를 관리한다.
- [FACT] 시간 기준은 기존 방식대로 축소는 unscaled, 중간 대기는 WaitForSeconds의 scaled 시간이다. timeScale=1에서 지정된 구간 합은 1.26초이며 프레임 대기·프레임 단위 진행에 따른 시간이 추가된다.
- 검증: 변경 diff와 호출 흐름을 정적으로 확인했다. Unity 컴파일·Play Mode 실행은 수행하지 않았다.
- 재검토 조건: timeScale 변경 중에도 대기를 실제 시간 0.18초로 유지해야 하면 대기의 시간 기준을 재검토한다.

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

