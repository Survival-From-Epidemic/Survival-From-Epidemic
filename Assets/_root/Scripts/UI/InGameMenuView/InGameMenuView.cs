using System;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Game;
using _root.Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _root.Scripts.UI.InGameMenuView
{
    public class InGameMenuView : GameView
    {
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private UIImage policyImage;
        [SerializeField] private UIImage adminImage;
        [SerializeField] private UIImage serviceImage;
        [SerializeField] private GameObject policyGrid;
        [SerializeField] private GameObject adminGrid;
        [SerializeField] private GameObject serviceGrid;
        [SerializeField] private Image clicker;
        [SerializeField] private TextMeshProUGUI clickerTitle;
        [SerializeField] private TextMeshProUGUI clickerDescription;
        [SerializeField] private UIImage clickerBackground;
        [SerializeField] private UIImage clickerPreObjectImage;
        [SerializeField] private Image clickerPreObjectIcon;
        [SerializeField] private TextMeshProUGUI clickerPreObjectText1;
        [SerializeField] private TextMeshProUGUI clickerPreObjectText2;
        [SerializeField] private TextMeshProUGUI clickerCost;
        [SerializeField] private TextMeshProUGUI clickerCostEnableInfo;
        [SerializeField] private TextMeshProUGUI clickerCostDisableInfo;
        [SerializeField] private TextMeshProUGUI clickerText;
        [SerializeField] private InGameMenuPage inGameMenuPage = InGameMenuPage.Policy;
        [SerializeField] private Sprite[] enableSprites;
        [SerializeField] private Sprite[] disableSprites;
        [SerializeField] private Sprite previewSprite;

        [SerializeField] private UIImage graphButton;
        [SerializeField] private InGameMenuGraph graphType;
        [SerializeField] private GameObject[] graphs;

        private UnityAction<BaseEventData> _closer;
        private int _currentCost;
        private List<Image>[] _graphImages;
        private List<TextMeshProUGUI>[] _graphTexts;
        private Dictionary<string, ImageData> _images;

        private void Awake()
        {
            _images = new Dictionary<string, ImageData>();
            inGameMenuPage = InGameMenuPage.Policy;
            policyGrid.gameObject.SetActive(true);
            adminGrid.gameObject.SetActive(true);
            serviceGrid.gameObject.SetActive(true);
            Revoker(policyGrid.transform);
            Revoker(adminGrid.transform);
            Revoker(serviceGrid.transform);
            policyGrid.gameObject.SetActive(true);
            adminGrid.gameObject.SetActive(false);
            serviceGrid.gameObject.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();
            policyImage.onClickDown.AddListener(_ =>
            {
                policyImage.isSelected = true;
                adminImage.isSelected = false;
                serviceImage.isSelected = false;
                policyImage.UpdateImage();
                adminImage.UpdateImage();
                serviceImage.UpdateImage();
                inGameMenuPage = InGameMenuPage.Policy;
                policyGrid.SetActive(true);
                adminGrid.SetActive(false);
                serviceGrid.SetActive(false);
                Rerender();
            });
            adminImage.onClickDown.AddListener(_ =>
            {
                policyImage.isSelected = false;
                adminImage.isSelected = true;
                serviceImage.isSelected = false;
                policyImage.UpdateImage();
                adminImage.UpdateImage();
                serviceImage.UpdateImage();
                inGameMenuPage = InGameMenuPage.Administration;
                policyGrid.SetActive(false);
                adminGrid.SetActive(true);
                serviceGrid.SetActive(false);
                Rerender();
            });
            serviceImage.onClickDown.AddListener(_ =>
            {
                policyImage.isSelected = false;
                adminImage.isSelected = false;
                serviceImage.isSelected = true;
                policyImage.UpdateImage();
                adminImage.UpdateImage();
                serviceImage.UpdateImage();
                inGameMenuPage = InGameMenuPage.Service;
                policyGrid.SetActive(false);
                adminGrid.SetActive(false);
                serviceGrid.SetActive(true);
                Rerender();
            });

            _closer = _ =>
            {
                clicker.DOFade(0, 0.2f).SetUpdate(true);
                clickerTitle.DOFade(0, 0.2f).SetUpdate(true);
                clickerDescription.DOFade(0, 0.2f).SetUpdate(true);
                clickerCost.DOFade(0, 0.2f).SetUpdate(true);
                clickerCostEnableInfo.DOFade(0, 0.2f).SetUpdate(true);
                clickerCostDisableInfo.DOFade(0, 0.2f).SetUpdate(true);
                clickerText.DOFade(0, 0.2f).SetUpdate(true);
                clickerBackground.image.DOFade(0, 0.2f).SetUpdate(true)
                    .OnComplete(() =>
                    {
                        clicker.gameObject.SetActive(false);
                        clickerBackground.gameObject.SetActive(false);
                        clickerPreObjectImage.gameObject.SetActive(false);
                        clickerText.gameObject.SetActive(false);
                    });
            };

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

            clickerBackground.onClickDown.AddListener(_closer);

            graphButton.onClickDown.AddListener(_ => graphType = (InGameMenuGraph)(((int)graphType + 1) % 3));
        }

        protected override void Update()
        {
            base.Update();
            if (clickerCost.isActiveAndEnabled)
            {
                clickerCost.color = MoneyManager.Instance.HasMoney(_currentCost) ? Color.white : Color.red;
            }

            for (var i = 0; i < 3; i++) graphs[i].SetActive(i == (int)graphType);

            var images = _graphImages[(int)graphType];
            var texts = _graphTexts[(int)graphType];
            var valueManager = ValueManager.Instance;
            switch (graphType)
            {
                case InGameMenuGraph.Person:
                    images[0].fillAmount = (float)valueManager.person.healthyPerson / valueManager.person.totalPerson;
                    images[1].fillAmount = (float)valueManager.person.infectedPerson / valueManager.person.totalPerson;
                    images[2].fillAmount = (float)valueManager.person.deathPerson / valueManager.person.totalPerson;

                    texts[0].text = $"{valueManager.person.healthyPerson}명";
                    texts[1].text = $"{valueManager.person.infectedPerson}명";
                    texts[2].text = $"{valueManager.person.deathPerson}명";

                    for (var i = 0; i < 3; i++) texts[0].rectTransform.anchoredPosition = new Vector2(0, GetGraphYPos(images[i].fillAmount));
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

                    for (var i = 0; i < 4; i++) texts[0].rectTransform.anchoredPosition = new Vector2(0, GetGraphYPos(images[i].fillAmount));
                    break;
                case InGameMenuGraph.Disease:
                    images[0].fillAmount = TimeManager.Instance.ModificationWeight() * 0.08f;
                    images[1].fillAmount = valueManager.disease.infectivity * valueManager.disease.infectWeight * valueManager.disease.infectPower
                                           / valueManager.disease.infectivity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnEnable()
        {
            Rerender();
        }

        private static float GetGraphYPos(float value) => 32 - 332 * value;

        private static int SellPricer(DateTime date)
        {
            var today = TimeManager.Instance.today;
            Debugger.Log($"today: {today} / buy: {date}");
            if (date.AddDays(14) >= today) return 2500;
            if (date.AddDays(30) >= today) return 6000;
            return 20000;
        }

        private void Rerender()
        {
            var localDataManager = LocalDataManager.Instance;
            var bought = new HashSet<string>();
            var enable = new HashSet<string>();
            foreach (var (key, _) in _images)
            {
                var data = localDataManager.GetGridData(localDataManager.GetKey(key));
                if (localDataManager.IsBought(key)) bought.Add(key);
                else if (data.parent.Count <= 0 || data.parent.TrueForAll(v => localDataManager.IsBought(v))) enable.Add(key);
            }

            foreach (var (key, value) in _images)
            {
                var data = localDataManager.GetGridData(localDataManager.GetKey(key));
                if (bought.Contains(key))
                {
                    value.uiImage.gameObject.SetActive(true);
                    value.uiImage.ForceChangeImage(enableSprites[(int)inGameMenuPage]);
                    value.icon.gameObject.SetActive(true);
                    value.text1.gameObject.SetActive(true);
                    value.text2.gameObject.SetActive(true);
                }
                else if (enable.Contains(key))
                {
                    value.uiImage.gameObject.SetActive(true);
                    value.uiImage.ForceChangeImage(disableSprites[(int)inGameMenuPage]);
                    value.icon.gameObject.SetActive(true);
                    value.text1.gameObject.SetActive(true);
                    value.text2.gameObject.SetActive(true);
                }
                else if (data.parent.Exists(v => enable.Contains(v)))
                {
                    value.uiImage.gameObject.SetActive(true);
                    value.uiImage.ForceChangeImage(previewSprite);
                    value.icon.gameObject.SetActive(false);
                    value.text1.gameObject.SetActive(false);
                    value.text2.gameObject.SetActive(false);
                }
                else
                {
                    value.uiImage.gameObject.SetActive(false);
                    value.icon.gameObject.SetActive(false);
                    value.text1.gameObject.SetActive(false);
                    value.text2.gameObject.SetActive(false);
                }
            }
        }

        private void Revoker(Transform trans)
        {
            for (var i = trans.childCount - 1; i >= 0; i--)
            {
                var obj = trans.GetChild(i);
                var uiImage = obj.GetComponent<UIImage>();

                // Debugger.Log(uiImage.name);
                var imageData = new ImageData
                {
                    uiImage = uiImage,
                    icon = obj.GetChild(0).GetComponent<Image>(),
                    text1 = obj.GetChild(1).GetComponent<TextMeshProUGUI>(),
                    text2 = obj.GetChild(2).GetComponent<TextMeshProUGUI>()
                };
                var localDataManager = LocalDataManager.Instance;
                var data = localDataManager.GetGridData(uiImage.name);
                uiImage.onClickDown.AddListener(_ =>
                {
                    if (data.parent.Count > 0 && !data.parent.TrueForAll(v => localDataManager.IsBought(v))) return;
                    var isBought = localDataManager.IsBought(data.name);
                    _currentCost = data.weight * (isBought ? SellPricer(localDataManager.GetBuy(data.name)) : 30000);
                    var canBuy = isBought || MoneyManager.Instance.HasMoney(_currentCost);
                    var uiImagePosition = uiImage.image.rectTransform.position;
                    clicker.gameObject.SetActive(true);
                    clickerText.gameObject.SetActive(true);
                    clicker.DOFade(0.85f, 0.2f).SetUpdate(true);
                    clickerTitle.DOFade(1f, 0.2f).SetUpdate(true);
                    clickerDescription.DOFade(1f, 0.2f).SetUpdate(true);
                    clickerCost.DOFade(1f, 0.2f).SetUpdate(true);
                    clickerText.DOFade(1f, 0.2f).SetUpdate(true);
                    if (!isBought) clickerCostEnableInfo.DOFade(1f, 0.2f).SetUpdate(true);
                    else clickerCostDisableInfo.DOFade(1f, 0.2f).SetUpdate(true);

                    clicker.rectTransform.position = new Vector3(uiImagePosition.x + (uiImagePosition.x >= 1.5 ? -4f : 4f), uiImagePosition.y);
                    clickerText.rectTransform.position = new Vector3(uiImagePosition.x, uiImagePosition.y - 0.8f);

                    clickerTitle.text = data.name;
                    clickerDescription.text = data.message;

                    clickerCost.text = $"{_currentCost:n0}\uffe6";

                    clickerCost.color = canBuy ? Color.white : Color.red;

                    if (isBought)
                    {
                        if (data.child.Count <= 0 || data.child.TrueForAll(v => !localDataManager.IsBought(v)))
                        {
                            clickerText.color = Color.white;
                            clickerText.text = "다시 클릭해 비활성화";
                        }
                        else
                        {
                            clickerText.color = Color.red;
                            clickerText.text = "하위 노드가 활성화 되어 있음";
                        }
                    }
                    else
                    {
                        if (canBuy)
                        {
                            clickerText.color = Color.white;
                            clickerText.text = "다시 클릭해 활성화";
                        }
                        else
                        {
                            clickerText.color = Color.red;
                            clickerText.text = "돈 부족!";
                        }
                    }

                    clickerPreObjectImage.onClickDown.RemoveAllListeners();
                    clickerPreObjectImage.onClickDown.AddListener(_ =>
                    {
                        if (isBought)
                        {
                            if (data.child.Count <= 0 || data.child.TrueForAll(v => !localDataManager.IsBought(v)))
                            {
                                LocalDataManager.Instance.Sell(data.name);
                                MoneyManager.Instance.AddMoney(_currentCost);
                            }
                            else return;
                        }
                        else
                        {
                            if (!MoneyManager.Instance.RemoveMoney(_currentCost)) return;
                            LocalDataManager.Instance.Buy(data.name);
                        }

                        _closer.Invoke(null);
                        Rerender();
                    });

                    clickerPreObjectImage.gameObject.SetActive(true);
                    clickerPreObjectImage.image.rectTransform.position = uiImagePosition;
                    clickerPreObjectImage.image.sprite = uiImage.image.sprite;
                    clickerPreObjectIcon.sprite = imageData.icon.sprite;
                    clickerPreObjectText1.text = imageData.text1.text;
                    clickerPreObjectText2.text = imageData.text2.text;
                    clickerBackground.gameObject.SetActive(true);
                    clickerBackground.image.DOFade(0.85f, 0.2f).SetUpdate(true);
                });
                _images.Add(data.name, imageData);
            }
        }

        public override void OnTimeChanged(DateTime dateTime)
        {
            dateText.text = dateTime.ToShortDateString();
        }

        [Serializable]
        public struct ImageData
        {
            public UIImage uiImage;
            public Image icon;
            public TextMeshProUGUI text1;
            public TextMeshProUGUI text2;
        }
    }
}