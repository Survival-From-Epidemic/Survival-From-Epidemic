using System;
using System.Collections.Generic;
using _root.Scripts.SingleTon;
using Newtonsoft.Json;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class LocalDataManager : SingleMono<LocalDataManager>
    {
        private Dictionary<string, DateTime> _buyDictionary;
        private float _deathVolume;
        private GridAuthority _gridAuthority;
        private Dictionary<string, GridData> _gridDataDictionary;
        private GridDisease _gridDisease;

        private Dictionary<string, string> _keySet;

        protected override void Awake()
        {
            base.Awake();
            _keySet = new Dictionary<string, string>();
            _buyDictionary = new Dictionary<string, DateTime>();
            var textAsset = Resources.Load<TextAsset>("Data/grid_data");
            _gridDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, GridData>>(textAsset.text);

            foreach (var (key, value) in _gridDataDictionary)
            {
                _keySet.Add(value.name, key);
                Debugger.Log(value);
            }
        }

        public string GetKey(string key) => _keySet[key];

        public void Buy(string key)
        {
            _buyDictionary.Add(key, TimeManager.Instance.today);
            UpdateDisease();
        }

        public void Sell(string key)
        {
            _buyDictionary.Remove(key);
            UpdateDisease();
        }

        public bool IsBought(string key) => _buyDictionary.ContainsKey(key);

        // public DateTime GetBuy(string key) => _buyDictionary.TryGetValue(key, out var value) ? value : TimeManager.Instance.today;
        public DateTime GetBuy(string key) => _buyDictionary[key];

        public GridData GetGridData(string key) => _gridDataDictionary[key];

        public GridDisease GetGridDisease() => _gridDisease;
        public GridAuthority GetGridAuthority() => _gridAuthority;

        private void UpdateDisease()
        {
            _gridDisease = new GridDisease
            {
                infectivity = 0,
                infectPower = 0,
                infectWeight = 0,
                modificationDecrease = 0
            };
            _gridAuthority = new GridAuthority
            {
                annoy = 0,
                concentration = 0,
                mask = 0,
                study = 0
            };
            foreach (var (key, _) in _buyDictionary)
            {
                var data = _gridDataDictionary[_keySet[key]];
                _gridDisease.infectWeight += data.disease.infectWeight;
                _gridDisease.infectivity += data.disease.infectivity;
                _gridDisease.infectPower += data.disease.infectPower;
                _gridDisease.modificationDecrease += data.disease.modificationDecrease;

                _gridAuthority.annoy += data.authority.annoy;
                _gridAuthority.concentration += data.authority.concentration;
                _gridAuthority.mask += data.authority.mask;
                _gridAuthority.study += data.authority.study;
            }
        }

        [Serializable]
        public struct GridData
        {
            public string name;
            public string message;
            public float weight;
            public List<string> parent;
            public List<string> child;
            public GridDisease disease;
            public GridAuthority authority;

            public override string ToString() => JsonUtility.ToJson(this);
        }

        [Serializable]
        public struct GridDisease
        {
            public float infectWeight;
            public float infectivity;
            public float infectPower;
            public int modificationDecrease;
        }

        [Serializable]
        public struct GridAuthority
        {
            public int study;
            public int concentration;
            public int mask;
            public int annoy;
        }
    }
}