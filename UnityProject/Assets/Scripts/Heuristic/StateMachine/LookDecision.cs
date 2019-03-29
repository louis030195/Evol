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
			RaycastHit hit;

			Debug.DrawRay(controller.eyes.position,
				controller.eyes.forward.normalized * controller.parameters.lookRange, Color.green);

			if (Physics.SphereCast(controller.eyes.position, controller.parameters.lookSphereCastRadius,
				    controller.eyes.forward, out hit, controller.parameters.lookRange)
			    && controller.parameters.tags.Any(t => hit.collider.CompareTag(t)))
			{
				controller.chaseTarget = hit.transform;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}