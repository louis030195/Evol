using System.Collections;
using System.Collections.Generic;
using Evol.Agents;
using UnityEngine;

namespace Evol.Game.Spell
{
    public class DamageBall : SpellBase
    {
        protected override void Start()
        {
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 15;
            // Destroy the bullet after 5 seconds
            Destroy(gameObject, 5.0f);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.GetComponent<LivingBeingAgent>() != null)
                collider.gameObject.GetComponent<LivingBeingAgent>().LivingBeing.Life -= 10;
            else if(collider.gameObject.GetComponent<Health>() != null)
                    collider.gameObject.GetComponent<Health>().TakeDamage(10);
                
            Destroy(gameObject);
        }
    }
}