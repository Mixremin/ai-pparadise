using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BabyCarePanel : OnScreenUI
{
    public Text DiaperText;
    public Text BottleText;

    public void SetDiaperCount(int _value)
    {
        DiaperText.text = _value.ToString();
        if (_value <= 0)
            DiaperText.transform.parent.localScale = Vector3.zero;
    }

    public void SetBottleCount(int _value)
    {
        BottleText.text = _value.ToString();

        if (_value <= 0)
        {
            BottleText.transform.parent.localScale = Vector3.zero;
            DiaperText.transform.parent.localPosition = BottleText.transform.parent.localPosition;
        }
    }
}
