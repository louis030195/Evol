using UnityEngine;

namespace Evol
{
    public class GrassGrowManager : MonoBehaviour
    {
        private float maxSize;
        private float growRate;
        private float scale;
        
        public void Start()
        {
            maxSize = Random.Range(0.5f, 1.5f);
            growRate = Random.Range(0.01f, 0.02f);
        }

        public void Update()
        {
            if(scale < maxSize)
            {
                this.transform.localScale = Vector3.one * scale;
                scale += growRate * Time.deltaTime;
            }
        }
    }
}