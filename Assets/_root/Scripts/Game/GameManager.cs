using System;
using _root.Scripts.Managers.UI;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Game
{
    public class GameManager : SingleMono<GameManager>
    {
        [SerializeField] public GameEndType gameEndType;

        public void GameEnd(GameEndType type)
        {
            gameEndType = type;
            TimeManager.Instance.gameEndDate = DateTime.Today.AddDays(1);
        }
    }
}