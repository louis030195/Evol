using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
	public class BurningSteps : Ability
	{
		protected override void Initialize()
		{
		}

		protected override void TriggerAbility()
		{
		}

		protected override void UpdateAbility()
		{
		}

		protected override void StopAbility()
		{
		}

		private void OnTriggerStay(Collider other)
		{
			var parent = other.transform.parent; // Not all object have a parent
			var health = other.gameObject.GetComponent<Health>() ? other.gameObject.GetComponent<Health>() :
				parent ? parent.gameObject.GetComponent<Health>() : null;
			if(health != null)
				health.TakeDamage((int)abilityData.stat.damage, caster.GetPhotonView().Owner);
		}
	}
}
