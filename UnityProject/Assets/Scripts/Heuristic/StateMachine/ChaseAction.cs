using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Actions/Chase")]
	public class ChaseAction : Action
	{
		public override void Act(StateController controller)
		{
			Chase(controller);
		}

		private void Chase(StateController controller)
		{
			controller.movement.MoveTo(controller.chaseTarget ? controller.chaseTarget.position : Vector3.zero);
		}
	}
}