using System;
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

        protected override void Awake()
        {
            base.Awake();
            Time.timeScale = 0;
            _isLoaded = false;
        }

        private void Start()
        {
            RequestData();
            StartCoroutine(AutoSaving());
        }

        private IEnumerator AutoSaving()
        {
            yield return new WaitUntil(() => _isLoaded);
            while (true)
            {
                new Networking.Put<KGlobalData>("/progresses", kGlobalData)
                    .OnError(() =>
                    {
                        Debugger.Log("Error!!");
                    })
                    .Build();
                yield return new WaitForSecondsRealtime(10);
            }
        }

        private void RequestData()
        {
            new Networking.Get<KGlobalData>("/progresses")
                .OnResponse(body =>
                {
                    Debugger.Log("Done");
                    kGlobalData = body;
                    _isLoaded = true;
                    RegisterData();
                })
                .OnError(() =>
                {
                    _isLoaded = true;
                })
                .Build();
        }

        private void RegisterData()
        {
            try
            {
                GameManager.Instance.RegisterData(kGlobalData);
            }
            catch (Exception)
            { // ignored
            }
            try
            {
                LocalDataManager.Instance.RegisterData(kGlobalData);
            }
            catch (Exception)
            { // ignored
            }
            try
            {
                MoneyManager.Instance.RegisterData(kGlobalData);
            }
            catch (Exception)
            { // ignored
            }
            try
            {
                NewsManager.Instance.RegisterData(kGlobalData);
            }
            catch (Exception)
            { // ignored
            }
            try
            {
                ServerDataManager.Instance.RegisterData(kGlobalData);
            }
            catch (Exception)
            { // ignored
            }
            try
            {
                TimeManager.Instance.RegisterData(kGlobalData);
            }
            catch (Exception)
            { // ignored
            }
            try
            {
                ValueManager.Instance.RegisterData(kGlobalData);
            }
            catch (Exception)
            { // ignored
            }
        }
    }
}