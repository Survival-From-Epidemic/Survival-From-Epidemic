using System;
using System.Collections;
using _root.Scripts.Attribute;
using _root.Scripts.Managers;
using _root.Scripts.SingleTon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _root.Scripts.Game
{
    public class TimeManager : SingleMono<TimeManager>
    {
        [Range(0.01f, 2f)] [SerializeField] public float timeScale = 1f;
        [SerializeField] public int date;
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

        private bool _globalInfected;
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
            modificationCount = 0;
            today = startDate = DateTime.Today;

            infectGlobalDateSerialized = (infectGlobalDate = startDate.AddDays(Random.Range(7, 15))).ToShortDateString();
            infectDateSerialized = (infectDate = infectGlobalDate.AddDays(Random.Range(7, 15))).ToShortDateString();
            pcrDateSerialized = (pcrDate = infectGlobalDate.AddDays(Random.Range(14, 21))).ToShortDateString();
            kitDateSerialized = (kitDate = startDate.AddDays(Random.Range(84, 105))).ToShortDateString();
            vaccineStartDateSerialized = (vaccineStartDate = startDate.AddDays(Random.Range(154, 203))).ToShortDateString();

            StartCoroutine(DayCycle());
        }

        public float ModificationWeight() => modificationCount * 20f / (modificationCount + 30f);

        private Disease NewDisease() =>
            new()
            {
                infectivity = Mathf.CeilToInt(ValueManager.Instance.person.totalPerson * Random.Range(0.0005f, 0.01f) * ModificationWeight()),
                infectWeight = Random.Range(2f, 8f) * ModificationWeight()
            };

        private IEnumerator DayCycle()
        {
            while (true)
            {
                today = startDate.AddDays(date);
                // Debug.unityLogger.Log($"dayCycle: {today.ToShortDateString()}");
                ValueManager.Instance.Cycle();
                UIManager.Instance.UpdateTime(today);

                if (!_globalInfected && today >= infectGlobalDate)
                {
                    Debugger.Log("First Infected");
                    _globalInfected = true;
                }

                if (!ValueManager.Instance.diseaseEnabled && today >= infectDate)
                {
                    Debugger.Log("Disease Enabled");
                    nextModificationDateSerialized = (nextModificationDate = today.AddDays(Random.Range(30, 90))).ToShortDateString();
                    ValueManager.Instance.diseaseEnabled = true;
                    modificationCount++;
                    ValueManager.Instance.disease = NewDisease();
                }

                if (!ValueManager.Instance.pcrEnabled && today >= pcrDate)
                {
                    Debugger.Log("PCR Enabled");
                    ValueManager.Instance.pcrEnabled = true;
                }

                if (!ValueManager.Instance.kitEnabled && today >= kitDate)
                {
                    Debugger.Log("Self-Kit Enabled: 40% Chance");
                    ValueManager.Instance.kitEnabled = true;
                    ValueManager.Instance.kitChance = 40;
                    nextKitUpgradeDateSerialized = (nextKitUpgradeDate = today.AddDays(Random.Range(60, 120))).ToShortDateString();
                }

                if (ValueManager.Instance.kitEnabled && ValueManager.Instance.kitChance < 70 && today >= nextKitUpgradeDate)
                {
                    ValueManager.Instance.kitChance += 15;
                    Debugger.Log($"Increased Self-Kit Chance: {ValueManager.Instance.kitChance}%");
                    nextKitUpgradeDateSerialized = (nextKitUpgradeDate = today.AddDays(Random.Range(60, 120))).ToShortDateString();
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
                    Debugger.Log("Vaccine Research Enabled");
                    ValueManager.Instance.vaccineResearch = true;
                    vaccineEndDateSerialized = (vaccineEndDate = startDate.AddDays(Random.Range(679, 728))).ToShortDateString();
                }

                if (ValueManager.Instance.vaccineResearch && today >= vaccineEndDate)
                {
                    Debugger.Log("Vaccine Completion");
                    ValueManager.Instance.vaccineEnded = true;
                }

                date++;
                yield return new WaitForSeconds(timeScale);
            }
        }
    }
}