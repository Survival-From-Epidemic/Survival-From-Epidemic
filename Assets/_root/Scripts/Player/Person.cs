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
        [SerializeField] public bool outOfControl;
        public PersonData personData;
        private Coroutine _nextPathCoroutine;
        private float _speedMultiply;

        private void Awake()
        {
            PathManager.Instance.AddPerson(this);
            aiPath = GetComponent<AIPath>();
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            StartNextPath();
        }

        public void PreInfected(PersonData data)
        {
            personData = data;

            if (LocalDataManager.Instance.IsBought("의심 학생 격리 1"))
            {
                meshRenderer.materials[0].DOColor(Color.yellow, 2f);
                InfectCheck();
            }

            // if (LocalDataManager.Instance.IsBought("학생 격리 1")) Isolation();
        }

        public void InfectCheck()
        {
            StartCoroutine(_NurseOffice());
            // if (LocalDataManager.Instance.IsBought("학생 격리 1")) Isolation();
        }

        private IEnumerator _NurseOffice()
        {
            outOfControl = true;
            meshRenderer.materials[0].DOColor(Color.yellow, 2f);
            StopNextPath();
            SetSpeed(1f);
            aiPath.destination = PathManager.Instance.GetNurseOfficePosition();
            yield return new WaitUntil(() => aiPath.reachedDestination);

            yield return new WaitForSeconds(Random.Range(5f, 12f));
            if (LocalDataManager.Instance.IsBought("학생 격리 1"))
            {
                Isolation();
            }
            else
            {
                outOfControl = false;
                StartNextPath();
            }
        }

        public void Isolation()
        {
            StartCoroutine(_Isolation());
        }

        private IEnumerator _Isolation()
        {
            outOfControl = true;
            StopNextPath();
            meshRenderer.materials[0].DOColor(Color.red, 2f);
            SetSpeed(2.5f);
            aiPath.destination = PathManager.Instance.GetIsolationPosition();
            yield return new WaitUntil(() => aiPath.reachedDestination);
            gameObject.SetActive(false);
        }

        public void UnInfected()
        {
            personData = null;
            outOfControl = false;
            OutDormitory();
        }

        // public void UnInfected()
        // {
        //     infected = false;
        //     meshRenderer.materials[0].DOColor(Color.white, 2f);
        //     Show();
        //     personData = null;
        //     _isolation = false;
        // }

        public void EnterDormitory()
        {
            if (outOfControl) return;
            StopNextPath();
            StartCoroutine(_EnterDormitory());
        }

        public void OutDormitory()
        {
            if (outOfControl) return;
            transform.position = PathManager.Instance.GetBackPosition();
            StartNextPath();
        }

        private void StopNextPath()
        {
            if (_nextPathCoroutine != null) StopCoroutine(_nextPathCoroutine);
        }

        private void StartNextPath()
        {
            if (outOfControl) return;
            if (PathManager.Instance.dormitory)
            {
                EnterDormitory();
                return;
            }

            _nextPathCoroutine = StartCoroutine(NextPath());
        }

        private IEnumerator _EnterDormitory()
        {
            SetSpeed(1.5f);
            aiPath.destination = PathManager.Instance.GetBackPosition();
            yield return new WaitUntil(() => aiPath.reachedDestination);
            gameObject.SetActive(false);
        }

        private IEnumerator NextPath()
        {
            gameObject.SetActive(true);
            while (true)
            {
                SetSpeed(1);
                aiPath.destination = PathManager.Instance.GetRandomPosition();
                yield return new WaitUntil(() => aiPath.reachedDestination);
                yield return new WaitForSeconds(5f);
            }
        }

        public void UpdateSpeed() => aiPath.maxSpeed = 8 * _speedMultiply * TimeManager.Instance.timeScale;

        public void SetSpeed(float multi)
        {
            _speedMultiply = multi;
            aiPath.maxSpeed = 8 * _speedMultiply * TimeManager.Instance.timeScale;
        }
    }
}