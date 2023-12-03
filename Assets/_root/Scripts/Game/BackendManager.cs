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
                var nowData = new KGlobalData()
                {
                    kGameManager = GameManager.Instance.Parse(),
                    kNewsManager = NewsManager.Instance.Parse(),
                    kTimeManager = TimeManager.Instance.Parse(),
                    kLocalDataManager = LocalDataManager.Instance.Parse(),
                    kMoneyManager = MoneyManager.Instance.Parse(),
                    kServerDataManager = ServerDataManager.Instance.Parse(),
                    kValueManager = ValueManager.Instance.Parse(),
                    lastSaveDate = DateTime.Now.ToShortDateString()
                };
                new Networking.Put<KGlobalData>("/progresses", nowData)
                    .AddHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AccessToken")}")
                    .AddHeader("Content-Type", "application/json")
                    .OnError(() =>
                    {
                        Debugger.Log("Error!!");
                    })
                    .Build();
                yield return new WaitForSecondsRealtime(50);
            }
        }

        private void RequestData()
        {
            new Networking.Get<KGlobalData>("/progresses")
                .AddHeader("Authorization", $"Bearer {PlayerPrefs.GetString("AccessToken")}")
                .AddHeader("Content-Type", "application/json")
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