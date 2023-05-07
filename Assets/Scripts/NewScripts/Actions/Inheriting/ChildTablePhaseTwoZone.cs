using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTablePhaseTwoZone : UsableObjectActionZone
{
    [HideInInspector]
    public NPCBaby baby;
    public NPCWaypoint ApproachStartWpt;

    private BabyCarePanel thisUIPanel;

    public void CreatePanel ()
    {
        GameObject newPanel = Instantiate(GameManager.Instance.BabyCarePanelTemplate.gameObject, GameManager.Instance.UICanvas.transform);
        thisUIPanel = newPanel.GetComponent<BabyCarePanel>();
        thisUIPanel.SetTarget(baby.transform.parent.transform);
        UpdateUIPanel();
    }

    private void UpdateUIPanel()
    {
        for (int z = 0; z < stackablesLeft.Count; z++)
        {
            if (stackablesLeft[z].Type == StackableType.Diaper)
                thisUIPanel.SetDiaperCount(stackablesLeft[z].Quantity);
            if (stackablesLeft[z].Type == StackableType.Bottle)
                thisUIPanel.SetBottleCount(stackablesLeft[z].Quantity);
        }
    }

    public override void PlayerStayInZone(float _delta, float _timerModifier, bool _indication)
    {
        base.PlayerStayInZone(_delta, _timerModifier, false);
        UpdateUIPanel();
    }

    protected override void SuccessfullyFinish()
    {
        SetReadiness(false);
        baby.RecolourCloth();
        baby.SetReadyToReturn();
        baby.GetComponent<Animator>().SetBool("Cry", false);
        Destroy(thisUIPanel.gameObject);
    }
}
