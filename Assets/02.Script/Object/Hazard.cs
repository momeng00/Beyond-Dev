using UnityEngine;

public class Hazard : Block
{

    public LayerMask layerMask;
    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            Debug.Log("찍힘");
            //GameManager.Instance.OnReset?.Invoke(); 이제 이게 아니라 다른곳에서 호출
            CharacterControl characterControl = collision.gameObject.GetComponent<CharacterControl>();
            if (characterControl != null)
            {
                EffectManager.instance.FocusOnPosition(characterControl);
            }
        }
    }

    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
}