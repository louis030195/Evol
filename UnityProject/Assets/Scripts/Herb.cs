using UnityEngine;

/// <summary>
/// This class handle the behaviour of the herb gameobject
/// </summary>
public class Herb : MonoBehaviour {

    public float OffsetX { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            // Destroy it ?
            // Destroy(transform.gameObject);
            transform.position = new Vector3(Random.Range(-5f, 5f) + OffsetX, transform.position.y, Random.Range(-5f, 5f));
        }
    }
    
}
