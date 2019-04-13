using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Decisions/ActiveState")]
	public class ActiveStateDecision : Decision
	{
		public override bool Decide(StateController controller)
		{
			bool chaseTargetIsActive = controller.target ?? controller.target.gameObject.activeSelf;
			return chaseTargetIsActive;
		}
	}
}