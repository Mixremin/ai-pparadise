using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScreenUI : MonoBehaviour
{
    private RectTransform thisTr;
    private Transform targetTr;

    public void SetTarget(Transform _target)
    {
        targetTr = _target;
        SetPosition();
    }

    public void SetPosition ()
    {
        if (thisTr == null)
            thisTr = this.GetComponent<RectTransform>();

        if (targetTr != null)
            thisTr.SetPositionAndRotation(Camera.main.WorldToScreenPoint(targetTr.position), Quaternion.identity);
    }

    private void LateUpdate()
    {
        SetPosition();
    }
}
