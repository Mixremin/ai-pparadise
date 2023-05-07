using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceptionActionZone : ActionZone
{
    public NPCQueue ControlledQueue;
    private bool npcReached;

    protected override void SuccessfullyFinish()
    {
        base.SuccessfullyFinish();

        FlyingObjects.Instance.CreateEffect(GameManager.Instance.FlyingMoneyPrefab, ControlledQueue.QueuePlaces[0].transform,
            GameManager.Instance.PlayerFXPoint, actParams.Reward, true);

        SetReadiness(false);
        npcReached = false;
        NPCMain firstNPC = ControlledQueue.RemoveFirstNPC();
        EventController.TriggerEvent(EventController.EventTypes.ReceptionActionFinished, "");
        if (firstNPC == null) return;

        // trying to find an available dresser
        UsableObject dresser = GameManager.Instance.GetUsableGroupByType(UsableObjectGroupType.Dressers).GetReadyObject();
        if (dresser == null)
        {
            firstNPC.InitPhase(NPCPhases.WalkingToReceptionSofa);
            firstNPC.StartMovement("ReceptionSofaApproachWpt1");
        }
        else
        {
            dresser.UsedByNPC = firstNPC;
            dresser.isOccupied = false;
            firstNPC.InitPhase(NPCPhases.WalkingToDresser);
            firstNPC.MoveToUsableObject(dresser);
        }
    }

    private void OnNPCReachQueueFirstPlace(string _dummyParam)
    {
        npcReached = true;
        SetReadiness(true);
    }

    public override void PlayerStayInZone(float _delta, float _timerModifier, bool _indication)
    {
        if (actParams.BaseDuration <= 0)
            actParams = GameManager.Instance.GetActionParameters(ActionZoneType.Reception);

        base.PlayerStayInZone(_delta, _timerModifier, _indication);
    }

    public override void PlayerLeaveZone()
    {
        StopAllCoroutines();
        if (Timer > 0)
            SetReadiness(npcReached);

        base.PlayerLeaveZone();
        ActionTimer.Instance.SetIndication(0, 0);
    }

    IEnumerator CheckReadinessCycle()
    {
        base.SetParameters();

        do
        {
            yield return new WaitForSeconds(actParams.BaseCooldown);
            SetReadiness(npcReached);
        } while (true);
    }

    private void Start()
    {
        SetReadiness(false);
        npcReached = false;
        EventController.AddListener(EventController.EventTypes.NPCReachedFirstReceptionQueuePlace, OnNPCReachQueueFirstPlace);
        StartCoroutine(CheckReadinessCycle());
    }
}
