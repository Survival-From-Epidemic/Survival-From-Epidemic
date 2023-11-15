using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Attribute;
using _root.Scripts.SingleTon;
using _root.Scripts.Utils;
using UnityEngine;
using static _root.Scripts.Game.SymptomType;

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
        private HashSet<PersonData> _persons;

        private void Start()
        {
            _persons = new HashSet<PersonData>();
        }

        private static SymptomType GenerateSymptomType()
        {
            return Random.Range(0f, 100f) switch
            {
                < 5 => Nothing,
                < 60 => Weak,
                < 85 => Normal,
                < 95 => Strong,
                _ => Emergency
            };
        }

        private float GetInfectPower()
        {
            var rate = (float)_persons.Count / person.totalPerson;
            return 1 + rate * rate * 25;
        }

        public void Cycle()
        {
            if (!diseaseEnabled) return;

            var now = TimeManager.Instance.date;

            if (person.healthyPerson > 0)
                if (disease.infectWeight.Chance())
                {
                    disease.infectPower = GetInfectPower();
                    var count = Mathf.FloorToInt(Mathf.Min(disease.infectivity, person.healthyPerson) * disease.infectPower);
                    person.healthyPerson -= count;
                    _persons.Add(new PersonData
                    {
                        catchDate = now,
                        deathWeight = 0,
                        isInfected = false,
                        recoverWeight = 0,
                        symptomType = GenerateSymptomType()
                    });
                }

            foreach (var p in _persons)
            {
                if (pcrEnabled)
                    if (p.catchDate + Mathf.FloorToInt(p.symptomType.SymptomPcrDate() * Random.Range(1f, 2f)) <= now)
                        p.isInfected = true;

                if (kitEnabled && kitChance.Chance())
                    if (p.catchDate + Mathf.FloorToInt(p.symptomType.SymptomPcrDate() * Random.Range(0.5f, 1.5f)) <= now)
                        p.isInfected = true;

                switch (p.symptomType)
                {
                    case Nothing:
                        p.recoverWeight += Random.Range(3f, 10f);
                        break;
                    case Weak:
                        p.recoverWeight += Random.Range(3f, 10f);
                        p.deathWeight += Random.Range(0.5f, 2f);
                        break;
                    case Normal:
                        p.recoverWeight += Random.Range(2f, 6f);
                        p.deathWeight += Random.Range(1f, 4f);
                        break;
                    case Strong:
                        p.recoverWeight += Random.Range(1f, 4f);
                        p.deathWeight += Random.Range(2f, 6f);
                        break;
                    case Emergency:
                        p.recoverWeight += Random.Range(0.5f, 2f);
                        p.deathWeight += Random.Range(3f, 10f);
                        break;
                }

                if (vaccineEnded) p.recoverWeight += Random.Range(30, 60);

                if (p.symptomType is not Emergency && p.deathWeight >= 50)
                {
                    p.recoverWeight = 40;
                    p.deathWeight = 40;
                    p.symptomType++;
                }

                if (p.symptomType is Emergency && p.recoverWeight >= 80)
                {
                    p.recoverWeight = 40;
                    p.deathWeight = 40;
                    p.symptomType--;
                }
            }

            person.deathPerson += _persons.RemoveWhere(p => p.deathWeight >= 100);
            person.infectedPerson = _persons.Count(p => p.isInfected);
            _persons.RemoveWhere(p => p.recoverWeight >= 100);

            if (Debugger.IsDebug()) persons = _persons.ToList();
            else if (persons.Count > 0) persons = new List<PersonData>();
            person.healthyPerson = person.totalPerson - person.deathPerson - person.infectedPerson;
        }
    }
}