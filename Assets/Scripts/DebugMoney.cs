using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMoney : MonoBehaviour
{
    public void Click ()
    {
        PlayerChar.Instance.ChangeMoney(1000);
    }
}
