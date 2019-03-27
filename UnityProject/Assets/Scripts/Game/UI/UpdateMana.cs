using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    [RequireComponent(typeof(Mana))]
    public class UpdateMana : UpdateStat
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            GetComponent<Mana>().OnManaChanged.AddListener(UpdateUI);
            base.Start();
        }
    }
}