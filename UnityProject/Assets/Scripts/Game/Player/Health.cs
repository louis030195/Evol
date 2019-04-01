﻿using System;
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
    // TODO: Consider moving these classes somewhere else xD
    public class IntFloatEvent : UnityEvent<int, float> { }
    public class FloatEvent : UnityEvent<float> { }
    public class Health : MonoBehaviour, IPunObservable
    {
        [Header("Parameters")] 
        public int maxHealth = 100;
        public bool destroyOnDeath;
        public FloatEvent OnHealthChanged = new FloatEvent();
        
        [Header("Audio")] 
        [Tooltip("Audio source")] public AudioSource HealthAudio;
        [Tooltip("Audio to play when dying.")] public AudioClip Dying;
        [Tooltip("Audio to play when getting hit")] public AudioClip[] GettingHitClips;
        [Tooltip("Death effects to spill around")]public GameObject[] DeathEffects;

        [Header("Animations")]
        public string[] GettingHitAnimations;
        public string[] DyingAnimations;
        
        private int currentHealth;
        public int CurrentHealth => currentHealth;
        [HideInInspector] public bool dead;

        // List of shields with the spell name which did this shield
        [SerializeField]
        public List<Tuple<string, int>> currentShields;
        
        private Animator animator;

        
        private void Start()
        {
            currentHealth = maxHealth;
            animator = GetComponent<Animator>();
            currentShields = new List<Tuple<string, int>>();
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
        
        private void Audio()
        {
            if (HealthAudio)
            {
                if (!dead)
                    HealthAudio.clip = GettingHitClips[Random.Range(0, GettingHitClips.Length)];
                else
                    HealthAudio.clip = Dying;

                if (!HealthAudio.isPlaying)
                {
                    HealthAudio.Play();
                }
            }
        }

        public void TakeDamage(int amount)
        {
            if (GetComponent<PhotonView>() && !GetComponent<PhotonView>().IsMine)
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
                    if (DeathEffects.Length > 0)
                        Destroy(
                            Instantiate(DeathEffects[Random.Range(0, DeathEffects.Length)],
                                new Vector3(transform.position.x, transform.position.y, transform.position.z),
                                new Quaternion(0, 0, 0, 0)), 3);
                    
                    // Dead, say to server this object is dead
                    PhotonNetwork.RaiseEvent(0, new object[] { gameObject.tag }, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                }

                if (DyingAnimations.Length > 0)
                {
                    var maxRandom = DyingAnimations.Length == 1 ? 0 : DyingAnimations.Length;
                    // If there is death animations for this object
                    animator.SetBool(DyingAnimations[Random.Range(0, maxRandom)],
                        true); // TODO: not rly useful if destroyed ... (maybe should add death delay idk)
                }
            }
            
            if (CurrentHealth > 0 && GettingHitAnimations.Length > 0) // If there is getting hit animations for this object
            {
                var maxRandom = GettingHitAnimations.Length == 1 ? 0 : GettingHitAnimations.Length;
                animator.SetBool(GettingHitAnimations[Random.Range(0, maxRandom)], true);
            }

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