using UnityEngine;

namespace _root.Scripts.SingleTon
{
    public class SingleMono<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T _instance;
        
        public static T Instance => _instance ? _instance : _instance = FindObjectOfType<T>();

        private void Awake()
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}