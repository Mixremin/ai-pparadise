using System.Collections;
using UnityEngine;

// scales of unlockable duplicates in percent
[System.Serializable]
public enum DuplicateScales
{
    DressingCabin = 50,
    VendingMachine = 65,
    Chair = 62,
    Echo = 80,
    ClinicBed = 70,
    ChildTable = 100,
    ChangingTable = 95
}

public class UnlockableObject : MonoBehaviour
{
    [SerializeField] private string SaveGameID;
    [SerializeField] private int unlockingCost;
    [SerializeField] private DuplicateScales duplicateScale;
    [SerializeField] private bool axisFix;
    [SerializeField] private bool priceAxisFix;
    [SerializeField] private GameObject[] additionalObjects;
    [SerializeField] private float priceTagScale = 1f;

    [Header("Passive Income")]
    [SerializeField] private int PassiveIncomePerTick = 0;
    [SerializeField] private float TickDelay = 2f;
    [SerializeField] private GameObject RewardPrefab;
    [SerializeField] private GameObject IncomeFXTarget;

    public bool ActivationStatus { get; private set; }

    private const float DUPLICATE_DEFAULT_Y = 1.5f;
    private const float DUPLICATE_ROTATION_SPEED = 45f;
    private const float DUPLICATE_VERTICAL_RATE = 2f;
    private const float DUPLICATE_VERTICAL_AMP = 0.1f;
    private const float PRICE_TAG_ELEVATION = 0.025f;
    private const float ADDITIONAL_OBJECT_UNLOCK_DELAY = 0.15f;

    private Animation thisAnim;
    private Transform thisTr;
    private Transform objTr;
    private Transform duplicateTr;
    private BoxCollider originalCollider;
    private PriceTag priceTag;
    private float timer;
    private float activationTimer;
    private float incomeTimer;

    public int GetUnlockingCost()
    {
        return unlockingCost;
    }

    IEnumerator UnlockAdditionals()
    {
        for (int z = 0; z < additionalObjects.Length; z++)
        {
            yield return new WaitForSeconds(ADDITIONAL_OBJECT_UNLOCK_DELAY);

            additionalObjects[z].gameObject.name = "Additional";
            additionalObjects[z].gameObject.tag = "Untagged";
            Animation anim = additionalObjects[z].AddComponent<Animation>();
            anim.AddClip(UnlockableObjectController.Instance.AnimationClipAdd, "unlock");
            anim.clip = UnlockableObjectController.Instance.AnimationClipAdd;
            anim.enabled = true;
            anim.Play("unlock");
            additionalObjects[z].transform.localScale = Vector3.one;
        }
    }

    private void ActivateObject(bool activatedFromStart)
    {
        ActionTimer.Instance.SetIndication(0, 0);
        objTr.localScale = Vector3.one;
        ActivationStatus = true;
        thisAnim.Play("unlock");
        priceTag.Close();
        PlayerChar.Instance.ChangeMoney(-unlockingCost);
        originalCollider.enabled = true;
        this.gameObject.tag = "Untagged";
        EventController.TriggerEvent(EventController.EventTypes.ObjectWasUnlocked, this.name);
        if (SaveGameID.Trim() != "")
            PlayerPrefs.SetInt(SaveGameID, 1);

        StartCoroutine(UnlockAdditionals());

        this.TryGetComponent(out UsableObject obj);
        if (obj != null)
            if (obj.PartOfGroup != null)
                obj.PartOfGroup.GetNewNPCFromSofa(obj);
        /*
        if (!activatedFromStart)
            TabtaleEvents.OnObjectUnlocked(duplicateScale);
        */
    }

    public void AddActivationTimer(float _delta)
    {
        if (ActivationStatus)
            return;

        activationTimer += _delta;

        if (activationTimer < GameManager.Instance.ObjectUnlockDuration)
        {
            ActionTimer.Instance.SetIndication(activationTimer, GameManager.Instance.ObjectUnlockDuration);
            return;
        }
        ActivateObject(false);
    }

    public void PlayerLeave()
    {
        if (!ActivationStatus)
        {
            activationTimer = 0;
            ActionTimer.Instance.SetIndication(0, 0);
        }
        else
        {
            originalCollider.isTrigger = false;
            this.gameObject.tag = "UnlockedObject";
        }
    }

    GameObject CreateObjectContainer(ref Transform _setTransform, string _namePrefix)
    {
        _setTransform = new GameObject().transform;
        _setTransform.parent = thisTr;
        _setTransform.name = _namePrefix + "Model";
        _setTransform.transform.localRotation = Quaternion.identity;
        _setTransform.transform.localPosition = Vector3.zero;
        _setTransform.transform.localScale = Vector3.one;
        GameObject objContainer = new GameObject();
        objContainer.transform.parent = _setTransform;
        objContainer.name = _namePrefix + "Container";
        objContainer.transform.localRotation = Quaternion.identity;
        objContainer.transform.localPosition = Vector3.zero;
        objContainer.transform.localScale = Vector3.one;

        return objContainer;
    }

