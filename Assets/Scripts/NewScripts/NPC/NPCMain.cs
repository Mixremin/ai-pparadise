using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCPhases
{
    NewlyCreated = 0,
    InReceptionQueue = 1,
    WalkingToReceptionSofa = 2,
    WalkingToDresser = 3
}

public class NPCMain : MonoBehaviour
{
    public NPCPhases CurrentPhase { get; private set; }
    [HideInInspector]
    public NPCMovements thisMovements;
    [HideInInspector]
    public Animator thisAnim;
    [HideInInspector]
    public UsableObjectGroup GroupToPoll;
    public Transform BabyAttachPt;
    public NPCSofa thisSofa;
    private NPCCustomiser thisCustomiser;

    private void SetIntoReceptionQueue()
    {
        NPCQueue rq = GameManager.Instance.GetQueueByType(NPCQueuesType.Reception);
        rq.EnterQueue(this);
    }

    private IEnumerator PollGroup()
    {
        if (GroupToPoll == null)
            yield break;

        do
        {
            if ((thisSofa != null) && (thisSofa.NPCs.Count > 0) && (thisSofa.NPCs[thisSofa.NPCs.Count - 1].name == this.name))
            {

                UsableObject uo = GroupToPoll.GetReadyObject();
                if (uo != null)
                {
                    NPCMain npc = GroupToPoll.GetNewNPCFromSofa(uo);
                    if (npc != null)
                        if (npc.name == this.name)
                            thisAnim.SetBool("Sitting", false);
                    yield break;
                }
            }
            yield return new WaitForSeconds(2f);
        } while (GroupToPoll != null);
    }

    public void StopGroupPolling()
    {
        StopCoroutine(PollGroup());
    }

    public void MoveToBaby(NPCWaypoint _wpt)
    {
        thisMovements.LastSequence = true;
        thisAnim.SetBool("Sitting", false);
        thisAnim.SetBool("Walking", true);
        thisAnim.speed = GameManager.Instance.LastWptSpeedMod;

        this.transform.parent = null;
        thisSofa.RemoveNPCFromList(this.name);

        if (thisSofa != null)
            thisMovements.Teleport(thisSofa.ApproachPoint.transform.position, thisSofa.ApproachPoint.transform.rotation, _wpt);
        else
            thisMovements.StartMovement(_wpt);
    }

    public void InitPhase(NPCPhases _newPhase)
    {
        CurrentPhase = _newPhase;

        Debug.Log (this.name + " : " + _newPhase.ToString());

        // newly created NPCs are going towards entry point
        if (CurrentPhase == NPCPhases.NewlyCreated)
            thisMovements.StartMovement("NPC Entry Point");

        // setting into the reception queue
        if (CurrentPhase == NPCPhases.InReceptionQueue)
            SetIntoReceptionQueue();

        // moving to the dresser (currently empty)
        if (CurrentPhase == NPCPhases.WalkingToDresser)
        {

        }
    }

    public void ReachedWaypoint (NPCWaypoint _wptReached)
    {
        // reached the entry point, entering queue
        if (_wptReached.name == "NPC Entry Point")
        {
            if (!GameManager.Instance.StraightToTheDeliveryRoom)
                InitPhase(NPCPhases.InReceptionQueue);
            else
                thisMovements.StartMovement("WardRoomFirst");
        }

        // reached the first place of reception queue
        if (_wptReached.name == GameManager.Instance.GetQueueByType(NPCQueuesType.Reception).QueuePlaces[0].name)
            EventController.TriggerEvent(EventController.EventTypes.NPCReachedFirstReceptionQueuePlace, "");

        // it's a sequential waypoint
        if (_wptReached.WptType == WaypointType.Sequential)
        {
            if (_wptReached.NextPoint != null)
            {
                Debug.Log(this.name + " reached sequential waypoint " + _wptReached.name + ", proceeding to " + _wptReached.NextPoint.name);
                thisMovements.StartMovement(_wptReached.NextPoint);
            }
            else
            {
                Debug.LogError("Sequential waypoint " + _wptReached.name + " doesn't have a next waypoint in settings.");
            }
        }

        // sofa placing waypoint
        if (_wptReached.WptType == WaypointType.ReachSofa)
        {
            thisAnim.SetBool("Sitting", true);
            _wptReached.Sofa.PlaceNPC(this.gameObject);
            GroupToPoll = _wptReached.Sofa.AssociatedGroup;
            StartCoroutine(PollGroup());
        }

        // object usage waypoint
        if (_wptReached.WptType == WaypointType.UseObject)
            UseObject(_wptReached.Object);

        // next tile waypoint
        if (_wptReached.WptType == WaypointType.NextTile) {
            if (_wptReached.Tile.UnlockedStatus)
                thisMovements.StartMovement(_wptReached.NextPointIfOpen);
            else
                thisMovements.StartMovement(_wptReached.NextPointIfClosed);
        }

        // trying the action from the object group
        if (_wptReached.WptType == WaypointType.TryGroupedObject)
        {
            UsableObject obj = _wptReached.ObjectGroup.GetReadyObject();
            if (obj == null)
                thisMovements.StartMovement(_wptReached.FallbackOption);
            else
            {
                thisMovements.StartMovement(obj.ApproachWpt);
                obj.UsedByNPC = this;
            }
        }

        // last waypoint
        if (_wptReached.WptType == WaypointType.LastWaypoint)
            GameManager.Instance.RemoveNPC(this.gameObject);

        // child taking waypoint
        if (_wptReached.WptType == WaypointType.TakeChild)
            StartCoroutine(TakeBaby(_wptReached));
    }

