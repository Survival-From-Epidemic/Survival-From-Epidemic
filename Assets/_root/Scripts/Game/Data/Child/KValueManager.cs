using System;
using System.Collections.Generic;

namespace _root.Scripts.Game.Data.Child
{
    [Serializable]
    public class KValueManager
    {
        public bool diseaseEnabled;
        public bool pcrEnabled;
        public bool kitEnabled;
        public int kitChance;
        public bool vaccineResearch;
        public bool vaccineEnded;

        public LocalDataManager.LocalGridData localGridData;

        public Disease preDisease;
        public Disease disease;

        public Person person;

        public List<PersonData> persons;

        public float banbal;
        public float authority;
        public int currentBanbal;
        public int banbalDate;
        public int authorityDate;

        public int authorityGoodDate;

        public float currentAuthority;
    }
}