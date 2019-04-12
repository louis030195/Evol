using System;
using System.Linq;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public class FireShield : Ability
    {

        public GameObject burningSteps;

        protected override void Initialize()
        {
            // We're allowed to only one shield of this kind per character
            if (caster.GetComponent<Health>().currentShields
                .Any(currentShield => currentShield.Item1.Equals("FireShield")))
                Destroy(gameObject);
        }

        protected override void TriggerAbility()
        {
            
            // Optimization
            var casterTransform = caster.transform;
            var casterPosition = casterTransform.position;
            var myTransform = transform;
            
            myTransform.parent = casterTransform;
            
            // Maybe shouldnt be child, so it still lasts after end of shield
            PhotonNetwork.Instantiate(burningSteps.name, Vector3.zero, Quaternion.identity).transform.SetParent(myTransform);
            
            // For some reason the position is random ?
            myTransform.position = new Vector3(casterPosition.x,
                casterPosition.y + 0.1f,
                casterPosition.z); 
            myTransform.Rotate(-90, 0, 0);
            
            // TODO: implement a shield class, its better
            caster.GetComponent<Health>().currentShields.Add(Tuple.Create("FireShield", (int)abilityData.stat.shield));
            
            Destroy(gameObject, 10.0f);
        }

        protected override void UpdateAbility()
        {
            // TODO: Balance this heal
            if(Time.frameCount % 20 == 0)
                caster.GetComponent<Health>().GetHealed((int)abilityData.stat.heal);
        }

        protected override void StopAbility()
        {
            // Remove the FireShield
            caster.GetComponent<Health>().currentShields
                .Remove(caster.GetComponent<Health>().currentShields.Find(currentShield => currentShield.Item1.Equals("FireShield")));
        }
    }
}