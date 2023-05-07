using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBaby : MonoBehaviour
{
    public SkinnedMeshRenderer SkinMaterialRenderer;
    public int SkinMaterialIndex;
    public MeshRenderer ClothMaterialRenderer;
    public int ClothMaterialIndex;
    [HideInInspector]
    public NPCMain Mother;
    [HideInInspector]
    public bool ReadyToReturn;
    [HideInInspector]
    public ChildTableActionZone InActionZone;
    private Animator thisAnim;
    private ParticleSystem thisPs;

    private float ShittingTimer;

    public void StartShittingTimer()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        ShittingTimer = Random.Range(GameManager.Instance.ShittingTimerMin, GameManager.Instance.ShittingTimerMax);
    }

    public void RecolourCloth()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        Material[] mats = ClothMaterialRenderer.materials;
        mats[ClothMaterialIndex].color = GameManager.Instance.ClothColours.Evaluate(Random.Range(0f, 1f));
        ClothMaterialRenderer.materials = mats;
    }

    public void Init(NPCMain _mother)
    {
        thisAnim = GetComponent<Animator>();
        Mother = _mother;

        Material[] mats = SkinMaterialRenderer.materials;
        mats[SkinMaterialIndex].color = Mother.transform.GetChild(0).GetChild(0).GetComponent<NPCCustomiser>().SetSkinClr;
        SkinMaterialRenderer.materials = mats;

        RecolourCloth();
    }

    private void ShitSelf()
    {
        ShittingTimer = -1;
        thisAnim.speed = 1f;
        thisPs.Play();
        InActionZone.ReadyPhaseTwo();
    }

    IEnumerator MoveToPlayerStack()
    {
        yield return new WaitForSeconds(0.3f);
        PlayerStacking.Instance.AddToStack(this.gameObject);
        EventController.TriggerEvent(EventController.EventTypes.BabiesInStackAmountChanged, "");
    }

    IEnumerator CallMother()
    {
        yield return new WaitForSeconds(0.6f);
        Mother.MoveToBaby(InActionZone.PhaseTwoZone.ApproachStartWpt);
    }


    public void SetReadyToReturn()
    {
        //ReadyToReturn = true;
        thisPs.Stop();

        //StartCoroutine(MoveToPlayerStack());
        StartCoroutine(CallMother()); 
    }

    private void Update()
    {
        if (ShittingTimer > 0)
        {
            ShittingTimer -= Time.deltaTime;
            if (ShittingTimer <= 0)
                ShitSelf();
        }
    }

    private void Start()
    {
        ShittingTimer = -1;
        thisAnim = GetComponent<Animator>();

        thisPs = GetComponent<ParticleSystem>();
        thisPs.Stop();
        ReadyToReturn = false;
    }
}
