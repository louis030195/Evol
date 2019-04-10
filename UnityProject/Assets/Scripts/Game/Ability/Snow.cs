using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
	public class Snow : Ability
	{
		// We could imagine that the wizard start flying and some blocks of hard snow fall down, slowing everyone ...

		public GameObject SnowPrefab;

		protected override void Initialize()
		{
			var position = caster.transform.position;
			transform.position = new Vector3(position.x,
				position.y + 0.1f,
				position.z);
			transform.Rotate(-90, 0, 0);


			for (int i = 0; i < Random.Range(5, 15); i++)
			{
				var randomPositionAround = transform.position;
				randomPositionAround.y += 0.5f;
				randomPositionAround.x += Random.Range(-5, 5);
				randomPositionAround.z += Random.Range(-5, 5);

				StartCoroutine(SlowlyDisappear(Instantiate(
					SnowPrefab.transform.GetChild(Random.Range(0, SnowPrefab.transform.childCount - 1)),
					randomPositionAround, Quaternion.identity).gameObject));
			}
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
		
		private IEnumerator SlowlyDisappear(GameObject go)
		{

			yield return new WaitForSeconds(5f);
			Destroy(go);

		}
	}
}