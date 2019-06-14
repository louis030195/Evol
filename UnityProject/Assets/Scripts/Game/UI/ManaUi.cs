using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    public class ManaUi : StatUi
    {
        // Start is called before the first frame update
        private void Start()
        {
            var mana = target.GetComponent<Mana>();
            if (mana == null)
            {
                throw new UnityException($"ManaUi - target has no Mana component");
            }
            mana.OnManaChanged.AddListener(UpdateUI);
            target.GetComponent<PhotonView>().ObservedComponents.Add(this);
        }
    }
}