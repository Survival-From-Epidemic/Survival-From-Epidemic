﻿using System;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Attribute;
using _root.Scripts.SingleTon;

namespace _root.Scripts.Game
{
    public class ServerDataManager : SingleMono<ServerDataManager>
    {
        [ReadOnly] public int[] nodeBuy;
        [ReadOnly] public int[] nodeSell;
        [ReadOnly] public int[] money;
        private List<TimeLeap> _timeLeapData;

        private void Start()
        {
            nodeBuy = new int[3];
            nodeSell = new int[3];
            money = new int[2];
            _timeLeapData = new List<TimeLeap>();
        }

        public void RecordTime()
        {
            var mad1 = LocalDataManager.Instance.IsBought("의료 지원 1");
            var mad2 = LocalDataManager.Instance.IsBought("의료 지원 2");
            var mad3 = LocalDataManager.Instance.IsBought("의료 지원 3");
            var mad4 = LocalDataManager.Instance.IsBought("의료 지원 4");

            var deathValue = mad4 ? 0.06f : mad3 ? 0.2f : mad2 ? 0.45f : mad1 ? 0.75f : 1;
            var valueManager = ValueManager.Instance;

            _timeLeapData.Add(new TimeLeap
            {
                date = TimeManager.Instance.date,
                nodeBuy = nodeBuy,
                nodeSell = nodeSell,
                money = money,
                authority = new[] { 1 - ValueManager.Instance.banbal, 1 - ValueManager.Instance.authority },
                person = ValueManager.Instance.person,
                diseaseGraph = new[]
                {
                    TimeManager.Instance.ModificationWeight() * 0.1f * deathValue,
                    valueManager.preDisease.infectivity * valueManager.preDisease.infectWeight * 0.01f * valueManager.preDisease.infectPower / valueManager.disease.infectivity
                },
                personGraph = new[]
                {
                    valueManager.personsSet.Count(v => v.symptomType is SymptomType.Nothing or SymptomType.Weak),
                    valueManager.personsSet.Count(v => v.symptomType is SymptomType.Normal),
                    valueManager.personsSet.Count(v => v.symptomType is SymptomType.Strong),
                    valueManager.personsSet.Count(v => v.symptomType is SymptomType.Emergency)
                }
            });
        }

        public void ForEachTimeLeap(Action<TimeLeap> action)
        {
            foreach (var timeLeap in _timeLeapData) action(timeLeap);
        }

        [Serializable]
        public struct TimeLeap
        {
            public int date;
            public int[] nodeBuy;
            public int[] nodeSell;
            public int[] money;
            public float[] authority;
            public Person person;
            public float[] diseaseGraph;
            public int[] personGraph;
        }
    }
}