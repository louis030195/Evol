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
			RaycastHit hit;

			Debug.DrawRay(controller.eyes.position,
				controller.eyes.forward.normalized * controller.parameters.attackRange, Color.red);

			if (Physics.SphereCast(controller.eyes.position, controller.parameters.lookSphereCastRadius,
				    controller.eyes.forward, out hit, controller.parameters.attackRange)
			    && controller.parameters.tags.Any(t => hit.collider.CompareTag(t)))
			{
				controller.attack.Fire(hit.collider.gameObject, controller.parameters.attackForce, controller.parameters.attackRate);
			}
		}
	}
}