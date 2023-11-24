using System;

namespace _root.Scripts.Game.Data.Child
{
    [Serializable]
    public class KTimeManager
    {
        public int speedIdx;
        public float timeScale;
        public int date;
        public int nextNews;
        public int modificationCount;

        public string infectDate;
        public string infectGlobalDate;
        public string kitDate;
        public string nextKitUpgradeDate;
        public string nextModificationDate;
        public string pcrDate;
        public string startDate;
        public string today;
        public string vaccineEndDate;
        public string vaccineStartDate;

        public int lastMoneyMonth;
        public bool started;

        public bool globalInfected;
    }
}