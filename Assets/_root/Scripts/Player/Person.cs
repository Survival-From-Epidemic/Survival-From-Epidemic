using System.Collections;
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
        private Coroutine _coroutine;

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

        public void Infected()
        {
            infected = true;
            meshRenderer.materials[0].DOColor(Color.red, 3f);
        }

        public void UnInfected()
        {
            infected = false;
            meshRenderer.materials[0].DOColor(Color.white, 3f);
        }

        public void GoBack()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            aiPath.maxSpeed = 12;
            StartCoroutine(_GoBack());
        }

        public void Show()
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            aiPath.maxSpeed = 8;
            transform.position = PathManager.Instance.GetBackPosition();
            _coroutine = StartCoroutine(NextPath());
        }

        private IEnumerator _GoBack()
        {
            aiPath.destination = PathManager.Instance.GetBackPosition();
            yield return new WaitUntil(() => aiPath.reachedDestination);
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