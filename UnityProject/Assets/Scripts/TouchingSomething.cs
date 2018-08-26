using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TouchingSomething : MonoBehaviour {
    [Header("Detect something")] public GameObject touchedGameObject;

    /// <summary>
    /// Check for collision with something.
    /// </summary>
    void OnCollisionEnter(Collision col)
    {
        if (col.transform.gameObject != null)
        {
            touchedGameObject = col.transform.gameObject;
        }
    }

    /// <summary>
    /// Check for end of collision and reset flag appropriately.
    /// </summary>
    void OnCollisionExit(Collision other)
    {
        if (other.transform.gameObject != touchedGameObject)
        {
            touchedGameObject = null;
        }
    }
    
}
