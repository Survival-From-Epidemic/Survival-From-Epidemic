using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Player
{
    public class Player : SingleMono<Player>
    {
        [SerializeField] public float speed = 5;
    }
}