using System;
using _root.Scripts.Game.Data;
using _root.Scripts.Managers.UI;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class GameManager : SingleMono<GameManager>, IDataUpdateable
    {
        [SerializeField] public bool gameEnd;
        [SerializeField] public GameEndType gameEndType;

        private void Start()
        {
            gameEnd = false;
        }

        public void RegisterData(KGlobalData kGlobalData)
        {
            gameEnd = kGlobalData.kGameManager.gameEnd;
            gameEndType = kGlobalData.kGameManager.gameEndType;
        }

        public void GameEnd(GameEndType type)
        {
            gameEnd = true;
            gameEndType = type;
            TimeManager.Instance.gameEndDate = DateTime.Today.AddDays(1);
        }
    }
}