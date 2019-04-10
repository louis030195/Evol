using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Item;
using Evol.Game.Player;
using Evol.Utils;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Ability
{
    public abstract class Ability : MonoBehaviour
    {
        public AbilityData abilityData;
         
        protected float initializationTime;
        [HideInInspector] public GameObject caster;

        private void OnEnable()
        {
            initializationTime = Time.timeSinceLevelLoad;
            Initialize();
            TriggerAbility();
        }


        private void Update()
        {
            UpdateAbility();
        }

        private void OnDisable()
        {
            StopAbility();
        }

        protected abstract void Initialize();
        protected abstract void TriggerAbility();
        protected abstract void UpdateAbility();
        protected abstract void StopAbility();

        protected IEnumerator DestroyAfter(int seconds)
        {
            yield return new WaitForSeconds(seconds);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}