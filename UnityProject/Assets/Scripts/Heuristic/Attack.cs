using System.Collections;
using System.Collections.Generic;
using Evol.Game.Ability;
using Evol.Game.Player;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Evol.Heuristic
{
	public class Attack : MonoBehaviour
	{	
		[Header("Audio")]
		public AudioSource AttackingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
		[Tooltip("I guess it's for grunt noise (not for ability audio)")] public AudioClip[] AttackClip;                // Audio that plays when each attack is fired.

		[Header("Abilities")] 
		public AbilityData[] abilities;
		
		private int[] attacksTrigger;
		private Animator animator;
		private float[] elapsedTime;

		private void Start()
		{
			animator = GetComponent<Animator>();
			elapsedTime = new float[abilities.Length];
			// Set up the references.
			attacksTrigger = new int[abilities.Length];
			for (var i = 0; i < attacksTrigger.Length; i++)
			{
				attacksTrigger[i] = Animator.StringToHash($"attack{i}"); // Spell 1 = Anim 1 ...
				if(attacksTrigger[i] == 0) print($"failed hash {gameObject.name}");
			}
		}
		
		public void AttackNow(GameObject target)
		{
			if (AttackingAudio)
			{
				AttackingAudio.clip = AttackClip[Random.Range(0, AttackClip.Length)];
				if (!AttackingAudio.isPlaying) // To avoid overlapping sound
				{
					AttackingAudio.Play();
				}
			}
			
			// TODO: slow movement when attacking
			
			var chosenAbility = Random.Range(0, elapsedTime.Length); // Atm choose random ability
			
			if (!(Time.time > elapsedTime[chosenAbility])) return; // TODO: handle mana also ?
			elapsedTime[chosenAbility] = Time.time + abilities[chosenAbility].stat.cooldown;
			
			// Trigger the animation
			animator.SetTrigger(attacksTrigger[chosenAbility]);
			
			// Rotate the AI toward the target, instanciate spell
			// The step size is equal to speed times frame time.
			var step = 10 * Time.deltaTime;
			var newDir = Vector3.RotateTowards(transform.forward, target.transform.position, step, 0.0f);
			Debug.DrawRay(transform.position, newDir, Color.magenta);

			// Move our position a step closer to the target.
			transform.rotation = Quaternion.LookRotation(newDir);
			
			// Use the mana
			// mana.UseMana((int)ability.stat.manaCost);
			
			// Spawn the spell
			var go = PhotonNetwork.Instantiate(abilities[chosenAbility].prefab.name, transform.forward, transform.rotation);
			var abilityInstance = go.GetComponent<Ability>();
			abilityInstance.Ready();
			abilityInstance.Fire();
		}
	}
}