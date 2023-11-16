using System;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Attribute;
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

        [Space] [SerializeField] public Disease disease;

        [Space] [SerializeField] public Person person;

        [ReadOnly] [SerializeField] private List<PersonData> persons;
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
                < 60 => Weak,
                < 85 => Normal,
                < 95 => Strong,
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
            if (!diseaseEnabled) return;

            var now = TimeManager.Instance.date;

            disease.infectPower = GetInfectPower();
            if (person.healthyPerson > 0)
            {
                if ((disease.infectWeight * disease.infectPower).Chance())
                {
                    var count = Mathf.FloorToInt(Mathf.Min(disease.infectivity, person.healthyPerson));
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

            foreach (var p in personsSet)
            {
                if (pcrEnabled)
                    if (p.catchDate + Mathf.FloorToInt(p.symptomType.SymptomPcrDate() * Random.Range(1f, 2f)) <= now)
                        p.isInfected = true;

                if (kitEnabled && kitChance.Chance())
                    if (p.catchDate + Mathf.FloorToInt(p.symptomType.SymptomPcrDate() * Random.Range(0.5f, 1.5f)) <= now)
                        p.isInfected = true;

                p.recoverWeight += Random.Range(0.5f, 1f);
                switch (p.symptomType)
                {
                    case Nothing:
                        break;
                    case Weak:
                        p.deathWeight += Random.Range(0.1f, 0.5f);
                        break;
                    case Normal:
                        p.deathWeight += Random.Range(0.4f, 0.8f);
                        break;
                    case Strong:
                        p.deathWeight += Random.Range(0.8f, 2f);
                        break;
                    case Emergency:
                        p.deathWeight += Random.Range(1.5f, 4f);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (vaccineEnded) p.recoverWeight += Random.Range(10, 25);

                // if (p.symptomType is not Emergency && p.deathWeight >= 50)
                // {
                //     p.recoverWeight = 30;
                //     p.deathWeight = 0;
                //     p.symptomType++;
                // }
                //
                // if (p.symptomType is Emergency && p.recoverWeight >= 80)
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
            person.healthyPerson = person.totalPerson - person.deathPerson - person.infectedPerson;
        }
    }
}