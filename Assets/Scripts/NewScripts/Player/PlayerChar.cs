using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar : MonoBehaviour
{
    static PlayerChar thisInstance;
    public int Money { get; private set; }
    [HideInInspector]
    public float TriggerDelay;


    // singleton behaviour
    public static PlayerChar Instance
    {
        get
        {
            if (!thisInstance)
            {
                thisInstance = FindObjectOfType(typeof(PlayerChar)) as PlayerChar;

                if (!thisInstance)
                    Debug.LogError("No active PlayerChar script found in scene.");
            }

            return thisInstance;
        }
    }

    // placeholder
    public float ActionSpeedModifier ()
    {
        return 1f;
    }

    public void FinishAction()
    {
        ActionTimer.Instance.SetIndication(0, 0);
    }

    public void ChangeMoney(int _delta)
    {
        int delta = _delta;
        if ((GameManager.Instance.CreativeMode) && (!GameManager.Instance.CreativeProMode) && (!GameManager.Instance.CreativeNormalMode))
        {
            delta /= 5;
            if (delta == 0)
                delta = 1;
        }


        Money += delta;
        MoneyDisplay.Instance.UpdateCounter(Money);
        PlayerPrefs.SetInt("Money", Money);
    }

    public void HitByFlyingObject (GameObject _obj)
    {
        _obj.TryGetComponent(out StackableObject stackable);
        if (stackable == null)
            return;

        if ((stackable.Disposable) && (!PlayerStacking.Instance.StackingAvailable()))
        {
            // too much stuff
            Destroy(_obj);
            return;
        }

        PlayerStacking.Instance.AddToStack(_obj);
        FlyingFXObject ffo = _obj.GetComponent<FlyingFXObject>();
        ffo.ResetRotations();
        Destroy(ffo);

        _obj.TryGetComponent(out NPCBaby baby);
            if (baby != null)
                EventController.TriggerEvent(EventController.EventTypes.BabiesInStackAmountChanged, "");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ActionZone az))
            az.PlayerEnterZone ();
    }

    private void OnTriggerStay(Collider other)
    {
        float delta = Time.deltaTime;
        if (GameManager.Instance.CreativeProMode)
            delta *= 2f;
        if ((GameManager.Instance.CreativeMode) && (!GameManager.Instance.CreativeProMode) && (!GameManager.Instance.CreativeNormalMode))
            delta *= 0.6f;

        TriggerDelay += delta;
        if (TriggerDelay < GameManager.Instance.TriggerActivationDelay)
            return;

        if (other.gameObject.TryGetComponent(out ActionZone az))
            az.PlayerStayInZone(delta, ActionSpeedModifier(), true);

        if (other.gameObject.TryGetComponent(out UnlockableObject uo))
            if (Money >= uo.GetUnlockingCost())
                uo.AddActivationTimer(delta);

        if (other.gameObject.tag == "TileUnlockTrigger")
        {
            PriceTag pt = other.transform.GetChild(0).GetComponent<PriceTag>();
            LevelTile tile = other.transform.parent.parent.GetComponent<LevelTile>();

            if (pt == null) return;
            if (tile == null) return;
            if (pt.Price > Money) return;
            if (pt.Price <= 0) return;

            tile.AddUnlockTimer(delta, other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ActionZone az))
            az.PlayerLeaveZone();

        if (other.gameObject.TryGetComponent(out UnlockableObject uo))
            uo.PlayerLeave();

        if (other.gameObject.tag == "TileUnlockTrigger")
        {
            LevelTile tile = other.transform.parent.parent.GetComponent<LevelTile>();
            tile.PlayerLeaveTrigger();
        }
    }

    private void Start()
    {
        TriggerDelay = 0;
    }
}
