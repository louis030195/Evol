using Evol.Utils;
using UnityEngine;

namespace Evol
{
    public class GrassGrowManager : MonoBehaviour
    {
        public float MaxSize = 1;
        public float GrowRate = 0.015f;
        public float MinRandomness = 0.5f;
        public float MaxRandomness = 1.5f;
        
        private float maxSize;
        private float growRate;
        private float scale;
        private bool aboveGround;
        
        public void Awake()
        {
            maxSize = Random.Range(MaxSize * MinRandomness, MaxSize * MaxRandomness); // Add a bit of randomness for heterogeneity
            growRate = Random.Range(GrowRate * MinRandomness, GrowRate * MaxRandomness);
            Grow(); // Set the initial size directly to avoid seeing the transition to a lower size ...
        }

        public void Update()
        {
            Grow();

            if (!aboveGround)
            {
                transform.position = Position.AboveGround(transform.position, GetComponent<Collider>().bounds.size.y);
                aboveGround = true;
            }
        }

        private void Grow()
        {
            if(scale < maxSize)
            {
                transform.localScale = Vector3.one * scale;
                scale += growRate * Time.deltaTime;
            }
        }
    }
}