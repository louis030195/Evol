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
			// the origin of the ray need to start a bit behind
			var castOrigin = controller.eyes.position - controller.eyes.forward * controller.parameters.lookSphereCastRadius; 

			// TODO: LayerMask.GetMask("Player") optimization
			Debug.DrawRay(castOrigin,
				controller.eyes.forward.normalized * controller.parameters.attackRange, Color.red);

			if (Physics.SphereCast(castOrigin, controller.parameters.lookSphereCastRadius,
				    controller.eyes.forward, out var hit, controller.parameters.attackRange)
			    && controller.parameters.enemies.Any(t => hit.collider.CompareTag(t)))
			{
				controller.attack.AttackNow(hit.collider.gameObject);
			}
		}
	}
}