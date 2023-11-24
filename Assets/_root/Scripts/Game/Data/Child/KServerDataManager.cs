using System;
using System.Collections.Generic;

namespace _root.Scripts.Game.Data.Child
{
    [Serializable]
    public class KServerDataManager
    {
        public List<KTimeLeap> timeLeaps;
    }

    [Serializable]
    public class KTimeLeap
    {
        public int date;
        public List<int> nodeBuy;
        public List<int> nodeSell;
        public List<int> money;
        public List<float> authority;
        public KPerson person;
        public List<float> diseaseGraph;
        public List<int> personGraph;
    }

    [Serializable]
    public class KPerson
    {
        public int totalPerson;
        public int healthyPerson;
        public int deathPerson;
        public int infectedPerson;
    }
}