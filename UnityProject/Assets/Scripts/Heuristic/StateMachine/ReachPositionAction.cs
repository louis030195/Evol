using System.Collections;
using System.Collections.Generic;
using Evol.Utils;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
    [CreateAssetMenu(menuName = "Evol/StateMachine/Actions/ReachPosition")]
    public class ReachPositionAction : Action
    {
        public override void Act(StateController controller)
        {
            ReachPosition(controller);
        }

        private void ReachPosition(StateController controller)
        {
            if (controller.movement.RemainingDistance <= controller.movement.StoppingDistance && !controller.movement.PathPending) 
            {
                controller.movement.MoveTo(Vector3.zero);
            }
			
        }
    }
}