using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ClientUpgradePanel : BasePanel
    {
        [SerializeField] private Button _moneyUpgrade;
        [SerializeField] private Text _moneyUpgradePrice;
        [SerializeField] private Button _speedUpgrade;
        [SerializeField] private Text _speedUpgradePrice;

        private void Start()
        {
            _moneyUpgrade.onClick.AddListener(UpgradeMoneyFromClients);
            _speedUpgrade.onClick.AddListener(UpgradeClientsSpeed);
            DisableMaxUpgrades();
            UpdatePrices();
        }

        private void UpgradeClientsSpeed()
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney, '-' + _speedUpgradePrice.text);
            EventController.TriggerEvent(EventController.EventTypes.UpgradeClientsSpeed, string.Empty);
            DisableMaxUpgrades();
            UpdatePrices();
        }

        private void UpgradeMoneyFromClients()
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney, '-' + _moneyUpgradePrice.text);
            EventController.TriggerEvent(EventController.EventTypes.UpgradeMoneyFromClients, string.Empty);
            DisableMaxUpgrades();
            UpdatePrices();
        }

        protected override void DisableMaxUpgrades()
        {
/*            if (_moneyUpgrade.interactable &&
                GameManager.IsMaxLevel(UpgradesLevels.MoneyFromClients, UpgradesData.MoneyFromClients))
            {
                _moneyUpgrade.interactable = false;
            }

            if (_speedUpgrade.interactable &&
                GameManager.IsMaxLevel(UpgradesLevels.ClientsSpeed, UpgradesData.ClientsSpeed))
            {
                _speedUpgrade.interactable = false;
            }*/
        }

        protected override void UpdatePrices()
        {
            _moneyUpgradePrice.text = UpgradesData
                .GetUpgradeByLevel(UpgradesLevels.MoneyFromClients + 1, UpgradesData.MoneyFromClients).Price.ToString();
            _speedUpgradePrice.text = UpgradesData
                .GetUpgradeByLevel(UpgradesLevels.ClientsSpeed + 1, UpgradesData.ClientsSpeed).Price.ToString();
        }
    }
}