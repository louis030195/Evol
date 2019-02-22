using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using UnityEngine;

namespace Evol.Heuristic
{
	public class Attack : MonoBehaviour
	{
		public AudioSource AttackingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
		public AudioClip AttackClip;                // Audio that plays when each attack is fired.

		private float elapsedTime = 0;
		
		public void Fire(GameObject target, int attackForce, int attackRate)
		{        
			if (!(Time.time > elapsedTime)) return;
			elapsedTime = Time.time + attackRate;
			target.GetComponent<Health>().TakeDamage(attackForce);
		}
	}
}