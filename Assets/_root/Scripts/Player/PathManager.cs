using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using DG.Tweening;
using UnityEngine;

namespace _root.Scripts.Player
{
    public class PathManager : SingleMono<PathManager>
    {
        [SerializeField] private GameObject personPrefab;
        [SerializeField] private int testPersons = 50;
        [SerializeField] private Vector3 backPosition;
        [SerializeField] private Vector2 backPositionSize;

        // private List<AIAnchor> _anchors;
        private List<AIAnchor> _anchorPosition;
        private Coroutine _cycle;

        private bool _goBack;
        private List<Person> _persons;

        protected override void Awake()
        {
            base.Awake();
            DOTween.SetTweensCapacity(2000, 50);
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
                    yield return new WaitForSeconds(40);
                    foreach (var p in _persons)
                        p.GoBack();
                }
                else
                {
                    yield return new WaitForSeconds(15);
                    foreach (var p in _persons)
                        p.Show();
                }

                _goBack = !_goBack;
            }
        }

        public void Modify()
        {
            var person = ValueManager.Instance.person;
            var personObjectCount = _persons.Count;
            var infectedObject = _persons.Where(p => p.infected).ToList();

            var delta = (float)person.infectedPerson / person.deathPerson;
            var need = Mathf.FloorToInt(personObjectCount * delta) - infectedObject.Count;

            if (person.infectedPerson <= 0)
                foreach (var p in infectedObject)
                    p.UnInfected();
            else if (need > 0)
            {
                for (var i = 0; i < need; i++)
                {
                    if (infectedObject.Count <= 0)
                    {
                        _persons[Random.Range(0, _persons.Count)].Infected();
                        continue;
                    }

                    var target = infectedObject[Random.Range(0, infectedObject.Count)];
                    var targetPosition = target.transform.position;
                    _persons.Where(p => !p.infected)
                        .OrderBy(p => (p.transform.position - targetPosition).sqrMagnitude)
                        .First()
                        .Infected();
                }
            }
            else
                for (var i = Mathf.Abs(need); i > 0; i--)
                    infectedObject[^1].UnInfected();
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

        public Vector3 GetBackPosition() => backPosition + new Vector3(backPositionSize.x * (Random.value - 0.5f), 0.1f, backPositionSize.y * (Random.value - 0.5f));

        public Vector3 GetRandomPosition() => _anchorPosition[Random.Range(0, _anchorPosition.Count)].GetPosition();
    }
}