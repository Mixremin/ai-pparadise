using UnityEngine;

public class FlyingFXObject : MonoBehaviour
{
    public float JumpDuration;
    public float InitialSpeed;
    public float Acceleration;
    public Vector3 rotationVector;
    public bool DestroyOnHit = true;
    [HideInInspector]
    public GameObject Origin;
    private Quaternion originalRotation;
    public float RotationSpeedMax = 260f;
    public float DirectionRandomFactor = 18f;

    Transform thisTr;
    Transform objectTr;
    Transform target;
    Quaternion randomDirection;

    float rotation;
    float speed;
    float timer;
    bool acting;
    bool rotating;

    public void SetRotationActivity (bool _act)
    {
        rotating = _act;
    }

    public void SetFlyingActivity (bool _act)
    {
        acting = _act;
    }

    public void ResetRotations()
    {
        objectTr.localRotation = originalRotation;
        this.transform.localRotation = Quaternion.identity;
    }

    public void Init(float _delay, Transform _target)
    {
        timer = _delay;
        target = _target;
    }

    void Start()
    {
        thisTr = this.transform;
        objectTr = thisTr.GetChild(0);
        thisTr.localScale = Vector3.zero;
        originalRotation = objectTr.localRotation;
        timer = 0;
        acting = false;
        rotating = true;
        speed = InitialSpeed;

        randomDirection = Quaternion.Euler (Random.Range(-DirectionRandomFactor, DirectionRandomFactor),
            Random.Range(-DirectionRandomFactor, DirectionRandomFactor),
            Random.Range(-DirectionRandomFactor, DirectionRandomFactor));

        rotation = Random.Range(-RotationSpeedMax, RotationSpeedMax);
    }

    void LateUpdate()
    {
        if (timer > 0)
            timer -= Time.deltaTime;

        if (!acting && (timer <= 0))
        {
            thisTr.localScale = Vector3.one;

            this.TryGetComponent(out NPCBaby b);
            if (b != null)
                thisTr.localScale = GameManager.Instance.BabyScale * Vector3.one;

            acting = true;
            timer = JumpDuration;
        }

        if (acting)
        {
            if (rotating)
            {
                Vector3 rt = rotationVector * rotation * Time.deltaTime;
                objectTr.Rotate(rt);
            }

            if (timer > 0)
                timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;

            float moveDistance = Time.deltaTime * speed;
            if (Vector3.Distance(thisTr.position, target.position) < moveDistance)
            {
                if (DestroyOnHit)
                    Destroy(this.gameObject);
                else
                    PlayerChar.Instance.HitByFlyingObject(this.gameObject);
            }
            else
            {
                Vector3 targetVector = (target.position - thisTr.position).normalized;

                float lerpFactor = 0;
                if (JumpDuration > 0)
                    lerpFactor = timer / JumpDuration;

                Vector3 movement = Vector3.Lerp(targetVector, randomDirection * Vector3.up, lerpFactor);
                thisTr.localPosition += movement * moveDistance;
                speed += Acceleration * Time.deltaTime;
            }
        }
    }
}
