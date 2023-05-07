using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StackableRequirements
{
    public StackableType Type;
    public int Min;
    public int Max;
    [HideInInspector]
    public int Quantity;
}

public class UsableObjectActionZone : ActionZone
{
    public string DelayedFinishAnimation;
    public int DelayedChangeItemState = -1;
    public float DelayedFinishTime = 0f;
    public UsableObject AssociatedObject;
    public Transform FXTargetTransform;
    public Transform ConsumableTransform;
    public GameObject ConsumableObject;
    public StackableRequirements[] StackableRequirements;
    [HideInInspector]
    public List<StackableRequirements> stackablesLeft;
    private GameObject consumable;

    IEnumerator DelayedFinish()
    {
        SetReadiness(false);

        if (DelayedFinishAnimation != "")
            AssociatedObject.UsedByNPC.thisAnim.SetBool(DelayedFinishAnimation, true);

        NPCOutfitStates outfit = AssociatedObject.UsedByNPC.transform.GetChild(0).GetChild(0).GetComponent<NPCOutfitStates>();
        int orig = outfit.CurrentState;
        if (DelayedChangeItemState > -1)
            outfit.SetState(DelayedChangeItemState);

        if (DelayedFinishTime > 0)
            yield return new WaitForSeconds(DelayedFinishTime);

        if (DelayedChangeItemState > -1)
            outfit.SetState(orig);

        if (DelayedFinishAnimation != "")
            AssociatedObject.UsedByNPC.thisAnim.SetBool(DelayedFinishAnimation, false);

        AssociatedObject.FinishObjectUsage(AssociatedObject.LeaveWpt.NextPoint);

        base.SuccessfullyFinish();

        FlyingObjects.Instance.CreateEffect(GameManager.Instance.FlyingMoneyPrefab, this.transform,
            GameManager.Instance.PlayerFXPoint, actParams.Reward, true);

        //        if (AssociatedObject.PartOfGroup != null)
        //            AssociatedObject.PartOfGroup.GetNewNPCFromSofa(AssociatedObject);

        AssociatedObject.UsedByNPC = null;
        AssociatedObject.isOccupied = false;
    }

    protected override void SuccessfullyFinish()
    {
        if (consumable != null)
            consumable.SetActive(false);
        StartCoroutine(DelayedFinish());
    }

    private bool CheckOnEnoughStackables()
    {
        for (int z = 0; z < stackablesLeft.Count; z++)
            if (stackablesLeft[z].Quantity > 0)
                return false;

        return true;
    }

    private bool PlayerHaveNeededStackables()
    {
        for (int z = 0; z < stackablesLeft.Count; z++)
            for (int k = 0; k < PlayerStacking.Instance.Stack.Count; k++)
                if (stackablesLeft[z].Type == PlayerStacking.Instance.Stack[k].GetComponent<StackableObject>().Type)
                    if (stackablesLeft[z].Quantity > 0)
                        return true;

        return false;
    }

    private void TryConsumeStackables()
    {
        if (!PlayerHaveNeededStackables()) return;

        if (Timer < GameManager.Instance.StackConsumeDelay)
        {
            Timer += Time.deltaTime;
            if (ZoneType != ActionZoneType.ChildTablePhaseTwo)
            {
                if (PlayerStacking.Instance.Stack.Count > 0)
                    ActionTimer.Instance.SetIndication(Timer, GameManager.Instance.StackConsumeDelay);
            }
            else
                ActionTimer.Instance.SetIndication(0, 0);
            return;
        }

        Timer = 0;
        ActionTimer.Instance.SetIndication(0, 0);

        for (int z = 0; z < stackablesLeft.Count; z++)
        {
            Transform target = FXTargetTransform;
            if (target == null)
                target = AssociatedObject.transform;

            if (stackablesLeft[z].Quantity > 0)
                if (PlayerStacking.Instance.TryRemoveObjectOfType(stackablesLeft[z].Type, target))
                {
                    stackablesLeft[z].Quantity -= 1;
                    if (stackablesLeft[z].Quantity > 0)
                        return;

                    break;
                }
        }

        if (CheckOnEnoughStackables())
            SuccessfullyFinish();
    }

    public override void PlayerStayInZone(float _delta, float _timerModifier, bool _indication)
    {
        SetParameters();

        if (StackableRequirements.Length == 0)
            base.PlayerStayInZone(_delta, _timerModifier, stackablesLeft.Count <= 0);
        else
            TryConsumeStackables();
    }

    public override void SetReadiness(bool _ready)
    {
        base.SetReadiness(_ready);
        Random.InitState(System.DateTime.Now.Millisecond);
        if (consumable != null)
            consumable.SetActive(_ready);

        if ((_ready) && (StackableRequirements.Length > 0))
        {
            stackablesLeft = new List<StackableRequirements>();
            for (int z = 0; z < StackableRequirements.Length; z++)
            {
                StackableRequirements newR = new StackableRequirements();
                newR.Type = StackableRequirements[z].Type;
                if (StackableRequirements[z].Min != StackableRequirements[z].Max + 1)
                    newR.Quantity = Random.Range(StackableRequirements[z].Min, StackableRequirements[z].Max + 1);
                else
                    newR.Quantity = StackableRequirements[z].Max;
                stackablesLeft.Add(newR);
            }
        }
    }

    public override void PlayerLeaveZone()
    {
        base.PlayerLeaveZone();
        ActionTimer.Instance.SetIndication(0, 0);
    }

    private void Start()
    {
        if (ConsumableObject != null)
        {
            consumable = GameObject.Instantiate(ConsumableObject, ConsumableTransform);
            consumable.transform.localPosition = Vector3.zero;
            consumable.TryGetComponent(out FlyingFXObject ffo);
            if (ffo != null)
                Destroy(ffo);
            Rotator r = consumable.AddComponent<Rotator>();
            r.rotationDirection = new Vector3(0, 90f, 0);
        }

        SetReadiness(false);
        if (AssociatedObject != null)
            AssociatedObject.ActionZoneMarker = this;
    }
}
