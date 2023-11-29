using Pathfinding;

namespace _root.Scripts.Player
{
    public class GizmoFixedAIPath : AIPath
    {
        protected override void OnDrawGizmos()
        {
            if (Player.Instance.selectedPerson && (gameObject.GetInstanceID() == Player.Instance.selectedPerson.GetInstanceID() || Player.Instance.gizmoOn))
                base.OnDrawGizmos();
        }
    }
}