using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Misc;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Evol.Game.Player
{
    public class FloatEvent : UnityEvent<float> { }
    public class Health : MonoBehaviour, IPunObservable
    {
        [Header("Parameters")] 
        public int maxHealth = 100;
        public bool destroyOnDeath;
        public FloatEvent OnHealthChanged = new FloatEvent();
        
        [Header("Audio")] 
        [Tooltip("Audio source")] public AudioSource healthAudio;
        [Tooltip("Audio to play when dying.")] public AudioClip dying;
        [Tooltip("Audio to play when getting hit")] public AudioClip[] gettingHit;
        [Tooltip("Death effects to spill around")]public GameObject[] deathEffects;
        
        private Animator anim;
        private int currentHealth;
        public int CurrentHealth => currentHealth;
        [HideInInspector] public bool dead; // Has the bot been reduced beyond zero health yet?

        /// <summary>
        /// List of shields with the spell name which did this shield
        /// </summary>
        [SerializeField]
        public List<Tuple<string, int>> currentShields;
        


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
            if (currentShields != null)
            {
                for (var i = 0; i < currentShields.Count; i++)
                    currentShields[i] = Tuple.Create(currentShields[i].Item1,
                        currentShields[i].Item2 < 0 ? 0 : currentShields[i].Item2);
            }
        }

        private void Start()
        {
            currentHealth = maxHealth;
            anim = GetComponent<Animator>();
            currentShields = new List<Tuple<string, int>>();
        }

        public void TakeDamage(int amount)
        {
            if (!GetComponent<PhotonView>().IsMine)
                return;

            // Lose life if all the shields have been broken
            currentHealth -= amount > currentShields.Sum(s => s.Item2) ? amount - currentShields.Sum(s => s.Item2) : 0;

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
                    // Dead, say to server this player is dead
                    PhotonNetwork.RaiseEvent(0, new object[] { gameObject.tag }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
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


            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            // Update UI
            OnChangeHealth();
        }

        private void OnChangeHealth()
        {
            OnHealthChanged.Invoke((float)currentHealth / maxHealth);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // Synchronize life
            if (stream.IsWriting)
            {
                stream.SendNext(currentHealth);
            }
            else
            {
                currentHealth = (int) stream.ReceiveNext();
            }
        }
    }
}