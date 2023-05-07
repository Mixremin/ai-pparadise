using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PriceTagAssociation
{
    Tile = 0,
    Object = 1
}

public class PriceTag : MonoBehaviour
{
    public GameObject OpeningFXPrefab;
    public int Price { get; private set; }

    [SerializeField] private string SaveGameID;
    private Transform thisTr;
    private Text priceText;
    [HideInInspector]
    public PriceTagAssociation association;
    [HideInInspector]
    public string associatedObjectName;
    private bool savegameLocked;

    private const float DRAWDOWN_TIME = 0.5f;
    private const float DRAWDOWN_PAUSE = 0.1f;

    IEnumerator Drawdown()
    {
        FlyingObjects.Instance.CreateEffect(OpeningFXPrefab, GameObject.FindGameObjectWithTag("PlayerFX").transform, this.transform, Price, true);

        float t = DRAWDOWN_TIME;
        float price2;

        do
        {
            price2 = Price * Mathf.Pow((t / DRAWDOWN_TIME), 1.5f);
            priceText.text = Mathf.FloorToInt(price2).ToString();
            t -= Time.deltaTime;

            yield return null;
        } while (t > 0);

        priceText.text = "0";
        yield return new WaitForSeconds(DRAWDOWN_PAUSE);

        if (SaveGameID != "")
        {
            PlayerPrefs.SetInt(SaveGameID, 1);
            savegameLocked = true;
        }

        thisTr.localScale = Vector3.zero;
    }

    private void OnTileUnlockingStarted(string _param)
    {
        if (association != PriceTagAssociation.Tile)
            return;

        string[] paramSpl = _param.Split('/');
        if (paramSpl.Length != 2) return;

        // have nothing to do with associated object
        if (paramSpl[1] != associatedObjectName)
            return;

        // this tile
        if (paramSpl[0] == thisTr.parent.parent.parent.name)
            StartCoroutine(Drawdown());
        else
            thisTr.localScale = Vector3.zero;
    }

    public void Close ()
    {
        StartCoroutine(Drawdown());
    }

    public void Activate(int _price, PriceTagAssociation _association, string _objectname, float _scale)
    {
        if (savegameLocked)
        {
            Price = 0;
            return;
        }

        thisTr.localScale = Vector3.one * _scale;
        priceText.text = _price.ToString();
        association = _association;
        associatedObjectName = _objectname;
        Price = _price;
    }    

    void Awake()
    {
        thisTr = this.transform;

        savegameLocked = ((SaveGameID != "") && (PlayerPrefs.GetInt(SaveGameID) > 0));

        thisTr.transform.localScale = Vector3.zero;
        priceText = thisTr.GetChild(0).GetChild(1).GetComponent<Text>();
        EventController.AddListener(EventController.EventTypes.TileUnlockingStarted, OnTileUnlockingStarted);
    }
}
