using System.Collections;
using System.Collections.Generic;
using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.UI
{
    [RequireComponent(typeof(Health))]
    public class UpdateHealth : UpdateStat
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            GetComponent<Health>().OnHealthChanged.AddListener(UpdateUI);
            base.Start();
        }
    }
}