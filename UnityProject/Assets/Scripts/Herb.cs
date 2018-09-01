using UnityEngine;

/// <summary>
/// This class handle the behaviour of the herb gameobject
/// </summary>
public class Herb : MonoBehaviour {

    float groundSize;
    float offsetX;

    private void Start()
    {
        groundSize = transform.parent.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x / 2*0.8f;
        offsetX = transform.parent.position.x;
        transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, transform.position.y,
            Random.Range(-groundSize, groundSize));
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, transform.position.y,
                Random.Range(-groundSize, groundSize));
        }
    }
    
}
