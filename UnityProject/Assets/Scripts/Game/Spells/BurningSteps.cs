using System.Collections;
using System.Collections.Generic;
using Evol.Agents;
using UnityEngine;

namespace Evol.Game.Spell
{
	public class BurningSteps : DamageBall
	{

		private void OnCollisionEnter(Collision other)
		{
			if (other.gameObject.GetComponent<LivingBeingAgent>() != null)
				other.gameObject.GetComponent<LivingBeingAgent>().LivingBeing.Life -= 1;
			else if(other.gameObject.GetComponent<Health>() != null)
				other.gameObject.GetComponent<Health>().TakeDamage(1);
                
			Destroy(gameObject);
		}
	}
}
