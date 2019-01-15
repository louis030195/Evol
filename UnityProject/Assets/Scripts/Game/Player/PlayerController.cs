using System.Collections;
using System.Collections.Generic;
using Evol.Game.Spell;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;


namespace Evol.Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        private PhotonView photonView;
        protected float[] nextSpell;
        private Animator anim;
        private Mana mana;

        public List<SpellObject> Spells;
        public Transform BulletSpawn;
        [HideInInspector] public bool Lock;
            
        //public Texture2D cursorTexture;
        //private Vector2 cursorHotspot;
        


        protected virtual void Start()
        {

            nextSpell = new float[Spells.Count];
            mana = GetComponent<Mana>();
            gameObject.AddComponent<AudioListener>();
            anim = GetComponent<Animator>();
            photonView = GetComponent<PhotonView>();
            
            //cursorHotspot = new Vector2 (cursorTexture.width / 2, cursorTexture.height / 2);

            if (!photonView.IsMine)
            {
                transform.GetChild(0).GetComponent<Camera>().enabled = false; // TODO: cleaner solution ?
                Destroy(this);
            }
        }

        void Update()
        {

            if (!Lock)
            {
                var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
                var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
                if (Input.GetAxis("Vertical") > 0)
                {
                    anim.SetFloat("Input X", Input.GetAxis("Vertical"));
                    anim.SetFloat("Input Z", Input.GetAxis("Horizontal"));
                    anim.SetBool("Moving", true);
                }
                else
                    anim.SetBool("Moving", false);
               
                
                Screen.lockCursor = false;
                Cursor.visible = false;
                
                transform.Rotate(0, x, 0);
                GetComponent<Rigidbody>().AddForce(transform.forward * z);
                
                SpellInput();
            }
        }

        float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }
        
        protected virtual void SpellInput()
        {
            // TODO: input from input parameters
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {

                // TODO: make RPC work
                //photonView.RPC(nameof(CmdSpell), RpcTarget.All, 0);
                CmdSpell(0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                CmdSpell(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CmdSpell(2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                CmdSpell(3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                CmdSpell(4);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CmdSpell(5);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                CmdSpell(6);
            }
        }


        // TODO: USE EVENT IN ANIMATION TO PROC THE SPELL AT SPECIFIC TIME OF THE ANIMATION,
        // TODO: SAME FOR FOOT STEP NOISE WHEN FOOT HIT GROUND DURING ANIMATION
        [PunRPC]
        protected void CmdSpell(int spell)
        {
            // If the player try to throw 5th spell but ain't got a 5th spell for example or spell not rdy
            if (Spells.Count < spell || Time.time < nextSpell[spell] || Spells[spell].ManaCost > mana.CurrentMana) return;

            // Set spell cooldown
            nextSpell[spell] = Time.time + Spells[spell].Cooldown;

            // Use the mana
            mana.UseMana(Spells[spell].ManaCost);

            // Spawn the spellInstance on the Clients
            var go = PhotonNetwork.Instantiate(Spells[spell].SpellPrefab.name, BulletSpawn.position,
                BulletSpawn.rotation);
            go.GetComponent<SpellBase>().Caster =
                gameObject; // this is useful for some spells that need the position of the caster

        }


    }
}