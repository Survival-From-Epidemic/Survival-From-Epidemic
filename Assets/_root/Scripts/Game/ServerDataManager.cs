﻿using System;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Attribute;
using _root.Scripts.Game.Data;
using _root.Scripts.Game.Data.Child;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class ServerDataManager : SingleMono<ServerDataManager>, IDataUpdateable
    {
        [ReadOnly] public int[] nodeBuy;
        [ReadOnly] public int[] nodeSell;
        [ReadOnly] public int[] money;
        [SerializeField] private List<TimeLeap> timeLeapData;

        protected override void Awake()
        {
            base.Awake();
            New();
        }

        public void RegisterData(KGlobalData kGlobalData)
        {
            timeLeapData = kGlobalData.kServerDataManager.timeLeaps
                .Select(v => v.ToTimeLeap())
                .ToList();
        }

        public KServerDataManager Parse()
        {
            var kServerDataManager = new KServerDataManager
            {
                timeLeaps = _timeLeapData.Select(timeLeap => new KTimeLeap
                {
                    date = timeLeap.date,
                    nodeBuy = timeLeap.nodeBuy.ToList(),
                    nodeSell = timeLeap.nodeSell.ToList(),
                    money = timeLeap.money.ToList(),
                    authority = timeLeap.authority.ToList(),
                    person = new KPerson
                    {
                        deathPerson = timeLeap.person.deathPerson,
                        healthyPerson = timeLeap.person.healthyPerson,
                        infectedPerson = timeLeap.person.infectedPerson,
                        totalPerson = timeLeap.person.totalPerson
                    },
                    diseaseGraph = timeLeap.diseaseGraph.ToList(),
                    personGraph = timeLeap.personGraph.ToList()
                }).ToList()
            };
            return kServerDataManager;
        }

        private void New()
        {
            nodeBuy = new int[3];
            nodeSell = new int[3];
            money = new int[2];
            timeLeapData = new List<TimeLeap>();
        }

        public void RecordTime()
        {
            var mad1 = LocalDataManager.Instance.IsBought("의료 지원 1");
            var mad2 = LocalDataManager.Instance.IsBought("의료 지원 2");
            var mad3 = LocalDataManager.Instance.IsBought("의료 지원 3");
            var mad4 = LocalDataManager.Instance.IsBought("의료 지원 4");

            var deathValue = mad4 ? 0.06f : mad3 ? 0.2f : mad2 ? 0.45f : mad1 ? 0.75f : 1;
            var valueManager = ValueManager.Instance;

            timeLeapData.Add(new TimeLeap
            {
                date = TimeManager.Instance.date,
                nodeBuy = nodeBuy,
                nodeSell = nodeSell,
                money = money,
                authority = new[] { Mathf.Clamp(ValueManager.Instance.banbal, 0, 1), Mathf.Clamp(1 - ValueManager.Instance.authority, 0, 1) },
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

        public TimeLeap GetTimeLeap(int time) => timeLeapData[time];

        public int TimeLeapLength() => timeLeapData.Count;

        public void ForEachTimeLeap(Action<TimeLeap, DateTime> action)
        {
            foreach (var timeLeap in timeLeapData) action(timeLeap, TimeManager.Instance.startDate.AddDays(timeLeap.date));
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