using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public class HealthUi : StatUi
    {
        // Start is called before the first frame update
        private void Start()
        {
            var health = target.GetComponent<Health>();
            if (health == null)
            {
                throw new UnityException($"HealthUi - target has no Health component");
            }
            health.OnHealthChanged.AddListener(UpdateUI);
            target.GetComponent<PhotonView>()?.ObservedComponents.Add(this);
        }
    }
}