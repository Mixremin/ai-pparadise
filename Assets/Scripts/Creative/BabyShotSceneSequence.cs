using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyShotSceneSequence : MonoBehaviour
{
    public bool PlayerMovementAnimation;
    public bool PlayerStackingAnimation;
    public bool PlayerCatchingAnimation;
    public int BirthTrigger;
    public Animator PlayerAnimator;
    public Animator[] WomenAnimators;

    private bool oldPlayerMoveAnim;
    private bool oldPlayerStackAnim;
    private bool oldPlayerCatchAnim;
    private int oldBirthTrigger;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovementAnimation = false;
        oldPlayerMoveAnim = false;
        PlayerStackingAnimation = false;
        oldPlayerStackAnim = false;
        PlayerCatchingAnimation = false;
        oldPlayerCatchAnim = false;
        BirthTrigger = -1;
        oldBirthTrigger = -1;

        for (int z = 0; z < WomenAnimators.Length; z++)
            WomenAnimators[z].SetBool("Laying", true);

    }
    
    // Update is called once per frame
    void Update()
    {
    
        if (PlayerMovementAnimation != oldPlayerMoveAnim)
        {
            PlayerAnimator.SetBool("Walking", PlayerMovementAnimation);
            oldPlayerMoveAnim = PlayerMovementAnimation;
        }

        if (PlayerCatchingAnimation != oldPlayerCatchAnim)
        {
            PlayerAnimator.SetBool("Catching", PlayerCatchingAnimation);
            oldPlayerCatchAnim = PlayerCatchingAnimation;
        }

        if (BirthTrigger != oldBirthTrigger)
        {
            Debug.Log("B" + BirthTrigger);
            WomenAnimators[BirthTrigger].SetTrigger("Birth");
            oldBirthTrigger = BirthTrigger;
        }

    }
}
