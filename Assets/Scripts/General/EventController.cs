using System;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    // event types & descriptions
    public enum EventTypes
    {
        AppStarted = 0,
        TileUnlockingStarted = 1,
        TileUnlocked = 4,
        PlayerLevelUp = 7,
        OpenReceptionDoors = 8,
        GlobalVarsUpdated = 9,
        ProgressQueue = 10,
        ObjectWasUnlocked = 11,
        UpgradeClientsSpeed = 12,
        UpgradeMoneyFromClients = 13,
        UpgradeVendingCapacity = 14,
        UpgradeVendingSpeed = 15,
        UpgradeCharacterCapacity = 16,
        UpgradeCharacterSpeed = 17,
        UpgradeAssistantCapacity = 18,
        UpgradeAssistantSpeed = 19,
        HireAssistant = 20,
        NPCLandedAtActionPoint = 21,
        UpdateActionPoints = 22,
        OpenUpgradeStaffUIWindow = 23,
        SetMovementLock = 24,
        OpenUpgradeVendingUIWindow = 25,
        OpenUpgradeClientUIWindow = 26,
        UIOpened = 27,
        UIClosed = 28,

        NPCReachedFirstReceptionQueuePlace = 101,
        ReceptionActionFinished = 102,
        StackingQuantityChanged = 103,
        BabiesInStackAmountChanged = 104
    }

    Dictionary<EventTypes, Action<string>> eventList;
    static EventController eventCtrl;

    // controller must be in a single instance
    public static EventController Instance
    {
        get
        {
            if (!eventCtrl)
            {
                eventCtrl = FindObjectOfType(typeof(EventController)) as EventController;

                if (eventCtrl)
                    eventCtrl.Init();
                else
                    Debug.LogError("No active EventController script found in scene.");
            }

            return eventCtrl;
        }
    }

    // event list initialising
    void Init()
    {
        if (eventList == null)
            eventList = new Dictionary<EventTypes, Action<string>>();
    }

    // adding an event listener
    public static void AddListener(EventTypes _eventType, Action<string> _listener)
    {
        Action<string> thisEvent = null;
        // add to present listeners of this event
        if (Instance.eventList.TryGetValue(_eventType, out thisEvent))
        {
            thisEvent += _listener;
            Instance.eventList[_eventType] = thisEvent;
        }
        // or register a new record for this event
        else
        {
            thisEvent += _listener;
            Instance.eventList.Add(_eventType, thisEvent);
        }
    }

    // deleting a listener
    public static void RemoveListener(EventTypes _eventType, Action<string> _listener)
    {
        // error-proof
        if (eventCtrl == null) return;

        Action<string> thisEvent;
        if (Instance.eventList.TryGetValue(_eventType, out thisEvent))
        {
            thisEvent -= _listener;
            Instance.eventList[_eventType] = thisEvent;
        }
    }

    // triggering an event
    public static void TriggerEvent(EventTypes _eventType, string _runParameter)
    {
//        if (_eventType != EventTypes.SetActionTimer)
//            Debug.Log ("Event: " + _eventType.ToString() + " :: " + _runParameter);

        if (Instance.eventList.TryGetValue(_eventType, out Action<string> thisEvent))
            thisEvent.Invoke(_runParameter);
    }
}