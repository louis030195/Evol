using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using UnityEngine;

namespace Evol.Heuristic
{
	public class Attack : MonoBehaviour
	{
		[Header("Audio")]
		public AudioSource AttackingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
		public AudioClip[] AttackClip;                // Audio that plays when each attack is fired.

		
		[Header("Animations")]
		public string[] AttackingAnimations;
		public string[] CastingAnimations; // Ranged ?
		
		private Animator animator;
		private float elapsedTime = 0;

		private void Start()
		{
			animator = GetComponent<Animator>();
		}

		public void Fire(GameObject target, int attackForce, int attackRate)
		{
			if (AttackingAudio)
			{
				AttackingAudio.clip = AttackClip[Random.Range(0, AttackClip.Length)];
				if (!AttackingAudio.isPlaying) // To avoid overlapping sound
				{
					AttackingAudio.Play();
				}
			}

			if (animator)
			{
				if (AttackingAnimations.Length > 0)
				{
					// This hack is because of unity random int excluding max value
					var maxRandom = AttackingAnimations.Length == 1 ? 0 : AttackingAnimations.Length;
					// If there is attacking animations for this object
					animator.SetBool(AttackingAnimations[Random.Range(0, maxRandom)],
						true); // Random.Range exclude max value thats why + 1
				}
			}
			
			if (!(Time.time > elapsedTime)) return;
			elapsedTime = Time.time + attackRate;
			target.GetComponent<Health>().TakeDamage(attackForce);
		}
	}
}