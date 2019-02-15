using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Loop
{
	public class Tree : MonoBehaviour {

		private void OnDestroy()
		{
			// GetComponentInParent<ForestArea>().Full = false;
		}
	}
}

