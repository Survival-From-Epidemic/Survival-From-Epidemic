﻿using UnityEngine;

namespace _root.Scripts.Player
{
    public class AIAnchor : MonoBehaviour
    {
        [SerializeField] private Vector2 distance;

        private void Awake()
        {
            PathManager.Instance.AddAnchor(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(distance.x, 0, distance.y));
        }

        public Vector3 GetPosition() => transform.position + new Vector3(distance.x * (Random.value - 0.5f), 0, distance.y * (Random.value - 0.5f));
    }
}