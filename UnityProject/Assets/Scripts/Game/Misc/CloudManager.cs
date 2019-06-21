using UnityEngine;
using System.Collections;
using Evol.Utils;

namespace Evol.Game.Misc
{
    public class CloudManager : MonoBehaviour
    {
        //Set this variable to your Cloud Prefab through the Inspector
        public GameObject[] cloudPrefabs;

        //Set this variable to how often you want the Cloud Manager to make clouds in seconds.
        //For Example, I have this set to 2
        public float delay = 2f;

        //If you ever need the clouds to stop spawning, set this variable to false, by doing: CloudManagerScript.spawnClouds = false;
        public static bool spawnClouds = true;

        // Use this for initialization
        void Start()
        {
            //Begin SpawnClouds Coroutine
            StartCoroutine(SpawnClouds());
        }

        IEnumerator SpawnClouds()
        {
            //This will always run
            while (true)
            {
                //Only spawn clouds if the boolean spawnClouds is true
                while (spawnClouds)
                {
                    var randomRotation = Random.Range(0, 360);
                    //Instantiate Cloud Prefab and then wait for specified delay, and then repeat
                    Instantiate(cloudPrefabs.PickRandom(), Vector3.zero, new Quaternion(0, randomRotation, 0, randomRotation));
                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }
}