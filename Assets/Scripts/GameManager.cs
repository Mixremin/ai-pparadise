using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using Tabtale.TTPlugins;

[System.Serializable]
public struct ActionParameters
{
    public ActionZoneType ZoneType;
    public float BaseDuration;
    public float BaseCooldown;
    public int Reward;
}

public class GameManager : MonoBehaviour
{
    static GameManager gmInstance;

    [Header("General")] public Canvas UICanvas;
    public float ObjectUnlockDuration = 2.5f;
    public float TileUnlockDuration = 3.5f;
    public float NPCTeleportDuration = 0.25f;
    public float PlayerMovementSpeed = 5f;
    public float TriggerActivationDelay = 0.3f;
    public NPCWaypoint LastWptSequenceStart;
    public float LastWptSpeedMod = 1.5f;
    public float NPCScale = 1f;
    public float NPCWaypointReachDistance = 0.1f;
    public BabyCarePanel BabyCarePanelTemplate;

    [Header("Stacking")] public int MaxStackSize = 8;
    public float StackConsumeDelay = 0.5f;
    public Color NormalStackTextColor;
    public Color FullStackTextColor;

    [Header("Babies")] public GameObject BabyPrefab;
    public Gradient ClothColours;
    public float ShittingTimerMin = 3f;
    public float ShittingTimerMax = 5f;

    [Header("Controls")] public float SwipeThreshold = 0.04f;
    public float SwipeFullSpeed = 0.2f;

    public ActionParameters[] ActionParams;

    [Header("Debug")] public bool CreativeMode = false;
    public bool CreativeNormalMode = false;
    public bool CreativeProMode = false;
    public float BabyScale = 1f;
    public GameObject CreativeBabyPrefab;
    public int StartingPlayerMoney = 0;
    public int StartingPlayerLevel = 1;
    public bool StraightToTheDeliveryRoom = false;
    public float NPCSpeedMod = 1f;
    public bool FirstBorn = false;

    [Header("NPC Spawning")] public Transform NPCStartingPoint;
    public float NPCCreationDelay = 10f;
    public int OneTileNPCAmount = 6;
    public Color NormalNPCTextColor;
    public Color FullNPCTextColor;
    public Text NPCCounterText;

    [Header("Birth")] public float SpawnDelay = 1f;
    public float LeaveDelay = 1f;
    public float BirthProcessDuration = 0.8f;
    public Text BirthCounter;

    [Header("NPC Movements")] public float NPCMovementSpeed = 2f;
    public float NPCRotationSpeed = 360f;

    [Header("Effects")] public GameObject FlyingMoneyPrefab;
    public Transform PlayerFXPoint;

    [Header("Old Shit")] public int MaxReceptionQueue = 5;
    public float ThrowToActivityDuration = 0.5f;
    public float ThrowToClosedTileDuration = 1.5f;
    public GameObject BabyModel;

    public UpgradesData UpgradesData;
    public UpgradesLevels UpgradesLevels;
    public StaffManager StaffManager;

    public NPCWaypoint[] NPCWaypoints { get; private set; }
    public NPCQueue[] NPCQueues { get; private set; }
    public UsableObjectGroup[] UsableObjectGroups { get; private set; }

    [HideInInspector] public int npcActive;
    [HideInInspector] public int BabiesBorn;
    private int tilesOpened;
    private int npcCounter;
    private int npcLimit;

    // singleton behaviour
    public static GameManager Instance
    {
        get
        {
            if (!gmInstance)
            {
                gmInstance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (gmInstance == null)
                    Debug.LogError("No active GameManager script found in scene.");
            }

            return gmInstance;
        }
    }

    private void Awake()
    {
        //TTPCore.Setup();
        //TabtaleEvents.OnTileUnlocked(1);

        if (!PlayerPrefs.HasKey("Money"))
            PlayerChar.Instance.ChangeMoney(GameManager.Instance.StartingPlayerMoney);
        else
        {
            int x = PlayerPrefs.GetInt(key: "Money");
            if (x < 1300)
            {
                PlayerChar.Instance.ChangeMoney(_delta: 1300);
                PlayerPrefs.SetInt("Money", 1300);
            }
            else
            {
                PlayerChar.Instance.ChangeMoney(x);
            }
        }
        //        if (GameManager.Instance.CreativeProMode)
        //            PlayerChar.Instance.ChangeMoney(Random.Range(20000, 30000));
    }

