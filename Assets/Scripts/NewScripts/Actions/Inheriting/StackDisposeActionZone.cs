using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackDisposeActionZone : ActionZone
{
    public GameObject DisposingPoint;

    protected override void SuccessfullyFinish()
    {
        base.SuccessfullyFinish();
        SetReadiness(false);
        ActionTimer.Instance.SetIndication(0, 0);

        int found;
        do
        {
            found = -1;

            for (int z = 0; z < PlayerStacking.Instance.Stack.Count; z++)
                if (PlayerStacking.Instance.Stack[z].GetComponent<StackableObject>().Disposable)
                {
                    found = z;
                    break;
                }

            if (found != -1)
            {
                PlayerStacking.Instance.RemoveFromStack(found, DisposingPoint.transform);
                EventController.TriggerEvent(EventController.EventTypes.StackingQuantityChanged, "");
            }
        } while (found > -1);
    }

    public override void PlayerLeaveZone()
    {
        base.PlayerLeaveZone();
        ActionTimer.Instance.SetIndication(0, 0);
    }

    private void CheckReadiness (string _dummyParam)
    {
        SetReadiness(false);

        for (int z = 0; z < PlayerStacking.Instance.Stack.Count; z++)
            if (PlayerStacking.Instance.Stack[z].GetComponent<StackableObject>().Disposable)
            {
                SetReadiness(true);
                return;
            }
    }

    void Start()
    {
        SetReadiness(false);
        EventController.AddListener(EventController.EventTypes.StackingQuantityChanged, CheckReadiness);
    }
}
