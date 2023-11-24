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

        public ServerDataManager.TimeLeap ToTimeLeap() => new()
        {
            date = date,
            nodeBuy = nodeBuy.ToArray(),
            nodeSell = nodeSell.ToArray(),
            money = money.ToArray(),
            authority = authority.ToArray(),
            person = new Person
            {
                deathPerson = person.deathPerson,
                healthyPerson = person.healthyPerson,
                infectedPerson = person.infectedPerson,
                totalPerson = person.totalPerson
            },
            diseaseGraph = diseaseGraph.ToArray(),
            personGraph = personGraph.ToArray()
        };
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