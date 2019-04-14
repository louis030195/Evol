using System.Linq;
using Evol.Game.Item;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class MagicPunch : Ability
    {
        private ParticleSystem particle;

        protected override void Initialize()
        {
            particle = GetComponent<ParticleSystem>();
            StartCoroutine(DestroyAfter((int)abilityData.stat.lifeLength));
        }

        protected override void TriggerAbility()
        {
            // particle.Play();
            
        }

        /// <summary>
        /// This is the hitbox triggered when hitting object with health and not triggered by others
        /// </summary>
        /// <param name="other"></param>
        private void OnParticleCollision(GameObject other)
        {
            /*
            if (ApplyDamage(other))
            {
                print("hit");
                PhotonNetwork.Destroy(gameObject);
            }*/
        }

        protected override void UpdateAbility()
        {
        }

        protected override void StopAbility()
        {
        }
    }
}