using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIElement : MonoBehaviour
{
    public UnityEvent OnCustomClick;
    virtual public void Selected()
    {

    }
    virtual public void UnSelected()
    {

    }
}
