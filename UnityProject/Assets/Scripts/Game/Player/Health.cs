using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Random = UnityEngine.Random;

public class Health : MonoBehaviour
{
    public const int maxHealth = 100;
    public bool destroyOnDeath;

    public int currentHealth = maxHealth;
    
    /// <summary>
    /// List of shields with the spell name which did this shield
    /// </summary>
    public List<Tuple<string,int>> currentShields;

    public RectTransform healthBar;

    private Animator anim;
    [HideInInspector] public bool dead;                // Has the bot been reduced beyond zero health yet?
    public AudioSource healthAudio;                   // The audio source to play.
    public AudioClip[] gettingHit;                      // Audio to play when the bot is getting hit.
    public AudioClip dying;                           // Audio to play when the bot is dying.
    public GameObject[] deathEffects;

    private void Audio()
    {
        if (healthAudio)
        {
            if (!dead)
                healthAudio.clip = gettingHit[Random.Range(0,gettingHit.Length)];
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
        for(var i = 0; i < currentShields.Count; i++)
            currentShields[i] = Tuple.Create(currentShields[i].Item1, currentShields[i].Item2 < 0 ? 0 : currentShields[i].Item2);
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
        
        if (currentHealth <= 0 && !dead)
        {

            dead = true;
            if (destroyOnDeath)
            {
                Destroy(gameObject);
                if (deathEffects.Length > 0)
                    Destroy(Instantiate(deathEffects[Random.Range(0, deathEffects.Length)], new Vector3(transform.position.x, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0)), 3);
            }

            else
            {
                currentHealth = maxHealth;
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
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Lose life when hit by a carnivorous animal
        if(other.collider.CompareTag("Carnivorous"))
            TakeDamage(10);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Synchronize life
        if (stream.IsWriting) {
            stream.SendNext(currentHealth);
        } else
        {
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}