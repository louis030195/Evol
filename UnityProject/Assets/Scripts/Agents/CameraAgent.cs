using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Evol.Agents
{

    public class CameraAgent : Agent
    {

        public List<GameObject> ThingsToWatch { get; set; }

        private Camera cam;
        private Plane[] planes;
        private int amountActions = 0;

        public override void InitializeAgent()
        {
            ThingsToWatch = new List<GameObject>();
            cam = GetComponent<Camera>();
            cam = Camera.main;
            planes = GeometryUtility.CalculateFrustumPlanes(cam);

        }


        public override void CollectObservations()
        {
            AddVectorObs(gameObject.transform.rotation);
            AddVectorObs(gameObject.transform.position);
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            int thingsSaw = 0;
            foreach (GameObject thingToWatch in ThingsToWatch)
            {
                if (GeometryUtility.TestPlanesAABB(planes, thingToWatch.GetComponentInChildren<Collider>().bounds))
                {
                    thingsSaw++;
                    //Debug.Log("thingToWatch has been detected!");
                }
                else
                {
                    //Debug.Log("Nothing has been detected");
                }
            }

            if (amountActions >= 1000)
            {
                //AddReward(-10f);
                // print("I finished after " + amountActions + " actions");
                amountActions = 0;
                Done();
            }

            AddReward(1f * thingsSaw);

            //if (transform.rotation.z > 150)

            if (Vector3.Distance(transform.position, transform.parent.Find("Ground").position) > 100)
                AddReward(-1f);
            /*
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                if (hit.distance > 20f)
                    AddReward(-1f);
            }*/


            // Move
            transform.Rotate(new Vector3(Mathf.Clamp(vectorAction[0], -1f, 1f),
                    0f,
                    0f),
                Time.fixedDeltaTime * 500 * Mathf.Clamp(vectorAction[1], -1f, 1f));

            transform.Translate(new Vector3(Mathf.Clamp(vectorAction[2], -1f, 1f),
                0f,
                Mathf.Clamp(vectorAction[3], -1f, 1f)));

            amountActions++;
        }



        /// <summary>
        /// Loop over body parts and reset them to initial conditions.
        /// </summary>
        public override void AgentReset()
        {
            float groundSize = transform.parent.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x;
            float offsetX = transform.parent.position.x;
            transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, 50f,
                Random.Range(-groundSize, groundSize));
            transform.rotation = new Quaternion(Random.Range(0, 360), 0, 0, 0);
        }
    }
}