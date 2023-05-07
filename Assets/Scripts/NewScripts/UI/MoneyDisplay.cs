using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplay : MonoBehaviour
{
    static MoneyDisplay thisInstance;

    private Text thisText;
    private int targetAmount;
    private float currentAmount;
    private float timer;
    private float startAmount;

    private const float REACH_TIME = 0.4f;

    // singleton behaviour
    public static MoneyDisplay Instance
    {
        get
        {
            if (!thisInstance)
            {
                thisInstance = FindObjectOfType(typeof(MoneyDisplay)) as MoneyDisplay;

                if (!thisInstance)
                    Debug.LogError("No active MoneyDisplay script found in scene.");
            }

            return thisInstance;
        }
    }

    public void UpdateCounter(int _amount)
    {
        startAmount = currentAmount;
        targetAmount = _amount;
        timer = REACH_TIME;
    }

    void Start()
    {
        startAmount = 0;
        timer = -1;
        thisText = GetComponent<Text>();
    }

    void Update()
    {
        if ((timer <= 0) && (timer > -1))
        {
            timer = -1;
            thisText.text = targetAmount.ToString();
            currentAmount = targetAmount;
        }
        else
        {
            timer -= Time.deltaTime;
            currentAmount = Mathf.Lerp(targetAmount, startAmount, timer / REACH_TIME);
            thisText.text = Mathf.FloorToInt(currentAmount).ToString();
        }
    }
}
