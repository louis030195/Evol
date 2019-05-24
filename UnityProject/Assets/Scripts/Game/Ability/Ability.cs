using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Item;
using Evol.Game.Player;
using Evol.ML;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public abstract class Ability : MonoBehaviour
    {
        
        public AbilityData abilityData;
        [HideInInspector] public GameObject caster;
        [HideInInspector] public List<RuneData> runes = new List<RuneData>();
        [HideInInspector] public Vector3 target;
        [HideInInspector] public List<string> alliesTag = new List<string>();
        [HideInInspector] public List<string> enemiesTag = new List<string>();
        
        protected float initializationTime;
        
        
        private void OnEnable()
        {
            initializationTime = Time.timeSinceLevelLoad;
        }
        
        /// <summary>
        /// Ready function is useful when we instanciate the ability so we can initialize the properties THEN run the initialize of the ability
        /// </summary>
        public void Ready()
        {
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
        public bool ApplyDamage(GameObject other)
        {
            if (other == caster || !enemiesTag.Any(other.CompareTag)) return false; // Suicide isn't allowed :/ or killing not an ennemy ?
            
            // The hitbox is on the mesh which is sometimes on a child
            var parent = other.transform.parent; // Not all object have a parent
            var health = other.gameObject.GetComponent<Health>() ? other.gameObject.GetComponent<Health>() :
                parent ? parent.gameObject.GetComponent<Health>() : null;
            if (health && caster)
            {
                // print($"I dealt {abilityData.stat.damage} damage to {other.name}");
                var killerAgent = caster.gameObject.GetComponent<KillerAgent>();
                if(killerAgent != null)
                    killerAgent.hitEnemy.Invoke(abilityData.stat.damage);
                
                // The caster.GetPhotonView() should only occur if the caster is non networked (agent training case)
                health.TakeDamage((int)abilityData.stat.damage, caster && caster.GetPhotonView() ? caster.GetPhotonView().Owner : null);
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