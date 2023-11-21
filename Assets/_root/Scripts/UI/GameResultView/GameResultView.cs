using System;
using System.Collections;
using _root.Scripts.Game;
using _root.Scripts.Managers;
using _root.Scripts.Managers.Sound;
using _root.Scripts.Managers.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.GameResultView
{
    public class GameResultView : View
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI[] nodeTexts;
        [SerializeField] private TextMeshProUGUI[] moneyTexts;
        [SerializeField] private TextMeshProUGUI[] authorityTexts;
        [SerializeField] private Image[] authorityBars;
        [SerializeField] private TextMeshProUGUI[] authorityBarTexts;
        [SerializeField] private UIImage resetButton;
        [SerializeField] private UIImage pauseButton;
        [SerializeField] private UIImage playButton;
        [SerializeField] private UIImage skipButton;
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private TextMeshProUGUI dateCountText;
        [SerializeField] private UIImage mainButton;

        [SerializeField] private int time;
        [SerializeField] private bool isPlay;

        private Coroutine _coroutine;

        private void Start()
        {
            pauseButton.isSelected = !isPlay;
            playButton.isSelected = isPlay;
            pauseButton.UpdateImage();
            playButton.UpdateImage();

            resetButton.onClickDown.AddListener(_ =>
            {
                time = 0;
                UpdateUI();
            });
            pauseButton.onClickDown.AddListener(_ =>
            {
                if (_coroutine != null) StopCoroutine(_coroutine);
            });
            playButton.onClickDown.AddListener(_ => _coroutine = StartCoroutine(Run()));
            skipButton.onClickDown.AddListener(_ =>
            {
                time = ServerDataManager.Instance.TimeLeapLength() - 1;
                UpdateUI();
            });
            mainButton.onClickDown.AddListener(_ => UIManager.Instance.EnableUI(UIElements.GameStart));

            switch (GameManager.Instance.gameEndType)
            {
                case GameEndType.Win:
                    SoundManager.Instance.PlayEffectSound(SoundKey.WinSound);
                    SoundManager.Instance.PlaySound(SoundKey.WinBackground);
                    break;
                case GameEndType.Banbal:
                case GameEndType.Authority:
                default:
                    SoundManager.Instance.PlayEffectSound(SoundKey.LoseSound);
                    SoundManager.Instance.PlaySound(SoundKey.LoseBackground);
                    break;
            }
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private IEnumerator Run()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.05f);
                time++;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            title.text = GameManager.Instance.gameEndType switch
            {
                GameEndType.Win => "바이러스로부터 살아남았습니다. 당신의 판단 덕분에 학교를 지켜낼 수 있었습니다.",
                GameEndType.Banbal => "당신의 판단으로 학생과 학부모의 불만과 반발이 극심해져 더 이상 활동이 불가합니다.",
                GameEndType.Authority => "당신의 판단으로 인해 많은 사상자가 발생했습니다. 방역에 도움이 되지 않아 권위를 실추당했습니다.",
                _ => throw new ArgumentOutOfRangeException()
            };
            var serverDataManager = ServerDataManager.Instance;
            var timeLeap = serverDataManager.GetTimeLeap(time);

            for (var i = 0; i < nodeTexts.Length; i++)
            {
                nodeTexts[i].text = $"구매 {serverDataManager.nodeBuy[i]:n0}회 / 판매 {serverDataManager.nodeSell[i]:n0}회";
            }

            for (var i = 0; i < moneyTexts.Length; i++)
            {
                moneyTexts[i].text = $"{serverDataManager.money[i]:n0}￦";
            }

            for (var i = 0; i < authorityBars.Length; i++)
            {
                authorityBars[i].fillAmount = timeLeap.authority[i];
                authorityBarTexts[i].text = $"{timeLeap.authority[i] * 100:n0}%";
            }

            dateText.text = TimeManager.Instance.startDate.AddDays(timeLeap.date).ToShortDateString();
            dateCountText.text = $"{timeLeap.date:n0} DAY";
        }
    }
}