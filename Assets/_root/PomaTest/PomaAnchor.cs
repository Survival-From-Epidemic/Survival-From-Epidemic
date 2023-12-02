using UnityEngine;

namespace _root.PomaTest
{
    public class PomaAnchor : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "flag.png", false);
        }
    }
}