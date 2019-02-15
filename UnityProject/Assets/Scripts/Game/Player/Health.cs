﻿using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

namespace Evol.Game.Player
{
    public class Health : MonoBehaviour, IPunObservable
    {
        public const int maxHealth = 100;
        public bool destroyOnDeath;

        [SerializeField]
        public int CurrentHealth { get; private set; } = maxHealth;

        /// <summary>
        /// List of shields with the spell name which did this shield
        /// </summary>
        [SerializeField]
        public List<Tuple<string, int>> currentShields;

        [SerializeField]
        public RectTransform healthBar;

        private Animator anim;
        [HideInInspector] public bool dead; // Has the bot been reduced beyond zero health yet?
        public AudioSource healthAudio; // The audio source to play.
        public AudioClip[] gettingHit; // Audio to play when the bot is getting hit.
        public AudioClip dying; // Audio to play when the bot is dying.
        public GameObject[] deathEffects;

        private void Audio()
        {
            if (healthAudio)
            {
                if (!dead)
                    healthAudio.clip = gettingHit[Random.Range(0, gettingHit.Length)];
                else
                    healthAudio.clip = dying;

                if (!healthAudio.isPlaying)
                {
                    healthAudio.Play();
                }
            }
        }


        private void Update()
        {
            // Clipping shields
            for (var i = 0; i < currentShields.Count; i++)
                currentShields[i] = Tuple.Create(currentShields[i].Item1,
                    currentShields[i].Item2 < 0 ? 0 : currentShields[i].Item2);
        }

        private void Start()
        {
            anim = GetComponent<Animator>();
            currentShields = new List<Tuple<string, int>>();
        }

        public void TakeDamage(int amount)
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;

            // Lose life if all the shields have been broken
            CurrentHealth -= amount > currentShields.Sum(s => s.Item2) ? amount - currentShields.Sum(s => s.Item2) : 0;

            // Substract to the shields
            var rest = amount;
            var tmp = 0;
            for (var i = 0; i < currentShields.Count; i++)
            {
                if (rest > 0)
                    continue;
                tmp = currentShields[i].Item2;
                currentShields[i] = Tuple.Create(currentShields[i].Item1, currentShields[i].Item2 - rest);
                rest -= tmp;
            }

            // Update UI
            OnChangeHealth();

            if (CurrentHealth <= 0 && !dead)
            {

                dead = true;
                if (destroyOnDeath)
                {
                    Destroy(gameObject);
                    if (deathEffects.Length > 0)
                        Destroy(
                            Instantiate(deathEffects[Random.Range(0, deathEffects.Length)],
                                new Vector3(transform.position.x, transform.position.y, transform.position.z),
                                new Quaternion(0, 0, 0, 0)), 3);
                }
                else
                {
                    var content = new object[] { "lol" }; // Array contains the target position and the IDs of the selected units
                    var raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
                    PhotonNetwork.RaiseEvent(0, content, raiseEventOptions, SendOptions.SendReliable);
                }
            }
            //else
            //    anim.SetTrigger("isAttacked");

            Audio();
        }

        public void GetHealed(int amount)
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;


            CurrentHealth += amount;
            if (CurrentHealth > maxHealth)
            {
                CurrentHealth = maxHealth;
            }

            // Update UI
            OnChangeHealth();
        }

        private void OnChangeHealth()
        {
            healthBar.sizeDelta = new Vector2(CurrentHealth, healthBar.sizeDelta.y);
        }

        private void OnCollisionEnter(Collision other)
        {
            // Lose life when hit by a carnivorous animal
            if (other.collider.CompareTag("Carnivorous"))
                TakeDamage(10);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Synchronize life
            if (stream.IsWriting)
            {
                stream.SendNext(CurrentHealth);
                stream.SendNext(healthBar.sizeDelta);
            }
            else
            {
                CurrentHealth = (int) stream.ReceiveNext();
                healthBar.sizeDelta = (Vector2) stream.ReceiveNext();
            }
        }
    }
}