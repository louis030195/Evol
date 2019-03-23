using Evol.Utils;
using UnityEngine;

namespace Evol
{
    public class GrassGrowManager : MonoBehaviour
    {
        private float maxSize;
        private float growRate;
        private float scale;
        private bool aboveGround;
        
        public void Awake()
        {
            maxSize = Random.Range(0.5f, 1.5f);
            growRate = Random.Range(0.01f, 0.02f);
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