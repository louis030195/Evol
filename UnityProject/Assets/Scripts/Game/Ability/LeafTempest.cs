using System.Linq;
using Evol.Game.Item;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class LeafTempest : Ability
    {
        private ParticleSystem particle;
        private Vector3 moveDirection;

        protected override void Initialize()
        {
            particle = GetComponent<ParticleSystem>();
            StartCoroutine(DestroyAfter((int)abilityData.stat.lifeLength));
        }

        protected override void TriggerAbility()
        {
            particle.Play();
            transform.localScale *= abilityData.stat.scale;
            
            // Has empower rune ?
            if (runes.Any(r => r.effect == RuneEffect.Empower))
            {
                transform.localScale *= 2;
            }

            var previousPosition = transform.position;
            
            // Look target   
            Vector3 pos;
            if (target == Vector3.zero)
            {
                var cam = caster.GetComponentInChildren<Camera>();
                if(cam == null) return; // Rarely happen
                pos = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.nearClipPlane));
            }
            else
            {
                pos = target;
            }
            
            transform.LookAt(pos); 

            // Has duplicate rune ?
            if (runes.Any(r => r.effect == RuneEffect.Duplicate))
            {
                var go = PhotonNetwork.Instantiate(abilityData.prefab.name, new Vector3(previousPosition.x, previousPosition.y, previousPosition.z + 3),
                    Quaternion.identity);
                go.transform.LookAt(pos);
                go.GetComponent<Ability>().Fire();
            }
        }

        /// <summary>
        /// This is the hitbox triggered when hitting object with health and not triggered by others
        /// </summary>
        /// <param name="other"></param>
        private void OnParticleCollision(GameObject other)
        {
            //print($"collision with {other.name}");
            ApplyDamage(other);
            // Destroy only if hitting health
        }

        protected override void UpdateAbility()
        {
        }

        protected override void StopAbility()
        {
        }
    }
}