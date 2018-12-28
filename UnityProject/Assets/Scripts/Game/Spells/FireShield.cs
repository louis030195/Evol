﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Agents;
using Evol.Game.Player;
using UnityEngine;

namespace Evol.Game.Spell
{
    public class FireShield : SpellBase
    {

        public GameObject BurningSteps;
        
        protected override void Start()
        {
            // TODO: maybe see for common class base for shields ?
            // We're allowed to only one shield of this kind per character
            if (Caster.GetComponent<Health>().currentShields
                .Any(currentShield => currentShield.Item1.Equals("FireShield")))
                Destroy(gameObject);

            Instantiate(BurningSteps, transform);
            Caster.GetComponent<Animator>().SetTrigger("Attack2Trigger");
            
            transform.parent = Caster.transform;
            // For some reason the position is random ?
            transform.position = new Vector3(Caster.transform.position.x,
                Caster.transform.position.y + 0.1f,
                Caster.transform.position.z); 
            transform.Rotate(-90, 0, 0);
            // TODO: implement a shield class, its better
            Caster.GetComponent<Health>().currentShields.Add(Tuple.Create("FireShield", 50));
            
            Destroy(gameObject, 10.0f);
        }

        private void OnDestroy()
        {
            // Remove the FireShield
            Caster.GetComponent<Health>().currentShields
                .Remove(Caster.GetComponent<Health>().currentShields.Find(currentShield => currentShield.Item1.Equals("FireShield")));
        }

        private void Update()
        {
            // TODO: Balance this heal
            if(Time.frameCount % 20 == 0)
                Caster.GetComponent<Health>().GetHealed(1);
        }
    }
}