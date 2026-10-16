using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorItem : ClearCondition, IReset
{
    [SerializeField] private List<NormalPopUp> pickupPopUps;
    [SerializeField, Min(0f)] private float pickupPopUpDelay = 0.2f;
    private Coroutine pickupPopUpCoroutine;
    private bool satisfied = false;
    public bool Satisfied
    {
        get 
        {
            return satisfied; 
        }
        set
        {
            satisfied = value;
            OnCheck?.Invoke();
        }
    }
    public LayerMask layerMask;
    private void Start()
    {
        GameManager.Instance.OnReset += ResetAction;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    public override bool IsSatisfied()
    {
        return Satisfied;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.GetComponent<SpriteRenderer>().enabled)
            return;
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            Satisfied = true;
            //AudioManager.Instance.PlayOneShotSFXAudio(AudioName.ItemSound);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

            if (pickupPopUps != null && pickupPopUps.Exists(popup => popup != null))
            {
                StopPickupPopUps();
                pickupPopUpCoroutine = StartCoroutine(ShowPickupPopUps());
            }
        }
        
    }
    private IEnumerator ShowPickupPopUps()
    {
        foreach (NormalPopUp popup in pickupPopUps)
        {
            if (popup == null)
                continue;

            popup.ToggleEvent(true);
            yield return new WaitForSeconds(pickupPopUpDelay);
        }

        pickupPopUpCoroutine = null;
    }

    private void StopPickupPopUps()
    {
        if (pickupPopUpCoroutine == null)
            return;

        StopCoroutine(pickupPopUpCoroutine);
        pickupPopUpCoroutine = null;
    }

    private void OnDisable()
    {
        StopPickupPopUps();
    }

    virtual protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }

    public void InitializeReset()
    {
        throw new System.NotImplementedException();
    }

    public void ResetAction()
    {
        StopPickupPopUps();
        Satisfied = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    public override void ClearAction()
    {
        throw new NotImplementedException();
    }
}