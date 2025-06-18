using UnityEngine;

public interface IDetect
{
    void DetectEnter();
    void DetectExit();
    void DetectAction(GameObject sender);
}