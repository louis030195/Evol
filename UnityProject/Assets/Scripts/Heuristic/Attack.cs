using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Ability;
using Evol.Game.Player;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Debug = System.Diagnostics.Debug;

namespace Evol.Heuristic
{
	public class Attack : MonoBehaviour
	{	
		[HideInInspector] public UnityEvent onAbilityAnimationStart;
		[HideInInspector] public UnityEvent onAbilityAnimationEnd;
		[HideInInspector] public List<string> alliesTag = new List<string>();
		[HideInInspector] public List<string> enemiesTag = new List<string>();
		[HideInInspector] public bool offline; // For training no need online mode;

		[Tooltip("Whether to turn toward the target on attack")] public bool rotateTowardTarget = true;
		
		[Header("Audio")]
		public AudioSource attackingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
		[Tooltip("I guess it's for grunt noise (not for ability audio)")] public AudioClip[] attackClip;                // Audio that plays when each attack is fired.

		[Header("Abilities")] 
		public AbilityData[] abilities;
		
		private int[] attacksTrigger;
		private Animator animator;
		private float[] elapsedTime;
		private int currentAbility = -1;
		private bool isCasting;
		private Vector3 currentTarget;

		private void Start()
		{
			animator = GetComponent<Animator>();
			Debug.Assert(abilities.Length > 0, "No ability set");
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
				// print($"{gameObject.name} start casting");
			});
			onAbilityAnimationEnd.AddListener(() =>
			{
				// Reset current ability to -1, allow to say that we stopped casting
				currentAbility = -1;
				isCasting = false;
				// print($"{gameObject.name} stop casting");
			});
			
			foreach (var t in abilities)
			{
				if (!t.prefab) continue; // In case our ability has no prefab (melee)
				var ability = t.prefab.GetComponent<Ability>();
				ability.alliesTag = alliesTag;
				ability.enemiesTag = enemiesTag;
				// Set the caster
				ability.caster = gameObject;
			}

			// Specific case with basic attack melee
			var meleeHit = GetComponentInChildren<MeleeHit>();
			if (meleeHit)
			{
				meleeHit.enemiesTag = enemiesTag;
				meleeHit.alliesTag = alliesTag;
			}

			// print($"how many animtioncallback {gameObject.name}-{animator.GetBehaviours<AnimationCallback>().Length}");
			if (animator) animator.GetBehaviours<AnimationCallback>().ToList().ForEach(a => a.targets.Add(gameObject));
		}
		
		public void AttackNow(Vector3 target)
		{
			currentTarget = target; // Maybe not so clean
			// When do we reset to null this target ?
			if (attackingAudio && attackingAudio.isActiveAndEnabled)
			{
				attackingAudio.clip = attackClip.PickRandom();
				if (!attackingAudio.isPlaying) // To avoid overlapping sound
				{
					attackingAudio.Play();
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
			if(animator) animator.SetTrigger(attacksTrigger[currentAbility]);
			
			// Rotate the AI toward the target, instanciate spell
			// The step size is equal to speed times frame time.
			//var step = 10 * Time.deltaTime;
			//var newDir = Vector3.RotateTowards(transform.forward, target, step, 0.0f);
			// Debug.DrawRay(transform.position, newDir, Color.magenta);
			if(rotateTowardTarget) transform.LookAt(target); 
			// Move our position a step closer to the target.
			//var quaternion = Quaternion.LookRotation(newDir);
			//quaternion.x = 0.0f;
			//quaternion.z = 0.0f;
			//transform.rotation = quaternion;

			// Use the mana
			// mana.UseMana((int)ability.stat.manaCost);
			// print($"{gameObject.name} throw spell {abilities[currentAbility].prefab.name}");
		}

		public void AnimationEventSpawnAbility()
		{
			GameObject go;
			var position = transform.position;
			// Spawn the spell
			if (!offline)
			{
				// print($"{gameObject.name} throw spell {abilities[currentAbility].prefab.name}");
				go = PhotonNetwork.Instantiate(abilities[currentAbility].prefab.name,
					new Vector3(position.x, position.y + 5, position.z),
					Quaternion.identity);
			}
			else
			{
				// print($"{gameObject.name} throw spell {abilities[currentAbility].prefab.name}");
				go = Instantiate(abilities[currentAbility].prefab,
					new Vector3(position.x, position.y + 5, position.z),
					Quaternion.identity);
			}

			var abilityInstance = go.GetComponent<Ability>();
			abilityInstance.target = currentTarget;
			abilityInstance.Ready();
			abilityInstance.Fire();
		}
	}
}