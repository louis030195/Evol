using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
    [CreateAssetMenu(menuName = "Evol/StateMachine/Decisions/ReachedPosition")]
    public class ReachedPositionDecision : Decision
    {
        public GameObject position;
        public float precision = 10.0f;
        public override bool Decide(StateController controller)
        {
            return Vector3.Distance(controller.transform.position, position.transform.position) < precision;
        }
    }
}