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
            // TODO: wait the good time of animation to throw spell
            Caster.GetComponent<Animator>().SetTrigger("Attack1Trigger");
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 15;
            // Destroy the bullet after 5 seconds
            Destroy(gameObject, 5.0f);
        }


        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<LivingBeingAgent>() != null)
                other.gameObject.GetComponent<LivingBeingAgent>().LivingBeing.Life -= 10;
            else if(other.gameObject.GetComponent<Health>() != null)
                other.gameObject.GetComponent<Health>().TakeDamage(10);
                
            Destroy(gameObject);
        }
    }
}