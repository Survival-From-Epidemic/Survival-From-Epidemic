using System;
using _root.Scripts.Game.Data.Child;

namespace _root.Scripts.Game.Data
{
    [Serializable]
    public class KGlobalData
    {
        public KGameManager kGameManager;
        public KLocalDataManager kLocalDataManager;
        public KMoneyManager kMoneyManager;
        public KNewsManager kNewsManager;
        public KServerDataManager kServerDataManager;
        public KTimeManager kTimeManager;
        public KValueManager kValueManager;
        public string lastSaveDate;
    }
}