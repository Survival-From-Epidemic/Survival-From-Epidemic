using System;
using System.Collections.Generic;
using _root.Scripts.Game;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.InGameMenuView
{
    public class InGameMenuView : View
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
        [SerializeField] private Image clickerPreObjectImage;
        [SerializeField] private Image clickerPreObjectIcon;
        [SerializeField] private TextMeshProUGUI clickerPreObjectText1;
        [SerializeField] private TextMeshProUGUI clickerPreObjectText2;
        [SerializeField] private TextMeshProUGUI clickerCost;
        [SerializeField] private TextMeshProUGUI clickerCostEnableInfo;
        [SerializeField] private TextMeshProUGUI clickerCostDisableInfo;
        [SerializeField] private InGameMenuPage inGameMenuPage = InGameMenuPage.Policy;
        [SerializeField] private Sprite[] enableSprites;
        [SerializeField] private Sprite[] disableSprites;
        [SerializeField] private Sprite previewSprite;

        private Dictionary<string, ImageData> _images;

        private void Awake()
        {
            _images = new Dictionary<string, ImageData>();
            Revoker(policyGrid.transform);
            Revoker(adminGrid.transform);
            Revoker(serviceGrid.transform);
        }

        private void Start()
        {
            policyImage.onClickDown.AddListener(_ =>
            {
                inGameMenuPage = InGameMenuPage.Policy;
                policyGrid.SetActive(true);
                adminGrid.SetActive(false);
                serviceGrid.SetActive(false);
                Rerender();
            });
            adminImage.onClickDown.AddListener(_ =>
            {
                inGameMenuPage = InGameMenuPage.Administration;
                policyGrid.SetActive(false);
                adminGrid.SetActive(true);
                serviceGrid.SetActive(false);
                Rerender();
            });
            serviceImage.onClickDown.AddListener(_ =>
            {
                inGameMenuPage = InGameMenuPage.Service;
                policyGrid.SetActive(false);
                adminGrid.SetActive(false);
                serviceGrid.SetActive(true);
                Rerender();
            });

            clickerBackground.onClickDown.AddListener(_ =>
            {
                clicker.DOFade(0, 0.4f);
                clickerTitle.DOFade(0, 0.4f);
                clickerDescription.DOFade(0, 0.4f);
                clickerCost.DOFade(0, 0.4f);
                clickerCostEnableInfo.DOFade(0, 0.4f);
                clickerCostDisableInfo.DOFade(0, 0.4f);
                clickerBackground.image.DOFade(0, 0.4f)
                    .OnComplete(() =>
                    {
                        clicker.gameObject.SetActive(false);
                        clickerBackground.gameObject.SetActive(false);
                        clickerPreObjectImage.gameObject.SetActive(false);
                    });
            });
        }

        private void OnEnable()
        {
            Rerender();
        }

        private static int SellPricer(DateTime date)
        {
            var today = TimeManager.Instance.today;
            if (date >= today.AddDays(-14)) return 15000;
            if (date >= today.AddDays(-30)) return 8000;
            if (date >= today.AddDays(-60)) return 3500;
            return 500;
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
                else if (data.parent.Count <= 0 || data.parent.Exists(v => localDataManager.IsBought(v))) enable.Add(key);
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
                var data = LocalDataManager.Instance.GetGridData(uiImage.name);
                uiImage.onClickDown.AddListener(_ =>
                {
                    var isBought = LocalDataManager.Instance.IsBought(data.name);
                    var uiImagePosition = uiImage.image.rectTransform.position;
                    clicker.gameObject.SetActive(true);
                    clicker.DOFade(0.85f, 0.4f);
                    clickerTitle.DOFade(1f, 0.4f);
                    clickerDescription.DOFade(1f, 0.4f);
                    clickerCost.DOFade(1f, 0.4f);
                    if (!isBought) clickerCostEnableInfo.DOFade(1f, 0.4f);
                    else clickerCostDisableInfo.DOFade(1f, 0.4f);

                    Debugger.Log(uiImagePosition);
                    clicker.rectTransform.position = new Vector3(uiImagePosition.x + (uiImagePosition.x >= 1.5 ? -4.5f : 4.5f), uiImagePosition.y);

                    clickerTitle.text = data.name;
                    clickerDescription.text = data.message;
                    clickerCost.text = isBought
                        ? $"{data.weight * 30000}"
                        : $"{data.weight * SellPricer(LocalDataManager.Instance.GetBuy(data.name) ?? TimeManager.Instance.today)}";

                    clickerPreObjectImage.gameObject.SetActive(true);
                    clickerPreObjectImage.rectTransform.position = uiImagePosition;
                    clickerPreObjectImage.sprite = uiImage.image.sprite;
                    clickerPreObjectIcon.sprite = imageData.icon.sprite;
                    clickerPreObjectText1.text = imageData.text1.text;
                    clickerPreObjectText2.text = imageData.text2.text;
                    clickerBackground.gameObject.SetActive(true);
                    clickerBackground.image.DOFade(0.85f, 0.4f);
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