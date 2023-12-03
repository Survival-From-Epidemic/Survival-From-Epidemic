using System;
using System.Collections.Generic;
using _root.Scripts.Game.Data;
using _root.Scripts.Game.Data.Child;
using _root.Scripts.Managers;
using _root.Scripts.Managers.Sound;
using _root.Scripts.SingleTon;
using Newtonsoft.Json;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class LocalDataManager : SingleMono<LocalDataManager>, IDataUpdateable
    {
        private Dictionary<string, DateTime> _buyDictionary;
        private float _deathVolume;
        private Dictionary<string, GridData> _gridDataDictionary;
        private Dictionary<string, int> _gridIdxDictionary;

        private Dictionary<string, string> _keySet;

        protected override void Awake()
        {
            base.Awake();
            _keySet = new Dictionary<string, string>();
            _buyDictionary = new Dictionary<string, DateTime>();
            _gridIdxDictionary = new Dictionary<string, int>();
            var textAsset = Resources.Load<TextAsset>("Data/grid_data");
            _gridDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, GridData>>(textAsset.text);

            foreach (var (key, value) in _gridDataDictionary)
            {
                _keySet.Add(value.name, key);
                _gridIdxDictionary.Add(value.name, value.nodeIdx);
            }
        }

        public void RegisterData(KGlobalData kGlobalData)
        {
            _buyDictionary = new Dictionary<string, DateTime>();
            foreach (var kLocalDataPair in kGlobalData.kLocalDataManager.pairs)
            {
                _buyDictionary.Add(kLocalDataPair.key, Convert.ToDateTime(kLocalDataPair.date));
            }
        }

        public KLocalDataManager Parse()
        {
            var kLocalDataManager = new KLocalDataManager();
            kLocalDataManager.pairs = new List<KLocalDataPair>();
            foreach (var kLocalDataPair in _buyDictionary)
            {
                kLocalDataManager.pairs.Add(new KLocalDataPair()
                {
                    key = kLocalDataPair.Key, 
                    date = kLocalDataPair.Value.ToShortDateString()
                });
            }
            return kLocalDataManager;
        }

        public string GetKey(string key) => _keySet[key];

        public void Buy(string key)
        {
            ServerDataManager.Instance.nodeBuy[_gridIdxDictionary[key]]++;
            _buyDictionary.Add(key, TimeManager.Instance.today);
            switch (key)
            {
                case "연구 지원 1":
                    TimeManager.Instance.VaccineUpgrade(Mathf.CeilToInt((float)TimeManager.Instance.GetVaccinePercent() * 20));
                    break;
                case "연구 지원 2":
                    TimeManager.Instance.VaccineUpgrade(Mathf.CeilToInt((float)TimeManager.Instance.GetVaccinePercent() * 40));
                    break;
                case "연구 지원 3":
                    TimeManager.Instance.VaccineUpgrade(Mathf.CeilToInt((float)TimeManager.Instance.GetVaccinePercent() * 60));
                    break;
            }

            SoundManager.Instance.PlayEffectSound(SoundKey.BuySound);
            UpdateDisease();
        }

        public void Sell(string key)
        {
            ServerDataManager.Instance.nodeSell[_gridIdxDictionary[key]]++;
            _buyDictionary.Remove(key);
            SoundManager.Instance.PlayEffectSound(SoundKey.SellSound);
            UpdateDisease();
        }

        public bool IsBought(string key) => _buyDictionary.ContainsKey(key);

        // public DateTime GetBuy(string key) => _buyDictionary.TryGetValue(key, out var value) ? value : TimeManager.Instance.today;
        public DateTime GetBuy(string key) => _buyDictionary[key];

        public GridData GetGridData(string key) => _gridDataDictionary[key];

        private void UpdateDisease()
        {
            var gridDisease = new GridDisease
            {
                infectivity = 0,
                infectPower = 0,
                infectWeight = 0,
                modificationDecrease = 0
            };
            var gridAuthority = new GridAuthority
            {
                annoy = 0,
                concentration = 0,
                mask = 0,
                study = 0
            };
            foreach (var (key, _) in _buyDictionary)
            {
                var data = _gridDataDictionary[_keySet[key]];
                gridDisease.infectWeight += data.disease.infectWeight;
                gridDisease.infectivity += data.disease.infectivity;
                gridDisease.infectPower += data.disease.infectPower;
                gridDisease.modificationDecrease += data.disease.modificationDecrease;

                gridAuthority.annoy += data.authority.annoy;
                gridAuthority.concentration += data.authority.concentration;
                gridAuthority.mask += data.authority.mask;
                gridAuthority.study += data.authority.study;
            }

            ValueManager.Instance.localGridData.gridDisease = gridDisease;
            ValueManager.Instance.localGridData.gridAuthority = gridAuthority;
        }

        [Serializable]
        public class LocalGridData
        {
            public GridDisease gridDisease;
            public GridAuthority gridAuthority;
        }

        [Serializable]
        public struct GridData
        {
            public int nodeIdx;
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