using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UpgradesData")]
public class UpgradesData : ScriptableObject
{
    [Header("Clients")] public Upgrade[] ClientsSpeed;
    public Upgrade[] MoneyFromClients;

    [Header("Vending")] public Upgrade[] VendingCapacity;
    public Upgrade[] VendingSpeed;

    [Header("Character")] public Upgrade[] CharacterCapacity;
    public Upgrade[] CharacterSpeed;

    [Header("Staff")] [FormerlySerializedAs("AssistantCapacity")]
    public Upgrade[] AssistantsCapacity;

    public Upgrade[] AssistantsSpeed;
    public Upgrade[] AssistantsPurchase;

    public Upgrade GetUpgradeByLevel(int level, Upgrade[] upgrades)
    {
        foreach (var upgrade in upgrades)
        {
            if (level == upgrade.Level)
            {
                return upgrade;
            }
        }

        return upgrades[upgrades.Length - 1];
    }

    [System.Serializable]
    public struct Upgrade
    {
        public int Level;
        public float Value;
        public int Price;
    }
}