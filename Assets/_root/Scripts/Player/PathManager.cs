using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _root.Scripts.Player
{
    public class PathManager : SingleMono<PathManager>
    {
        [SerializeField] private GameObject personPrefab;
        [SerializeField] private int testPersons = 50;
        [SerializeField] private AIAnchor nurseOffice;
        [SerializeField] private Vector3 backPosition;
        [SerializeField] private Vector2 backPositionSize;
        [SerializeField] private Vector3 isolationPosition;
        [SerializeField] private Vector2 isolationPositionSize;

        public bool dormitory;
        public bool dormIn;

        // private List<AIAnchor> _anchors;
        private List<AIAnchor> _anchorPosition;
        private Coroutine _cycle;
        private List<Person> _persons;

        protected override void Awake()
        {
            base.Awake();
            // _anchors = new List<AIAnchor>();
            _persons = new List<Person>();
            _anchorPosition = new List<AIAnchor>();
        }

        private void Start()
        {
            var personTotalPerson = ValueManager.Instance == null ? testPersons : ValueManager.Instance.person.totalPerson;
            for (var i = 0; i < personTotalPerson; i++)
            {
                Instantiate(personPrefab, GetRandomPosition(), Quaternion.identity, transform);
            }

            _cycle = StartCoroutine(Cycle());
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(backPosition, "flag.png", false);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(backPosition, new Vector3(backPositionSize.x, 0, backPositionSize.y));
            Gizmos.DrawIcon(isolationPosition, "flag 1.png", false);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(isolationPosition, new Vector3(isolationPositionSize.x, 0, isolationPositionSize.y));
        }

        private IEnumerator Cycle()
        {
            while (true)
            {
                if (!dormitory)
                {
                    yield return new WaitForSeconds(45);
                    dormIn = true;
                    ForeachPerson(p => p.EnterDormitory());
                }
                else
                {
                    yield return new WaitForSeconds(15);
                    dormIn = false;
                    ForeachPersonDelay(p => p.OutDormitory(), 0.02f);
                }

                _persons.RemoveAll(p => p == null);

                dormitory = !dormitory;
            }
        }

        private void ForeachPerson(Action<Person> person)
        {
            for (var i = _persons.Count - 1; i >= 0; i--)
            {
                if (_persons[i] == null) _persons.RemoveAt(i);
                person(_persons[i]);
            }
        }

        private void ForeachPersonDelay(Action<Person> person, float delay)
        {
            StartCoroutine(_ForeachPersonDelay(person, delay));
        }

        private IEnumerator _ForeachPersonDelay(Action<Person> person, float delay)
        {
            for (var i = _persons.Count - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(delay);
                try
                {
                    if (_persons[i] == null) _persons.RemoveAt(i);
                    person(_persons[i]);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
        }

        private void ForeachWherePerson(Func<Person, bool> func, Action<Person> person)
        {
            var persons = _persons.Where(func).ToList();
            for (var i = persons.Count - 1; i >= 0; i--)
            {
                if (persons[i] == null) persons.RemoveAt(i);
                person(persons[i]);
            }
        }

        // public void Death(Person person)
        // {
        //     // _persons.RemoveAt(_persons.FindIndex(p => p.GetInstanceID() == person.GetInstanceID()));
        // }

        public void UpdateSpeed()
        {
            ForeachPerson(person => person.UpdateSpeed());
        }

        public void Clear()
        {
            ForeachWherePerson(p => p.allocatedPersonData && !p.outOfControl && !p.inNurse, person => person.Isolation());
        }

        public void Isolation()
        {
            ForeachWherePerson(p => p.allocatedPersonData && !p.outOfControl, person => person.InfectCheck());
        }

        public void Modify(PersonData personData)
        {
            // var infectedObject = _persons.Where(p => p.allocatedPersonData && !p.outOfControl && !p.inNurse).ToList();
            // Person targetPerson;
            // if (infectedObject.Count <= 0)
            // {
            //     targetPerson = _persons[Random.Range(0, _persons.Count)];
            // }
            // else
            // {
            //     var target = infectedObject[Random.Range(0, infectedObject.Count)];
            //     var pos = target.transform.position;
            //     targetPerson = _persons.Where(p => !p.allocatedPersonData)
            //         .OrderBy(p => (p.transform.position - pos).sqrMagnitude)
            //         .First();
            // }
            var targetPerson = _persons[Random.Range(0, _persons.Count)];

            personData.personObject = targetPerson;
            targetPerson.PreInfected(personData);
        }

        public void Modify(List<PersonData> personDataList)
        {
            if (personDataList.Count <= 0) return;
            Debugger.Log("PathManager - Modified");
            foreach (var personData in personDataList) Modify(personData);


            // var changed = after.infectedPerson - before.infectedPerson;
            //
            // if (changed > 0)
            // {
            //     var infectedObject = _persons.Where(p => p.personData != null).ToList();
            //     for (var i = 0; i < changed; i++)
            //     {
            //         if (infectedObject.Count <= 0)
            //         {
            //             var p = _persons[Random.Range(0, _persons.Count)];
            //             personDataList[i].personObject = p;
            //             p.PreInfected(personDataList[i]);
            //             infectedObject.Add(p);
            //             continue;
            //         }
            //
            //         var target = infectedObject[Random.Range(0, infectedObject.Count)];
            //         var targetPosition = target.transform.position;
            //         var targetPerson = _persons.Where(p => p.personData == null)
            //             .OrderBy(p => (p.transform.position - targetPosition).sqrMagnitude)
            //             .First();
            //         personDataList[i].personObject = targetPerson;
            //         targetPerson.PreInfected(personDataList[i]);
            //         infectedObject.Add(targetPerson);
            //     }
            // }

            // var person = ValueManager.Instance.person;
            // var personObjectCount = _persons.Count;
            //
            // var delta = (float)person.infectedPerson / person.deathPerson;
            // var need = Mathf.FloorToInt(personObjectCount * delta) - infectedObject.Count;
            //
            // if (person.infectedPerson <= 0)
            //     foreach (var p in infectedObject)
            //         p.UnInfected();
            // else if (need > 0)
            // {
            //     for (var i = 0; i < need; i++)
            //     {
            //         if (infectedObject.Count <= 0)
            //         {
            //             _persons[Random.Range(0, _persons.Count)].Infected();
            //             continue;
            //         }
            //
            //         var target = infectedObject[Random.Range(0, infectedObject.Count)];
            //         var targetPosition = target.transform.position;
            //         _persons.Where(p => !p.infected)
            //             .OrderBy(p => (p.transform.position - targetPosition).sqrMagnitude)
            //             .First()
            //             .Infected();
            //     }
            // }
            // else
            //     for (var i = Mathf.Abs(need); i > 0; i--)
            //         infectedObject[^1].UnInfected();
        }

        public void AddPerson(Person person)
        {
            _persons.Add(person);
        }

        public void AddAnchor(AIAnchor anchor)
        {
            // _anchors.Add(anchor);
            _anchorPosition.Add(anchor);
        }

        public Vector3 GetBackPosition() => backPosition
                                            + new Vector3(backPositionSize.x * (Random.value - 0.5f), 0f, backPositionSize.y * (Random.value - 0.5f));

        public Vector3 GetNurseOfficePosition() => nurseOffice.GetPosition();

        public Vector3 GetIsolationPosition() => isolationPosition
                                                 + new Vector3(isolationPositionSize.x * (Random.value - 0.5f), 0f, isolationPositionSize.y * (Random.value - 0.5f));

        public Vector3 GetRandomPosition() => _anchorPosition[Random.Range(0, _anchorPosition.Count)].GetPosition();
    }
}