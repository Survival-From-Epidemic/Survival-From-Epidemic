﻿using System;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.SingleTon;
using _root.Scripts.UI.InGameView;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace _root.Scripts.Game
{
    public class NewsManager : SingleMono<NewsManager>
    {
        [SerializeField] private TextMeshProUGUI newsTitleText;
        [SerializeField] private TextMeshProUGUI newsDescriptionText;
        [SerializeField] private TextMeshProUGUI newsChatText;
        [SerializeField] private List<News> importantNewsList;
        [SerializeField] private List<string> newsList;

        private Coroutine _newsCycle;
        private Sequence _newsSequence;

        private HashSet<int> _importantNewsLeft;
        private HashSet<string> _newsLeft;

        protected override void Awake()
        {
            base.Awake();
            var news = Resources.Load<TextAsset>("Data/news_data");
            newsList = JsonConvert.DeserializeObject<List<string>>(news.text);

            var importantNews = Resources.Load<TextAsset>("Data/important_news");
            importantNewsList = JsonConvert.DeserializeObject<List<News>>(importantNews.text);

            _newsLeft = LinqUtility.ToHashSet(newsList);
            _importantNewsLeft = LinqUtility.ToHashSet(importantNewsList.Select(v => v.id));
            _newsSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .SetLoops(-1)
                .Pause()
                .OnStart(() => newsChatText.rectTransform.DOAnchorPosX(680, 0))
                .Append(newsChatText.rectTransform.DOAnchorPosX(-1000, 14f))
                .OnStepComplete(ShowRandomNews)
                .SetDelay(3);
        }

        public bool IsNotShowed(int id) => _importantNewsLeft.Contains(id);

        public void ShowNews(int id)
        {
            if (!_importantNewsLeft.Contains(id)) return;
            _importantNewsLeft.Remove(id);
            var first = importantNewsList.First(v => v.id == id);
            newsChatText.text = newsTitleText.text = first.title;
            newsDescriptionText.text = first.description;

            NewsObject.Instance.gameObject.SetActive(true);
            NewsObject.Instance.image.DOFade(1f, 0.2f);
            newsTitleText.DOFade(1f, 0.2f);
            newsDescriptionText.DOFade(1f, 0.2f);

            _newsSequence.Restart();
        }

        public void ShowRandomNews()
        {
            if (_newsLeft.Count <= 0) _newsLeft = LinqUtility.ToHashSet(newsList);
            var str = _newsLeft.ToArray()[Random.Range(0, _newsLeft.Count)];
            _newsLeft.Remove(str);
            newsChatText.text = str;
            _newsSequence.Restart();
        }

        [Serializable]
        public struct News
        {
            public int id;
            public string title;
            public string description;
        }
    }
}