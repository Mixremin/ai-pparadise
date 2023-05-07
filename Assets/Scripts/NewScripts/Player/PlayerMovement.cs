using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool MovingStatus { get; private set; }
    public bool LockedStatus { get; private set; }

    private Transform thisTr;
    private float swipeThrPixels;
    private float swipeMaxPixels;
    private Vector3 tapPosition;
    private bool tappedStatus;
    private bool withStack;

    private const float CHECK_SECTOR = 30f;
    private const float FORWARD_CHECK = 0.1f;
    private const float RAYCAST_HEIGHT = 10f;
    private const float ACTION_ALIGNING_TIME = 0.5f;
    private Animator anim;

    public void SetMovementWithStack (bool _stack)
    {
        withStack = _stack;

        Animator anim = GetComponent<PlayerLevels>().CurrentModel.GetComponent<Animator>();
        anim.SetBool("Stacking", _stack);
    }

    // raycast-check for incoming collisions and moving out of the location
    private bool CheckMovementAvailability ()
    {
        for (int z = -2; z < 3; z++)
        {
            Vector3 rayOrigin = new Vector3(thisTr.position.x, thisTr.position.y + RAYCAST_HEIGHT, thisTr.position.z);
            Vector3 additional = Quaternion.Euler(new Vector3(0, (CHECK_SECTOR * z)/2, 0)) * thisTr.forward;
            additional = additional.normalized * FORWARD_CHECK * GameManager.Instance.PlayerMovementSpeed;

            if (Physics.Raycast(rayOrigin + additional, Vector3.down, out RaycastHit checkRay))
            {
                if ((!checkRay.collider.isTrigger) && (checkRay.collider.gameObject.tag != "TileModel"))
                    return false;
            }
            else
                return false;
        }

        return true;
    }

    // main movement method
    private void MoveForward(float spd)
    {
        if (!CheckMovementAvailability()) return;

        Vector3 delta = thisTr.forward * (GameManager.Instance.PlayerMovementSpeed * spd) * Time.deltaTime;
        float yPos = thisTr.localPosition.y;
        thisTr.localPosition = thisTr.localPosition + delta;
        thisTr.localPosition = new Vector3(thisTr.localPosition.x, yPos, thisTr.localPosition.z);
    }

    // starting a movement, calculating rotation angles
    void StartMovement()
    {
        if (anim == null)
            anim = GetComponent<PlayerLevels>().CurrentModel.GetComponent<Animator>();
        anim.speed = 1;
        if (LockedStatus) return;
        float magn = (Input.mousePosition - tapPosition).magnitude;
        if ((magn < swipeThrPixels) && (!MovingStatus)) return;

        // interrupt only trigger timer, not the already activated zones
        if (!ActionTimer.Instance.ActiveStatus())
            PlayerChar.Instance.TriggerDelay = 0;
        GetComponent<PlayerLevels>().CurrentModel.GetComponent<Animator>().SetBool("Walking", true);

        MovingStatus = true;
        Vector3 mpFix = new Vector3(Input.mousePosition.x, Input.mousePosition.z, Input.mousePosition.y) -
            new Vector3(tapPosition.x, tapPosition.z, tapPosition.y);

        Quaternion qt = Quaternion.LookRotation(mpFix, Vector3.up);
        Vector3 qtV = qt.eulerAngles;
        qtV.z = 0;
        qtV.x = 0;
        thisTr.localRotation = Quaternion.Euler(qtV);
        float spd = Mathf.Clamp01((magn - swipeThrPixels) / ((swipeMaxPixels - swipeThrPixels)));

        anim.speed = spd;
        MoveForward(spd);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (!LockedStatus))
        {
            tappedStatus = true;
            MovingStatus = false;
            tapPosition = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            tappedStatus = false;
            MovingStatus = false;
            GetComponent<PlayerLevels>().CurrentModel.GetComponent<Animator>().SetBool("Walking", false);
        }

        if (Input.GetMouseButton(0) && tappedStatus && (!LockedStatus))
            StartMovement();
    }

    void OnSetMovementLock (string _param)
    {
        LockedStatus = (_param != "0");
    }

    void Start()
    {
        float x = Screen.width * GameManager.Instance.SwipeThreshold;
        float y = Screen.height * GameManager.Instance.SwipeThreshold;
        float maxX = Screen.width * GameManager.Instance.SwipeFullSpeed;
        float maxY = Screen.height * GameManager.Instance.SwipeFullSpeed;

        if (x > y)
        {
            swipeThrPixels = y;
            swipeMaxPixels = maxY;
        }
        else
        {
            swipeThrPixels = x;
            swipeMaxPixels = maxX;
        }

        thisTr = this.transform;
        tappedStatus = false;
        LockedStatus = false;

        EventController.AddListener(EventController.EventTypes.SetMovementLock, OnSetMovementLock);
    }
}
