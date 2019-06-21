using UnityEngine;
using System.Collections;

namespace Evol.Game.Misc
{
    public class Cloud : MonoBehaviour
    {
        //Set these variables to whatever you want the slowest and fastest speed for the clouds to be, through the inspector.
        //If you don't want clouds to have randomized speed, just set both of these to the same number.
        //For Example, I have these set to 2 and 5
        private float minSpeed = 2;
        private float maxSpeed = 7;

        //Set these variables to the lowest and highest y values you want clouds to spawn at.
        //For Example, I have these set to 1 and 4
        private float minY = 50f;
        private float maxY = 90f;
        
        
        private Color alphaColor;
        private float timeToFade = 10.0f;

        private float speed;
        private float camWidth;

        private void Start()
        {
            alphaColor = GetComponent<MeshRenderer>().material.color;
            alphaColor.a = 0;
            transform.position = new Vector3(Random.Range(-300, 300), Random.Range(minY, maxY), Random.Range(-300, 300));
            //Set Cloud Movement Speed, and Position to random values within range defined above
            speed = Random.Range(minSpeed, maxSpeed);
            // GetComponent<MeshRenderer>().material.color = Color.Lerp(alphaColor, GetComponent<MeshRenderer>().material.color, timeToFade * Time.deltaTime);
        }

        // Update is called once per frame
        private void Update()
        {
            //Translates the cloud to the right at the speed that is selected
            transform.Translate(speed * Time.deltaTime, 0, 0);
            //If cloud is off Screen, Destroy it.
            if (transform.position.z < -100)
            {
                //Destroy(gameObject, timeToFade);
                GetComponent<MeshRenderer>().material.color = Color.Lerp(GetComponent<MeshRenderer>().material.color, alphaColor, timeToFade * Time.deltaTime);
            }
        }
    }
}