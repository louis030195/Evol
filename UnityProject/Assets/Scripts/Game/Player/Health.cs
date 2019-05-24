using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Evol.Game.Misc;
using Evol.Utils;
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
    public class Health : MonoBehaviour
    {
        [Header("Parameters")] 
        public int maxHealth = 100;
        public bool destroyOnDeath;
        public FloatEvent OnHealthChanged = new FloatEvent();
        
        [Header("Audio")] 
        public AudioSource HealthAudio;
        [Tooltip("Clips to play when dying.")] public AudioClip[] DyingClips;
        [Tooltip("Clips to play when getting hit")] public AudioClip[] GettingHitClips;
        [Tooltip("Death effects to spill around")]public GameObject[] DeathEffects;

        [Header("Animations")]
        public string[] GettingHitAnimations;
        public string[] DyingAnimations;
        
        private int currentHealth;
        public int CurrentHealth => currentHealth;
        [HideInInspector] public bool dead;

        // List of shields with the spell name which did this shield
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
            if (currentShields != null) // TODO: remove from update, use event ...wwwwww
            {
                for (var i = 0; i < currentShields.Count; i++)
                    currentShields[i] = Tuple.Create(currentShields[i].Item1,
                        currentShields[i].Item2 < 0 ? 0 : currentShields[i].Item2);
            }
        }
        
        private void Audio()
        {
            if (HealthAudio && HealthAudio.isActiveAndEnabled)
            {
                HealthAudio.clip = !dead ? GettingHitClips.PickRandom() : DyingClips.PickRandom();
                if (!HealthAudio.isPlaying)
                {
                    HealthAudio.Play();
                }
            }
        }

        public void TakeDamage(int amount, Photon.Realtime.Player dealer = null)
        {
            if (gameObject.GetPhotonView() && !gameObject.GetPhotonView().IsMine)
                return;

            if (dealer != null)
            {
                if (dealer.CustomProperties.ContainsKey("damageDealt")
                ) // TODO: maybe could split by target (Monster, friendly fire, ...)
                {
                    // Damage taken also ?
                    dealer.CustomProperties["damageDealt"] = (int) dealer.CustomProperties["damageDealt"] + amount;
                }
                else
                {
                    dealer.CustomProperties.Add("damageDealt", amount);
                }
            }

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
                    if(gameObject.GetPhotonView()) PhotonNetwork.Destroy(gameObject);
                    else Destroy(gameObject);
                    if (DeathEffects.Length > 0) // Unused, prob not ready for working
                        Destroy(
                            Instantiate(DeathEffects[Random.Range(0, DeathEffects.Length)],
                                new Vector3(transform.position.x, transform.position.y, transform.position.z),
                                new Quaternion(0, 0, 0, 0)), 3);
                    
                    // Dead, say to server this object is dead, only thrown if destroyOnDeath ?
                    if(PhotonNetwork.InRoom)
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
                //print($"myname {gameObject.name}");
                var maxRandom = GettingHitAnimations.Length == 1 ? 0 : GettingHitAnimations.Length;
                // Debug.Log($"I am { gameObject.name }");
                animator.SetTrigger(GettingHitAnimations[Random.Range(0, maxRandom)]);
            }

            Audio();
        }

        public void GetHealed(int amount)
        {
            if (!gameObject.GetPhotonView().IsMine)
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
    }
}