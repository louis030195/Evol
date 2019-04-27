using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	public abstract class Decision : ScriptableObject
	{
		[Tooltip("Whether the decision is about an ally or an enemy")] public bool ally;
		public abstract bool Decide(StateController controller);

		/// <summary>
		/// From a raycasthit, will return if we hit a decision target
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="hit"></param>
		/// <returns></returns>
		protected bool HitTarget(StateController controller, Collider hit)
		{
			// If we hit something and it's an enemy
			if (!ally)
			{
				if (!controller.parameters.enemies.Any(hit.CompareTag)) return false;
				controller.target = hit.transform;
				return true;
			}

			// If it's an ally
			if (!controller.parameters.allies.Any(hit.CompareTag)) return false;
			controller.target = hit.transform;
			return true;
		}
	}
}