using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class Debugger : SingleMono<Debugger>
    {
        [SerializeField] private bool debug = true;

        public static void Log(object msg)
        {
            if (Instance.debug) Debug.unityLogger.Log(msg);
        }

        public static bool IsDebug() => Instance.debug;
    }
}