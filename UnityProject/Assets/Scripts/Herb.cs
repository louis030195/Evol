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
            print("Hit by HerbivorousAgent");
            HerbivorousAgent herbivorousAgent = collision.collider.GetComponent<HerbivorousAgent>();
            herbivorousAgent.getLivingBeing().Satiety += 100;
            // herbivorousAgent.AddReward(50f);
            herbivorousAgent.Done();
            
            // transform.position = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
            //Destroy(transform.gameObject, 1.0f);
        }
    }
    
}
