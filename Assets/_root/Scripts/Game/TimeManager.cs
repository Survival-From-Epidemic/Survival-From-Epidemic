using System;
using System.Collections;
using _root.Scripts.Attribute;
using _root.Scripts.Game.Data;
using _root.Scripts.Managers;
using _root.Scripts.Managers.Sound;
using _root.Scripts.Managers.UI;
using _root.Scripts.Player;
using _root.Scripts.SingleTon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _root.Scripts.Game
{
    public class TimeManager : SingleMono<TimeManager>, IDataUpdateable
    {
        private static int _beforeSpeedIdx;
        [SerializeField] public int speedIdx;
        [Range(0.01f, 4f)] [SerializeField] public float timeScale = 1.5f;
        [SerializeField] public int date;
        [SerializeField] public int nextNews;
        public int modificationCount;

        [ReadOnly] [SerializeField] private string infectDateSerialized;
        [ReadOnly] [SerializeField] private string infectGlobalDateSerialized;
        [ReadOnly] [SerializeField] private string kitDateSerialized;
        [ReadOnly] [SerializeField] private string nextKitUpgradeDateSerialized;
        [ReadOnly] [SerializeField] private string nextModificationDateSerialized;
        [ReadOnly] [SerializeField] private string pcrDateSerialized;
        [ReadOnly] [SerializeField] private string startDateSerialized;
        [ReadOnly] [SerializeField] private string todaySerialized;
        [ReadOnly] [SerializeField] private string vaccineEndDateSerialized;
        [ReadOnly] [SerializeField] private string vaccineStartDateSerialized;

        [SerializeField] private int lastMoneyMonth;

        [SerializeField] private bool started;

        public bool globalInfected;
        private double _vaccineTotalDays;
        public DateTime gameEndDate;
        public DateTime infectDate;
        public DateTime infectGlobalDate;
        public DateTime kitDate;
        public DateTime nextKitUpgradeDate;
        public DateTime nextModificationDate;
        public DateTime pcrDate;
        public DateTime startDate;
        public DateTime today;
        public DateTime vaccineEndDate;
        public DateTime vaccineStartDate;

        private void Start()
        {
            speedIdx = 1;
            modificationCount = 0;
            date = 0;
            today = startDate = DateTime.Today;
            gameEndDate = DateTime.MaxValue;

            lastMoneyMonth = today.Month;
            infectGlobalDateSerialized = (infectGlobalDate = startDate.AddDays(Random.Range(7, 15))).ToShortDateString();
            infectDateSerialized = (infectDate = infectGlobalDate.AddDays(Random.Range(7, 15))).ToShortDateString();
            pcrDateSerialized = (pcrDate = infectGlobalDate.AddDays(Random.Range(14, 21))).ToShortDateString();
            kitDateSerialized = (kitDate = startDate.AddDays(Random.Range(84, 105))).ToShortDateString();
            vaccineStartDateSerialized = (vaccineStartDate = startDate.AddDays(Random.Range(154, 203))).ToShortDateString();
            vaccineEndDateSerialized = (vaccineEndDate = startDate.AddDays(Random.Range(709, 808))).ToShortDateString();
            _vaccineTotalDays = (vaccineEndDate - vaccineStartDate).TotalDays;
            nextNews = Random.Range(14, 44);

            NewsManager.Instance.ShowNews(0);
            SoundManager.Instance.PlaySound(SoundKey.GameBackground);

            StartCoroutine(DayCycle());
        }

        public void RegisterData(KGlobalData kGlobalData)
        {
            speedIdx = kGlobalData.kTimeManager.speedIdx;
            timeScale = kGlobalData.kTimeManager.timeScale;
            date = kGlobalData.kTimeManager.date;
            nextNews = kGlobalData.kTimeManager.nextNews;
            modificationCount = kGlobalData.kTimeManager.modificationCount;
            infectDate = Convert.ToDateTime(kGlobalData.kTimeManager.infectDate);
            kitDate = Convert.ToDateTime(kGlobalData.kTimeManager.kitDate);
            nextKitUpgradeDate = Convert.ToDateTime(kGlobalData.kTimeManager.nextKitUpgradeDate);
            nextModificationDate = Convert.ToDateTime(kGlobalData.kTimeManager.nextModificationDate);
            pcrDate = Convert.ToDateTime(kGlobalData.kTimeManager.pcrDate);
            startDate = Convert.ToDateTime(kGlobalData.kTimeManager.startDate);
            today = Convert.ToDateTime(kGlobalData.kTimeManager.today);
            vaccineEndDate = Convert.ToDateTime(kGlobalData.kTimeManager.vaccineEndDate);
            vaccineStartDate = Convert.ToDateTime(kGlobalData.kTimeManager.vaccineStartDate);
            lastMoneyMonth = kGlobalData.kTimeManager.lastMoneyMonth;
            started = kGlobalData.kTimeManager.started;
            globalInfected = kGlobalData.kTimeManager.globalInfected;
        }

        public static void Pause()
        {
            if (Instance.speedIdx == 0) return;
            _beforeSpeedIdx = Instance.speedIdx;
            SpeedCycle(0);
        }

        public static void UnPause()
        {
            SpeedCycle(_beforeSpeedIdx);
        }

        public static void SpeedCycle(int idx = -1) => Instance._SpeedCycle(idx);

        private void _SpeedCycle(int idx)
        {
            if (UIManager.Instance.GetKey() is UIElements.InGameMenu) return;
            if (idx == -1) speedIdx = (speedIdx + 1) % 4;
            else speedIdx = idx;
            // Debugger.Log($"SpeedCycle: {idx}");
            switch (speedIdx)
            {
                case 0:
                    Time.timeScale = 0;
                    break;
                case 1:
                    Time.timeScale = 1;
                    // Instance.timeScale = 1.5f;
                    break;
                case 2:
                    Time.timeScale = 2.1f;
                    // Instance.timeScale = 0.75f;
                    break;
                case 3:
                    Time.timeScale = 4.2f;
                    // Instance.timeScale = 0.35f;
                    break;
            }

            PathManager.Instance.UpdateSpeed();
        }

        public void VaccineUpgrade(int days)
        {
            vaccineEndDateSerialized = vaccineEndDate.AddDays(-days).ToShortDateString();
        }

        public float ModificationWeight() => modificationCount * 20f / (modificationCount + 30f);

        private Disease NewDisease() =>
            new()
            {
                infectivity = Mathf.CeilToInt(ValueManager.Instance.person.totalPerson * Random.Range(0.0005f, 0.0175f) * ModificationWeight()),
                infectWeight = Random.Range(2.5f, 9f) * ModificationWeight()
            };

        private void NewsCycle()
        {
            if (today >= infectGlobalDate.AddDays(3)) NewsManager.Instance.ShowNews(3);

            if (ValueManager.Instance.person.infectedPerson > 0) NewsManager.Instance.ShowNews(6);

            if (ValueManager.Instance.person.deathPerson > 0) NewsManager.Instance.ShowNews(30);

            if (today >= infectDate.AddDays(100)) NewsManager.Instance.ShowNews(11);

            if (today >= infectDate.AddDays(150)) NewsManager.Instance.ShowNews(13);

            if (today >= infectDate.AddDays(220)) NewsManager.Instance.ShowNews(21);

            if (today >= infectDate.AddDays(300)) NewsManager.Instance.ShowNews(17);

            if (today >= infectDate.AddDays(340)) NewsManager.Instance.ShowNews(12);

            if (today >= infectDate.AddDays(400)) NewsManager.Instance.ShowNews(20);
        }

        private IEnumerator DayCycle()
        {
            while (true)
            {
                UIManager.Instance.UpdateTime(today);
                yield return new WaitForSeconds(timeScale);

                if (GameManager.Instance.gameEnd)
                {
                    Debugger.Log("Game ENDED");
                    yield break;
                }

                ValueManager.Instance.Cycle();
                today = startDate.AddDays(date);
                // Debug.unityLogger.Log($"dayCycle: {today.ToShortDateString()}");
                if (lastMoneyMonth != today.Month)
                {
                    lastMoneyMonth = today.Month;
                    if (globalInfected)
                    {
                        var money = 80000;
                        if (ValueManager.Instance.authorityGoodDate >= 30) money += 40000;
                        MoneyManager.Instance.AddMoneyNotify(money);
                    }
                }

                if (!started && today >= startDate.AddDays(1))
                {
                    started = true;
                    NewsManager.Instance.ShowNews(1);
                }

                NewsCycle();

                if (today >= gameEndDate)
                {
                    UIManager.Instance.EnableUI(UIElements.GameResult);
                }

                if (!globalInfected && today >= infectGlobalDate)
                {
                    NewsManager.Instance.ShowNews(2);
                    MoneyManager.Instance.AddMoneyNotify(500000);
                    Debugger.Log("First Infected");
                    globalInfected = true;
                }

                if (!ValueManager.Instance.diseaseEnabled && today >= infectDate)
                {
                    NewsManager.Instance.ShowNews(4);
                    Debugger.Log("Disease Enabled");
                    nextModificationDateSerialized = (nextModificationDate = today.AddDays(Random.Range(30, 90))).ToShortDateString();
                    ValueManager.Instance.diseaseEnabled = true;
                    modificationCount++;
                    ValueManager.Instance.disease = NewDisease();
                }

                if (!ValueManager.Instance.pcrEnabled && today >= pcrDate)
                {
                    NewsManager.Instance.ShowNews(5);
                    Debugger.Log("PCR Enabled");
                    ValueManager.Instance.pcrEnabled = true;
                }

                if (!ValueManager.Instance.kitEnabled && today >= kitDate)
                {
                    NewsManager.Instance.ShowNews(8);
                    Debugger.Log("Self-Kit Enabled: 40% Chance");
                    ValueManager.Instance.kitEnabled = true;
                    ValueManager.Instance.kitChance = 40;
                    nextKitUpgradeDateSerialized = (nextKitUpgradeDate = today.AddDays(Random.Range(60, 120))).ToShortDateString();
                }

                if (ValueManager.Instance.kitEnabled)
                {
                    if (ValueManager.Instance.kitChance <= 40 && NewsManager.Instance.IsNotShowed(9) && today.AddDays(30) >= nextKitUpgradeDate)
                    {
                        NewsManager.Instance.ShowNews(9);
                    }

                    if (ValueManager.Instance.kitChance < 70 && today >= nextKitUpgradeDate)
                    {
                        if (ValueManager.Instance.kitChance <= 40)
                        {
                            NewsManager.Instance.ShowNews(10);
                        }
                        else
                        {
                            NewsManager.Instance.ShowNews(14);
                        }

                        ValueManager.Instance.kitChance += 15;
                        Debugger.Log($"Increased Self-Kit Chance: {ValueManager.Instance.kitChance}%");
                        nextKitUpgradeDateSerialized = (nextKitUpgradeDate = today.AddDays(Random.Range(60, 120))).ToShortDateString();
                    }
                }

                if (ValueManager.Instance.diseaseEnabled && today >= nextModificationDate)
                {
                    Debugger.Log("Disease Modified");
                    modificationCount++;
                    ValueManager.Instance.disease = NewDisease();
                    nextModificationDateSerialized = (nextModificationDate = nextModificationDate.AddDays(Random.Range(30, 90))).ToShortDateString();
                }

                if (!ValueManager.Instance.vaccineResearch && today >= vaccineStartDate)
                {
                    NewsManager.Instance.ShowNews(31);
                    Debugger.Log("Vaccine Research Enabled");
                    ValueManager.Instance.vaccineResearch = true;
                }

                if (ValueManager.Instance.vaccineResearch)
                {
                    switch (GetVaccinePercent())
                    {
                        case >= 0.95:
                            NewsManager.Instance.ShowNews(22);
                            break;
                        case >= 0.85:
                            NewsManager.Instance.ShowNews(19);
                            break;
                        case >= 0.7:
                            NewsManager.Instance.ShowNews(18);
                            break;
                        case >= 0.4:
                            NewsManager.Instance.ShowNews(15);
                            break;
                        case >= 0.2:
                            NewsManager.Instance.ShowNews(7);
                            break;
                    }

                    if (today >= vaccineEndDate)
                    {
                        NewsManager.Instance.ShowNews(23);
                        Debugger.Log("Vaccine Completion");
                        ValueManager.Instance.vaccineEnded = true;

                        if (today >= vaccineEndDate.AddDays(7))
                        {
                            GameManager.Instance.GameEnd(GameEndType.Win);
                        }
                    }
                }

                date++;
                nextNews--;

                if (nextNews <= 0)
                {
                    NewsManager.Instance.ShowRandomNews();
                    nextNews = Random.Range(14, 44);
                }

                ServerDataManager.Instance.RecordTime();
            }
        }

        public double GetVaccinePercent() => 1 - (vaccineEndDate - today).TotalDays / _vaccineTotalDays;
    }
}