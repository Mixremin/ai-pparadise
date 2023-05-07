using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackingRestockActionZone : ActionZone
{
    [SerializeField]
    private StackableType ObjectsType;
    public StackableType Type => ObjectsType;
    public GameObject RestockingObject;
    public Transform CreateAtPoint;

    protected override void SuccessfullyFinish()
    {
        base.SuccessfullyFinish();
        FlyingObjects.Instance.CreateEffect(RestockingObject, CreateAtPoint, GameManager.Instance.PlayerFXPoint, 1, false);
        if (PlayerStacking.Instance.Stack.Count < (GameManager.Instance.MaxStackSize - 1))
            base.EnterCooldown();
        else
            base.SetReadiness(false);
    }

    public override void PlayerLeaveZone()
    {
        base.PlayerLeaveZone();
        ActionTimer.Instance.SetIndication(0, 0);
        SetReadiness(true);
    }

    public override void PlayerStayInZone(float _delta, float _timerModifier, bool _indication)
    {
        if (actParams.BaseDuration <= 0)
            actParams = GameManager.Instance.GetActionParameters(ZoneType);

        base.PlayerStayInZone(_delta, _timerModifier, false);
    }

    private void CheckActionAvailability(string _dummyParam)
    {
        SetReadiness(PlayerStacking.Instance.StackingAvailable());
//        Debug.Log(PlayerStacking.Instance.Stack.Count.ToString() + " in stack / " + GameManager.Instance.MaxStackSize.ToString());
    }

    private void Start()
    {
        EventController.AddListener(EventController.EventTypes.StackingQuantityChanged, CheckActionAvailability);
    }
}
