using System.Linq;
using Evol.Game.Item;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class BoneSpear : Ability
    {

        protected override void Initialize()
        {
            StartCoroutine(DestroyAfter((int)abilityData.stat.lifeLength));
        }

        protected override void TriggerAbility()
        {
            transform.localScale *= abilityData.stat.scale;
            
            // Has empower rune ?
            transform.localScale *= (1 + runes.Count(r => r.effect == RuneEffect.Empower));
        }

        /// <summary>
        /// This is the hitbox triggered when hitting object with health and not triggered by others
        /// </summary>
        /// <param name="other"></param>
        private void OnParticleCollision(GameObject other)
        {
            print($"Collided with {other.name}");
            ApplyDamage(other);
            //PhotonNetwork.Destroy(gameObject); // Destroy only if hitting health
        }

        protected override void UpdateAbility()
        {
        }

        protected override void StopAbility()
        {
            // Has duplicate rune ? Will proc again for each duplicate rune
            if (runes.Any(r => r.effect == RuneEffect.Duplicate))
            {
                runes.Remove(runes.Last()); // Remove 1 rune
                var go = PhotonNetwork.Instantiate(abilityData.prefab.name, transform.position,
                    Quaternion.identity);
                go.GetComponent<Ability>().runes = runes; // Pass my runes to the next proc
                go.GetComponent<Ability>().Fire();
            }
        }
    }
}