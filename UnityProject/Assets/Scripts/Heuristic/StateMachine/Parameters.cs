using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Heuristic.StateMachine
{
	[CreateAssetMenu(menuName = "Evol/StateMachine/Parameters")]
	public class Parameters : ScriptableObject
	{
		public int lookSphereCastRadius;
		public int lookRange;
		public int attackRange;
		public int attackForce;
		public int attackRate;
		public int moveSpeed;
		public int searchingTurnSpeed;
		public int searchDuration;
	}
}