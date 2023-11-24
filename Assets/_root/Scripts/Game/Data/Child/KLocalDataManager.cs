using System;
using System.Collections.Generic;

namespace _root.Scripts.Game.Data.Child
{
    [Serializable]
    public class KLocalDataManager
    {
        public List<KLocalDataPair> pairs;
    }

    [Serializable]
    public class KLocalDataPair
    {
        public string key;
        public string date;
    }
}