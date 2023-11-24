using _root.Scripts.Game.Data;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class BackendManager : SingleMono<BackendManager>
    {
        [SerializeField] private KGlobalData kGlobalData;

        protected override void Awake()
        {
            base.Awake();
            RequestData();
        }

        private void RequestData()
        {
            //TODO: write code
        }

        private void ApplyData()
        {
        }
    }
}