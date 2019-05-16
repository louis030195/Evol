using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Actions/Chase")]
	public class ChaseAction : Action
	{
		public bool ally;
		public override void Act(StateController controller)
		{
			Chase(controller);
		}

		private void Chase(StateController controller)
		{
			if (!ally)
			{
				if (controller.target)
				{
					Debug.DrawLine(controller.transform.position, controller.target.GetComponent<Collider>().ClosestPoint(controller.transform.position));
					controller.movement.MoveTo(controller.target.position);
				}
			}
			else
			{
				if (controller.master)
				{
					Debug.DrawLine(controller.transform.position, controller.master.GetComponent<Collider>().ClosestPoint(controller.transform.position));
					controller.movement.MoveTo(controller.master.position);
				}
			}
		}
	}
}