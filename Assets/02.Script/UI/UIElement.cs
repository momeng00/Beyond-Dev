using UnityEngine;
using UnityEngine.Events;

public abstract class UIElement : MonoBehaviour
{
    public UnityEvent OnCustomClick;
    virtual public void Selected()
    {

    }
    virtual public void UnSelected()
    {

    }
    virtual public void Action()
    {
        OnCustomClick?.Invoke();
    }
}
