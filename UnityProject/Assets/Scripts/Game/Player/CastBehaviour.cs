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
        // Should we move CharacterData to another script specific to character stats ? or not because its too much related to spells ?
        public Transform bulletSpawn;
        [Tooltip("Key name in the input manager to throw spell (with index next to it like Spell1, Spell2 ...)")] 
        public string spellKey = "Spell";
        public IntFloatEvent onSpellThrown = new IntFloatEvent();

        
        private float[] nextSpell;
        private Mana mana;
        private Health health;
        private bool cast;
        private int currentSpell = -1; // Spell threw
        private int[] attacksTrigger;
        private PlayerManager playerManager;
        
        protected virtual void Start()
        {      
	        // Multiplayer, deactivate the component if it's not mine
	        if(gameObject.GetPhotonView() != null && !gameObject.GetPhotonView().IsMine)
		        enabled = false;

	        playerManager = GetComponent<PlayerManager>();
	        
	        // Set up the references.
	        attacksTrigger = new int[playerManager.characterData.abilities.Length];
	        for (var i = 0; i < attacksTrigger.Length; i++)
	        {
		        attacksTrigger[i] = Animator.StringToHash($"Attack0");//Animator.StringToHash($"Attack{i}"); // Temporary until we have good animations
	        }
	        
	        nextSpell = new float[playerManager.characterData.abilities.Length];
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
			// If cursor is visible lock the casting
			if (Cursor.visible)
				return;
			
			// Rotate toward the point we're aiming at before animating
			Rotating();
			
			// Check if a spell key has been pressed
			for (var i = 0; i < playerManager.characterData.abilities.Length; i++)
			{
				if (!Input.GetButtonDown($"{spellKey}{i}")) continue;
				currentSpell = i;
				break;
			}
			// In order:
			// Check if we just threw a spell
			// Check if we press the casting button
			// Check if we are on the ground
			// Check if we have a spell corresponding to the pressed button
			// Check if the spell cooldown is ready
			// Check if we have enough mana
			// Check if it's my photonview
			// Check if i'm not in a room (debugging)
			// TODO: order the most improbable first in order to gain performance (avoid checking all others)
			if (!cast && 
			    behaviourManager.IsGrounded() &&
			    currentSpell != -1 &&
			    Time.time > nextSpell[currentSpell] &&
			    playerManager.characterData.abilities[currentSpell].GetComponent<Ability.Ability>().abilityData.stat.manaCost < mana.currentMana &&
			    (photonView.IsMine || !PhotonNetwork.InRoom)) // InRoom check is for offline mode (mostly debugging)
			{
				StartCoroutine(nameof(CastOn));
			} else if (cast && currentSpell == -1) // Just finished casting
			{
				StartCoroutine(nameof(CastOff));
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
				// Spells with bool anim ?
				// behaviourManager.GetAnim.SetTrigger(characterData.Spells[currentSpell].Animation);
				// behaviourManager.LockTempBehaviour(behaviourCode);

				// Slow movement
				behaviourManager.GetAnim.SetFloat(speedFloat, 0);
				
				// This state overrides the active one.
				behaviourManager.OverrideWithBehaviour(this);
			}
		}
		
		// Rotate the player to match correct orientation, according to camera.
		private void Rotating()
		{
			var forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
			// Player is moving on ground, Y component of camera facing is not relevant.
			forward.y = 0.0f;
			forward = forward.normalized;

			// Always rotates the player according to the camera horizontal rotation in aim mode.
			var targetRotation = Quaternion.Euler(0, behaviourManager.GetCamScript.GetH, 0);

			var minSpeed = Quaternion.Angle(transform.rotation, targetRotation);

			// Rotate entire player to face camera.
			behaviourManager.SetLastDirection(forward);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);

		}

		public void InstanciateSpell()
		{
			if (currentSpell == -1)
				return;

			var ability = playerManager.characterData.abilities[currentSpell].GetComponent<Ability.Ability>();
				
			// Set spell cooldown
			nextSpell[currentSpell] = Time.time + ability.abilityData.stat.cooldown;
            
			// Throw event to say that we threw a spell
			onSpellThrown.Invoke(currentSpell, ability.abilityData.stat.cooldown);

			// Use the mana
			mana.UseMana((int)ability.abilityData.stat.manaCost);
			
			// Spawn the spell
			var go = PhotonNetwork.Instantiate(playerManager.characterData.abilities[currentSpell].name, bulletSpawn.position, bulletSpawn.rotation);
			go.GetComponent<Ability.Ability>().abilityData.stat = ability.abilityData.stat;
			
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