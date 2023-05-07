using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObjects : MonoBehaviour
{
    static FlyingObjects foInstance;
    public GameObject DefaultObject;

    const float EFFECT_DURATION = 0.5f;

    // singleton behaviour
    public static FlyingObjects Instance
    {
        get
        {
            if (!foInstance)
            {
                foInstance = FindObjectOfType(typeof(FlyingObjects)) as FlyingObjects;

                if (foInstance == null)
                    Debug.LogError("No active FlyingObjects script found in scene.");
            }

            return foInstance;
        }
    }

    public List<GameObject> CreateEffect (GameObject _fromPrefab, Transform _origin, Transform _target, int _count, bool _destroyOnHit)
    {
        if (_count == 0) return new List<GameObject>();

        List<GameObject> result = new List<GameObject>();

        int fxCount = 1 + Mathf.FloorToInt(_count / 20f);

        for (int z = 0; z < fxCount; z++)
        {
            GameObject template = _fromPrefab;
            if (template == null)
                template = DefaultObject;
            GameObject newObj = Instantiate(template);
            FlyingFXObject fxo = newObj.GetComponent<FlyingFXObject>();
            if (fxo == null)
            {
                Destroy(newObj);
                Debug.LogError("No FlyingFXObject script found on " + _fromPrefab.name);
                return result;
            }
            newObj.transform.position = _origin.position;
            fxo.DestroyOnHit = _destroyOnHit;
            result.Add(newObj);

            float delayTime = 0;
            if (EFFECT_DURATION > 0)
                delayTime = (z/ fxCount - 1) * EFFECT_DURATION;

            fxo.Init(delayTime, _target);
        }

        return result;
    }
}