    void PrepareObjects()
    {
        thisTr = this.transform;
        thisTr.gameObject.tag = "UnlockableObject";

        thisAnim = this.gameObject.AddComponent<Animation>();
        thisAnim.AddClip(UnlockableObjectController.Instance.AnimationClip, "unlock");
        thisAnim.clip = UnlockableObjectController.Instance.AnimationClip;
        thisAnim.enabled = true;
        thisAnim.Stop();

        // creating an object model container
        GameObject objContainer = CreateObjectContainer(ref objTr, "Object");

        // moving renderer and filter to the container
        MeshRenderer mr = objContainer.AddComponent<MeshRenderer>();
        MeshFilter mf = objContainer.AddComponent<MeshFilter>();
        mr.materials = this.GetComponent<MeshRenderer>().materials;
        mf.mesh = this.GetComponent<MeshFilter>().mesh;
        Destroy(this.GetComponent<MeshRenderer>());
        Destroy(this.GetComponent<MeshFilter>());

        // collider management
        originalCollider = thisTr.gameObject.GetComponent<BoxCollider>();
        originalCollider.isTrigger = true;

        // creating a smaller duplicate
        GameObject dpContainer = CreateObjectContainer(ref duplicateTr, "Duplicate");
        dpContainer.transform.localScale = Vector3.one * (float)duplicateScale / 100f;
        dpContainer.transform.localPosition = Vector3.zero;

        // adding mesh renderer and filter
        mr = dpContainer.AddComponent<MeshRenderer>();
        mr.material = UnlockableObjectController.Instance.DuplicateObjectMaterial;
        mf = dpContainer.AddComponent<MeshFilter>();
        mf.mesh = objContainer.GetComponent<MeshFilter>().mesh;

        // adding a price tag
        GameObject priceContainer = new GameObject();
        priceContainer.name = "PriceTag";
        priceContainer.transform.parent = thisTr;
        priceContainer.transform.localRotation = Quaternion.identity;
        priceContainer.transform.localScale = Vector3.one;
        GameObject pt = GameObject.Instantiate(UnlockableObjectController.Instance.PriceTagPrefab);
        priceTag = pt.GetComponent<PriceTag>();
        pt.transform.SetParent(priceContainer.transform);
        pt.transform.localPosition = Vector3.zero;

        // fuck... artists will never stop messing with Y and Z
        if (!axisFix)
            priceContainer.transform.localPosition = new Vector3(0, 0, PRICE_TAG_ELEVATION);
        else
            priceContainer.transform.localPosition = new Vector3(0, PRICE_TAG_ELEVATION, 0);
        pt.transform.localRotation = Quaternion.Euler(-90, -90, -90);
        pt.transform.rotation = Quaternion.Euler(pt.transform.rotation.eulerAngles.x, 0, pt.transform.rotation.eulerAngles.z);

        if (axisFix)
            priceContainer.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
        if (priceAxisFix)
            priceContainer.transform.Rotate(new Vector3(90, 0, 90), Space.Self);

        // hiding additional object
        for (int z = 0; z < additionalObjects.Length; z++)
            additionalObjects[z].transform.localScale = Vector3.zero;
    }

    void Start()
    {
        PrepareObjects();

        ActivationStatus = false;
        timer = 0;
        activationTimer = 0;
        incomeTimer = 0;

        if ((SaveGameID.Trim() != "") && PlayerPrefs.HasKey(SaveGameID) && (PlayerPrefs.GetInt(SaveGameID) > 0))
            unlockingCost = 0;

        if (unlockingCost > 0)
        {
            objTr.localScale = Vector3.zero;
            priceTag.Activate(unlockingCost, PriceTagAssociation.Object, "", priceTagScale);
        }
        else
        {
            ActivateObject(true);
            originalCollider.isTrigger = false;
            this.gameObject.tag = "UnlockedObject";
        }
    }

    private void ProcessIncome()
    {
        if ((PassiveIncomePerTick == 0) || (TickDelay == 0)) return;

        incomeTimer += Time.deltaTime;
        if (incomeTimer < TickDelay) return;

        incomeTimer -= TickDelay;

        PlayerChar.Instance.ChangeMoney(PassiveIncomePerTick);
        FlyingObjects.Instance.CreateEffect(RewardPrefab, this.transform, IncomeFXTarget.transform, 1, true);
    }

    void Update()
    {
        if (!ActivationStatus)
        {
            timer += (Time.deltaTime * DUPLICATE_VERTICAL_RATE);

            float y = DUPLICATE_DEFAULT_Y + (Mathf.Sin(timer) * DUPLICATE_VERTICAL_AMP);
            if (!axisFix)
                duplicateTr.localPosition = new Vector3(0, 0, y);
            else
                duplicateTr.localPosition = new Vector3(0, y, 0);
        }

        if (!axisFix)
            duplicateTr.Rotate(new Vector3(0, 0, Time.deltaTime * DUPLICATE_ROTATION_SPEED), Space.Self);
        else
            duplicateTr.Rotate(new Vector3(0, Time.deltaTime * DUPLICATE_ROTATION_SPEED, 0), Space.Self);

        if (ActivationStatus)
            ProcessIncome();
    }
}
