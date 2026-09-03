using System.Collections.Generic;
using UnityEngine;
public interface IInteractable
{
    void OnCameraEnter();
    void OnCameraExit();
}

public class InteractObject : MonoBehaviour, IInteractable
{
    [SerializeField]
    private List<InteractAction> actions = new();
    public CameraInteractionDetector detector;
    private void Start()
    {
        if (detector != null)
        {
            detector.Register(this);
        }
    }
    public void OnCameraEnter()
    {
        foreach (var action in actions)
        {
            action.Execute();
        }
    }

    public void OnCameraExit()
    {

    }
}