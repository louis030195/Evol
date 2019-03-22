﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Decisions/EnnemyAround")]
	public class EnnemyAroundDecision : Decision
	{

		public override bool Decide(StateController controller)
		{
			return EnnemyAround(controller);;
		}

		private bool EnnemyAround(StateController controller)
		{
			var colliders = Physics.OverlapSphere(controller.transform.position, controller.parameters.lookSphereCastRadius);
			foreach (var c in colliders)
			{
				if(c.CompareTag("Player") || c.CompareTag("Carnivorous"))
				{
					controller.chaseTarget = c.transform;
					return true;
				}
			}

			return false;
		}
	}
}