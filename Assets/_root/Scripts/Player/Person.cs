using System.Collections;
using _root.Scripts.Game;
using Pathfinding;
using UnityEngine;

namespace _root.Scripts.Player
{
    public class Person : MonoBehaviour
    {
        [SerializeField] private AIPath aiPath;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] public bool inNurse;
        [SerializeField] public bool outOfControl;
        public PersonData personData;
        public bool allocatedPersonData;
        private Coroutine _nextPathCoroutine;
        private float _speedMultiply;

        private void Awake()
        {
            PathManager.Instance.AddPerson(this);
            aiPath = GetComponent<AIPath>();
            meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            StartNextPath();
        }

        private void Update()
        {
            // if (aiPath.canSearch && !aiPath.hasPath)
            // {
            //     StopAllCoroutines();
            //     OutDormitory();
            // }
        }

        public void PreInfected(PersonData data)
        {
            // Debugger.Log("============ Person : PreInfected");
            allocatedPersonData = true;
            personData = data;

            if (LocalDataManager.Instance.IsBought("의심 학생 격리 1"))
            {
                InfectCheck();
            }

            // if (LocalDataManager.Instance.IsBought("학생 격리 1")) Isolation();
        }

        public void InfectCheck()
        {
            // Debugger.Log("============ Person : InfectCheck");
            gameObject.SetActive(true);
            StartCoroutine(_NurseOffice());
            // if (LocalDataManager.Instance.IsBought("학생 격리 1")) Isolation();
        }

        private IEnumerator _NurseOffice()
        {
            // Debugger.Log("============ Person : NurseOffice Coroutine");
            inNurse = true;
            meshRenderer.material.color = Color.yellow;
            StopNextPath();
            SetSpeed(1f);
            aiPath.destination = PathManager.Instance.GetNurseOfficePosition();
            yield return new WaitUntil(() => aiPath.reachedDestination || !allocatedPersonData);
            inNurse = false;
        }

        public void NurseOut()
        {
            // Debugger.Log("============ Person : NurseOut");
            inNurse = false;
            if (!meshRenderer) return;
            meshRenderer.material.color = Color.red;
            StopAllCoroutines();
            if (LocalDataManager.Instance.IsBought("학생 격리 1"))
            {
                Isolation();
            }
            else
            {
                StartNextPath();
            }
        }

        public void Isolation()
        {
            // Debugger.Log("============ Person : Isolation");
            StopAllCoroutines();
            gameObject.SetActive(true);
            StartCoroutine(_Isolation());
        }

        private IEnumerator _Isolation()
        {
            outOfControl = true;
            StopNextPath();
            meshRenderer.material.color = Color.red;
            SetSpeed(2.5f);
            aiPath.destination = PathManager.Instance.GetIsolationPosition();
            yield return new WaitUntil(() => aiPath.reachedDestination);
            gameObject.SetActive(false);
        }

        public void UnInfected()
        {
            StopAllCoroutines();
            // Debugger.Log("============ Person : UnInfected");
            meshRenderer.material.color = Color.white;
            allocatedPersonData = true;
            outOfControl = false;
            OutDormitory();
        }

        public void Death()
        {
            // PathManager.Instance.Death(this);
            Destroy(gameObject);
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
            // Debugger.Log("============ Person : EnterDormitory");
            if (outOfControl || inNurse) return;
            StopNextPath();
            gameObject.SetActive(true);
            StartCoroutine(_EnterDormitory());
        }

        public void OutDormitory()
        {
            // Debugger.Log("============ Person : OutDormitory");
            if (outOfControl || inNurse) return;
            gameObject.SetActive(true);
            aiPath.Teleport(PathManager.Instance.GetBackPosition() + Vector3.up);
            // transform.position = PathManager.Instance.GetBackPosition() + Vector3.up;
            StartNextPath();
        }

        private void StopNextPath()
        {
            if (_nextPathCoroutine != null) StopCoroutine(_nextPathCoroutine);
        }

        private void StartNextPath()
        {
            if (outOfControl || inNurse) return;
            if (PathManager.Instance.dormIn) return;
            gameObject.SetActive(true);
            StopNextPath();
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
            while (true)
            {
                SetSpeed(1);
                aiPath.destination = PathManager.Instance.GetRandomPosition();
                yield return new WaitUntil(() => aiPath.reachedDestination);
                yield return new WaitForSeconds(8f);
            }
        }

        public void UpdateSpeed() => aiPath.maxSpeed = 4 * _speedMultiply / (TimeManager.Instance.timeScale / 1.5f);

        public void SetSpeed(float multi)
        {
            _speedMultiply = multi;
            UpdateSpeed();
        }
    }
}