using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonUIElement : UIElement
{
    public Button btn;
    [Tooltip("Button.onClick을 키보드 실행에도 사용합니다. 기존 Custom 이벤트를 이관한 UI에서 활성화하세요.")]
    [SerializeField] private bool useButtonClickEvent;

    public override void Action()
    {
        if (!useButtonClickEvent)
        {
            base.Action();
            return;
        }

        if (btn == null || !btn.IsActive() || !btn.IsInteractable())
            return;

        btn.onClick.Invoke();
    }

    override public void Selected()
    {
        base.Selected();
        if (btn != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(btn.gameObject);
    }
    public override void UnSelected()
    {
        base.UnSelected(); 
    }

}
