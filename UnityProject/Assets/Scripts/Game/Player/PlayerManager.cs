using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Evol.Game.Player
{
    public class PlayerManager : MonoBehaviourPun, IPunObservable
    {
        [Tooltip("The current Health of our player")]
        public float Health = 1f;
        
        public float speed = 10f;

        public Text Informations;

        private float lastSynchronizationTime = 0f;
        private float syncDelay = 0f;
        private float syncTime = 0f;
        private Vector3 syncStartPosition = Vector3.zero;
        private Vector3 syncEndPosition = Vector3.zero;

        private PhotonView photonView;

        private Rigidbody rigidbody;

        [SerializeField] private GameObject playerCamera;
        [SerializeField] private MonoBehaviour[] playerControlScripts;
        
        //True, when the user is firing
        private bool IsFiring;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            photonView = GetComponent<PhotonView>();
            lastSynchronizationTime = Time.time;
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        public void Start()
        {
            /*
            if (!photonView.IsMine)
            {
                playerCamera.SetActive(false);
                foreach (MonoBehaviour m in playerControlScripts)
                {
                    m.enabled = false;
                }
            }
*/
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
            
            InputMovement();
        }

  

        private void InputMovement()
        {
            if (Input.GetKey(KeyCode.Z))
            {
                rigidbody.MovePosition(rigidbody.position + Vector3.forward * speed * Time.deltaTime);
                Informations.text = $"photonView.IsMine {photonView.IsMine} - Z - cam {GetComponent<Camera>()?? GetComponent<Camera>().enabled}";
            }

            if (Input.GetKey(KeyCode.S))
            {
                rigidbody.MovePosition(rigidbody.position - Vector3.forward * speed * Time.deltaTime);
                Informations.text = $"photonView.IsMine {photonView.IsMine} - S - cam {GetComponent<Camera>()?? GetComponent<Camera>().enabled}";
            }

            if (Input.GetKey(KeyCode.D))
            {
                rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);
                Informations.text = $"photonView.IsMine {photonView.IsMine} - D - cam {GetComponent<Camera>()?? GetComponent<Camera>().enabled}";
            }

            if (Input.GetKey(KeyCode.Q))
            {
                rigidbody.MovePosition(rigidbody.position - Vector3.right * speed * Time.deltaTime);
                Informations.text = $"photonView.IsMine {photonView.IsMine} - Q - cam {GetComponent<Camera>()?? GetComponent<Camera>().enabled}";
            }
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