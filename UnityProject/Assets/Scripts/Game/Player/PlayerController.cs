using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    public GameObject[] spells;
    public float[] spellCooldowns;
    protected float[] nextSpell;
    public Transform bulletSpawn;
    public Animator anim;
    [HideInInspector] public Health health;

    private PhotonView photonView;
    
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();

        nextSpell = new float[spellCooldowns.Length];
        
        health = GetComponent<Health>();
        gameObject.AddComponent<AudioListener>();

        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        // ~~~~~~~~~~~~~~~~~~~~~~~~~ Custom gravity
        /*
        RaycastHit hit;
        float heightAboveGround = 0;
        Vector3 pos = transform.position;
        if (Physics.Raycast(pos, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            heightAboveGround = hit.distance;
        }
        pos.y -= heightAboveGround - 0.1f;
        transform.position = pos;*/
        // ~~~~~~~~~~~~~~~~~~~~~~~~

        if (!photonView.IsMine)
        {
            this.transform.GetChild(0).GetComponent<Camera>().enabled = false; // TODO: cleaner solution ?
            return;
        }

        this.transform.GetChild(0).GetComponent<Camera>().enabled = true;

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
        //transform.Translate(0, 0, z);
        GetComponent<Rigidbody>().AddForce(transform.forward * z);

        SpellInput();

    }

    protected virtual void SpellInput()
    {
        // TODO: input from input parameters
        if (Input.GetKeyDown(KeyCode.Mouse0) && spells.Length > 0 && Time.time > nextSpell[0])
        {
            nextSpell[0] = Time.time + spellCooldowns[0];
            GetComponent<Animator>().SetTrigger("Attack1Trigger");
            StartCoroutine(CmdSpell(spells[0]));
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && spells.Length > 1 && Time.time > nextSpell[1])
        {
            nextSpell[1] = Time.time + spellCooldowns[1];
            GetComponent<Animator>().SetTrigger("Attack2Trigger");
            StartCoroutine(CmdSpell(spells[1]));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && spells.Length > 2 && Time.time > nextSpell[2])
        {
            nextSpell[2] = Time.time + spellCooldowns[2];
            StartCoroutine(CmdSpell(spells[2]));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && spells.Length > 3 && Time.time > nextSpell[3])
        {
            nextSpell[3] = Time.time + spellCooldowns[3];
            CmdSpell(spells[3]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && spells.Length > 4 && Time.time > nextSpell[4])
        {
            nextSpell[4] = Time.time + spellCooldowns[4];
            CmdSpell(spells[4]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) && spells.Length > 5 && Time.time > nextSpell[5])
        {
            nextSpell[5] = Time.time + spellCooldowns[5];
            CmdSpell(spells[5]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && spells.Length > 6 && Time.time > nextSpell[6])
        {
            nextSpell[6] = Time.time + spellCooldowns[6];
            CmdSpell(spells[6]);
        }
    }


    // This [PunRPC] code is called on the Client …
    // … but it is run on the Server!
    [PunRPC]
    protected IEnumerator CmdSpell(GameObject spell)
    {
        
        
        //GetComponent<Animator>().ResetTrigger("Attack1Trigger");
        
        yield return new WaitForSeconds(0.7f);
        // Spawn the spellInstance on the Clients
        var go = PhotonNetwork.InstantiateSceneObject(spell.name, bulletSpawn.position, bulletSpawn.rotation);
        go.GetComponent<SpellBase>().Caster = gameObject; // this is useful for some spells that need the position of the caster

    }


}