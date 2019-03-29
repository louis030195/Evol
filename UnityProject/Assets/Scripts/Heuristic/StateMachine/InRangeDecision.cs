using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
    [CreateAssetMenu(menuName = "Evol/StateMachine/Decisions/InRange")]
    public class InRange : Decision
    {

        public override bool Decide(StateController controller)
        {
            return CheckIfInRange(controller);;
        }

        private bool CheckIfInRange(StateController controller)
        {
            if (Physics.SphereCast(controller.eyes.position, controller.parameters.lookSphereCastRadius,
                    controller.eyes.forward, out var hit, controller.parameters.attackRange)
                && controller.parameters.tags.Any(t => hit.collider.CompareTag(t)))
            {
                controller.chaseTarget = hit.transform;
                return true;
            }

            return false;
        }
    }
}