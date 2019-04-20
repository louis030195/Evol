using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		[HideInInspector] public UnityEvent onAbilityAnimationStart;
		[HideInInspector] public UnityEvent onAbilityAnimationEnd;
		
		[Header("Audio")]
		public AudioSource AttackingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
		[Tooltip("I guess it's for grunt noise (not for ability audio)")] public AudioClip[] AttackClip;                // Audio that plays when each attack is fired.

		[Header("Abilities")] 
		public AbilityData[] abilities;
		
		private int[] attacksTrigger;
		private Animator animator;
		private float[] elapsedTime;
		private int currentAbility = -1;
		private bool isCasting;
		private GameObject currentTarget;

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
			
			onAbilityAnimationStart.AddListener(() =>
			{
				// Start the casting animation
				isCasting = true;
			});
			onAbilityAnimationEnd.AddListener(() =>
			{
				// Reset current ability to -1, allow to say that we stopped casting
				currentAbility = -1;
				isCasting = false;
			});
			
			foreach (var t in abilities)
			{
				if (!t.prefab) continue; // In case our ability has no prefab (melee)
				var ability = t.prefab.GetComponent<Ability>();
				// Set the caster
				ability.caster = gameObject;
			}
	        // print($"how many animtioncallback {gameObject.name}-{animator.GetBehaviours<AnimationCallback>().Length}");
			animator.GetBehaviours<AnimationCallback>().ToList().ForEach(a => a.targets.Add(gameObject));
		}
		
		public void AttackNow(GameObject target)
		{
			currentTarget = target; // Maybe not so clean
			// When do we reset to null this target ?
			if (AttackingAudio)
			{
				AttackingAudio.clip = AttackClip[Random.Range(0, AttackClip.Length)];
				if (!AttackingAudio.isPlaying) // To avoid overlapping sound
				{
					AttackingAudio.Play();
				}
			}
			
			// TODO: slow movement when attacking
			
			currentAbility = Random.Range(0, elapsedTime.Length); // Atm choose random ability

			if (isCasting || !(Time.time > elapsedTime[currentAbility]))
			{
				return; // TODO: handle mana also ?
			}
		
			elapsedTime[currentAbility] = Time.time + abilities[currentAbility].stat.cooldown;
			
			// Trigger the animation
			animator.SetTrigger(attacksTrigger[currentAbility]);
			
			// Rotate the AI toward the target, instanciate spell
			// The step size is equal to speed times frame time.
			var step = 10 * Time.deltaTime;
			var newDir = Vector3.RotateTowards(transform.forward, target.transform.position, step, 0.0f);
			// Debug.DrawRay(transform.position, newDir, Color.magenta);

			// Move our position a step closer to the target.
			transform.rotation = Quaternion.LookRotation(newDir);
			
			// Use the mana
			// mana.UseMana((int)ability.stat.manaCost);
		}

		public void AnimationEventSpawnAbility()
		{
			// print("lol");
			// if (currentAbility == -1) return; // This is strange, shouldn't happen
			// TODO : spell
			// Spawn the spell
			var go = PhotonNetwork.Instantiate(abilities[currentAbility].prefab.name,
				new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), transform.rotation);
			var abilityInstance = go.GetComponent<Ability>();
			abilityInstance.target = currentTarget;
			abilityInstance.Ready();
			abilityInstance.Fire();
		}
	}
}