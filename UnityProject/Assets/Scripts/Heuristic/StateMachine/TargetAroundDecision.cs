using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Decisions/TargetAround")]
	public class TargetAroundDecision : Decision
	{

		public override bool Decide(StateController controller)
		{
			return TargetAround(controller);;
		}

		private bool TargetAround(StateController controller)
		{
			var colliders = Physics.OverlapSphere(controller.transform.position, controller.parameters.lookSphereCastRadius);
			foreach (var c in colliders)
			{
				if(controller.parameters.tags.Any(t => c.CompareTag(t)))
				{
					controller.chaseTarget = c.transform;
					return true;
				}
			}

			return false;
		}
	}
}