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
    public class TrainManager : MonoBehaviour
    {
        /// <summary>
        /// Worker carnivorous-herbivorous
        /// </summary>
        [Header("Workers")] [Space(10)] public Worker WorkerCarniHerbi;
        
        /// <summary>
        /// Scale of the carnivorous-herbivorous worker's ground
        /// </summary>
        public int GroundScale = 10;
        
        /// <summary>
        /// List of all the brains needed
        /// </summary>
        public List<Brain> Brains; 

        [Header("Items To Spawn")] [Space(10)] public List<GameObject> ItemsToSpawn;

        /// <summary>
        /// Whether or not reset the workers on genocide
        /// </summary>
        [Header("Misc")] [Space(10)] public bool ResetWorkers = true;
        
        /// <summary>
        /// Whether or not activating curriculum training
        /// </summary>
        public bool Curriculum;

        /// <summary>
        /// List of instanciated workers and steps
        /// </summary>
        private List<Utils.Tuple<int, GameObject>> workerObjects;
        private int frames;
        
        /// <summary>
        /// Pool storing herbivorous go
        /// </summary>
        private Pool herbivorousPool;
        
        /// <summary>
        /// Pool storing carnivorous go
        /// </summary>
        private Pool carnivorousPool;
        
        /// <summary>
        /// Pool storing herbs go
        /// </summary>
        private Pool herbPool;
        
        /// <summary>
        /// Pool storing gods go
        /// </summary>
        private Pool godPool;
        
        /// <summary>
        /// Academy handling communication with python
        /// </summary>
        private Academy evolAcademy;
        
        // Monitoring
        private MetricServer metricServer;
        private Counter resetCounter;
        private Gauge herbivorousInUseGauge;
        private Gauge carnivorousInUseGauge;
        private Gauge herbivorousSpecieLifeExpectancyGauge;
        private Gauge carnivorousSpecieLifeExpectancyGauge;
        

        // Use this for initialization
        private void Start()
        {
            metricServer = new MetricServer(port: 1234);
            metricServer.Start();
            
            resetCounter = Metrics.CreateCounter("reset", "How many times the worker has been reset");
            herbivorousInUseGauge =
                Metrics.CreateGauge("herbivorousInUse", "Current total amount of herbivorous agents");
            carnivorousInUseGauge =
                Metrics.CreateGauge("carnivorousInUse", "Current total amount of carnivorous agents");
            herbivorousSpecieLifeExpectancyGauge =
                Metrics.CreateGauge("herbivorousSpecieLifeExpectancy", "Life expectancy of herbivorous specie");
            carnivorousSpecieLifeExpectancyGauge =
                Metrics.CreateGauge("carnivorousSpecieLifeExpectancy", "Life expectancy of carnivorous specie");
            
            evolAcademy = FindObjectOfType<EvolAcademy>();


            workerObjects = new List<Utils.Tuple<int, GameObject>>();

            // Instanciate pools to spawn / release objects
            herbivorousPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("herbivorous")));
            carnivorousPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("carnivorous")));
            herbPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("food")));
            godPool = new Pool(ItemsToSpawn.FirstOrDefault(go => go.CompareTag("god")));
           

            // Find the brains in the list
            herbivorousPool.Brain =
                Brains.FirstOrDefault(brain => "Herbivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);
            carnivorousPool.Brain =
                Brains.FirstOrDefault(brain => "Carnivorous" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);
            godPool.Brain =
                Brains.FirstOrDefault(brain => "God" == Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1]);


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

                workerObjects.Add(new Utils.Tuple<int, GameObject>(0, workerObject));

            }
            
            GameObject godChild = godPool.GetObject();
            godChild.GetComponent<GodAgent>().CarnivorousPool = carnivorousPool;
            godChild.GetComponent<GodAgent>().HerbivorousPool = herbivorousPool;
            godChild.SetActive(true);
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

            foreach (var workerObject in workerObjects)
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
                if (workerObject.second.GetComponentsInChildren<CarnivorousAgent>().Length == 0
                    || workerObject.second.GetComponentsInChildren<HerbivorousAgent>().Length == 0
                    || workerObject.second.GetComponentsInChildren<LivingBeingAgent>().Length >
                    WorkerCarniHerbi.AmountOfAgentsToAdd * 10)
                {
                    // Length of the worker
                    workerObject.first = evolAcademy.GetStepCount() - workerObject.first;
                    
                    if (workerObject.second.GetComponentsInChildren<CarnivorousAgent>().Length == 0)
                        carnivorousSpecieLifeExpectancyGauge.Set(workerObject.first);
                    if (workerObject.second.GetComponentsInChildren<HerbivorousAgent>().Length == 0)
                        herbivorousSpecieLifeExpectancyGauge.Set(workerObject.first);
                    

                    resetCounter.Inc(1.1);
                    ReleaseAgentsInWorker(workerObject.second);
                    for (int i = 0; i < WorkerCarniHerbi.AmountOfAgentsToAdd; i++)
                    {
                        GameObject carnivorousChild = carnivorousPool.GetObject();
                        carnivorousChild.transform.parent = workerObject.second.transform;
                        carnivorousChild.SetActive(true);
                        carnivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.second.transform);
                        carnivorousChild.GetComponent<LivingBeingAgent>().LivingBeing.Speed =
                            evolAcademy.resetParameters["speed"];

                        GameObject herbivorousChild = herbivorousPool.GetObject();
                        herbivorousChild.transform.parent = workerObject.second.transform;
                        herbivorousChild.SetActive(true);
                        herbivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.second.transform);
                    }
                }
            }
        }

        private void ReleaseAgentsInWorker(GameObject currentWorker)
        {
            foreach (HerbivorousAgent agent in currentWorker.GetComponentsInChildren<HerbivorousAgent>())
            {
                agent.GetComponent<LivingBeingManager>().ResetStats();
                agent.Done();
                herbivorousPool.ReleaseObject(agent.gameObject);
            }

            foreach (CarnivorousAgent agent in currentWorker.GetComponentsInChildren<CarnivorousAgent>())
            {
                agent.GetComponent<LivingBeingManager>().ResetStats();
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