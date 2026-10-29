using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(11000)]
public class CameraInteractionDetector : MonoBehaviour
{
    private sealed class DetectionTarget
    {
        public InteractObject interactable;
        public CameraVisibility visibility;
        public bool isInside;
        public bool removed;
    }

    [SerializeField]
    private Camera targetCamera;

    private readonly List<DetectionTarget> targets = new();
    private bool isChecking;
    private bool hasRemovedTargets;

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

    private void OnDestroy()
    {
        // Release cached references even if surviving targets still hold this component.
        if (isChecking)
        {
            for (int i = 0; i < targets.Count; i++)
                targets[i].removed = true;
            hasRemovedTargets = true;
            return;
        }
        targets.Clear();
    }

    private void CheckCameraBounds()
    {
        // UI may use its own Canvas camera, or no camera at all (Overlay).
        // Let each sensor resolve the applicable camera instead of skipping the entire list.
        if (targetCamera == null)
            targetCamera = Camera.main;

        isChecking = true;
        // Targets registered by an event are first checked on the next update.
        int count = targets.Count;
        try
        {
            for (int i = 0; i < count; i++)
            {
                DetectionTarget target = targets[i];
                if (target.removed)
                    continue;

                if (target.interactable == null || target.visibility == null)
                {
                    target.removed = true;
                    hasRemovedTargets = true;
                    continue;
                }

                bool isInside = target.visibility.IsVisible(0f, targetCamera);
                if (target.isInside == isInside)
                    continue;

                // Store the state before the callback, which may disable this target.
                target.isInside = isInside;
                if (isInside)
                    target.interactable.OnCameraEnter();
                else
                    target.interactable.OnCameraExit();

                if (this == null || !isActiveAndEnabled)
                    break;
            }
        }
        finally
        {
            isChecking = false;
            RemoveMarkedTargets();
        }
    }

    public void Register(InteractObject interactable)
    {
        if (interactable == null)
            return;

        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].removed && ReferenceEquals(targets[i].interactable, interactable))
                return;
        }

        // 등록 시 참조를 저장하고, 기존 오브젝트에 감지 컴포넌트가 없으면 추가한다.
        CameraVisibility visibility = interactable.GetComponent<CameraVisibility>();
        if (visibility == null)
            visibility = interactable.gameObject.AddComponent<CameraVisibility>();

        targets.Add(new DetectionTarget
        {
            interactable = interactable,
            visibility = visibility
        });
    }

    public void Unregister(InteractObject interactable)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            DetectionTarget target = targets[i];
            if (!target.removed && ReferenceEquals(target.interactable, interactable))
            {
                target.removed = true;
                hasRemovedTargets = true;
                break;
            }
        }

        // Do not shift indices while an Enter/Exit callback is running.
        if (!isChecking)
            RemoveMarkedTargets();
    }

    private void RemoveMarkedTargets()
    {
        if (!hasRemovedTargets)
            return;

        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i].removed)
                targets.RemoveAt(i);
        }
        hasRemovedTargets = false;
    }
}
