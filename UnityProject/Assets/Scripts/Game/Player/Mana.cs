using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Random = UnityEngine.Random;

namespace Evol.Game.Player
{
    public class Mana : MonoBehaviour, IPunObservable
    {
        public int MaxMana = 100;

        // Adding accessor allow us to hide the field in inspector and limit setter to private
        // We could also use [HideInInspector] without accessors but the field could be changed outside of this class ...
        public FloatEvent OnManaChanged = new FloatEvent();
        [HideInInspector] public int currentMana;

        private void Start()
        {
            currentMana = MaxMana;
        }

        public void UseMana(int amount)
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;

            // Substract mana
            currentMana -= amount;

            // Clip minimum
            if (currentMana < 0) currentMana = 0;
            
            // Update UI
            OnChangeMana();
        }

        public void RechargeMana(int amount)
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;

            // Add mana
            currentMana += amount;
            
            // Clip maximum
            if (currentMana > MaxMana)
                currentMana = MaxMana;

            // Update UI
            OnChangeMana();
        }

        private void OnChangeMana()
        {
            OnManaChanged.Invoke((float)currentMana / MaxMana);
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Synchronize mana
            if (stream.IsWriting)
            {
                stream.SendNext(currentMana);
            }
            else
            {
                currentMana = (int) stream.ReceiveNext();
            }
        }
    }
}