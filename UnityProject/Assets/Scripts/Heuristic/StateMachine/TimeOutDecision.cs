using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
    [CreateAssetMenu(menuName = "Evol/StateMachine/Decisions/TimeOut")]
    public class TimeOutDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return TimeOut(controller);
        }

        private bool TimeOut(StateController controller)
        {
            return controller.CheckIfCountDownElapsed(controller.parameters.timeOut);
        }
    }
}