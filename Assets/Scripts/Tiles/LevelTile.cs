using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Tabtale.TTPlugins;

[System.Serializable]
public enum TileSides
{
    Error = -1,
    Left = 0,
    Right = 1,
    Front = 2,
    Back = 3,
    Count = 4
}

[System.Serializable]
public class NeighbouringTile
{
    public TileSides Side;
    public LevelTile Tile;
}

public class LevelTile : MonoBehaviour
{
    public int ClikLevel;
    public NeighbouringTile[] NeighbouringTiles;
    public int OpenCost;
    public float CameraConstraintZ;
    public string SaveGameID;
    public GameObject CollidersContainer;
    public bool UnlockedStatus { get; private set; }

    private GameObject ModelContainer;
    private GameObject ObjectsContainer;
    private GameObject LocksContainer;
    private GameObject OpeningFXPrefab;
    private Transform thisTr;
    private Animator thisAnim;
    private float unlockTimer;

    private const float SHOW_DELAY = 0.7f;
    private const float PRICE_TAG_SCALE = 0.5f;

    private TileSides GetLockSide(GameObject _triggeredObject)
    {
        if ((_triggeredObject == null) ||
            (_triggeredObject.transform.parent == null) ||
            (_triggeredObject.transform.parent.parent == null) ||
            (_triggeredObject.transform.parent.parent.name != this.name)) return TileSides.Error;

        string objName = _triggeredObject.name.ToLowerInvariant();

        for (int z = 0; z < (int)TileSides.Count; z++)
        {
            TileSides tsName = (TileSides)z;
            if (objName.Contains(tsName.ToString().ToLowerInvariant()))
                return tsName;
        }

        return TileSides.Error;
    }

    public void PlayerLeaveTrigger()
    {
        unlockTimer = 0;
        ActionTimer.Instance.SetIndication(0, 0);
    }

    public void AddUnlockTimer (float _delta, GameObject _triggeredObject)
    {
        unlockTimer += _delta;

        if (unlockTimer < GameManager.Instance.TileUnlockDuration)
        {
            ActionTimer.Instance.SetIndication(unlockTimer, GameManager.Instance.TileUnlockDuration);
            return;
        }

        TryUnlock(_triggeredObject);
    }

    private void TryUnlock(GameObject _triggeredObject)
    {
        TileSides side = GetLockSide(_triggeredObject);
        if (side == TileSides.Error)
        {
            Debug.LogWarning("Cannot unlock a tile from " + _triggeredObject.name);
            return;
        }

        // this.name - for price tags
        for (int z = 0; z < NeighbouringTiles.Length; z++)
            if (NeighbouringTiles[z].Side == side)
                if (!NeighbouringTiles[z].Tile.UnlockedStatus)
                {
                    EventController.TriggerEvent(EventController.EventTypes.TileUnlockingStarted, this.name + "/" + NeighbouringTiles[z].Tile.gameObject.name);
                    ActionTimer.Instance.SetIndication(0, 0); 
                    _triggeredObject.SetActive(false);
                }
    }

    IEnumerator UnlockTile()
    {
        //TabtaleEvents.OnTileUnlocked(ClikLevel);

        UnlockedStatus = true;

        yield return new WaitForSeconds(SHOW_DELAY);
        
 //       thisTr.localScale = Vector3.one;
        if (thisAnim != null)
            thisAnim.SetTrigger("Unlock");

        GameObject.FindObjectOfType<FollowingCamera>().AdvanceCamera(CameraConstraintZ);
        //GlobalVars.Instance.SetVar(GlobalVarOnUnlock, 1);

        EventController.TriggerEvent(EventController.EventTypes.TileUnlocked, this.name);

        if (SaveGameID.Trim() != "")
            PlayerPrefs.SetInt(SaveGameID, 1);

        if (CollidersContainer != null)
            CollidersContainer.tag = "TileModel";
    }

    private void OnTileUnlockingStarted(string _param)
    {
        string[] paramSpl = _param.Split('/');
        if (paramSpl.Length != 2) return;
        if (paramSpl[1] != this.name) return;
        if (UnlockedStatus) return;

        PlayerChar.Instance.ChangeMoney(-OpenCost);
        StartCoroutine(UnlockTile());
    }

    private LevelTile HaveNeighbouringTile (TileSides _side)
    {
        for (int z = 0; z < NeighbouringTiles.Length; z++)
            if (NeighbouringTiles[z].Side == _side)
                return NeighbouringTiles[z].Tile;

        return null;
    }

    private void SetTilePriceTag (GameObject _tagObject, TileSides _side)
    {
        LevelTile lt = HaveNeighbouringTile(_side);
        _tagObject.SetActive(lt != null);

        if (lt == null) return;

        PriceTag pt = _tagObject.GetComponent<PriceTag>();
        if (pt == null) return;

        pt.Activate(lt.OpenCost, PriceTagAssociation.Tile, lt.gameObject.name, PRICE_TAG_SCALE);
    }

    private void OrganisePriceTags()
    {
        for (int z = 0; z < LocksContainer.transform.childCount; z++)
        {
            string name = LocksContainer.transform.GetChild(z).name.ToLowerInvariant();
            if (name.Contains("left"))
                SetTilePriceTag(LocksContainer.transform.GetChild(z).GetChild(0).gameObject, TileSides.Left);
            else if (name.Contains("right"))
                SetTilePriceTag(LocksContainer.transform.GetChild(z).GetChild(0).gameObject, TileSides.Right);
            else if (name.Contains("front"))
                SetTilePriceTag(LocksContainer.transform.GetChild(z).GetChild(0).gameObject, TileSides.Front);
            else if (name.Contains("back"))
                SetTilePriceTag(LocksContainer.transform.GetChild(z).GetChild(0).gameObject, TileSides.Back);
        }
    }

    void Start()
    {
        thisTr = this.transform;
        UnlockedStatus = false;
        unlockTimer = 0;

        for (int z = 0; z < thisTr.childCount; z++)
        {
            if (thisTr.GetChild(z).name.Contains("Model"))
                ModelContainer = thisTr.GetChild(z).gameObject;
            else if (thisTr.GetChild(z).name.Contains("Objects"))
                ObjectsContainer = thisTr.GetChild(z).gameObject;
            else if (thisTr.GetChild(z).name.Contains("Locks"))
                LocksContainer = thisTr.GetChild(z).gameObject;
        }

        thisAnim = this.GetComponent<Animator>();

        // initial hiding if not pre-opened
        if (OpenCost > 0)
//            thisTr.localScale = Vector3.zero;
            thisTr.localScale = Vector3.one;
        else
                    GameObject.FindObjectOfType<FollowingCamera>().AdvanceCamera(CameraConstraintZ);        

        if ((SaveGameID.Trim() != "") && PlayerPrefs.HasKey(SaveGameID) && (PlayerPrefs.GetInt(SaveGameID) > 0))
            StartCoroutine(UnlockTile());

        OrganisePriceTags();

        // event listener
        EventController.AddListener(EventController.EventTypes.TileUnlockingStarted, OnTileUnlockingStarted);
    }
}
