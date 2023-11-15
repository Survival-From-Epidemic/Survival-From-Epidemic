using System;
using System.Collections.Generic;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class LocalDataManager : SingleMono<LocalDataManager>
    {
        private Dictionary<string, DateTime> _buyDictionary;
        private Dictionary<string, GridData> _gridDataDictionary;

        private void Start()
        {
            _buyDictionary = new Dictionary<string, DateTime>();
            var textAsset = Resources.Load<TextAsset>("Data/grid_data");
            _gridDataDictionary = JsonUtility.FromJson<Dictionary<string, GridData>>(textAsset.text);
        }

        public void Buy(string key) => _buyDictionary.Add(key, TimeManager.Instance.today);

        public void Sell(string key) => _buyDictionary.Remove(key);

        public bool IsBought(string key) => _buyDictionary.ContainsKey(key);

        public DateTime? GetBuy(string key)
        {
            if (_buyDictionary.TryGetValue(key, out var value)) return value;
            return null;
        }

        public GridData GetGridData(string key) => _gridDataDictionary[key];

        [Serializable]
        public struct GridData
        {
            public string name;
            public string message;
            public int weight;
        }
    }
}