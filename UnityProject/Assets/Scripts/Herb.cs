using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handle the behaviour of the herb gameobject
/// </summary>
public class Herb : MonoBehaviour {


    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            // Destroy it ?
            // Destroy(transform.gameObject);
        }
    }
    
}
