using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildReturnActionZone : ActionZone
{
/*   
    protected override void SuccessfullyFinish()
    {
        base.SuccessfullyFinish();
        ActionTimer.Instance.SetIndication(0, 0);

        int index;

        do
        {
            index = -1;

            for (int z = 0; z < PlayerStacking.Instance.Stack.Count; z++)
            {
                PlayerStacking.Instance.Stack[z].TryGetComponent(out NPCBaby b);
                if ((b != null) && (b.ReadyToReturn))
                {
                    index = z;
                    break;
                }
            }

            if (index > -1)
            {
                GameObject baby = PlayerStacking.Instance.Stack[index];
                PlayerStacking.Instance.Stack.RemoveAt(index);
                PlayerStacking.Instance.RecalculateStackAfterRemoving();

                NPCMain mNpc = baby.GetComponent<NPCBaby>().Mother.GetComponent<NPCMain>();
                baby.transform.SetParent(mNpc.BabyAttachPt);
                baby.transform.localRotation = Quaternion.identity;
                baby.transform.localPosition = Vector3.zero;

                mNpc.transform.SetParent(null);
                mNpc.thisAnim.SetBool("Sitting", false);
                mNpc.thisAnim.SetBool("CarryingBaby", true);
                mNpc.thisMovements.Teleport(GameManager.Instance.LastWptTeleportPoint.position, GameManager.Instance.LastWptTeleportPoint.rotation,
                    GameManager.Instance.LastWptSequenceStart.GetComponent<NPCWaypoint>());
            }
        } while (index == -1);

        EventController.TriggerEvent(EventController.EventTypes.BabiesInStackAmountChanged, "");
        StartCoroutine(CheckCooldown());
    }

    IEnumerator CheckCooldown()
    {
        SetReadiness(false);
        yield return new WaitForSeconds(actParams.BaseCooldown);
        OnBabiesInStackAmountChanged("");
    }

    void OnBabiesInStackAmountChanged(string _dummyParam)
    {
        SetReadiness(PlayerStacking.Instance.HaveBabiesInStack(true));
    }

    void Start()
    {
        SetReadiness(false);
        EventController.AddListener(EventController.EventTypes.BabiesInStackAmountChanged, OnBabiesInStackAmountChanged);
    }*/
}
