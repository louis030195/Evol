using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Evol.Game.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public float speed = 10f;

        private float lastSynchronizationTime = 0f;
        private float syncDelay = 0f;
        private float syncTime = 0f;
        private Vector3 syncStartPosition = Vector3.zero;
        private Vector3 syncEndPosition = Vector3.zero;

        private Rigidbody rigidbody;


        void Awake()
        {
            lastSynchronizationTime = Time.time;
        }

        void Update()
        {
            InputMovement();
        }


        private void InputMovement()
        {
            if (Input.GetKey(KeyCode.Z))
                rigidbody.MovePosition(rigidbody.position + Vector3.forward * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.S))
                rigidbody.MovePosition(rigidbody.position - Vector3.forward * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.D))
                rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.Q))
                rigidbody.MovePosition(rigidbody.position - Vector3.right * speed * Time.deltaTime);
        }

    }
}