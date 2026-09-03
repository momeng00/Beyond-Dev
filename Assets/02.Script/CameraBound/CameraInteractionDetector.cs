using System.Collections.Generic;
using UnityEngine;


public class CameraInteractionDetector : MonoBehaviour
{
    [SerializeField]
    private Camera targetCamera;

    // 카메라가 감지할 전체 상호작용 대상
    private readonly List<IInteractable> interactables = new();

    // 현재 카메라 영역 안에 있는 상호작용 대상
    private readonly HashSet<IInteractable> detected = new();

    private void Awake()
    {
        // 카메라가 지정되지 않았다면 Main Camera 사용
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        CheckCameraBounds();
    }

    private void CheckCameraBounds()
    {
        // 현재 카메라가 바라보는 월드 영역
        Bounds cameraBounds = GetCameraBounds();
        Debug.Log(
    $"Camera Bounds Center: {cameraBounds.center}, " +
    $"Size: {cameraBounds.size}"
);

        foreach (IInteractable interactable in interactables)
        {
            // IInteractable이 Component를 상속받은 경우에만 처리
            // Component를 상속받는 다는 것에 대한 개념
            if (interactable is not Component component)
                continue;

            // 상호작용 대상의 Renderer 영역 가져오기
            // 이게 뭐시여 왜 IInteractable의 renderer를 가져오지?
            Renderer renderer = component.GetComponent<Renderer>();
            Debug.Log("renderer= " + renderer);
            if (renderer == null)
                continue;

            // 오브젝트의 영역이 카메라 영역과 겹치는지 확인
            // 내부 기능인데 무슨 기능인지 설명
            bool isInside =
    cameraBounds.min.x <= renderer.bounds.max.x &&
    cameraBounds.max.x >= renderer.bounds.min.x &&
    cameraBounds.min.y <= renderer.bounds.max.y &&
    cameraBounds.max.y >= renderer.bounds.min.y;
            Debug.Log(
           $"{component.gameObject.name} | " +
           $"Object Center: {renderer.bounds.center} | " +
           $"Object Size: {renderer.bounds.size} | " +
           $"Inside: {isInside}"
       );
            Debug.Log("renderer 걉쳐짐? ---" + isInside );
            if (isInside)
            {
                // 처음 들어온 경우에만 Enter 이벤트 호출
                //이미 있다면 false를 반환하겠지?
                if (detected.Add(interactable))
                {
                    interactable.OnCameraEnter();
                    Debug.Log("renderer 감지 완료 실행");
                }
            }
            else
            {
                // 영역 밖으로 나간 경우에만 Exit 이벤트 호출
                // 이미 있는 것에 대한 체크가 이루어 지고있는가?
                if (detected.Remove(interactable))
                {
                    interactable.OnCameraExit();
                }
            }
        }
    }

    private Bounds GetCameraBounds()
    {
        // Orthographic 카메라의 화면 높이
        float height = targetCamera.orthographicSize * 2f;

        // 화면 비율에 따른 화면 너비
        float width = height * targetCamera.aspect;

        // 카메라 위치를 중심으로 화면 영역 생성
        return new Bounds(
            targetCamera.transform.position,
            new Vector3(width, height, 0f)
        );
    }

    public void Register(IInteractable interactable)
    {
        // 감지 대상에 등록
        if (!interactables.Contains(interactable))
        {
            interactables.Add(interactable);
        }
    }

    public void Unregister(IInteractable interactable)
    {
        // 감지 대상에서 제거
        interactables.Remove(interactable);

        // 현재 감지 목록에서도 제거
        detected.Remove(interactable);
    }
}