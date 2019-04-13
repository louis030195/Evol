using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Decisions/Look")]
	public class LookDecision : Decision
	{
		public override bool Decide(StateController controller)
		{
			return Look(controller);;
		}

		private bool Look(StateController controller)
		{
			return Physics.SphereCast(controller.eyes.position, controller.parameters.lookSphereCastRadius,
				       controller.eyes.forward, out var hit, controller.parameters.lookRange) && HitTarget(controller, hit.collider);
		}
	}
}