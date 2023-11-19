﻿using System;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Attribute;
using _root.Scripts.Managers;
using _root.Scripts.Managers.UI;
using _root.Scripts.SingleTon;
using _root.Scripts.Utils;
using UnityEngine;
using static _root.Scripts.Game.SymptomType;
using Random = UnityEngine.Random;

namespace _root.Scripts.Game
{
    public class ValueManager : SingleMono<ValueManager>
    {
        [SerializeField] public bool diseaseEnabled;
        [SerializeField] public bool pcrEnabled;
        [SerializeField] public bool kitEnabled;
        [SerializeField] public int kitChance;
        [SerializeField] public bool vaccineResearch;
        [SerializeField] public bool vaccineEnded;

        [Space] public LocalDataManager.LocalGridData localGridData;

        [Space] [SerializeField] public Disease preDisease;
        [SerializeField] public Disease disease;

        [Space] [SerializeField] public Person person;

        [ReadOnly] [SerializeField] private List<PersonData> persons;

        [SerializeField] public float banbal;
        [SerializeField] public float authority;
        [SerializeField] public int currentBanbal;
        [SerializeField] public int banbalDate;
        [SerializeField] public int authorityDate;

        [SerializeField] public int authorityGoodDate;

        [SerializeField] public float currentAuthority;
        public HashSet<PersonData> personsSet;

        private void Start()
        {
            personsSet = new HashSet<PersonData>();
        }

        private static SymptomType GenerateSymptomType()
        {
            return Random.Range(0f + TimeManager.Instance.ModificationWeight() * 8, 100f) switch
            {
                < 20 => Nothing,
                < 50 => Weak,
                < 80 => Normal,
                < 92.5f => Strong,
                _ => Emergency
            };
        }

        private float GetInfectPower()
        {
            var rate = (float)personsSet.Count / person.healthyPerson;
            return 1 + rate * rate * 5;
        }

