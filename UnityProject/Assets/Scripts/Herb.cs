using UnityEngine;

/// <summary>
/// This class handle the behaviour of the herb gameobject
/// </summary>
public class Herb : MonoBehaviour {
    

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            float groundSize = GetComponentInParent<Renderer>().bounds.size.x / 2;
            float offsetX = transform.parent.position.x;
            transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, transform.position.y,
                Random.Range(-groundSize, groundSize));
        }
    }
    
}
