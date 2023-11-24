using System;
using System.Collections.Generic;
using _root.Scripts.Attribute;
using UnityEngine;

namespace _root.Scripts.Game.Data.Child
{
    [Serializable]
    public class KValueManager
    {
        [SerializeField] public bool diseaseEnabled;
        [SerializeField] public bool pcrEnabled;
        [SerializeField] public bool kitEnabled;
        [SerializeField] public int kitChance;
        [SerializeField] public bool vaccineResearch;
        [SerializeField] public bool vaccineEnded;

        [Space] public LocalDataManager.LocalGridData localGridData;

        [Space] [SerializeField] public Disease preDisease;
        [SerializeField] public Disease disease;

        [Space] [SerializeField] public Person person;

        [ReadOnly] [SerializeField] private List<PersonData> persons;

        [SerializeField] public float banbal;
        [SerializeField] public float authority;
        [SerializeField] public int currentBanbal;
        [SerializeField] public int banbalDate;
        [SerializeField] public int authorityDate;

        [SerializeField] public int authorityGoodDate;

        [SerializeField] public float currentAuthority;
    }
}