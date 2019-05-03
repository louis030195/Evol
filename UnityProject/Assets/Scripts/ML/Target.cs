using UnityEngine;

namespace Evol.ML
{
    public class Target : MonoBehaviour
    {
        public float force = 100f;
        public int movementFrequency = 120;
        
        private Rigidbody rBody;
        private KillerAcademy academy;

        private void Start()
        {
            rBody = GetComponent<Rigidbody>();
            academy = GameObject.Find("Academy").GetComponent<KillerAcademy>();
        }

        private void FixedUpdate()
        {
            /*
            if (Time.frameCount % movementFrequency < 1)
            {
                transform.eulerAngles = new Vector3(0,
                    Mathf.LerpAngle(transform.eulerAngles.y, Random.Range(0, 360), Time.deltaTime * 1000), 0);
                rBody.AddForce(transform.forward * Random.Range(0, force), ForceMode.VelocityChange);
                var newScale = Random.Range(academy.targetSize - 2, academy.targetSize + 2);
                transform.localScale.Set(newScale, newScale, newScale);
            }*/
        }
    }
}