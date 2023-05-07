using System;
using UnityEngine;

public class UpgradesLevels : MonoBehaviour
{
    [SerializeField] private int _vendingCount;
    public int ClientsSpeed { get; private set; }
    public int MoneyFromClients { get; private set; }
    public int[] VendingCapacities { get; private set; }
    public int[] VendingSpeeds { get; private set; }
    public int CharacterCapacity { get; private set; }
    public int CharacterSpeed { get; private set; }
    public int AssistantsCapacity { get; private set; }
    public int AssistantsSpeed { get; private set; }

    private void Awake()
    {
        VendingCapacities = new int[_vendingCount];
        VendingSpeeds = new int[_vendingCount];
        LoadData();
    }

    private void Start()
    {
        EventController.AddListener(EventController.EventTypes.UpgradeClientsSpeed, UpgradeClientsSpeed);
        EventController.AddListener(EventController.EventTypes.UpgradeMoneyFromClients, UpgradeMoneyFromClients);
        EventController.AddListener(EventController.EventTypes.UpgradeVendingCapacity, UpgradeVendingCapacity);
        EventController.AddListener(EventController.EventTypes.UpgradeVendingSpeed, UpgradeVendingSpeed);
        EventController.AddListener(EventController.EventTypes.UpgradeCharacterCapacity, UpgradeCharacterCapacity);
        EventController.AddListener(EventController.EventTypes.UpgradeCharacterSpeed, UpgradeCharacterSpeed);
        EventController.AddListener(EventController.EventTypes.UpgradeAssistantCapacity, UpgradeAssistantCapacity);
        EventController.AddListener(EventController.EventTypes.UpgradeAssistantSpeed, UpgradeAssistantSpeed);
    }

    private void UpgradeClientsSpeed(string _)
    {
        ClientsSpeed++;
        SaveData(nameof(ClientsSpeed), ClientsSpeed);
    }

    private void UpgradeMoneyFromClients(string _)
    {
        MoneyFromClients++;
        SaveData(nameof(MoneyFromClients), MoneyFromClients);
    }

    private void UpgradeVendingCapacity(string index)
    {
        var i = Convert.ToInt32(index);
        VendingCapacities[i]++;
        SaveData(nameof(VendingCapacities) + i, VendingCapacities[i]);
    }

    private void UpgradeVendingSpeed(string index)
    {
        var i = Convert.ToInt32(index);
        VendingSpeeds[i]++;
        SaveData(nameof(VendingSpeeds) + i, VendingSpeeds[i]);
    }

    private void UpgradeCharacterCapacity(string _)
    {
        CharacterCapacity++;
        SaveData(nameof(CharacterCapacity), CharacterCapacity);
    }

    private void UpgradeCharacterSpeed(string _)
    {
        CharacterSpeed++;
        SaveData(nameof(CharacterSpeed), CharacterSpeed);
    }

    private void UpgradeAssistantCapacity(string _)
    {
        AssistantsCapacity++;
        SaveData(nameof(AssistantsCapacity), AssistantsCapacity);
    }

    private void UpgradeAssistantSpeed(string _)
    {
        AssistantsSpeed++;
        SaveData(nameof(AssistantsSpeed), AssistantsSpeed);
    }

    private void LoadData()
    {
        ClientsSpeed = PlayerPrefs.GetInt(nameof(ClientsSpeed), 1);
        MoneyFromClients = PlayerPrefs.GetInt(nameof(MoneyFromClients), 1);
        
        for (var i = 0; i < _vendingCount; i++)
        {
            VendingCapacities[i] = PlayerPrefs.GetInt(nameof(VendingCapacities) + i, 1);
            VendingSpeeds[i] = PlayerPrefs.GetInt(nameof(VendingSpeeds) + i, 1);
        }

        CharacterCapacity = PlayerPrefs.GetInt(nameof(CharacterCapacity), 1);
        CharacterSpeed = PlayerPrefs.GetInt(nameof(CharacterSpeed), 1);
        AssistantsCapacity = PlayerPrefs.GetInt(nameof(AssistantsCapacity), 1);
        AssistantsSpeed = PlayerPrefs.GetInt(nameof(AssistantsSpeed), 1);
    }

    private static void SaveData(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
}