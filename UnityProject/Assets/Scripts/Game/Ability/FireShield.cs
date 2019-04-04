using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Agents;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class FireShield : Ability
    {

        public GameObject BurningSteps;
        
        protected override void Start()
        {
            if (!gameObject.GetPhotonView().IsMine)
                return;
            base.Start();
            // TODO: maybe see for common class base for shields ?
            // We're allowed to only one shield of this kind per character
            if (caster.GetComponent<Health>().currentShields
                .Any(currentShield => currentShield.Item1.Equals("FireShield")))
                Destroy(gameObject);

            Instantiate(BurningSteps, transform);
            // Caster.Item1.GetComponent<Animator>().SetTrigger("Attack2Trigger");
            
            transform.parent = caster.transform;
            // For some reason the position is random ?
            transform.position = new Vector3(caster.transform.position.x,
                caster.transform.position.y + 0.1f,
                caster.transform.position.z); 
            transform.Rotate(-90, 0, 0);
            // TODO: implement a shield class, its better
            caster.GetComponent<Health>().currentShields.Add(Tuple.Create("FireShield", 50));
            
            Destroy(gameObject, 10.0f);
        }

        private void OnDestroy()
        {
            if (!gameObject.GetPhotonView().IsMine)
                return;
            // Remove the FireShield
            caster.GetComponent<Health>().currentShields
                .Remove(caster.GetComponent<Health>().currentShields.Find(currentShield => currentShield.Item1.Equals("FireShield")));
        }

        private void Update()
        {
            if (!gameObject.GetPhotonView().IsMine)
                return;
            // TODO: Balance this heal
            if(Time.frameCount % 20 == 0)
                caster.GetComponent<Health>().GetHealed(1);
        }
    }
}