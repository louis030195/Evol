using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Agents;
using Evol.Game.Item;
using Evol.Game.Player;
using Evol.Utils;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;

namespace Evol.Game.Ability
{
    
    public class Virus : Ability
    {
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
            if (targetHealth != null)
            {
                transform.SetParent(other.transform);
            }

            PhotonNetwork.Destroy(gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            StartCoroutine(SlowlyLoseLife(targetHealth));
        }

        private IEnumerator SlowlyLoseLife(Health health)
        {
            foreach(var i in Enumerable.Range(0, (int)abilityData.stat.lifeLength)) // 5 ticks of life loss
            {
                health.TakeDamage((int) abilityData.stat.damage, caster.GetPhotonView().Owner);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}