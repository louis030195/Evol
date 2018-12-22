using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    private PhotonView photonView;
    protected float[] nextSpell;
    
    public GameObject[] Spells;
    public float[] SpellCooldowns;
    public Transform BulletSpawn;
    private Animator Anim;
    [HideInInspector] public bool Lock;
    [HideInInspector] public Health Health;


    protected virtual void Start()
    {

        nextSpell = new float[SpellCooldowns.Length];
        
        Health = GetComponent<Health>();
        gameObject.AddComponent<AudioListener>();
        Anim = GetComponent<Animator>();
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

                Anim.SetFloat("Input X", Input.GetAxis("Vertical"));
                Anim.SetFloat("Input Z", Input.GetAxis("Horizontal"));
                Anim.SetBool("Moving", true);
            }
            else
                Anim.SetBool("Moving", false);

            transform.Rotate(0, x, 0);
            //transform.Translate(0, 0, z);
            GetComponent<Rigidbody>().AddForce(transform.forward * z);

            SpellInput();
        }
    }

    protected virtual void SpellInput()
    {
        // TODO: input from input parameters
        if (Input.GetKeyDown(KeyCode.Mouse0) && Spells.Length > 0 && Time.time > nextSpell[0])
        {
            nextSpell[0] = Time.time + SpellCooldowns[0];
            // TODO: make RPC work
            //photonView.RPC(nameof(CmdSpell), RpcTarget.All, 0);
            CmdSpell(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && Spells.Length > 1 && Time.time > nextSpell[1])
        {
            nextSpell[1] = Time.time + SpellCooldowns[1];
            CmdSpell(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && Spells.Length > 2 && Time.time > nextSpell[2])
        {
            nextSpell[2] = Time.time + SpellCooldowns[2];
            CmdSpell(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && Spells.Length > 3 && Time.time > nextSpell[3])
        {
            nextSpell[3] = Time.time + SpellCooldowns[3];
            CmdSpell(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && Spells.Length > 4 && Time.time > nextSpell[4])
        {
            nextSpell[4] = Time.time + SpellCooldowns[4];
            CmdSpell(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) && Spells.Length > 5 && Time.time > nextSpell[5])
        {
            nextSpell[5] = Time.time + SpellCooldowns[5];
            CmdSpell(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && Spells.Length > 6 && Time.time > nextSpell[6])
        {
            nextSpell[6] = Time.time + SpellCooldowns[6];
            CmdSpell(6);
        }
    }


    // This [PunRPC] code is called on the Client …
    // … but it is run on the Server!
    [PunRPC]
    protected void CmdSpell(int spell)
    {
        // Spawn the spellInstance on the Clients
        var go = PhotonNetwork.Instantiate(Spells[spell].name, BulletSpawn.position, BulletSpawn.rotation);
        //var go = Instantiate(Spells[spell], BulletSpawn.position, BulletSpawn.rotation);
        go.GetComponent<SpellBase>().Caster = gameObject; // this is useful for some spells that need the position of the caster

    }


}