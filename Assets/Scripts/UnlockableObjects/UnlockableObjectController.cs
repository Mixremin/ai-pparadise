using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableObjectController : MonoBehaviour
{
    static UnlockableObjectController uocInstance;
    public Material DuplicateObjectMaterial;
    public AnimationClip AnimationClip;
    public AnimationClip AnimationClipAdd;
    public GameObject PriceTagPrefab;

    // singleton behaviour
    public static UnlockableObjectController Instance
    {
        get
        {
            if (!uocInstance)
            {
                uocInstance = FindObjectOfType(typeof(UnlockableObjectController)) as UnlockableObjectController;

                if (uocInstance == null)
                    Debug.LogError("No active UnlockableObjectController script found in scene.");
            }

            return uocInstance;
        }
    }
}
