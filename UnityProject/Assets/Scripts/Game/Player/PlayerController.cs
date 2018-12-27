using System.Collections;
using System.Collections.Generic;
using Evol.Game.Spell;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    private PhotonView photonView;
    protected float[] nextSpell;
    private Animator anim;
    [HideInInspector] public int CurrentMana = 100;
    
    public List<SpellObject> Spells;
    public Transform BulletSpawn;
    [HideInInspector] public bool Lock;
    [HideInInspector] public Health Health;


    protected virtual void Start()
    {

        nextSpell = new float[Spells.Count];
        
        Health = GetComponent<Health>();
        gameObject.AddComponent<AudioListener>();
        anim = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();


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

            transform.Rotate(0, x, 0);
            GetComponent<Rigidbody>().AddForce(transform.forward * z);

            SpellInput();

            // TODO: maybe make a component to handle mana stuff ? or not ?
            if (CurrentMana > 100) CurrentMana = 100;
        }
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
        if (Spells.Count < spell || Time.time < nextSpell[spell] || Spells[spell].ManaCost > CurrentMana) return;
        
        // Set spell cooldown
        nextSpell[spell] = Time.time + Spells[spell].Cooldown;
        
        // Use the mana
        CurrentMana -= Spells[spell].ManaCost;
        
        // Spawn the spellInstance on the Clients
        var go = PhotonNetwork.Instantiate(Spells[spell].SpellPrefab.name, BulletSpawn.position, BulletSpawn.rotation);
        go.GetComponent<SpellBase>().Caster = gameObject; // this is useful for some spells that need the position of the caster

    }


}