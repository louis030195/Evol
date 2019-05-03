using System.Linq;
using Evol.Game.Item;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class DamageBall : Ability
    {
        private ParticleSystem particle;
        private Vector3 moveDirection;
        private float speed = 2000;

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
            
            // Throw forward   
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
            
            GetComponent<Rigidbody>().AddForce(transform.forward * speed);

            // Has duplicate rune ?
            if (runes.Any(r => r.effect == RuneEffect.Duplicate))
            {
                var go = PhotonNetwork.Instantiate(abilityData.prefab.name, new Vector3(previousPosition.x, previousPosition.y, previousPosition.z + 3),
                    Quaternion.identity);
                go.transform.LookAt(pos);
                go.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
                go.GetComponent<Ability>().Fire();
            }
        }


        /// <summary>
        /// This is the hitbox triggered when hitting object with health and triggered by others
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            ApplyDamage(other.gameObject);
            if(!other.isTrigger) PhotonNetwork.Destroy(gameObject); // Destroy anyway (if its not a trigger like object)
        }

        /// <summary>
        /// This is the hitbox triggered when hitting object with health and not triggered by others
        /// </summary>
        /// <param name="other"></param>
        private void OnParticleCollision(GameObject other)
        {
            if (ApplyDamage(other))
                PhotonNetwork.Destroy(gameObject);
            
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