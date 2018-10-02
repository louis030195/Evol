﻿using UnityEngine;
using Evol.Agents;

namespace Evol
{
    /// <summary>
    /// This class handle the behaviour of the herb gameobject
    /// </summary>
    public class Herb : MonoBehaviour
    {

        float groundSize;
        float offsetX;

        private void Start()
        {
            groundSize = transform.parent.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x / 2 * 0.8f;
            offsetX = transform.parent.position.x;
            transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, transform.position.y,
                Random.Range(-groundSize, groundSize));
        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.collider.GetComponent<HerbivorousAgent>() != null)
            {
                transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, transform.position.y,
                    Random.Range(-groundSize, groundSize));
            }
        }

        public void ResetPosition(Transform worker)
        {
            groundSize = worker.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x / 2 * 0.8f;
            offsetX = transform.parent.position.x;
            transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, 0.5f,
                Random.Range(-groundSize, groundSize));
            transform.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
        }
    }
}