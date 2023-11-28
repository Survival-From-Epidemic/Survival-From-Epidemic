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
            StartCoroutine(NextPath());
            var mat = meshRenderer.materials[0];
            var color = mat.color;
            color.a = 0;
            mat.color = color;
            mat.DOFade(1, 2f);
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

        private IEnumerator NextPath()
        {
            while (true)
            {
                aiPath.destination = PathManager.Instance.GetRandomPosition();
                yield return new WaitUntil(() => aiPath.reachedDestination);
                aiDestinationSetter.target = null;
                yield return new WaitForSeconds(5f);
            }
        }
    }
}