using System.Collections;
using System.Collections.Generic;
using Evol.Agents;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Spell
{
    public class DamageBall : SpellBase
    {
        protected override void Start()
        {
            if (!gameObject.GetPhotonView().IsMine)
                return;
            base.Start();
            // TODO: wait the good time of animation to throw spell (animation event)
            // Play animation
            Caster.Item1.GetComponent<Animator>().SetTrigger("Attack1Trigger");
            
            // Throw forward
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 15;
            
            // Destroy the bullet after 5 seconds
            Invoke(nameof(DestroyAfter), 5);
        }

        private void DestroyAfter()
        {
            PhotonNetwork.Destroy(gameObject.GetPhotonView());
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<LivingBeingAgent>() != null)
                other.gameObject.GetComponent<LivingBeingAgent>().LivingBeing.Life -= 10;
            else if(other.gameObject.GetComponent<Health>() != null)
                other.gameObject.GetComponent<Health>().TakeDamage(10);
                
            DestroyAfter();
        }
    }
}