        public void Cycle()
        {
            var gridAuthority = localGridData.gridAuthority;
            currentBanbal = gridAuthority.concentration + gridAuthority.mask + gridAuthority.annoy + gridAuthority.study;
            if (gridAuthority.concentration >= 10) NewsManager.Instance.ShowNews(28);
            if (currentBanbal >= 80)
            {
                if (banbalDate >= 3) NewsManager.Instance.ShowNews(27);
                if (banbalDate >= 7)
                {
                    GameManager.Instance.GameEnd(GameEndType.Banbal);
                }

                banbalDate++;
            }
            else
            {
                banbalDate = 0;
            }

            if ((float)person.deathPerson / person.totalPerson >= 0.15) NewsManager.Instance.ShowNews(25);

            currentAuthority = (person.deathPerson * 4 + person.infectedPerson / 1.33f) / person.totalPerson;
            switch (currentAuthority)
            {
                case >= 1:
                    authorityGoodDate = 0;
                    GameManager.Instance.GameEnd(GameEndType.Authority);
                    break;
                case >= 0.75f:
                    authorityGoodDate = 0;
                    NewsManager.Instance.ShowNews(26);
                    break;
                case <= 0.2f:
                    authorityGoodDate++;
                    if (TimeManager.Instance.today >= TimeManager.Instance.pcrDate && authorityGoodDate >= 30) NewsManager.Instance.ShowNews(29);
                    break;
                default:
                    authorityGoodDate = 0;
                    break;
            }

            banbal = Mathf.Lerp(banbal, currentBanbal / 80f, 0.1f);
            authority = Mathf.Lerp(authority, currentAuthority, 0.1f);

            disease.infectPower = GetInfectPower();
            if (diseaseEnabled)
            {
                var now = TimeManager.Instance.date;
                var gridDisease = localGridData.gridDisease;
                preDisease = new Disease
                {
                    infectivity = disease.infectivity - Mathf.FloorToInt(gridDisease.infectivity * (person.totalPerson / 750f)),
                    infectPower = disease.infectPower - gridDisease.infectPower,
                    infectWeight = disease.infectWeight - gridDisease.infectWeight
                };

                var total = person.totalPerson - personsSet.Count - person.deathPerson;
                var chance = preDisease.infectWeight * preDisease.infectPower;
                if (total > 0 && chance > 0)
                {
                    if (chance.Chance())
                    {
                        var count = Mathf.Max(Mathf.FloorToInt(Mathf.Min(preDisease.infectivity, total)), 0);
                        person.healthyPerson -= count;
                        for (var i = 0; i < count; i++)
                        {
                            personsSet.Add(new PersonData
                            {
                                catchDate = now,
                                deathWeight = 0,
                                isInfected = false,
                                recoverWeight = 0,
                                symptomType = GenerateSymptomType()
                            });
                        }
                    }
                }

                var mad1 = LocalDataManager.Instance.IsBought("의료 지원 1");
                var mad2 = LocalDataManager.Instance.IsBought("의료 지원 2");
                var mad3 = LocalDataManager.Instance.IsBought("의료 지원 3");
                var mad4 = LocalDataManager.Instance.IsBought("의료 지원 4");
                var kit1 = LocalDataManager.Instance.IsBought("자가 진단 키트 지원 1");
                var kit2 = LocalDataManager.Instance.IsBought("자가 진단 키트 지원 2");

                var infected = personsSet.Where(v => v.isInfected).ToList();
                if (mad4) mad4 = MoneyManager.Instance.RemoveMoney(infected.Count * 35);
                if (!mad4 && mad3) mad3 = MoneyManager.Instance.RemoveMoney(infected.Count * 20);
                if (!mad4 && !mad3)
                {
                    if (mad2) mad2 = MoneyManager.Instance.RemoveMoney(infected.Count(v => v.symptomType is Emergency or Strong or Normal) * 20);
                    if (!mad2 && mad1) mad1 = MoneyManager.Instance.RemoveMoney(infected.Count(v => v.symptomType is Emergency) * 20);
                }

                foreach (var p in personsSet)
                {
                    if (pcrEnabled)
                        if (p.catchDate + Mathf.FloorToInt(p.symptomType.SymptomPcrDate() * Random.Range(1f, 2f)) <= now)
                            p.isInfected = true;

                    if (kitEnabled && kitChance.Chance())
                        if (p.catchDate + Mathf.FloorToInt(p.symptomType.SymptomPcrDate() * Random.Range(0.5f, 1f) * (kit2 ? 0.33f : kit1 ? 0.7f : 1)) <= now)
                            p.isInfected = true;

                    p.recoverWeight += Random.Range(0.5f, 1f) + (mad4 ? Random.Range(0.2f, 0.6f) : 0);
                    switch (p.symptomType)
                    {
                        case Nothing:
                            break;
                        case Weak:
                            p.recoverWeight += mad3 ? Random.Range(0.1f, 0.3f) : 0;
                            p.deathWeight += Random.Range(0.2f, 0.5f) * (mad4 ? 0.3f : mad3 ? 0.4f : 1);
                            break;
                        case Normal:
                            p.recoverWeight += mad2 ? Random.Range(0.1f, 0.3f) : 0;
                            p.deathWeight += Random.Range(0.5f, 1f) * (mad4 ? 0.3f : mad2 ? 0.4f : 1);
                            break;
                        case Strong:
                            p.recoverWeight += mad2 ? Random.Range(0.1f, 0.3f) : 0;
                            p.deathWeight += Random.Range(1.5f, 3f) * (mad4 ? 0.15f : mad2 ? 0.25f : 1);
                            break;
                        case Emergency:
                            p.recoverWeight += mad1 ? Random.Range(0.1f, 0.3f) : 0;
                            p.deathWeight += Random.Range(2f, 6f) * (mad4 ? 0.15f : mad1 ? 0.25f : 1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (vaccineEnded) p.recoverWeight += Random.Range(0, 25f);

                    // if (p.symptomType is not Emergency && p.deathWeight >= 60)
                    // {
                    //     p.recoverWeight = 40;
                    //     p.deathWeight = 0;
                    //     p.symptomType++;
                    // }
                    //
                    // if (p.symptomType is Emergency && p.recoverWeight >= 60)
                    // {
                    //     p.recoverWeight = 10;
                    //     p.deathWeight = 10;
                    //     p.symptomType--;
                    // }
                }

                person.deathPerson += personsSet.RemoveWhere(p => p.deathWeight >= 100);
                person.infectedPerson = personsSet.Count(p => p.isInfected);
                personsSet.RemoveWhere(p => p.recoverWeight >= 100);

                if (Debugger.IsDebug()) persons = personsSet.ToList();
                else if (persons.Count > 0) persons = new List<PersonData>();
            }

            person.healthyPerson = person.totalPerson - person.deathPerson - person.infectedPerson;
        }
    }
}