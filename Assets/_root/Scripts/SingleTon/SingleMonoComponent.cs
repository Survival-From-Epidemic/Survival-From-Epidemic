using UnityEngine;

namespace SingleTon
{
    public class SingleMonoComponent<T, T2> : MonoBehaviour where T: MonoBehaviour where T2: Component
    {
        private static T2 _instance;
        
        public static T2 Instance => _instance ? _instance : _instance = FindObjectOfType<T>().GetComponent<T2>();

        private void Awake()
        {
            _instance = GetComponent<T2>();
            DontDestroyOnLoad(gameObject);
        }
    }
}