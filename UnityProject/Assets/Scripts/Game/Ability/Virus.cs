using System.Collections;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    
    public class Virus : Ability
    {
        private GameObject target;
        private Health targetHealth;
        protected override void Initialize()
        {
            StartCoroutine(DestroyAfter((int)abilityData.stat.lifeLength));
        }

        protected override void TriggerAbility()
        {
            transform.localScale *= abilityData.stat.scale;
            
            // Throw forward       
            var camera = caster.GetComponentInChildren<Camera>();
            var pos = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.nearClipPlane));
            transform.LookAt(pos); 
            GetComponent<Rigidbody>().AddForce(-transform.forward * 2000);
            // transform.Translate(Position.AboveGround(Vector3.forward * 10, 1));
        }

        protected override void UpdateAbility()
        {
        }

        protected override void StopAbility()
        {
        }
         

        private void OnTriggerEnter(Collider other)
        {
            // The hitbox is on the mesh which is sometimes on a child
            var parent = other.transform.parent; // Not all object have a parent
            targetHealth = other.gameObject.GetComponent<Health>() ? other.gameObject.GetComponent<Health>() :
                parent ? parent.gameObject.GetComponent<Health>() : null;
            
            // It will stick to the target until it hit another target
            if (other.gameObject != target && targetHealth != null)
            {
                target = other.gameObject;
                transform.SetParent(other.transform);
                StartCoroutine(SlowlyLoseLife(targetHealth));
            }
        }

        private IEnumerator SlowlyLoseLife(Health health)
        {
            var i = 0;
            while(i < (int)abilityData.stat.lifeLength) // ticks of life loss
            {
                health.TakeDamage((int)abilityData.stat.damage, caster.GetPhotonView().Owner);
                yield return new WaitForSeconds(1f);
                i++;
            }

            target = null;
            targetHealth = null;
            // PhotonNetwork.Destroy(gameObject);
        }
    }
}