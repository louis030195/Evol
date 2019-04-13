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
            if (GetComponentInParent<Mana>() == null)
            {
                throw new UnityException($"Update should be in a child of a mana component");
            }
            GetComponentInParent<Mana>().OnManaChanged.AddListener(UpdateUI);
        }
    }
}