using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Item;
using Evol.Game.Player;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public abstract class Ability : MonoBehaviour
    {
        
        public AbilityData abilityData;
        [HideInInspector] public GameObject caster;
        [HideInInspector] public List<RuneData> runes;
        
        protected float initializationTime;
        
        
        private void OnEnable()
        {
            
            initializationTime = Time.timeSinceLevelLoad;
            Initialize();
        }

        public void Fire()
        {
            TriggerAbility();
        }


        private void Update()
        {
            UpdateAbility();
        }

        private void OnDisable()
        {
            StopAbility();
        }

        protected abstract void Initialize();
        protected abstract void TriggerAbility();
        protected abstract void UpdateAbility();
        protected abstract void StopAbility();

        /// <summary>
        /// Delayed network destroy
        /// </summary>
        /// <param name="seconds">delay in seconds</param>
        /// <returns></returns>
        protected IEnumerator DestroyAfter(int seconds)
        {
            yield return new WaitForSeconds(seconds);
            PhotonNetwork.Destroy(gameObject);
        }

        /// <summary>
        /// Check if the gameobject has health component, if yes, deal damage, return if health component has been found
        /// </summary>
        /// <param name="other"></param>
        protected bool ApplyDamage(GameObject other)
        {
            // The hitbox is on the mesh which is sometimes on a child
            var parent = other.transform.parent; // Not all object have a parent
            var health = other.gameObject.GetComponent<Health>() ? other.gameObject.GetComponent<Health>() :
                parent ? parent.gameObject.GetComponent<Health>() : null;
            if (health != null)
            {
                health.TakeDamage((int)abilityData.stat.damage, caster.GetPhotonView().Owner);
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return abilityData.stat.ToString();
        }
    }
}