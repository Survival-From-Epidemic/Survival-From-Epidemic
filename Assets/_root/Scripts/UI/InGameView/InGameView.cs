using System.Collections;
using _root.Scripts.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.InGameView
{
    public class InGameView : GameView
    {
        [SerializeField] private TextMeshProUGUI banbalText;
        [SerializeField] private Image banbalImage;
        [SerializeField] private TextMeshProUGUI authorityText;
        [SerializeField] private Image authorityImage;
        [SerializeField] private TextMeshProUGUI healthyText;
        [SerializeField] private TextMeshProUGUI infectText;
        [SerializeField] private TextMeshProUGUI deadText;

        private Coroutine _coroutine;

        private void OnEnable()
        {
            _coroutine = StartCoroutine(TextUpdate());
        }

        private void OnDisable()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
        }

        private IEnumerator TextUpdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                banbalText.text = $"{100 - Mathf.FloorToInt(ValueManager.Instance.banbal * 100):n0}%";
                banbalImage.fillAmount = 1 - ValueManager.Instance.banbal;

                authorityText.text = $"{100 - Mathf.FloorToInt(ValueManager.Instance.authority * 100):n0}%";
                authorityImage.fillAmount = 1 - ValueManager.Instance.authority;

                healthyText.text = $"{ValueManager.Instance.person.healthyPerson}";
                infectText.text = $"{ValueManager.Instance.person.infectedPerson}";
                deadText.text = $"{ValueManager.Instance.person.deathPerson}";
            }
        }
    }
}