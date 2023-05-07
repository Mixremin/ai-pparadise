using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableObject : MonoBehaviour
{
    [HideInInspector]
    public NPCMain UsedByNPC;
    [HideInInspector]
    public bool isOccupied;
    [SerializeField]
    private NPCWaypoint ApproachWaypoint;
    [SerializeField]
    private NPCWaypoint ApproachWaypointFromSofa;
    [SerializeField]
    private NPCWaypoint LeaveWaypoint;
    [SerializeField]
    private Transform NPCPlacementPoint;
    [SerializeField]
    private string NPCAnimationVar;
    [HideInInspector]
    public ActionZone ActionZoneMarker;
    [HideInInspector]
    public UsableObjectGroup PartOfGroup;
    private UnlockableObject thisUo;
    public NPCWaypoint ApproachWpt => ApproachWaypoint;
    public NPCWaypoint ApproachWptSofa => ApproachWaypointFromSofa;
    public NPCWaypoint LeaveWpt => LeaveWaypoint;
    public Transform PlacementPoint => NPCPlacementPoint;
    public string AnimationVariable => NPCAnimationVar;
    private Quaternion npcOrigRotation;

    public void NPCEnter (NPCMain _npc)
    {
        isOccupied = true;
        UsedByNPC = _npc;
        ActionZoneMarker.SetReadiness(true);
        UsedByNPC.thisMovements.StopMovement();
        UsedByNPC.thisAnim.SetBool("Sitting", false);
        UsedByNPC.thisAnim.SetBool("Walking", false);
        UsedByNPC.thisAnim.SetBool(NPCAnimationVar, true);
        npcOrigRotation = UsedByNPC.transform.rotation;

        UsedByNPC.thisMovements.Teleport(PlacementPoint.position, PlacementPoint.rotation);
    }

    public bool IsReady()
    {
        return (IsUnlocked() && (UsedByNPC == null));
    }

    // -------- too lazy to implement inheritance here
    public void FinishObjectUsage (NPCWaypoint _nextPoint)
    {
        UsedByNPC.thisMovements.Teleport(LeaveWpt.transform.position, npcOrigRotation, _nextPoint);

        if (AnimationVariable.Trim() != "")
            UsedByNPC.thisAnim.SetBool(AnimationVariable.Trim(), false);

        if (ActionZoneMarker.actParams.ZoneType == ActionZoneType.Dresser)
            WorkDresser();
    }

    private void WorkDresser ()
    {
        UsedByNPC.transform.GetChild(0).GetChild(0).GetComponent<NPCOutfitStates>().SetState(1);
    }
    // -----------------------------------------------

    private bool IsUnlocked()
    {
        if (thisUo == null)
            return true;
        else
            return thisUo.ActivationStatus;
    }

    private void Start()
    {
        UsedByNPC = null;
        isOccupied = false;
        thisUo = GetComponent<UnlockableObject>();
        if (ActionZoneMarker != null) ActionZoneMarker.SetReadiness(false);
    }
}
