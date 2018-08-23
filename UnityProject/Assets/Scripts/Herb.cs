using UnityEngine;

/// <summary>
/// This class handle the behaviour of the herb gameobject
/// </summary>
public class Herb : MonoBehaviour {

    public float OffsetX { get; set; }
    public float groundSize { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            // Destroy it ?
            // Destroy(transform.gameObject);
            transform.position = new Vector3(Random.Range(-groundSize / 2, groundSize / 2) + OffsetX, transform.position.y,
                Random.Range(-groundSize / 2, groundSize / 2));
        }
    }
    
}