    public ActionParameters GetActionParameters(ActionZoneType _type)
    {
        for (int z = 0; z < ActionParams.Length; z++)
            if (ActionParams[z].ZoneType == _type)
                return ActionParams[z];

        return ActionParams[0];
    }

    public void UpdateActiveNPCCount()
    {
        NPCCounterText.text = npcActive.ToString() + "/" + npcLimit.ToString();
        if (npcActive < npcLimit)
            NPCCounterText.color = NormalNPCTextColor;
        else
            NPCCounterText.color = FullNPCTextColor;
    }

    private void SpawnNPC()
    {
        npcCounter++;
        npcActive++;
        GameObject newNPC = NPCGenerator.Instance.CreateNPC("NPC" + npcCounter.ToString(), NPCType.FemalePregnant);
        EventController.TriggerEvent(EventController.EventTypes.OpenReceptionDoors, "");
        newNPC.transform.SetPositionAndRotation(NPCStartingPoint.position, NPCStartingPoint.rotation);
        UpdateActiveNPCCount();
        newNPC.transform.localScale = Vector3.one * NPCScale;
    }

    public void RemoveNPC(GameObject _npc)
    {
        npcActive--;
        UpdateActiveNPCCount();
        Destroy(_npc);
    }

    IEnumerator NPCCreation()
    {
        npcCounter = 0;
        float timer;

        yield return new WaitForSeconds(1f);

        NPCQueue recQ = GetQueueByType(NPCQueuesType.Reception);

        do
        {
            if (recQ.CanEnqueue() && (npcActive < npcLimit))
            {
                SpawnNPC();
                timer = Instance.NPCCreationDelay;
            }
            else
                timer = 1f;

            yield return new WaitForSeconds(timer);
        } while (true);
    }

    public GameObject GetNPCWaypointByName(string _wptName)
    {
        for (int z = 0; z < NPCWaypoints.Length; z++)
            if (NPCWaypoints[z].name.ToLowerInvariant() == _wptName.ToLowerInvariant())
                return NPCWaypoints[z].gameObject;

        return null;
    }

    public NPCQueue GetQueueByType(NPCQueuesType _qType)
    {
        for (int z = 0; z < NPCQueues.Length; z++)
            if (NPCQueues[z].Type == _qType)
                return NPCQueues[z];

        return null;
    }

    public UsableObjectGroup GetUsableGroupByType(UsableObjectGroupType _gType)
    {
        for (int z = 0; z < UsableObjectGroups.Length; z++)
            if (UsableObjectGroups[z].Type == _gType)
                return UsableObjectGroups[z];

        return null;
    }

    private void OnTileUnlocked(string _dummyParam)
    {
        tilesOpened++;
        npcLimit = tilesOpened * OneTileNPCAmount;
        UpdateActiveNPCCount();
    }

    public void UpdateBirthCounter(int _delta)
    {
        BabiesBorn += _delta;
        BirthCounter.text = BabiesBorn.ToString();
        PlayerPrefs.SetInt("BabiesBorn", BabiesBorn);
    }

    private void Start()
    {
        npcLimit = OneTileNPCAmount;
        npcActive = 0;
        tilesOpened = 1;
        NPCQueues = FindObjectsOfType<NPCQueue>();
        UsableObjectGroups = FindObjectsOfType<UsableObjectGroup>();
        NPCWaypoints = FindObjectsOfType<NPCWaypoint>();
        BabiesBorn = PlayerPrefs.GetInt("BabiesBorn", 0);
        UpdateBirthCounter(0);

        UpdateActiveNPCCount();
        StartCoroutine(NPCCreation());
        EventController.AddListener(EventController.EventTypes.TileUnlocked, OnTileUnlocked);
    }
}