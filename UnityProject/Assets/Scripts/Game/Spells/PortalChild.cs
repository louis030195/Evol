using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Game.Spell
{
	public class PortalChild : MonoBehaviour {

		private void OnCollisionEnter(Collision other)
		{
			GetComponentInParent<Portal>().TeleportPosition(other.transform);
		}
	}
}
