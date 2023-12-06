using System;
using System.Collections;
using System.Collections.Generic;
using _root.Scripts.Game;
using _root.Scripts.Managers;
using _root.Scripts.Managers.Sound;
using _root.Scripts.Managers.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private GameObject[] graphs;

        [SerializeField] private int time;

        private Coroutine _coroutine;

        private TextMeshProUGUI _graphButtonText;
        private List<Image>[] _graphImages;
        private List<TextMeshProUGUI>[] _graphTexts;

        private void Start()
        {
            pauseButton.isSelected = false;
            playButton.isSelected = true;
            pauseButton.UpdateImage();
            playButton.UpdateImage();

            resetButton.onClickDown.AddListener(_ =>
            {
                time = 0;
                GraphUpdate();
            });
            pauseButton.onClickDown.AddListener(_ =>
            {
                if (_coroutine != null) StopCoroutine(_coroutine);
                pauseButton.isSelected = true;
                playButton.isSelected = false;
                pauseButton.UpdateImage();
                playButton.UpdateImage();
            });
            playButton.onClickDown.AddListener(_ =>
            {
                if (_coroutine != null) StopCoroutine(_coroutine);
                _coroutine = StartCoroutine(Run());
                pauseButton.isSelected = false;
                playButton.isSelected = true;
                pauseButton.UpdateImage();
                playButton.UpdateImage();
            });
            skipButton.onClickDown.AddListener(_ =>
            {
                time = ServerDataManager.Instance.TimeLeapLength() - 1;
                GraphUpdate();
            });
            mainButton.onClickDown.AddListener(_ =>
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                UIManager.Instance.EnableUI(UIElements.GameStart);
            });

            _graphImages = new[] { new List<Image>(), new(), new() };
            _graphTexts = new[] { new List<TextMeshProUGUI>(), new(), new() };
            for (var i = 0; i < graphs.Length; i++)
            {
                var trans = graphs[i].transform;
                var childCount = graphs[i].transform.childCount;
                for (var j = 0; j < childCount; j++)
                {
                    var child = trans.GetChild(j);
                    _graphImages[i].Add(child.GetComponent<Image>());
                    _graphTexts[i].Add(child.GetChild(1).GetComponent<TextMeshProUGUI>());
                }
            }

            GraphUpdate();
        }

        private void OnEnable()
        {
            SoundManager.Instance.StopAllLoopSound();

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

            title.text = GameManager.Instance.gameEndType switch
            {
                GameEndType.Win => "바이러스로부터 살아남았습니다. 당신의 판단 덕분에 학교를 지켜낼 수 있었습니다.",
                GameEndType.Banbal => "당신의 판단으로 학생과 학부모의 불만과 반발이 극심해져 더 이상 활동이 불가합니다.",
                GameEndType.Authority => "당신의 판단으로 인해 많은 사상자가 발생했습니다. 방역에 도움이 되지 않아 권위를 실추당했습니다.",
                _ => throw new ArgumentOutOfRangeException()
            };

            time = 0;
        }

        private void GraphUpdate()
        {
            var serverDataManager = ServerDataManager.Instance;
            var timeLeap = serverDataManager.GetTimeLeap(time);

            for (var i = 0; i < nodeTexts.Length; i++) nodeTexts[i].text = $"구매 {timeLeap.nodeBuy[i]:n0}회 / 판매 {timeLeap.nodeSell[i]:n0}회";

            for (var i = 0; i < moneyTexts.Length; i++) moneyTexts[i].text = $"{timeLeap.money[i]:n0}￦";

            for (var i = 0; i < authorityBars.Length; i++)
            {
                authorityBars[i].fillAmount = timeLeap.authority[i];
                authorityBarTexts[i].text = $"{timeLeap.authority[i] * 100:n0}%";
            }

            for (var i = 0; i < _graphImages.Length; i++)
            {
                var images = _graphImages[i];
                var texts = _graphTexts[i];

                switch (i)
                {
                    case 0:
                        images[0].fillAmount = (float)timeLeap.person.healthyPerson / timeLeap.person.totalPerson;
                        images[1].fillAmount = (float)timeLeap.person.infectedPerson / timeLeap.person.totalPerson;
                        images[2].fillAmount = (float)timeLeap.person.deathPerson / timeLeap.person.totalPerson;

                        texts[0].text = $"{timeLeap.person.healthyPerson}명";
                        texts[1].text = $"{timeLeap.person.infectedPerson}명";
                        texts[2].text = $"{timeLeap.person.deathPerson}명";
                        break;
                    case 1:
                        for (var j = 0; j < timeLeap.personGraph.Length; j++) images[j].fillAmount = (float)timeLeap.personGraph[j] / timeLeap.person.totalPerson;
                        for (var j = 0; j < timeLeap.personGraph.Length; j++) texts[j].text = $"{timeLeap.personGraph[j]}명";
                        break;
                    case 2:
                        for (var j = 0; j < timeLeap.diseaseGraph.Length; j++) images[j].fillAmount = timeLeap.diseaseGraph[j];
                        break;
                }

                for (var j = 0; j < texts.Count; j++) texts[j].rectTransform.anchoredPosition = new Vector2(0, GetGraphYPos(images[j].fillAmount));
            }

            dateText.text = TimeManager.Instance.startDate.AddDays(timeLeap.date).ToShortDateString();
            dateCountText.text = $"{timeLeap.date:n0} DAY";
        }

        private static float GetGraphYPos(float value) => 32 - 332 * (1 - value);

        private IEnumerator Run()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.03f);
                if (time >= ServerDataManager.Instance.TimeLeapLength() - 1)
                {
                    pauseButton.isSelected = true;
                    playButton.isSelected = false;
                    pauseButton.UpdateImage();
                    playButton.UpdateImage();
                    yield break;
                }

                time++;
                GraphUpdate();
            }
        }
    }
}