using System;
using System.Collections.Generic;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class LocalDataManager : SingleMono<LocalDataManager>
    {
        private Dictionary<string, GridData> _gridDataDictionary;

        private void Start()
        {
            var textAsset = Resources.Load<TextAsset>("Data/grid_data");
            _gridDataDictionary = JsonUtility.FromJson<Dictionary<string, GridData>>(textAsset.text);
        }

        public GridData GetGridData(string key) => _gridDataDictionary[key];

        [Serializable]
        public struct GridData
        {
            public string message;
            public int weight;
        }
    }
}