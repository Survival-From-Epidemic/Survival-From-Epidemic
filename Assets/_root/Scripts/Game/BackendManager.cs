using System.Collections;
using _root.Scripts.Game.Data;
using _root.Scripts.Managers;
using _root.Scripts.Network;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class BackendManager : SingleMono<BackendManager>
    {
        [SerializeField] private KGlobalData kGlobalData;
        private bool _isLoaded;
        private int _getCount;

        protected override void Awake()
        {
            base.Awake();
            _isLoaded = false;
            _getCount = 5;
            RequestData();
            StartCoroutine(AutoSaving());
        }

        private IEnumerator AutoSaving()
        {
            yield return new WaitUntil(() => _isLoaded);
            while (true)
            {
                yield return new WaitForSecondsRealtime(90);
                new Networking.Put<KGlobalData>("/progresses", kGlobalData)
                    .OnError(() =>
                    {
                        Debugger.Log("Error!!");
                    })
                    .Build();
            }
        }

        private void RequestData()
        {
            //TODO: write code
            if(_getCount-- <= 0) return;
            new Networking.Get<KGlobalData>("/progresses")
                .OnResponse(body =>
                {
                    Debugger.Log("");
                    kGlobalData = body;
                    _isLoaded = true;
                })
                .OnError(RequestData)
                .Build();
        }

        private void RegisterData()
        {
            GameManager.Instance.RegisterData(kGlobalData);
            LocalDataManager.Instance.RegisterData(kGlobalData);
            MoneyManager.Instance.RegisterData(kGlobalData);
            NewsManager.Instance.RegisterData(kGlobalData);
            ServerDataManager.Instance.RegisterData(kGlobalData);
            TimeManager.Instance.RegisterData(kGlobalData);
            ValueManager.Instance.RegisterData(kGlobalData);
        }
    }
}