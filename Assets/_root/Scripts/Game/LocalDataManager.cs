﻿using System;
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
        private Dictionary<string, GridData> _gridDataDictionary;

        private Dictionary<string, string> _keySet;

        protected override void Awake()
        {
            base.Awake();
            _keySet = new Dictionary<string, string>();
            _buyDictionary = new Dictionary<string, DateTime>();
            var textAsset = Resources.Load<TextAsset>("Data/grid_data");
            _gridDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, GridData>>(textAsset.text);

            foreach (var (key, value) in _gridDataDictionary) _keySet.Add(value.name, key);
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

        private void UpdateDisease()
        {
            var localGridData = ValueManager.Instance.localGridData;
            localGridData.gridDisease = new GridDisease
            {
                infectivity = 0,
                infectPower = 0,
                infectWeight = 0,
                modificationDecrease = 0
            };
            localGridData.gridAuthority = new GridAuthority
            {
                annoy = 0,
                concentration = 0,
                mask = 0,
                study = 0
            };
            foreach (var (key, _) in _buyDictionary)
            {
                var data = _gridDataDictionary[_keySet[key]];
                localGridData.gridDisease.infectWeight += data.disease.infectWeight;
                localGridData.gridDisease.infectivity += data.disease.infectivity;
                localGridData.gridDisease.infectPower += data.disease.infectPower;
                localGridData.gridDisease.modificationDecrease += data.disease.modificationDecrease;

                localGridData.gridAuthority.annoy += data.authority.annoy;
                localGridData.gridAuthority.concentration += data.authority.concentration;
                localGridData.gridAuthority.mask += data.authority.mask;
                localGridData.gridAuthority.study += data.authority.study;
            }
        }

        [Serializable]
        public struct LocalGridData
        {
            public GridDisease gridDisease;
            public GridAuthority gridAuthority;
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