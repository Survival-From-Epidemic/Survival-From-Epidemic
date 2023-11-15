using UnityEngine;

namespace _root.Scripts.SingleTon
{
    public class SingleMonoComponent<T, T2> : MonoBehaviour where T : MonoBehaviour where T2 : Component
    {
        private static T _instance;

        private static T2 _component;

        [SerializeField] private bool canBeDestroy;

        public static T Instance => _instance ? _instance : _instance = FindObjectOfType<T>();

        public static T2 Component => _component ? _component : _component = FindObjectOfType<T>().GetComponent<T2>();

        private void Awake()
        {
            _instance = this as T;
            _component = GetComponent<T2>();
            if (!canBeDestroy) DontDestroyOnLoad(gameObject);
        }
    }
}