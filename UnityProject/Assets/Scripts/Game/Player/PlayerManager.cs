using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.Player
{
    public class PlayerManager : MonoBehaviourPun, IPunObservable
    {
        [Tooltip("The current Health of our player")]
        public float WalkSpeed = 10f;
        public Text Informations;
        public AudioClip[] Clips;


        private Rigidbody rb;
        private Vector3 moveDirection;

        private AudioSource audioSource;
	
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        protected virtual void Start()
        {  
            rb = GetComponent<Rigidbody>();
        }



        protected virtual void Update()
        {
                
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
    
            // Normalizing vectors make sure they all have a magnitude of 1.
            // Since adding the two vectors together produces a vector whose magnitude is larger than 1,
            // it means the player will move faster going diagonally. So we normalize it
            moveDirection = (horizontalMovement * transform.right + verticalMovement * transform.forward).normalized;
            if((horizontalMovement != 0 || verticalMovement != 0) && !audioSource.isPlaying)
                audioSource.PlayOneShot(Clips[0]);
            
        }
        

        protected virtual void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);
            
            // Time.deltaTime helps to give speed more manageable units. Think of it like appending "per second".
            rb.AddForce(moveDirection * WalkSpeed * Time.deltaTime);
            rb.velocity += yVelFix;
            
            /*
            GetComponent<CrawlerAgent>().target.z = (transform.Find("Body").transform.localPosition.z == 0 ? 1 :
                                                        transform.Find("Body").transform.localPosition.z) * 
                                                    (Input.GetAxis("Horizontal") != 0 ? Input.GetAxis("Horizontal") * 10 : 1);
            GetComponent<CrawlerAgent>().target.x = (transform.Find("Body").transform.localPosition.x == 0 ? 1 :
                                                        transform.Find("Body").transform.localPosition.x) * 
                                                    (Input.GetAxis("Vertical") != 0 ? Input.GetAxis("Vertical") * 10 : 1);
                */
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //throw new System.NotImplementedException();
        }
        
    }
}