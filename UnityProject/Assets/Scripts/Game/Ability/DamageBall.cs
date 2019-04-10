using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Agents;
using Evol.Game.Item;
using Evol.Game.Player;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class DamageBall : Ability
    {
        private Vector3 moveDirection;
        private float speed = 2000;

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
            GetComponent<Rigidbody>().AddForce(-transform.forward * speed);
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
            var health = other.gameObject.GetComponent<Health>() ? other.gameObject.GetComponent<Health>() :
                parent ? parent.gameObject.GetComponent<Health>() : null;
            if(health != null)
                health.TakeDamage((int)abilityData.stat.damage);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}