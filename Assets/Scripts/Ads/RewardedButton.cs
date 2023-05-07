using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RewardedButton : MonoBehaviour
{
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        _button.interactable = MaxSdkService.Instance.IsRewardedReady;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(ShowRewarded);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(ShowRewarded);
    }

    private static void ShowRewarded()
    {
        MaxSdkService.Instance.ShowRewarded(() => PlayerChar.Instance.ChangeMoney(1000));
    }
}