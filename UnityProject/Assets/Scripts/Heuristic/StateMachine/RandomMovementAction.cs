using System.Collections;
using System.Collections.Generic;
using Evol.Utils;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Actions/RandomMovement")]
	public class RandomMovementAction : Action
	{
		public override void Act(StateController controller)
		{
			RandomMove(controller);
		}

		private void RandomMove(StateController controller)
		{
			controller.movement.MoveTo(Position.RandomPositionAround(controller.transform.position, 10));
		}
	}
}