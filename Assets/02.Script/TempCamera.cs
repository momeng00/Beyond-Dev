using UnityEngine;

public class TempCamera : MonoBehaviour
{
    [Header("Target Aspect Ratio")]
    [Tooltip("원하는 화면 비율의 가로 값입니다. (예: 16)")]
    public float targetAspectRatioWidth = 1920.0f;

    [Tooltip("원하는 화면 비율의 세로 값입니다. (예: 9)")]
    public float targetAspectRatioHeight = 1080.0f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        UpdateAspectRatio();
    }

    void Update()
    {
        // 에디터 모드이거나, 실제 게임 플레이 중 창 크기가 변경될 때를 대비해 Update에도 호출
        UpdateAspectRatio();
    }

    /// <summary>
    /// 현재 화면 비율을 확인하고 카메라의 Viewport Rect를 조절하는 함수
    /// </summary>
    public void UpdateAspectRatio()
    {
        // 목표 화면 비율 계산 (예: 16 / 9 = 1.777...)
        float targetAspect = targetAspectRatioWidth / targetAspectRatioHeight;

        // 현재 화면의 비율 계산
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // 현재 화면 비율에 맞춰 스케일 팩터 계산
        float scaleHeight = windowAspect / targetAspect;

        // Rect: 카메라가 화면의 어느 부분에 렌더링할지를 결정 (0~1 사이의 값)
        Rect cameraRect = mainCamera.rect;

        if (scaleHeight < 1.0f) // 현재 화면이 목표보다 세로로 더 길 경우 (레터박싱)
        {
            cameraRect.width = 1.0f;
            cameraRect.height = scaleHeight;
            cameraRect.x = 0;
            cameraRect.y = (1.0f - scaleHeight) / 2.0f; // 세로 중앙 정렬
        }
        else // 현재 화면이 목표보다 가로로 더 넓을 경우 (필러박싱)
        {
            float scaleWidth = 1.0f / scaleHeight;
            cameraRect.width = scaleWidth;
            cameraRect.height = 1.0f;
            cameraRect.x = (1.0f - scaleWidth) / 2.0f; // 가로 중앙 정렬
            cameraRect.y = 0;
        }

        // 계산된 Rect를 카메라에 적용
        mainCamera.rect = cameraRect;
    }
}
