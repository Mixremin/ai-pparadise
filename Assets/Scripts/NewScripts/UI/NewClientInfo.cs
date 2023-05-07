using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewClientInfo : MonoBehaviour
{
    public OnScreenUI Hint;
    public Transform Position;
    public ActionZone Zone;

    void Start()
    {
        Hint.SetTarget(Position.transform);
        StartCoroutine(Track());
    }

    IEnumerator Track()
    {
        do
        {
            yield return new WaitForSeconds(0.2f);
        } while (!Zone.ReadyStatus);
        Destroy(Hint.gameObject);
    }
}
