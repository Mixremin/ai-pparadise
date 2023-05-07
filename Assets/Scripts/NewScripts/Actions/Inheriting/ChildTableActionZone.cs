using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTableActionZone : ActionZone
{
    public Transform AttachPoint;
    public UnlockableObject UO;
    public ChildTablePhaseTwoZone PhaseTwoZone;

    public void ReadyPhaseTwo()
    {
        PhaseTwoZone.baby = AttachPoint.transform.GetChild(0).GetComponent<NPCBaby>();
        PhaseTwoZone.SetReadiness(true);
        PhaseTwoZone.CreatePanel();
    }

    IEnumerator MoveBabyOntoTheTable(GameObject _babyObj)
    {
        Quaternion startRt = _babyObj.transform.rotation;
        Vector3 startPos = _babyObj.transform.position;
        _babyObj.transform.SetParent(AttachPoint);
        float effTime = 0.2f;
        float t = 0;

        do
        {
            yield return null;
            t += Time.deltaTime;
            _babyObj.transform.SetPositionAndRotation(Vector3.Lerp(startPos, AttachPoint.transform.position, t / effTime),
                Quaternion.Lerp(startRt, AttachPoint.transform.rotation, t / effTime));
        } while (t < effTime);

        _babyObj.transform.localPosition = Vector3.zero;
        _babyObj.transform.localRotation = Quaternion.identity;
        NPCBaby b = _babyObj.GetComponent<NPCBaby>();
        b.InActionZone = this;
        b.StartShittingTimer();
    }

    public override void PlayerStayInZone(float _delta, float _timerModifier, bool _indication)
    {
        // throwing a baby onto the table
        if ((UO.ActivationStatus) && (AttachPoint.transform.childCount == 0) && (PlayerStacking.Instance.HaveBabiesInStack(false)))
        {
            GameObject b = PlayerStacking.Instance.GetBabyFromStack();
            if (b == null)
                return;
            StartCoroutine(MoveBabyOntoTheTable(b));
            SetReadiness(false);
            return;
        }

    }

    private void OnBabiesAmountChanged(string _dummyParam)
    {
        bool ready = false;

        // throwing a baby onto the table
        if (UO == null)
            UO = AttachPoint.parent.GetComponent<UnlockableObject>();

        if ((AttachPoint.transform.childCount == 0) && (PlayerStacking.Instance.HaveBabiesInStack(false)) && (UO.ActivationStatus))
            ready = true;

        SetReadiness(ready);
    }

    private void OnObjectUnlocked (string _name)
    {
        if ((UO != null) && (_name == UO.name))
            OnBabiesAmountChanged("");
    }

    private void Start()
    {
        UO = AttachPoint.parent.GetComponent<UnlockableObject>();
        SetReadiness(false);
        PhaseTwoZone.SetReadiness(false);

        EventController.AddListener(EventController.EventTypes.BabiesInStackAmountChanged, OnBabiesAmountChanged);
        EventController.AddListener(EventController.EventTypes.ObjectWasUnlocked, OnObjectUnlocked);
    }
}
