using System.Linq;
using Evol.Game.Item;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class Flamethrower : Ability
    {
        private ParticleSystem particle;
        private Vector3 moveDirection;
        private float speed = 2000;

        protected override void Initialize()
        {
            particle = GetComponent<ParticleSystem>();
            // StartCoroutine(DestroyAfter((int)abilityData.stat.lifeLength));
        }

        protected override void TriggerAbility()
        {
            // transform.SetParent(caster.transform);
            particle.Play();
            transform.localScale *= abilityData.stat.scale;
            
            // Has empower rune ?
            if (runes.Any(r => r.effect == RuneEffect.Empower))
            {
                transform.localScale *= 2;
            }
            
            // var cam = caster.GetComponentInChildren<Camera>();
            // var pos = Camera.main.transform.TransformDirection(transform.position);//cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.nearClipPlane));
            var pos = Input.mousePosition;
            pos += Camera.main.transform.forward * 1000f ; // Make sure to add some "depth" to the screen point 
            pos = Camera.main.ScreenToWorldPoint (pos);
            
            transform.LookAt(pos); 

            // Has duplicate rune ?
            if (runes.Any(r => r.effect == RuneEffect.Duplicate))
            {
                var go = PhotonNetwork.Instantiate(abilityData.prefab.name, new Vector3(pos.x + 2, pos.y, pos.z),
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
            ApplyDamage(other); //) PhotonNetwork.Destroy(gameObject); // Destroy only if hitting health
        }

        protected override void UpdateAbility()
        {
        }

        protected override void StopAbility()
        {
        }
    }
}