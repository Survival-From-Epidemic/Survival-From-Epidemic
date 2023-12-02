using System.Collections;
using _root.Scripts.Game;
using DG.Tweening;
using Pathfinding;
using UnityEngine;

namespace _root.Scripts.Player
{
    public class Person : MonoBehaviour
    {
        [SerializeField] private AIPath aiPath;
        [SerializeField] private AIDestinationSetter aiDestinationSetter;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] public bool infected;
        public PersonData personData;
        private Coroutine _coroutine;
        private bool _isolation;

        private void Awake()
        {
            PathManager.Instance.AddPerson(this);
            infected = false;
            aiPath = GetComponent<AIPath>();
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            _coroutine = StartCoroutine(NextPath());
        }

        public void Infected(PersonData data)
        {
            infected = true;
            meshRenderer.materials[0].DOColor(Color.red, 3f);
            personData = data;

            if (LocalDataManager.Instance.IsBought("학생 격리 1")) Isolation();
        }

        public void UnInfected()
        {
            infected = false;
            meshRenderer.materials[0].DOColor(Color.white, 3f);

            Show();
            personData = null;
            _isolation = false;
        }

        public void GoBack()
        {
            if (_isolation) return;
            if (_coroutine != null) StopCoroutine(_coroutine);
            aiPath.maxSpeed = 12;
            StartCoroutine(_GoBack());
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (_coroutine != null) StopCoroutine(_coroutine);
            aiPath.maxSpeed = 8;
            transform.position = PathManager.Instance.GetBackPosition();
            _coroutine = StartCoroutine(NextPath());
        }

        public void Isolation()
        {
            _isolation = true;
            aiPath.maxSpeed = 18;
            StartCoroutine(_Isolation());
        }

        private IEnumerator _Isolation()
        {
            aiPath.destination = PathManager.Instance.GetIsolationPosition();
            yield return new WaitUntil(() => aiPath.reachedDestination);
            gameObject.SetActive(false);
        }

        private IEnumerator _GoBack()
        {
            aiPath.destination = PathManager.Instance.GetBackPosition();
            yield return new WaitUntil(() => aiPath.reachedDestination);
            gameObject.SetActive(false);
        }

        private IEnumerator NextPath()
        {
            while (true)
            {
                aiPath.destination = PathManager.Instance.GetRandomPosition();
                yield return new WaitUntil(() => aiPath.reachedDestination);
                yield return new WaitForSeconds(5f);
            }
        }
    }
}