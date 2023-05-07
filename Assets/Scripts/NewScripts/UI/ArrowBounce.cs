using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBounce : MonoBehaviour
{
    public float Amplitude = 20f;
    public float Speed = 15f;
    private Vector3 origCoords;
    private float Timer;
    private RectTransform thisRt;

    // Start is called before the first frame update
    void Start()
    {
        Timer = 0;
        thisRt = this.GetComponent<RectTransform>();
        origCoords = thisRt.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime * Speed;

        float y = Mathf.Sin(Timer) * Amplitude;
        Vector3 pos = origCoords;
        pos.y += y;
        thisRt.anchoredPosition = pos;
    }
}
