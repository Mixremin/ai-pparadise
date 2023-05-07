using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] private Button _close;
        public bool IsShowing { get; private set; }
        protected GameManager GameManager => GameManager.Instance;
        protected UpgradesLevels UpgradesLevels => GameManager.UpgradesLevels;
        protected UpgradesData UpgradesData => GameManager.UpgradesData;
        protected StaffManager StaffManager => GameManager.StaffManager;

        private void Awake()
        {
            _close.onClick.AddListener(OnCloseButtonClicked);
        }

        protected abstract void DisableMaxUpgrades();
        protected abstract void UpdatePrices();

        public void Show(bool instant)
        {
            IsShowing = true;
            if (instant)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                StartCoroutine(ScaleRoutine(Vector3.zero, Vector3.one));
            }
            EventController.TriggerEvent(EventController.EventTypes.UIOpened, string.Empty);
        }

        private void OnCloseButtonClicked()
        {
            Hide(false);
            EventController.TriggerEvent(EventController.EventTypes.UIClosed, string.Empty);
        }

        protected void Hide(bool instant)
        {
            IsShowing = false;
            if (instant)
            {
                transform.localScale = Vector3.zero;
            }
            else
            {
                StartCoroutine(ScaleRoutine(Vector3.one, Vector3.zero));
            }
        }

        private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration = 0.5f)
        {
            var t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                transform.localScale = Vector3.Lerp(from, to, t);
                yield return null;
            }
        }
    }
}