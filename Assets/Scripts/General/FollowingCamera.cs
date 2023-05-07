using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public GameObject CameraTarget;
    public float MovementFactor = 1f;
    public float StopDistance = 0.2f;
    public float TileOpeningDistanceMod = 1.1f;
    public bool useCamConstraintZ = true;

    private float camConstraintZ;
    private Vector3 coordDiff;
    private Transform thisTr;
    private Animator _animator;

    public void AdvanceCamera(float _newConstraint)
    {
        if (camConstraintZ > _newConstraint)
            camConstraintZ = _newConstraint;
    }

    void OnTileUnlocked(string _dummyParam)
    {
        coordDiff = coordDiff * TileOpeningDistanceMod;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _animator.enabled = false;
        thisTr = this.transform;
        coordDiff = thisTr.localPosition - CameraTarget.transform.localPosition;

        EventController.AddListener(EventController.EventTypes.TileUnlocked, OnTileUnlocked);
    }

    void LateUpdate()
    {
        if ((CameraTarget == null) || (MovementFactor == 0)) return;
        Vector3 currentDiff = (CameraTarget.transform.localPosition + coordDiff) - (thisTr.localPosition);
        if (currentDiff.magnitude <= StopDistance) return;

        Vector3 newPos = Vector3.Lerp(thisTr.localPosition, CameraTarget.transform.localPosition + coordDiff, Time.deltaTime * MovementFactor);
        if (newPos.z < camConstraintZ && useCamConstraintZ)
            newPos.z = camConstraintZ;

        thisTr.localPosition = newPos;
    }
}
