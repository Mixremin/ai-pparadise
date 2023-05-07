using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthActionZone : ActionZone
{
    public UsableObject AssociatedWardBed;
    public Transform BabyCreationPoint;
    public FollowingCamera FollowingCamera;
    public Animator Animator;
    private float OverallDuration;

    private IEnumerator BirthTimer()
    {
        float f = 0;

        do
        {
            yield return null;
            f += Time.deltaTime;
            ActionTimer.Instance.SetIndication(f, GameManager.Instance.BirthProcessDuration);
        } while (f < GameManager.Instance.BirthProcessDuration);

        ActionTimer.Instance.SetIndication(1, 1);
        yield return new WaitForSeconds(0.5f);
        ActionTimer.Instance.SetIndication(0, 0);
    }

    private IEnumerator BirthAction()
    {
        if (GameManager.Instance.FirstBorn)
        {
            GameManager.Instance.FirstBorn = false;
            StartCoroutine(CameraZoom());
        }

        NPCMain npc = AssociatedWardBed.UsedByNPC;
        if (npc == null)
        {
            SetReadiness(true);
            yield break;
        }

        npc.thisAnim.SetTrigger("Birth");
        //StartCoroutine(BirthTimer());
        SetReadiness(false);
        GameManager.Instance.UpdateBirthCounter(1);
        yield return new WaitForSeconds(GameManager.Instance.SpawnDelay);

        GameObject template = GameManager.Instance.BabyPrefab;
        if (GameManager.Instance.CreativeMode)
            template = GameManager.Instance.CreativeBabyPrefab;

        List<GameObject> baby = FlyingObjects.Instance.CreateEffect(template, BabyCreationPoint, GameManager.Instance.PlayerFXPoint, 1, false);
        baby[0].GetComponent<NPCBaby>().Init(AssociatedWardBed.UsedByNPC);
        StackableObject so = baby[0].GetComponent<StackableObject>();
        so.SetHeight(GameManager.Instance.BabyScale * so.Height);
        baby[0].transform.localScale = Vector3.one * GameManager.Instance.BabyScale;
        yield return new WaitForSeconds(GameManager.Instance.LeaveDelay / 2f);
        AssociatedWardBed.UsedByNPC.thisAnim.SetBool("AfterBirth", true);
        AssociatedWardBed.UsedByNPC.thisAnim.SetBool("Laying", false);
        AssociatedWardBed.UsedByNPC.thisAnim.SetBool("Walking", true);
        yield return new WaitForSeconds(GameManager.Instance.LeaveDelay / 2f);


        baby[0].GetComponent<Animator>().SetBool("CreativeAnim", true);
        baby[0].GetComponent<Animator>().SetBool("Cry", true);


        AssociatedWardBed.FinishObjectUsage(AssociatedWardBed.LeaveWpt.NextPoint);
        AssociatedWardBed.UsedByNPC = null;
        AssociatedWardBed.isOccupied = false;
    }

    IEnumerator CameraZoom()
    {
        float effTime = 0.5f;
        float t = 0;
        if (FollowingCamera != null)
            FollowingCamera.enabled = false;
        do
        {
            t += Time.deltaTime;
            yield return null;
            //Camera.main.fieldOfView = Mathf.Lerp(50, 20, t / effTime);
            if (FollowingCamera != null)
            {
                Animator.enabled = true;
                Animator.SetBool("FirstBorn", true);
            }

        } while (t < effTime);

        yield return new WaitForSeconds(1f);
        t = 0;

        do
        {
            t += Time.deltaTime;
            yield return null;
            //Camera.main.fieldOfView = Mathf.Lerp(20, 50, t / effTime);
        } while (t < effTime);
        if (FollowingCamera != null)
        {
            FollowingCamera.enabled = true;
            Animator.SetBool("FirstBorn", false);
        }
        Camera.main.fieldOfView = 50f;
    }

    public override void PlayerLeaveZone()
    {
        base.PlayerLeaveZone();
        ActionTimer.Instance.SetIndication(0, 0);
    }

    public override void PlayerStayInZone(float _delta, float _timerModifier, bool _indication)
    {
        base.PlayerStayInZone(_delta, _timerModifier, false);
        //        ActionTimer.Instance.SetIndication(Timer2 + Timer3, OverallDuration);
    }

    protected override void SuccessfullyFinish()
    {
        base.SuccessfullyFinish();
        StartCoroutine(BirthAction());
    }

    void Start()
    {
        SetReadiness(false);
        AssociatedWardBed.ActionZoneMarker = this;
        OverallDuration = 0;
    }
}
