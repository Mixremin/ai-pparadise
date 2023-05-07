using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleInfoSpot : MonoBehaviour
{
    public OnScreenUI Image;
    public Transform TrackedTransform;
    public float Distance = 2f;
    bool itemIsUsed;

    IEnumerator Track()
    {
        itemIsUsed = false;
        do
        {
            yield return new WaitForSeconds(0.1f);

            if (Vector3.Distance(TrackedTransform.position, PlayerChar.Instance.transform.position) < Distance)
            {
                if (PlayerStacking.Instance.HasDisposableItems())
                {
                    Image.SetTarget(this.transform);
                    Image.transform.localScale = Vector3.one;
                }
            }
            else
            {
                Image.transform.localScale = Vector3.zero;
            }
        } while (true);
    }

    void Start()
    {
        StartCoroutine(Track());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
