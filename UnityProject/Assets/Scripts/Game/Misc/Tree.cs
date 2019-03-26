using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Misc
{
	public class Tree : MonoBehaviour {

		private void OnDestroy()
		{
			// GetComponentInParent<ForestArea>().Full = false;
		}
	}
}

