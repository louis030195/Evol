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
        public const int maxMana = 100;

        // Adding accessor allow us to hide the field in inspector and limit setter to private
        // We could also use [HideInInspector] without accessors but the field could be changed outside of this class ...
        public int CurrentMana { get; private set; } = maxMana;

        public RectTransform manaBar;


        public void UseMana(int amount)
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;

            // Substract mana
            CurrentMana -= amount;

            // Clip minimum
            if (CurrentMana < 0) CurrentMana = 0;
            
            // Update UI
            OnChangeMana();
        }

        public void RechargeMana(int amount)
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;

            // Add mana
            CurrentMana += amount;
            
            // Clip maximum
            if (CurrentMana > maxMana)
                CurrentMana = maxMana;

            // Update UI
            OnChangeMana();
        }

        private void OnChangeMana()
        {
            manaBar.sizeDelta = new Vector2(CurrentMana, manaBar.sizeDelta.y);
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Synchronize mana
            if (stream.IsWriting)
            {
                stream.SendNext(CurrentMana);
                stream.SendNext(manaBar.sizeDelta);
            }
            else
            {
                CurrentMana = (int) stream.ReceiveNext();
                manaBar.sizeDelta = (Vector2) stream.ReceiveNext();
            }
        }
    }
}