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
			return Physics.OverlapSphere(controller.transform.position, controller.parameters.lookSphereCastRadius).Select(c => HitTarget(controller, c)).FirstOrDefault();
		}
	}
}