using System.Collections;
using UnityEngine;

public class NPCMovements : MonoBehaviour
{
    private NPCMain thisMainScript;
    private NPCWaypoint targetWpt;
    private Transform thisTr;
    private Animator thisAnim;
    private bool movingStatus;

    // oh, god, what a shit
    private static readonly int Sitting = Animator.StringToHash("Sitting");
    private static readonly int Laying = Animator.StringToHash("Laying");
    private static readonly int HoldingBaby = Animator.StringToHash("HoldingBaby");
    private static readonly int WalkingWithBaby = Animator.StringToHash("WalkingWithBaby");

    public bool LastSequence;

    public void StartMovement (NPCWaypoint _targetWpt)
    {
        targetWpt = _targetWpt;
        StopAllCoroutines();
        StartCoroutine(MoveRoutine());
    }

    public void Teleport(Vector3 _targetPos, Quaternion _targetRt)
    {
        Teleport (_targetPos, _targetRt, null);
    }
    public void Teleport (Vector3 _targetPos, Quaternion _targetRt, NPCWaypoint _nextPoint)
    {
        StopMovement();
        StartCoroutine(TeleportRoutine(_targetPos, _targetRt, _nextPoint));
    }

    public void StartMovement (string _targetWptName)
    {
        targetWpt = GameManager.Instance.GetNPCWaypointByName (_targetWptName).GetComponent<NPCWaypoint>();

        if (targetWpt == null)
        {
            Debug.LogError("Can't move NPC to waypoint " + _targetWptName);
            return;
        }

        StartMovement(targetWpt);
    }

    public void StopMovement()
    {
        CheckAnimator();
        thisAnim.SetBool("Walking", false);
        movingStatus = false;
        StopAllCoroutines();
    }

    private void CheckAnimator()
    {
        if (thisAnim == null)
            thisAnim = this.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animator>();
    }

    private IEnumerator TeleportRoutine (Vector3 _targetPos, Quaternion _targetRt, NPCWaypoint _nextPoint)
    {
        float t = 0;
        Vector3 startPos = thisTr.position;
        Quaternion startRt = thisTr.rotation;

        do
        {
            yield return null;
            t += Time.deltaTime;
            float prg = Mathf.Clamp01(t / GameManager.Instance.NPCTeleportDuration);
            thisTr.position = Vector3.Lerp(startPos, _targetPos, prg);
            thisTr.rotation = Quaternion.Lerp(startRt, _targetRt, prg);
        } while (t < GameManager.Instance.NPCTeleportDuration);

        thisTr.position = _targetPos;
        thisTr.rotation = _targetRt;
        if (_nextPoint != null)
            StartMovement(_nextPoint);
    }

    private IEnumerator MoveRoutine()
    {
        if (thisMainScript == null)
            thisMainScript = this.GetComponent<NPCMain>();

        CheckAnimator();
        StartCoroutine(RotateRoutine(targetWpt.transform));
        movingStatus = true;
        thisAnim.SetBool("Walking", true);

        do
        {
            float spd = GameManager.Instance.NPCMovementSpeed * Time.deltaTime * GameManager.Instance.NPCSpeedMod;
            if (LastSequence)
                spd *= GameManager.Instance.LastWptSpeedMod;

            // keep the Y coordinate
            Vector3 newPos = Vector3.MoveTowards(thisTr.position, targetWpt.transform.position, spd);
            newPos.y = thisTr.position.y;
            thisTr.position = newPos;

            if (Vector3.Distance(thisTr.position, targetWpt.transform.position) < GameManager.Instance.NPCWaypointReachDistance)
                //(thisTr.position == targetWpt.transform.position)
            {
                StopMovement();
                thisMainScript.ReachedWaypoint(targetWpt);
            }

            yield return null;
        } while (movingStatus);
    }
    
    public void RotateTo (Transform _target)
    {
        StartCoroutine(RotateRoutine(_target));
    }

    private IEnumerator RotateRoutine(Transform _target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(_target.position - thisTr.position, thisTr.up);
        while (thisTr.rotation != targetRotation)
        {
            thisTr.rotation =
                Quaternion.RotateTowards(thisTr.rotation, targetRotation, GameManager.Instance.NPCRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void Start()
    {
        LastSequence = false;
        movingStatus = false;
        thisTr = this.transform;
        thisMainScript = this.GetComponent<NPCMain>();
    }


















    public void SetTargetAndStart(string _target)
    {

    }



    public void Sit ()
    {
        thisAnim.SetBool(Sitting, true);
    }

    public void WalkWithBaby()
    {
        thisAnim.SetBool(WalkingWithBaby, true);
    }

    public void StopSit()
    {
        thisAnim.SetBool(Sitting, false);
    }

    public void LayDown()
    {
        thisAnim.SetBool(Laying, true);
    }

    public void StopLaying()
    {
        thisAnim.SetBool(Laying, false);
    }

    public void HoldBaby()
    {
        thisAnim.SetBool(HoldingBaby, true);
    }

    public void StopHoldingBaby()
    {
        thisAnim.SetBool(HoldingBaby, false);
    }


}