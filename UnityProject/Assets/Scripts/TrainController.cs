﻿using Evol.Utils;
using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
using Evol.Agents;
using Prometheus;

namespace Evol
{
    /// <summary>
    /// This class handles all the training of the agents
    /// </summary>
    public class TrainController : MonoBehaviour
    {

        [Header("Workers")] [Space(10)] public Worker WorkerCarniHerbi;
        public int GroundScale = 10;
        public List<Brain> Brains; // Give all the brains you use in all workers

        [Header("Items To Spawn")] [Space(10)] public List<GameObject> ItemsToSpawn;

        [Header("Misc")] [Space(10)] public bool ResetWorkers = true;
        public bool Curriculum;


        private List<GameObject> workerObjects;
        private int frames;
        private Pool herbivorousPool;
        private Pool carnivorousPool;
        private Pool herbPool;
        private Pool godPool;
        private Academy evolAcademy;
        
        // Monitoring
        private MetricServer metricServer;
        private Counter resetCounter;
        private Gauge herbivorousInUseGauge;
        private Gauge carnivorousInUseGauge;
        

        // Use this for initialization
        private void Start()
        {
            
            metricServer = new MetricServer(port: 1234);
            metricServer.Start();
            
            resetCounter = Metrics.CreateCounter("resetCounter", "How many times the worker has been reset");
            herbivorousInUseGauge = Metrics.CreateGauge("herbivorousInUseGauge", "Current total amount of herbivorous agents");
            carnivorousInUseGauge = Metrics.CreateGauge("carnivorousInUseGauge", "Current total amount of carnivorous agents");
            
            evolAcademy = FindObjectOfType<EvolAcademy>();


            workerObjects = new List<GameObject>();

            // Instanciate pools to spawn / release objects
            herbivorousPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("herbivorous")));
            carnivorousPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("carnivorous")));
            herbPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("food")));
            //godPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("god")));
           

            // Find the brains in the list
            herbivorousPool.Brain =
                Brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);
            carnivorousPool.Brain =
                Brains.FirstOrDefault(brain => "Carnivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);
            //godPool.Brain =
            //    Brains.FirstOrDefault(brain => "God" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);


            SpawnWorkers();
        }

        private void SpawnWorkers()
        {
            int w = 0;
            for (; w < WorkerCarniHerbi.AmountOfWorkers; w++)
            {
                GameObject workerObject = Instantiate(WorkerCarniHerbi.WorkerPrefab,
                    new Vector3(2 * GroundScale * 10 * w, 0, 0),
                    new Quaternion(0, 0, 0, 0));
                workerObject.transform.Find("Ground").localScale = new Vector3(GroundScale, 1, GroundScale);
                for (int i = 0; i < WorkerCarniHerbi.AmountOfAgentsToAdd; i++)
                {
                    try
                    {
                        GameObject carnivorousChild = carnivorousPool.GetObject();
                        carnivorousChild.transform.parent = workerObject.transform;
                        carnivorousChild.SetActive(true);
                        carnivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);
                        //carnivorousChild.GetComponent<LivingBeingAgent>().LivingBeing.Speed =
                        //    evolAcademy.resetParameters["speed"];

                        GameObject herbivorousChild = herbivorousPool.GetObject();
                        herbivorousChild.transform.parent = workerObject.transform;
                        herbivorousChild.SetActive(true);
                        herbivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);
                        /*
                        GameObject godChild = godPool.GetObject();
                        godChild.transform.parent = workerObject.transform;
                        godChild.GetComponent<GodAgent>().CarnivorousPool = carnivorousPool;
                        godChild.GetComponent<GodAgent>().HerbivorousPool = herbivorousPool;
                        godChild.SetActive(true);
*/
                        GameObject herbChild = herbPool.GetObject();
                        herbChild.transform.parent = workerObject.transform;
                        herbChild.SetActive(true);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(
                            $"Object {WorkerCarniHerbi.WorkerPrefab.transform.GetChild(i).name} not found in the pool ex: {e.Message}");
                    }

                }

                workerObjects.Add(workerObject);

            }
        }

        /// <summary>
        /// Configures the agent to his curriculum, adjusting ground scale and amount of agents on academy reset
        /// </summary>
        private void ConfigureAgent()
        {
            GroundScale = (int) evolAcademy.resetParameters["ground_scale"];
            WorkerCarniHerbi.AmountOfAgentsToAdd = (int) evolAcademy.resetParameters["amount_of_agents"];
            WorkerCarniHerbi.AmountOfWorkers = (int) evolAcademy.resetParameters["amount_of_workers"];
        }

        /// <summary>
        /// This is used to reset the workers items, for example when there is a genocide,
        /// when going to the next curriculum lesson ...
        /// </summary>
        private void Reset()
        {
            // Curriculum can't be in the reverse order atm (can't increase amount of workers) anyway no point in doing that, yet?
            /*
            while (workerObjects.Count > WorkerCarniHerbi.AmountOfWorkers)
            {
                var tmp = workerObjects.Last();
                ReleaseAgentsInWorker(tmp);
                workerObjects.Remove(tmp);
                Destroy(tmp.gameObject); // TODO : we are destroying non-agents items, check if not breaking references
            }*/

            foreach (GameObject workerObject in workerObjects)
            {
                /*
                // If the ground scale changed it means new curriculum lesson
                if ((int) workerObject.transform.Find("Ground").localScale.x != GroundScale)
                {
                    // Reposition grounds to adapt to the new ground scale
                    workerObject.transform.position = new Vector3(
                        (int) ((workerObject.transform.position.x + GroundScale) * 2),
                        workerObject.transform.position.y,
                        workerObject.transform.position.z);

                    // Then set the new ground scale
                    workerObject.transform.Find("Ground").localScale = new Vector3(GroundScale,
                        1,
                        GroundScale);

                    // Adding herbs
                    for (int i = 0;
                        i < WorkerCarniHerbi.AmountOfAgentsToAdd -
                        workerObject.GetComponentsInChildren(typeof(Herb)).Length;
                        i++)
                    {
                        GameObject herbChild = herbPool.GetObject();
                        herbChild.transform.parent = workerObject.transform;
                        herbChild.SetActive(true);
                        herbChild.GetComponent<Herb>().ResetPosition(workerObject.transform);
                    }

                    foreach (Herb h in workerObject.GetComponentsInChildren<Herb>())
                        h.ResetPosition(workerObject.transform);
                }*/



                // The last condition is only useful in evolution mode
                if (workerObject.GetComponentsInChildren<CarnivorousAgent>().Length == 0
                    || workerObject.GetComponentsInChildren<HerbivorousAgent>().Length == 0
                    || workerObject.GetComponentsInChildren<LivingBeingAgent>().Length >
                    WorkerCarniHerbi.AmountOfAgentsToAdd * 10)
                {
                    resetCounter.Inc(1.1);
                    ReleaseAgentsInWorker(workerObject);
                    for (int i = 0; i < WorkerCarniHerbi.AmountOfAgentsToAdd; i++)
                    {
                        GameObject carnivorousChild = carnivorousPool.GetObject();
                        carnivorousChild.transform.parent = workerObject.transform;
                        carnivorousChild.SetActive(true);
                        carnivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);
                        carnivorousChild.GetComponent<LivingBeingAgent>().LivingBeing.Speed =
                            evolAcademy.resetParameters["speed"];

                        GameObject herbivorousChild = herbivorousPool.GetObject();
                        herbivorousChild.transform.parent = workerObject.transform;
                        herbivorousChild.SetActive(true);
                        herbivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);
                    }
                }
            }
        }

        private void ReleaseAgentsInWorker(GameObject currentWorker)
        {
            foreach (HerbivorousAgent agent in currentWorker.GetComponentsInChildren<HerbivorousAgent>())
            {
                agent.GetComponent<LivingBeingController>().ResetStats();
                agent.Done();
                herbivorousPool.ReleaseObject(agent.gameObject);
            }

            foreach (CarnivorousAgent agent in currentWorker.GetComponentsInChildren<CarnivorousAgent>())
            {
                agent.GetComponent<LivingBeingController>().ResetStats();
                agent.Done();
                carnivorousPool.ReleaseObject(agent.gameObject);
            }
        }


        private void FixedUpdate()
        {
            //if (Curriculum)
            //    ConfigureAgent();

            herbivorousInUseGauge.Set(herbivorousPool.inUse.Count);
            carnivorousInUseGauge.Set(carnivorousPool.inUse.Count);

            if (ResetWorkers)
            {
                Reset();
            }
            

        }

        private void OnApplicationQuit()
        {
            // Happens that after curriculum the scriptableobject worker isn't resetted
            // TODO: cleaner dynamic parameters reset
            WorkerCarniHerbi.AmountOfAgentsToAdd = 5;
            WorkerCarniHerbi.AmountOfWorkers = 10;
            Debug.Log("Application ending after " + Time.time + " seconds");
        }
    }
}