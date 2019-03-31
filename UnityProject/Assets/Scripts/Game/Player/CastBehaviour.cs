using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Player
{
    
    // CastBehaviour inherits from GenericBehaviour. This class corresponds to casting spells behaviour.
    public class CastBehaviour : GenericBehaviour
    {
        public Element Element;
        public CharacterData CharacterData;
        public Transform BulletSpawn;
        [HideInInspector] public bool Lock;
        public IntFloatEvent OnSpellThrown = new IntFloatEvent();

        
        protected float[] nextSpell;
        private Mana mana;
        private Health health;
        private bool cast;
        private int currentSpell = -1; // Spell threw
        private int[] attacksTrigger;
        
        protected virtual void Start()
        {
	        // Set up the references.
	        attacksTrigger = new int[CharacterData.Spells.Length];
	        for (var i = 0; i < attacksTrigger.Length; i++)
	        {
		        attacksTrigger[i] = Animator.StringToHash($"Attack{i}");
	        }
	        
	        
	        nextSpell = new float[CharacterData.Spells.Length];
	        mana = GetComponent<Mana>();
	        health = GetComponent<Health>();
	        // behaviourManager.SubscribeBehaviour(this);
	        Cursor.visible = false;
            
	        // For debugging in specific scene offline
	        if(!PhotonNetwork.InRoom)
		        PhotonNetwork.OfflineMode = true;
        }
        
		// Update is used to set features regardless the active behaviour.
		private void Update()
		{
			// Check if a spell key has been pressed
			for (var i = 0; i < CharacterData.Spells.Length; i++)
			{
				if (!Input.GetButtonDown($"Spell{i}")) continue;
				currentSpell = i;
				break;
			}
			// In order:
			// Check if we just threw a spell
			// Check if we press the casting button
			// Check if we have a spell corresponding to the pressed button
			// Check if the spell cooldown is ready
			// Check if we have enough mana
			// Check if it's my photonview
			// Check if i'm not in a room (debugging)
			// TODO: order the most improbable first in order to gain performance (avoid checking all others)
			if (!cast && currentSpell != -1 && Time.time > nextSpell[currentSpell] &&
			     CharacterData.Spells[currentSpell].ManaCost < mana.CurrentMana &&
			    (photonView.IsMine || !PhotonNetwork.InRoom)) // InRoom check is for offline mode (mostly debugging)
			{
				StartCoroutine(nameof(CastOn));
			} else if (cast && currentSpell == -1) // Just finished casting
			{
				StartCoroutine(nameof(CastOff));
				print("ok");
			}
			
			// No sprinting while casting.
			canSprint = !cast;
			
		}

		/// <summary>
		/// This coroutine start the animation of casting,
		/// IMPORTANT: don't forget to setup animation event to cast the spell at the right time in the animation or everything will break
		/// </summary>
		/// <returns></returns>
		private IEnumerator CastOn()
		{
			// yield return new WaitForSeconds(0.05f);
			// Casting is not possible.
			if (behaviourManager.GetTempLockStatus(behaviourCode) /*|| behaviourManager.IsOverriding(this)*/)
				yield return false;
			// Start casting.
			else
			{
				// Start the casting animation
				cast = true;
				
				// Trigger the animation
				behaviourManager.GetAnim.SetTrigger(attacksTrigger[currentSpell]);
				// behaviourManager.LockTempBehaviour(behaviourCode);

				// Slow movement
				behaviourManager.GetAnim.SetFloat(speedFloat, 0);
				
				// This state overrides the active one.
				behaviourManager.OverrideWithBehaviour(this);
			}
		}

		public void InstanciateSpell()
		{
			if (currentSpell == -1)
				return;	
				
			// Set spell cooldown
			nextSpell[currentSpell] = Time.time + CharacterData.Spells[currentSpell].Cooldown;
            
			// Throw event to say that we threw a spell
			OnSpellThrown.Invoke(currentSpell, CharacterData.Spells[currentSpell].Cooldown);

			// Use the mana
			mana.UseMana(CharacterData.Spells[currentSpell].ManaCost);

			// Spawn the spell
			var go = PhotonNetwork.Instantiate(CharacterData.Spells[currentSpell].SpellPrefab.name, BulletSpawn.position,
				BulletSpawn.rotation);
                
			go.GetComponent<SpellBase>().Caster =
				Tuple.Create(gameObject, Element); // this is useful for some spells that need the position of the caster

			currentSpell = -1; // Reset the current spell id
		}
		
		/// <summary>
		/// This coroutine stop the animation of casting,
		/// IMPORTANT: don't forget to setup animation event to cast the spell at the right time in the animation or everything will break
		/// </summary>
		/// <returns></returns>
		private IEnumerator CastOff()
		{
			// Say that we stop the casting animation
			cast = false;
			// yield return new WaitForSeconds(0.5f);
			// behaviourManager.UnlockTempBehaviour(behaviourCode);
			behaviourManager.RevokeOverridingBehaviour(this);
			yield return null;
		}
    }
}