using UnityEngine;

namespace _root.Scripts.SingleTon
{
    public class SingleMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        [SerializeField] private bool canBeDestroy;

        public static T Instance => _instance ? _instance : _instance = FindObjectOfType<T>();

        protected virtual void Awake()
        {
            _instance = this as T;
            if (!canBeDestroy) DontDestroyOnLoad(gameObject);
        }
    }
}