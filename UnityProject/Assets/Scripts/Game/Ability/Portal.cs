using System;
using System.Linq;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
	public class Portal : Ability
	{
		public GameObject Portal1;
		public GameObject Portal2;

		protected override void Initialize()
		{
		}

		protected override void TriggerAbility()
		{
			Portal2.transform.position = Position.AboveGround(Position.RandomPositionAround(Vector3.zero, 50), 5);

			// Destroy the portal after 20 seconds
			StartCoroutine(DestroyAfter((int)abilityData.stat.lifeLength));
		}

		protected override void UpdateAbility()
		{
		}

		protected override void StopAbility()
		{
		}

		public void TeleportPosition(Transform other)
		{
			var position = Portal2.transform.position;
			var position1 = Portal1.transform.position;
			other.transform.position = Vector3.Distance(other.position, position1) < 20 ? 
				new Vector3(position.x+5, position.y, position.z) :
				new Vector3(position1.x+5, position1.y, position1.z);
		}
	}
}
