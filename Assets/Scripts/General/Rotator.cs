using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationDirection;

    private void Update()
    {
        if (transform.localScale.x > 0)
            transform.localEulerAngles += rotationDirection * Time.deltaTime;
    }
}