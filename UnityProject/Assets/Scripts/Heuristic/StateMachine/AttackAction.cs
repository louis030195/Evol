using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Actions/Attack")]
	public class AttackAction : Action
	{
		public override void Act(StateController controller)
		{
			Attack(controller);
		}

		private void Attack(StateController controller)
		{
			controller.movement.Stop();
			if(controller.target != null) // This if shouldnt be needed because of ActivateStateDecision already checking but idk
				controller.attack.AttackNow(controller.target.GetComponent<Collider>().ClosestPointOnBounds(controller.transform.position));
		}
	}
}