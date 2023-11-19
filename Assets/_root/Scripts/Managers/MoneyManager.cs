using System;
using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using _root.Scripts.Utils;
using UnityEngine;

namespace _root.Scripts.Managers
{
    public class MoneyManager : SingleMono<MoneyManager>
    {
        [SerializeField] private int money;

        public int GetMoney() => money;

        public void AddMoney(int value)
        {
            money += Math.Max(0, value);
            ServerDataManager.Instance.money[0] += value;
        }

        public bool RemoveMoney(int value)
        {
            if (!value.IsNatural() || !HasMoney(value)) return false;
            ServerDataManager.Instance.money[1] += value;
            money -= value;
            return true;
        }

        public bool HasMoney(int value) => money >= value;
    }
}