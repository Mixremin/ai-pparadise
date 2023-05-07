using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StaffUpgradePanel : BasePanel
    {
        [SerializeField] private Button _characterCapacityUpgrade;
        [SerializeField] private Text _characterCapacityUpgradePrice;
        [SerializeField] private Button _characterSpeedUpgrade;
        [SerializeField] private Text _characterSpeedUpgradePrice;
        [SerializeField] private Button _assistantsSpeedUpgrade;
        [SerializeField] private Text _assistantsSpeedUpgradePrice;
        [SerializeField] private Button _assistantsCapacityUpgrade;
        [SerializeField] private Text _assistantsCapacityUpgradePrice;
        [SerializeField] private Button _staffHire;
        [SerializeField] private StaffHirePanel _staffHirePanel;

        private void Start()
        {
            _staffHire.onClick.AddListener(OpenStaffHirePanel);
            _characterCapacityUpgrade.onClick.AddListener(UpgradeCharacterCapacity);
            _characterSpeedUpgrade.onClick.AddListener(UpgradeCharacterSpeed);
            _assistantsSpeedUpgrade.onClick.AddListener(UpgradeAssistantsSpeed);
            _assistantsCapacityUpgrade.onClick.AddListener(UpgradeAssistantsCapacity);
            DisableMaxUpgrades();
            UpdatePrices();
        }

        private void UpgradeCharacterCapacity()
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney,
//                '-' + _characterCapacityUpgradePrice.text);
            EventController.TriggerEvent(EventController.EventTypes.UpgradeCharacterCapacity, string.Empty);
            DisableMaxUpgrades();
            UpdatePrices();
        }

        private void UpgradeCharacterSpeed()
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney,
//                '-' + _characterSpeedUpgradePrice.text);
            EventController.TriggerEvent(EventController.EventTypes.UpgradeCharacterSpeed, string.Empty);
            DisableMaxUpgrades();
            UpdatePrices();
        }

        private void UpgradeAssistantsSpeed()
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney,
//                '-' + _assistantsSpeedUpgradePrice.text);
            EventController.TriggerEvent(EventController.EventTypes.UpgradeAssistantSpeed, string.Empty);
            DisableMaxUpgrades();
            UpdatePrices();
        }

        private void UpgradeAssistantsCapacity()
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney,
//                '-' + _assistantsCapacityUpgradePrice.text);
            EventController.TriggerEvent(EventController.EventTypes.UpgradeAssistantCapacity, string.Empty);
            DisableMaxUpgrades();
            UpdatePrices();
        }

        private void OpenStaffHirePanel()
        {
            Hide(true);
            _staffHirePanel.Show(true);
        }

        protected override void DisableMaxUpgrades()
        {
            /*if (_characterCapacityUpgrade.interactable &&
                GameManager.IsMaxLevel(UpgradesLevels.CharacterCapacity, UpgradesData.CharacterCapacity))
            {
                _characterCapacityUpgrade.interactable = false;
            }

            if (_characterSpeedUpgrade.interactable &&
                GameManager.IsMaxLevel(UpgradesLevels.CharacterSpeed, UpgradesData.CharacterSpeed))
            {
                _characterSpeedUpgrade.interactable = false;
            }

            if (_assistantsCapacityUpgrade.interactable &&
                GameManager.IsMaxLevel(UpgradesLevels.AssistantsCapacity, UpgradesData.AssistantsCapacity))
            {
                _assistantsCapacityUpgrade.interactable = false;
            }

            if (_assistantsSpeedUpgrade.interactable &&
                GameManager.IsMaxLevel(UpgradesLevels.AssistantsSpeed, UpgradesData.AssistantsSpeed))
            {
                _assistantsSpeedUpgrade.interactable = false;
            }*/
        }

        protected override void UpdatePrices()
        {
            _characterCapacityUpgradePrice.text = UpgradesData
                .GetUpgradeByLevel(UpgradesLevels.CharacterCapacity + 1, UpgradesData.CharacterCapacity).Price
                .ToString();
            _characterSpeedUpgradePrice.text = UpgradesData
                .GetUpgradeByLevel(UpgradesLevels.CharacterSpeed + 1, UpgradesData.CharacterSpeed).Price.ToString();
            _assistantsCapacityUpgradePrice.text = UpgradesData
                .GetUpgradeByLevel(UpgradesLevels.AssistantsCapacity + 1, UpgradesData.AssistantsCapacity).Price
                .ToString();
            _assistantsSpeedUpgradePrice.text = UpgradesData
                .GetUpgradeByLevel(UpgradesLevels.AssistantsSpeed + 1, UpgradesData.AssistantsSpeed).Price.ToString();
        }
    }
}