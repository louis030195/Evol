﻿using System;
using System.Collections;
using System.Collections.Generic;
using Evol.Game.Spell;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;


namespace Evol.Game.Player
{

    public enum Element
    {
        Fire,
        Ice
    }
    public class PlayerController : MonoBehaviour, IPunObservable
    {
        private PhotonView photonView;
        protected float[] nextSpell;
        private Animator anim;
        private Mana mana;
        private Health health;
        private Rigidbody rigidBody;

        /// <summary>
        /// Used for spell specific stuff
        /// </summary>
        public Element Element;
        public CharacterData CharacterData;
        public Transform BulletSpawn;
        public float Speed = 1;
        [HideInInspector] public bool Lock;
        public IntFloatEvent OnSpellThrown = new IntFloatEvent();

        protected virtual void Start()
        {
            rigidBody = GetComponent<Rigidbody>();
            nextSpell = new float[CharacterData.Spells.Length];
            mana = GetComponent<Mana>();
            health = GetComponent<Health>();
            gameObject.AddComponent<AudioListener>();
            anim = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();
            
            //cursorHotspot = new Vector2 (cursorTexture.width / 2, cursorTexture.height / 2);

            if (PhotonNetwork.InRoom)
            {
                transform.GetChild(0).GetComponent<Camera>().enabled = false; // TODO: cleaner solution ?
            }
            
            Screen.lockCursor = false;
            Cursor.visible = false;
        }

        void Update()
        {

            if (!Lock && (photonView.IsMine || !PhotonNetwork.InRoom)) // InRoom check is for offline mode (mostly debugging)
            {
               
                var x = Input.GetAxis("Mouse X") * Time.deltaTime * 50.0f; 
                var y = Input.GetAxis("Horizontal") * Time.deltaTime * 4.0f; 
                var z = Input.GetAxis("Vertical") * Time.deltaTime * Speed;  
                
                
                if (Input.GetAxis("Vertical") > 0)
                {
                    anim.SetFloat("Input X", Input.GetAxis("Vertical"));
                    anim.SetFloat("Input Z", Input.GetAxis("Horizontal"));
                    anim.SetBool("Moving", true);
                }
                else
                    anim.SetBool("Moving", false);
         
                
                transform.Rotate(0, x, 0);
                // transform.Translate(y, 0, 0);
                rigidBody.AddForce(transform.forward * z);
                
                SpellInput();
            }
        }
        
        
        protected virtual void SpellInput()
        {
            // TODO: input from input parameters
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {

                // TODO: make RPC work
                // photonView.RPC(nameof(CmdSpell), RpcTarget.All, 0);
                CmdSpell(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // photonView.RPC(nameof(CmdSpell), RpcTarget.All, 1);
                CmdSpell(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // photonView.RPC(nameof(CmdSpell), RpcTarget.All, 2);
                CmdSpell(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                // photonView.RPC(nameof(CmdSpell), RpcTarget.All, 3);
                CmdSpell(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                // photonView.RPC(nameof(CmdSpell), RpcTarget.All, 4);
                CmdSpell(4);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                // photonView.RPC(nameof(CmdSpell), RpcTarget.All, 5);
                CmdSpell(5);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                // photonView.RPC(nameof(CmdSpell), RpcTarget.All, 6);
                CmdSpell(6);
            }
        }


        // TODO: USE EVENT IN ANIMATION TO PROC THE SPELL AT SPECIFIC TIME OF THE ANIMATION,
        // TODO: SAME FOR FOOT STEP NOISE WHEN FOOT HIT GROUND DURING ANIMATION
        // [PunRPC]
        protected void CmdSpell(int spell)
        {
            // If the player try to throw 5th spell but ain't got a 5th spell for example or spell not rdy
            if (CharacterData.Spells.Length < spell || Time.time < nextSpell[spell] || CharacterData.Spells[spell].ManaCost > mana.CurrentMana) return;

            // Set spell cooldown
            nextSpell[spell] = Time.time + CharacterData.Spells[spell].Cooldown;
            
            // Throw event to say that we threw a spell
            OnSpellThrown.Invoke(spell, CharacterData.Spells[spell].Cooldown);

            // Use the mana
            mana.UseMana(CharacterData.Spells[spell].ManaCost);

            // Spawn the spellInstance on the Clients
            /*
            var go = Instantiate(Spells[spell].SpellPrefab, BulletSpawn.position,
                BulletSpawn.rotation);
            */
            var go = PhotonNetwork.Instantiate(CharacterData.Spells[spell].SpellPrefab.name, BulletSpawn.position,
                BulletSpawn.rotation);
                
            go.GetComponent<SpellBase>().Caster =
                Tuple.Create(gameObject,
                    Element); // this is useful for some spells that need the position of the caster
            

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                /*
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(rigidBody.velocity);
                stream.SendNext(rigidBody.position);
                stream.SendNext(rigidBody.rotation);
                */
            }
            else
            {
                /*
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = (Quaternion) stream.ReceiveNext();
                rigidBody.velocity = (Vector3) stream.ReceiveNext();
                rigidBody.position = (Vector3) stream.ReceiveNext();
                rigidBody.rotation = (Quaternion) stream.ReceiveNext();
                */
            }
        }
    }
}