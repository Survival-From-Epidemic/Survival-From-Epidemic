using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Player
{
    public class PathManager : SingleMono<PathManager>
    {
        [SerializeField] private GameObject personPrefab;
        [SerializeField] private int testPersons = 50;
        [SerializeField] private Vector3 backPosition;
        [SerializeField] private Vector2 backPositionSize;
        [SerializeField] private Vector3 isolationPosition;
        [SerializeField] private Vector2 isolationPositionSize;

        // private List<AIAnchor> _anchors;
        private List<AIAnchor> _anchorPosition;
        private Coroutine _cycle;

        private bool _goBack;
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
        }

        private IEnumerator Cycle()
        {
            while (true)
            {
                if (!_goBack)
                {
                    yield return new WaitForSeconds(45);
                    foreach (var p in _persons)
                        p.GoBack();
                }
                else
                {
                    yield return new WaitForSeconds(15);
                    foreach (var p in _persons)
                    {
                        yield return new WaitForSeconds(0.02f);
                        p.Show();
                    }
                }

                _goBack = !_goBack;
            }
        }

        public void Clear()
        {
            foreach(var person in _persons.Where(p => p.infected)) person.Isolation();
        }

        public void Modify(Game.Person before, Game.Person after, List<PersonData> personData)
        {
            var changed = after.infectedPerson - before.infectedPerson;

            if (changed > 0)
            {
                var infectedObject = _persons.Where(p => p.infected).ToList();
                for (var i = 0; i < changed; i++)
                {
                    if (infectedObject.Count <= 0)
                    {
                        var p = _persons[Random.Range(0, _persons.Count)];
                        personData[i].personObject = p;
                        p.Infected(personData[i]);
                        infectedObject.Add(p);
                        continue;
                    }

                    var target = infectedObject[Random.Range(0, infectedObject.Count)];
                    var targetPosition = target.transform.position;
                    var targetPerson = _persons.Where(p => !p.infected)
                        .OrderBy(p => (p.transform.position - targetPosition).sqrMagnitude)
                        .First();
                    personData[i].personObject = targetPerson;
                    targetPerson.Infected(personData[i]);
                    infectedObject.Add(targetPerson);
                }
            }
            //
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
                                            + new Vector3(backPositionSize.x * (Random.value - 0.5f), 0.1f, backPositionSize.y * (Random.value - 0.5f));

        public Vector3 GetIsolationPosition() => isolationPosition
                                                 + new Vector3(isolationPositionSize.x * (Random.value - 0.5f), 0.1f, isolationPositionSize.y * (Random.value - 0.5f));

        public Vector3 GetRandomPosition() => _anchorPosition[Random.Range(0, _anchorPosition.Count)].GetPosition();
    }
}