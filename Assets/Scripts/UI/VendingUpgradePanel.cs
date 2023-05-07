using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class VendingUpgradePanel : BasePanel
    {
        [SerializeField] private Button _capacityUpgrade;
        [SerializeField] private Text _capacityUpgradePrice;
        [SerializeField] private Button _speedUpgrade;
        [SerializeField] private Text _speedUpgradePrice;
        private int _vendingIndex;

        private void Start()
        {
            _capacityUpgrade.onClick.AddListener(UpgradeVendingCapacity);
            _speedUpgrade.onClick.AddListener(UpgradeVendingSpeed);

            EventController.AddListener(EventController.EventTypes.OpenUpgradeVendingUIWindow, index =>
            {
                _vendingIndex = int.Parse(index);
                DisableMaxUpgrades();
                UpdatePrices();
                Show(false);
            });
        }

        private void UpgradeVendingCapacity()
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney,
//                '-' + _capacityUpgradePrice.text);
            EventController.TriggerEvent(EventController.EventTypes.UpgradeVendingCapacity, _vendingIndex.ToString());
            DisableMaxUpgrades();
            UpdatePrices();
        }

        private void UpgradeVendingSpeed()
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney,
//                '-' + _speedUpgradePrice.text);
            EventController.TriggerEvent(EventController.EventTypes.UpgradeVendingSpeed, _vendingIndex.ToString());
            DisableMaxUpgrades();
            UpdatePrices();
        }

        protected override void DisableMaxUpgrades()
        {
/*            if (_capacityUpgrade.interactable &&
                GameManager.IsMaxLevel(UpgradesLevels.VendingCapacities[_vendingIndex], UpgradesData.VendingCapacity))
            {
                _capacityUpgrade.interactable = false;
            }

            if (_speedUpgrade.interactable &&
                GameManager.IsMaxLevel(UpgradesLevels.VendingSpeeds[_vendingIndex], UpgradesData.VendingSpeed))
            {
                _speedUpgrade.interactable = false;
            }*/
        }

        protected override void UpdatePrices()
        {
            _capacityUpgradePrice.text = UpgradesData
                .GetUpgradeByLevel(UpgradesLevels.VendingCapacities[_vendingIndex] + 1, UpgradesData.VendingCapacity)
                .Price.ToString();
            _speedUpgradePrice.text = UpgradesData
                .GetUpgradeByLevel(UpgradesLevels.VendingSpeeds[_vendingIndex] + 1, UpgradesData.VendingSpeed).Price
                .ToString();
        }
    }
}