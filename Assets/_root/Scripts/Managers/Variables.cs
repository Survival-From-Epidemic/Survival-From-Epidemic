using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Managers
{
    public class Variables : SingleMono<Variables>
    {
        public Sprite playSprite;
        public Sprite pauseSprite;
        public Sprite fastPlaySprite;
        public Sprite superFastPlaySprite;
    }
}