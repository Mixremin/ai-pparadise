using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTimer : MonoBehaviour
{
    static ActionTimer thisInstance;

    private RectTransform thisTr;
    private Transform targetTr;
    private Camera usedCamera;
    private Image progressImage;
    private Text progressText;

    // singleton behaviour
    public static ActionTimer Instance
    {
        get
        {
            if (!thisInstance)
            {
                thisInstance = FindObjectOfType(typeof(ActionTimer)) as ActionTimer;

                if (!thisInstance)
                    Debug.LogError("No active ActionTimer script found in scene.");
            }

            return thisInstance;
        }
    }

    public bool ActiveStatus()
    {
        return (thisTr.localScale.x > 0f);
    }

    public void SetIndication (float _completed, float _overall)
    {
        if (thisTr == null)
            thisTr = this.GetComponent<RectTransform>();

        if (_overall <= 0)
        {
            thisTr.localScale = Vector3.zero;
            return;
        }

        thisTr.localScale = Vector3.one;
        progressImage.fillAmount = Mathf.Clamp01(_completed / _overall);
        progressText.text = Mathf.FloorToInt(progressImage.fillAmount * 100f).ToString() + "%";
    }

    void Start()
    {
        thisTr = this.GetComponent<RectTransform>();
        thisTr.localScale = Vector3.zero;
        progressImage = thisTr.GetChild(1).GetComponent<Image>();
        progressText = thisTr.GetChild(2).GetComponent<Text>();

        targetTr = GameObject.Find("PlayerTimerAttach").transform;
        usedCamera = GameObject.FindObjectOfType<Camera>().GetComponent<Camera>();
    }

    void LateUpdate()
    {
        Vector3 pos = usedCamera.WorldToScreenPoint(targetTr.position);
        thisTr.SetPositionAndRotation(pos, Quaternion.identity);
    }
}
