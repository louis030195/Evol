using System.Collections;
using System.Collections.Generic;
using Evol.Utils;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Actions/RandomMovement")]
	public class RandomMovementAction : Action
	{
		public int Radius = 10;
		public override void Act(StateController controller)
		{
			RandomMove(controller);
		}

		private void RandomMove(StateController controller)
		{
			if (controller.movement.RemainingDistance <= controller.movement.StoppingDistance && !controller.movement.PathPending) 
			{
				controller.movement.MoveTo(Position.RandomPositionAround(controller.transform.position, Radius));
			}
			
		}
	}
}