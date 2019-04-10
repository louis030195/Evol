using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Player;
using Evol.Game.Ability;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
	public class RechargeMana : Ability
	{
		public GameObject powerStream;

		private List<GameObject> stream = new List<GameObject>();

		protected override void Initialize()
		{
		}

		protected override void TriggerAbility()
		{
			var transform1 = transform;
			transform1.parent = caster.transform;
			// For some reason the position is random ?
			var position = caster.transform.position;
			transform1.position = new Vector3(position.x,
				position.y + 1f,
				position.z); 
			Destroy(gameObject, 10f);
		}

		protected override void UpdateAbility()
		{
			// TODO: Balance this
			// If there is a power source close enough
			
		var hitColliders = Physics.OverlapSphere(transform.position, 10f);
		var element = caster.GetComponent<PlayerManager>().characterData.element;
		if (hitColliders.Any(c => (element == Element.Fire && c.CompareTag("FireSource")) 
		                          || (element == Element.Ice && c.CompareTag("IceSource"))))
		{
			if (Time.frameCount % 10 == 0)
			{
				///// This could win the Guinness world record for DIRTIEST CODE EVER
				stream.Add(Instantiate(powerStream, hitColliders.First(c => (element == Element.Fire && c.CompareTag("FireSource")) 
					|| (element == Element.Ice && c.CompareTag("IceSource")))
					.transform.position, Quaternion.identity,
					transform.parent));
				stream.Last().AddComponent<Rigidbody>().useGravity = false;
				stream.Last().transform.LookAt(caster.transform);
				stream.Last().GetComponent<Rigidbody>().AddForce(transform.forward * 400, ForceMode.Acceleration);
				/////
				Destroy(stream.Last(), 5f);
				caster.gameObject.GetComponent<Mana>().RechargeMana(10);
			}
		}
		else
		{
			// Destroy the stream if we go out of range of the source
			stream.ForEach(Destroy);
		}
		}

		protected override void StopAbility()
		{
			stream?.ForEach(Destroy);
		}
	}
}