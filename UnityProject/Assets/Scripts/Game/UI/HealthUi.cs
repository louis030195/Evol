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
            if (GetComponentInParent<Health>() == null)
            {
                throw new UnityException($"Update should be in a child of a health component");
            }
            GetComponentInParent<Health>().OnHealthChanged.AddListener(UpdateUI);
            GetComponentInParent<PhotonView>().ObservedComponents.Add(this);
        }
    }
}