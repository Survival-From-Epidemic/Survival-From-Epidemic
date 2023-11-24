using System;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Game;
using _root.Scripts.Managers;
using _root.Scripts.UI.InGameMenuView;
using _root.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI
{
    public class GameView : View
    {
        private static InGameMenuGraph _graphType = InGameMenuGraph.Disease;

        [SerializeField] protected DefaultUI defaultUI;

        [SerializeField] private UIImage graphButton;
        [SerializeField] private GameObject[] graphs;

        private TextMeshProUGUI _graphButtonText;
        private List<Image>[] _graphImages;
        private List<TextMeshProUGUI>[] _graphTexts;

        protected virtual void Start()
        {
            defaultUI.playButton.onClickDown.AddListener(_ => TimeManager.SpeedCycle(TimeManager.Instance.speedIdx = (TimeManager.Instance.speedIdx + 1) % 4));

            InitGraph();
        }

        protected virtual void Update()
        {
            defaultUI.moneyText.text = $"{MoneyManager.Instance.GetMoney():n0}";
            defaultUI.playButtonImage.sprite = GetPlaySprite();

            UpdateGraph();
        }

        protected virtual void OnEnable()
        {
        }

        public override void OnTimeChanged(DateTime dateTime)
        {
            defaultUI.dateText.text = dateTime.ToShortDateString();
        }

        private Sprite GetPlaySprite()
        {
            if (Time.timeScale == 0) return Variables.Instance.pauseSprite;
            return TimeManager.Instance.timeScale switch
            {
                <= 0.6f => Variables.Instance.superFastPlaySprite,
                <= 1.2f => Variables.Instance.fastPlaySprite,
                _ => Variables.Instance.playSprite
            };
        }

        private void InitGraph()
        {
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

            _graphButtonText = graphButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            graphButton.onClickDown.AddListener(_ => _graphType = (InGameMenuGraph)(((int)_graphType + 1) % 3));
        }

        private void UpdateGraph()
        {
            for (var i = 0; i < 3; i++) graphs[i].SetActive(i == (int)_graphType);

            var mad1 = LocalDataManager.Instance.IsBought("의료 지원 1");
            var mad2 = LocalDataManager.Instance.IsBought("의료 지원 2");
            var mad3 = LocalDataManager.Instance.IsBought("의료 지원 3");
            var mad4 = LocalDataManager.Instance.IsBought("의료 지원 4");

            var deathValue = mad4 ? 0.06f : mad3 ? 0.2f : mad2 ? 0.45f : mad1 ? 0.75f : 1;

            var images = _graphImages[(int)_graphType];
            var texts = _graphTexts[(int)_graphType];
            var valueManager = ValueManager.Instance;
            switch (_graphType)
            {
                case InGameMenuGraph.Person:
                    images[0].fillAmount = (float)valueManager.person.healthyPerson / valueManager.person.totalPerson;
                    images[1].fillAmount = (float)valueManager.person.infectedPerson / valueManager.person.totalPerson;
                    images[2].fillAmount = (float)valueManager.person.deathPerson / valueManager.person.totalPerson;

                    texts[0].text = $"{valueManager.person.healthyPerson}명";
                    texts[1].text = $"{valueManager.person.infectedPerson}명";
                    texts[2].text = $"{valueManager.person.deathPerson}명";

                    for (var i = 0; i < 3; i++) texts[i].rectTransform.anchoredPosition = new Vector2(0, GetGraphYPos(images[i].fillAmount));
                    break;
                case InGameMenuGraph.State:
                    var var1 = valueManager.personsSet.Count(v => v.symptomType is SymptomType.Nothing or SymptomType.Weak);
                    var var2 = valueManager.personsSet.Count(v => v.symptomType is SymptomType.Normal);
                    var var3 = valueManager.personsSet.Count(v => v.symptomType is SymptomType.Strong);
                    var var4 = valueManager.personsSet.Count(v => v.symptomType is SymptomType.Emergency);

                    images[0].fillAmount = (float)var1 / valueManager.person.totalPerson;
                    images[1].fillAmount = (float)var2 / valueManager.person.totalPerson;
                    images[2].fillAmount = (float)var3 / valueManager.person.totalPerson;
                    images[3].fillAmount = (float)var4 / valueManager.person.totalPerson;

                    texts[0].text = $"{var1}명";
                    texts[1].text = $"{var2}명";
                    texts[2].text = $"{var3}명";
                    texts[3].text = $"{var4}명";

                    for (var i = 0; i < 4; i++) texts[i].rectTransform.anchoredPosition = new Vector2(0, GetGraphYPos(images[i].fillAmount));
                    break;
                case InGameMenuGraph.Disease:
                    images[0].fillAmount = TimeManager.Instance.ModificationWeight() * 0.1f * deathValue;
                    images[1].fillAmount = valueManager.preDisease.infectivity * valueManager.preDisease.infectWeight * 0.01f * valueManager.preDisease.infectPower
                                           / valueManager.disease.infectivity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _graphButtonText.text = _graphType.GetGraphName();
        }

        private static float GetGraphYPos(float value) => 32 - 332 * (1 - value);

        [Serializable]
        public class DefaultUI
        {
            public TextMeshProUGUI moneyText;
            public TextMeshProUGUI dateText;
            public UIImage playButton;
            public Image playButtonImage;
        }
    }
}