    IEnumerator TakeBaby (NPCWaypoint _reached)
    {
        Vector3 tPos = _reached.AttachPoint.transform.position;
        tPos.y = this.transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation(tPos - this.transform.position, this.transform.up);
        while (this.transform.rotation != targetRotation)
        {
            this.transform.rotation =
                Quaternion.RotateTowards(this.transform.rotation, targetRotation, GameManager.Instance.NPCRotationSpeed * Time.deltaTime);
            yield return null;
        }

        thisAnim.speed = 1f;
        thisAnim.SetBool("Walking", false);
        thisAnim.SetBool("Happy", true);
        yield return new WaitForSeconds(3f);
        thisAnim.SetBool("Happy", false);
        Transform babyTransform = _reached.AttachPoint.transform.GetChild(0);
        yield return new WaitForSeconds(0.45f);

        babyTransform.parent = BabyAttachPt;
        Vector3 startPos = babyTransform.localPosition;
        Quaternion startRt = babyTransform.localRotation;

        float effTime = 0.3f;
        float t = 0;
        do
        {
            yield return null;
            t += Time.deltaTime;

            babyTransform.localPosition = Vector3.Lerp (startPos, Vector3.zero, t / effTime);
            babyTransform.localRotation = Quaternion.Lerp (startRt, Quaternion.identity, t/effTime);
        } while (t < effTime);

        babyTransform.localRotation = Quaternion.identity;
        babyTransform.localPosition = Vector3.zero;

        thisAnim.speed = GameManager.Instance.LastWptSpeedMod;
        thisMovements.StartMovement(GameManager.Instance.LastWptSequenceStart);
        _reached.ActionZone.ResetChildTableZone(1.5f);
    }

    public void StartMovement(NPCWaypoint _targetWpt)
    {
        thisMovements.StartMovement(_targetWpt);
    }
    public void StartMovement (string _targetWptName)
    {
        thisMovements.StartMovement(_targetWptName);
    }

    public void MoveToUsableObject(UsableObject _obj)
    {
        thisMovements.StartMovement(_obj.ApproachWpt);
    }

    private IEnumerator PollObjectUsage (UsableObject _obj)
    {
        yield return new WaitForSeconds(0.5f);

        do
        {
            yield return new WaitForSeconds(0.5f);
        } while (_obj.isOccupied);

        UseObject(_obj);
    }

    private void UseObject (UsableObject _obj)
    {
        if (_obj == null) return;
        if (!_obj.GetComponent<UnlockableObject>().ActivationStatus) return;
        // sometimes they're already being used
        if (_obj.isOccupied)
        {
            StartCoroutine(PollObjectUsage(_obj));
            return;
        }

       if (_obj.AnimationVariable.Trim() != "")
            thisAnim.SetBool(_obj.AnimationVariable.Trim(), true);

        thisMovements.StopMovement();
        _obj.NPCEnter(this);
    }

    void Start()
    {
        thisAnim = this.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animator>();
        thisMovements = this.GetComponent<NPCMovements>();
        InitPhase(NPCPhases.NewlyCreated);
        GroupToPoll = null;
        thisCustomiser = this.transform.GetChild(0).GetChild(0).gameObject.GetComponent<NPCCustomiser>();
        BabyAttachPt = thisCustomiser.ChildAttachPt;
    }

    void Update()
    {
    }
}
