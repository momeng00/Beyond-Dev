using UnityEngine;
using UnityEngine.Events;
public interface IInteractable
{
    void OnCameraEnter();
    void OnCameraExit();
}

[RequireComponent(typeof(CameraVisibility))]
public class InteractObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private UnityEvent enterEvent = new();

    [SerializeField]
    private UnityEvent exitEvent = new();

    public CameraInteractionDetector detector;
    private CameraInteractionDetector registeredDetector;

    private void OnEnable()
    {
        if (detector != null)
        {
            registeredDetector = detector;
            registeredDetector.Register(this);
        }
    }

    private void OnDisable()
    {
        if (registeredDetector != null)
            registeredDetector.Unregister(this);
        registeredDetector = null;
    }

    public void OnCameraEnter()
    {
        enterEvent.Invoke();
    }

    public void OnCameraExit()
    {
        exitEvent.Invoke();
    }
}
