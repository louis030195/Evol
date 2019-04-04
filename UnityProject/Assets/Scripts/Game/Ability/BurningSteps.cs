using System.Collections;
using System.Collections.Generic;
using Evol.Agents;
using Evol.Game.Player;
using UnityEngine;

namespace Evol.Game.Ability
{
	public class BurningSteps : Ability
	{
		private void OnTriggerStay(Collider other)
		{
			if (other.gameObject.GetComponent<LivingBeingAgent>() != null)
				other.gameObject.GetComponent<LivingBeingAgent>().LivingBeing.Life -= 1;
			else if(other.gameObject.GetComponent<Health>() != null)
				other.gameObject.GetComponent<Health>().TakeDamage(1);
		}
	}
}
