using System;
using System.Collections.Generic;
using UnityEngine;

public class StaffManager : MonoBehaviour
{
    public Dictionary<int, bool> HiredStaff { get; private set; }

    private void Awake()
    {
        Load();
    }

    private void Start()
    {
        EventController.AddListener(EventController.EventTypes.HireAssistant, BuyAssistant);
    }

    private void BuyAssistant(string index)
    {
        var i = Convert.ToInt32(index);
        HiredStaff[i] = true;
        Save();
    }

    private void Load()
    {
        HiredStaff = new Dictionary<int, bool>();
        var assistantsPurchase = GameManager.Instance.UpgradesData.AssistantsPurchase;
        for (var i = 0; i < assistantsPurchase.Length; i++)
        {
            var value = PlayerPrefs.GetInt(nameof(HiredStaff) + i, 0);
            HiredStaff.Add(i, value == 1);
        }
    }

    private void Save()
    {
        for (var i = 0; i < HiredStaff.Count; i++)
        {
            PlayerPrefs.SetInt(nameof(HiredStaff) + i, HiredStaff[i] ? 1 : 0);
        }
    }
}