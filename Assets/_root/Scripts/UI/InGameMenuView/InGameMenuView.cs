using System;
using System.Collections.Generic;
using _root.Scripts.Game;
using _root.Scripts.Managers;
using _root.Scripts.Managers.Sound;
using _root.Scripts.Managers.UI;
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
        [SerializeField] private UIImage closeButton;
        [SerializeField] private TextMeshProUGUI panelNoText;

        private UnityAction<BaseEventData> _closer;
        private int _currentCost;

        private LocalDataManager.GridData _currentGridData;
        private Dictionary<string, ImageData> _images;

        private float _preRealGameTimeScale;
        private float _preTimeScale;

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
                if (!TimeManager.Instance.globalInfected) return;
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
                if (!TimeManager.Instance.globalInfected) return;
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
                if (!TimeManager.Instance.globalInfected) return;
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
                SoundManager.Instance.PlayEffectSound(SoundKey.InfoClose);
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

            clickerBackground.onClickDown.AddListener(_closer);
            clickerBackground.gameObject.SetActive(false);
            closeButton.onClickDown.AddListener(_ => UIManager.Instance.EnableUI(UIElements.InGame));
        }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F))
            {
                if (clickerBackground.isActiveAndEnabled)
                {
                    _closer.Invoke(null);
                }
                else
                {
                    UIManager.Instance.EnableUI(UIElements.InGame);
                }
            }

            if (clickerCost.isActiveAndEnabled)
            {
                clickerCost.color = MoneyManager.Instance.HasMoney(_currentCost) ? Color.white : Color.red;
                UpdateClickerText();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SoundManager.Instance.PlayEffectSound(SoundKey.PanelOpen);
            defaultUI.dateText.text = TimeManager.Instance.today.ToShortDateString();
            _preRealGameTimeScale = Time.timeScale;
            _preTimeScale = TimeManager.Instance.timeScale;
            Time.timeScale = 0;

            if (!policyGrid.gameObject.activeSelf && !adminGrid.gameObject.activeSelf && !serviceGrid.gameObject.activeSelf)
                policyGrid.gameObject.SetActive(true);
            Rerender();
        }

        private void OnDisable()
        {
            SoundManager.Instance.PlayEffectSound(SoundKey.PanelClose);
            Time.timeScale = _preRealGameTimeScale;
            TimeManager.Instance.timeScale = _preTimeScale;
        }

        private static int SellPricer(DateTime date)
        {
            var today = TimeManager.Instance.today;
            Debugger.Log($"today: {today} / buy: {date}");
            if (date.AddDays(14) >= today) return 3750;
            if (date.AddDays(30) >= today) return 10000;
            return 30000;
        }

        private void Rerender()
        {
            if (!TimeManager.Instance.globalInfected)
            {
                panelNoText.gameObject.SetActive(true);
                policyGrid.gameObject.SetActive(false);
                return;
            }

            panelNoText.gameObject.SetActive(false);
            var localDataManager = LocalDataManager.Instance;
            var bought = new HashSet<string>();
            var enable = new HashSet<string>();
            foreach (var (key, _) in _images)
            {
                if (!ValueManager.Instance.kitEnabled && key is "자가 진단 키트 지원 1" or "자가 진단 키트 지원 2") continue;
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
                else if (data.parent.Exists(v => enable.Contains(v) || bought.Contains(v)))
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

        private bool UpdateClickerText()
        {
            var isBought = LocalDataManager.Instance.IsBought(_currentGridData.name);
            var canBuy = isBought || MoneyManager.Instance.HasMoney(_currentCost);
            if (isBought)
            {
                if (_currentGridData.name is "연구 지원 1" or "연구 지원 2" or "연구 지원 3" or "학생 격리 1" or "학생 격리 2" or "의심 학생 격리 1" or "의심 학생 격리 2")
                {
                    clickerText.color = Color.red;
                    clickerText.text = "판매 불가";
                    return false;
                }

                if (_currentGridData.child.Count <= 0 || _currentGridData.child.TrueForAll(v => !LocalDataManager.Instance.IsBought(v)))
                {
                    clickerText.color = Color.white;
                    clickerText.text = "다시 클릭하여 환불";
                }
                else
                {
                    clickerText.color = Color.red;
                    clickerText.text = "하위 노드가 활성화됨";
                    return false;
                }
            }
            else
            {
                if (canBuy)
                {
                    clickerText.color = Color.white;
                    clickerText.text = "다시 클릭하여 구매";
                }
                else
                {
                    clickerText.color = Color.red;
                    clickerText.text = "돈 부족!";
                    return false;
                }
            }

            return true;
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
                _currentGridData = localDataManager.GetGridData(uiImage.name);
                uiImage.onClickDown.AddListener(_ =>
                {
                    _currentGridData = localDataManager.GetGridData(uiImage.name);
                    if (_currentGridData.parent.Count > 0 && !_currentGridData.parent.TrueForAll(v => localDataManager.IsBought(v))) return;
                    var isBought = localDataManager.IsBought(_currentGridData.name);

                    _currentCost = Mathf.FloorToInt(_currentGridData.weight * (isBought ? SellPricer(localDataManager.GetBuy(_currentGridData.name)) : 30000));
                    var canBuy = !isBought || MoneyManager.Instance.HasMoney(_currentCost);
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

                    clicker.rectTransform.position = new Vector3(uiImagePosition.x + (uiImagePosition.x >= 1050 ? -415f : 415f), uiImagePosition.y);
                    clickerText.rectTransform.position = new Vector3(uiImagePosition.x, uiImagePosition.y - 85f);

                    clickerTitle.text = _currentGridData.name;
                    clickerDescription.text = _currentGridData.message;

                    clickerCost.text = $"{_currentCost:n0}\uffe6";

                    clickerCost.color = canBuy ? Color.white : Color.red;

                    clickerPreObjectImage.onClickDown.RemoveAllListeners();

                    clickerPreObjectImage.gameObject.SetActive(true);
                    clickerPreObjectImage.image.rectTransform.position = uiImagePosition;
                    clickerPreObjectImage.image.sprite = uiImage.image.sprite;
                    clickerPreObjectIcon.sprite = imageData.icon.sprite;
                    clickerPreObjectText1.text = imageData.text1.text;
                    clickerPreObjectText2.text = imageData.text2.text;
                    clickerBackground.gameObject.SetActive(true);
                    clickerBackground.image.DOFade(0.85f, 0.2f).SetUpdate(true);
                    SoundManager.Instance.PlayEffectSound(SoundKey.InfoOpen);

                    clickerPreObjectImage.onClickDown.AddListener(_ =>
                    {
                        if (!UpdateClickerText()) return;
                        if (isBought)
                        {
                            LocalDataManager.Instance.Sell(_currentGridData.name);
                            MoneyManager.Instance.AddMoney(_currentCost);
                        }
                        else
                        {
                            if (!MoneyManager.Instance.RemoveMoney(_currentCost)) return;
                            LocalDataManager.Instance.Buy(_currentGridData.name);
                            clickerPreObjectImage.image.sprite = enableSprites[(int)inGameMenuPage];
                        }

                        _closer.Invoke(null);
                        Rerender();
                        clickerPreObjectImage.onClickDown.RemoveAllListeners();
                    });
                });
                _images.Add(_currentGridData.name, imageData);
            }
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