using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Decisions/Scan")]
	public class ScanDecision : Decision
	{
		public override bool Decide(StateController controller)
		{
			var noEnemyInSight = Scan(controller);
			return noEnemyInSight;
		}

		private bool Scan(StateController controller)
		{
			controller.movement.Stop();
			controller.transform.Rotate(0, controller.parameters.searchingTurnSpeed * Time.deltaTime, 0);
			return controller.CheckIfCountDownElapsed(controller.parameters.searchDuration);
		}
	}
}