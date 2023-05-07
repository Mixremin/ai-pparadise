using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StaffHirePanel : BasePanel
    {
        [SerializeField] private Button[] _assistantsHiring;
        [SerializeField] private Text[] _assistantsHiringPrices;
        [SerializeField] private Button _staffUpgrade;
        [SerializeField] private StaffUpgradePanel _staffUpgradePanel;

        private void Start()
        {
            _staffUpgrade.onClick.AddListener(OpenStaffUpgradePanel);
            for (var i = 0; i < _assistantsHiring.Length; i++)
            {
                var index = i;
                _assistantsHiring[i].onClick.AddListener(() => HireAssistant(index));
            }

            DisableMaxUpgrades();
            UpdatePrices();
            
            EventController.AddListener(EventController.EventTypes.OpenUpgradeStaffUIWindow, _ => Show(false));
        }

        private void HireAssistant(int index)
        {
//            EventController.TriggerEvent(EventController.EventTypes.ChangePlayerMoney,
//                '-' + _assistantsHiringPrices[index].text);
            EventController.TriggerEvent(EventController.EventTypes.HireAssistant, index.ToString());
            DisableMaxUpgrades();
        }

        private void OpenStaffUpgradePanel()
        {
            Hide(true);
            _staffUpgradePanel.Show(true);
        }

        protected override void DisableMaxUpgrades()
        {
            for (var i = 0; i < StaffManager.HiredStaff.Count; i++)
            {
                if (StaffManager.HiredStaff[i])
                {
                    _assistantsHiring[i].interactable = false;
                }
            }
        }

        protected override void UpdatePrices()
        {
            for (var i = 0; i < _assistantsHiringPrices.Length; i++)
            {
                _assistantsHiringPrices[i].text =
                    UpgradesData.GetUpgradeByLevel(i, UpgradesData.AssistantsPurchase).Price.ToString();
            }
        }
    }
}