using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUIElement : UIElement
{
    public Button btn;

    override public void Selected()
    {
        base.Selected();
        EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }
    public override void UnSelected()
    {
        base.UnSelected(); 
    }

}
