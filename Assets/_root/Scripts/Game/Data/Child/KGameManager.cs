using System;
using _root.Scripts.Managers.UI;
using UnityEngine;

namespace _root.Scripts.Game.Data.Child
{
    [Serializable]
    public class KGameManager
    {
        [SerializeField] public bool gameEnd;
        [SerializeField] public GameEndType gameEndType;
    }
}