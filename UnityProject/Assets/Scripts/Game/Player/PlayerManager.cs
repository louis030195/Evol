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
        public float Health = 1f;
        
        public float WalkSpeed = 10f;

        public Text Informations;

        private PhotonView photonView;

        private Rigidbody rb;
        private Vector3 moveDirection;

        [SerializeField] private GameObject playerCamera;
        [SerializeField] private MonoBehaviour[] playerControlScripts;
        
        //True, when the user is firing
        private bool IsFiring;


        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {  
            DeactivateNonLocal();

            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Only the local player should access his own camera and control scripts
        /// </summary>
        private void DeactivateNonLocal()
        {
            playerCamera.SetActive(photonView.IsMine);

            
            foreach (var playerControlScript in playerControlScripts)
            {
                playerControlScript.enabled = photonView.IsMine;
            }
        }

        private void Update()
        {
            /*
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {
                ProcessInputs();
                InputMovement();

                if (this.Health <= 0f)
                {
                    // Die
                }
            }
            
            */
            if (photonView.IsMine)
            {
                
                float horizontalMovement = Input.GetAxis("Horizontal");
                float verticalMovement = Input.GetAxis("Vertical");

                // Normalizing vectors make sure they all have a magnitude of 1.
                // Since adding the two vectors together produces a vector whose magnitude is larger than 1,
                // it means the player will move faster going diagonally. So we normalize it
                moveDirection = (horizontalMovement * transform.right + verticalMovement * transform.forward).normalized;
                
            }
        }

        private void FixedUpdate()
        {
            if(photonView.IsMine)
                Move();
        }

        private void Move()
        {
            
            Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);
            
            // Time.deltaTime helps to give speed more manageable units. Think of it like appending "per second".
            rb.AddForce(moveDirection * WalkSpeed * Time.deltaTime);
            rb.velocity += yVelFix;
            
            /*
            GetComponent<CrawlerAgent>().target = transform.forward * Input.GetAxis("Vertical");
            if (Input.GetKey(KeyCode.Space))
                transform.Translate(Vector3.up);
                */
        }

        /// <summary>
        /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {

                if (!IsFiring)
                {
                    IsFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (IsFiring)
                {
                    IsFiring = false;
                }
            }
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                // Network player, receive data
                IsFiring = (bool)stream.ReceiveNext();
                Health = (float)stream.ReceiveNext();
            }
        }
    }
}