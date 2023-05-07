using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionZoneType
{
    Undefined = 0,
    Reception = 1,
    Dresser = 2,
    Echo = 3,
    CafeChair = 4,
    WardBed = 5,
    StackableRestock = 6,
    StackableDispose = 7,
    ChildTable = 8,
    ChildTablePhaseTwo = 9,
    ChildReturn = 10
}

public class ActionZone : MonoBehaviour
{
    [HideInInspector]
    public ActionParameters actParams;
    [HideInInspector]
    public float Timer;
    public ActionZoneType ZoneType;
    [HideInInspector]
    public bool ReadyStatus;
    private bool parametersSet;

    private GameObject thisFX;

    private IEnumerator ResetZone(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        if (PlayerStacking.Instance.HaveBabiesInStack(false))
            SetReadiness(true);
    }

    public void ResetChildTableZone (float _delay)
    {
        StartCoroutine(ResetZone(_delay));
    }

    protected virtual void UpdateTimer(bool _indication)
    {
        SetParameters();
        if (_indication) ActionTimer.Instance.SetIndication(Timer, actParams.BaseDuration);
        if (Timer >= actParams.BaseDuration)
            SuccessfullyFinish();
    }

    protected virtual void EnterCooldown()
    {
        if (actParams.BaseCooldown <= 0)
            return;

        SetReadiness(false);
        StartCoroutine(ActionCooldown());
    }

    IEnumerator ActionCooldown()
    {
        yield return new WaitForSeconds(actParams.BaseCooldown);
        SetReadiness(true);
    }

    public virtual void SetParameters()
    {
        if (parametersSet)
            return;
        
        actParams = GameManager.Instance.GetActionParameters(ZoneType);
        parametersSet = true;
    }

    public virtual void SetReadiness(bool _ready)
    {
        ReadyStatus = _ready;
        this.GetComponent<Collider>().enabled = _ready;
        this.GetComponent<MeshRenderer>().enabled = _ready;

        if (thisFX == null)
            thisFX = this.transform.GetChild(0).gameObject;

        if (thisFX)
            thisFX.SetActive(_ready);
    }

    private void ResetTimer()
    {
        Timer = 0;
        ActionTimer.Instance.SetIndication(0, 0);
    }

    // hell knows what I'll have to put there, so it's a stub

    public virtual void PlayerEnterZone ()
    {
        ResetTimer();
    }

    public virtual void PlayerLeaveZone()
    {
        ResetTimer();
    }

    public virtual void PlayerStayInZone(float _delta, float _timerModifier, bool _indication)
    {
        if (ZoneType != ActionZoneType.ChildTable)
        {
            Timer += _delta * _timerModifier;
            UpdateTimer(_indication);
        }
    }

    protected virtual void SuccessfullyFinish()
    {
        if (ZoneType != ActionZoneType.ChildTable)
        {
            SetParameters();
            Timer = 0;
            ActionTimer.Instance.SetIndication(0, 0);

            PlayerChar.Instance.ChangeMoney(actParams.Reward);
        }
    }

    private void Start()
    {
        parametersSet = false;
        Timer = 0;
    }
}
