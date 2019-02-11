using Evol.Utils;
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
        /// List of instanciated workers and his life length in steps
        /// The Tuple(int, int) represents the latest reset step, the second, the life length
        /// </summary>
        private List<Utils.Tuple<Utils.Tuple<int, int>, GameObject>> workerObjects;
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

        /// <summary>
        /// Object of the god agent
        /// </summary>
        private GameObject godChild;
        
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
            if (!Application.isEditor)
            {
                metricServer = new MetricServer(port: 1234);
                metricServer.Start();
            }
            

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
           


            workerObjects = new List<Utils.Tuple<Utils.Tuple<int, int>, GameObject>>();

            // Instanciate pools to spawn / release objects
            herbivorousPool = new Pool(ItemsToSpawn.Find(go => go.name.Equals("HerbivorousAgent")), false);
            carnivorousPool = new Pool(ItemsToSpawn.Find(go => go.name.Equals("CarnivorousAgent")), false);
            herbPool = new Pool(ItemsToSpawn.Find(go => go.name.Equals("Herb")), false);
            godPool = new Pool(ItemsToSpawn.Find(go => go.name.Equals("GodAgent")), false);
            SpawnWorkers();
        }

        private void SpawnWorkers()
        {
            int w = 0;
            for (; w < WorkerCarniHerbi.AmountOfWorkers; w++)
            {
                var groundSize = WorkerCarniHerbi.WorkerPrefab.transform.Find("Ground").GetComponent<MeshRenderer>() == null
                    ? WorkerCarniHerbi.WorkerPrefab.transform.Find("Ground").GetComponent<Terrain>().terrainData.size.x / 2
                    : WorkerCarniHerbi.WorkerPrefab.transform.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x / 2;

                GameObject workerObject = Instantiate(WorkerCarniHerbi.WorkerPrefab,
                    new Vector3(2 * groundSize * 10 * w, 0, 0),
                    new Quaternion(0, 0, 0, 0));
                //workerObject.transform.Find("Ground").localScale = new Vector3(GroundScale, 1, GroundScale);
                for (int i = 0; i < WorkerCarniHerbi.AmountOfAgentsToAdd; i++)
                {
                    try
                    {
                        var carnivorousChild = carnivorousPool.GetObject();
                        carnivorousChild.transform.parent = workerObject.transform;
                        carnivorousChild.SetActive(true);
                        carnivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);
                        //carnivorousChild.GetComponent<LivingBeingAgent>().LivingBeing.Speed =
                        //    evolAcademy.resetParameters["speed"];

                        var herbivorousChild = herbivorousPool.GetObject();
                        herbivorousChild.transform.parent = workerObject.transform;
                        herbivorousChild.SetActive(true);
                        herbivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.transform);
                        


                        var herbChild = herbPool.GetObject();
                        herbChild.transform.parent = workerObject.transform;
                        herbChild.SetActive(true);

                    }
                    catch (Exception e)
                    {
                        Debug.Log(
                            $"Object {WorkerCarniHerbi.WorkerPrefab.transform.GetChild(i).name} not found in the pool ex: {e.Message}");
                    }

                }

                workerObjects.Add(new Utils.Tuple<Utils.Tuple<int, int>, GameObject>(new Utils.Tuple<int, int>(0, 0), workerObject));

            }
            
            godChild = godPool.GetObject();
            godChild.GetComponent<GodAgent>().CarnivorousPool = carnivorousPool;
            godChild.GetComponent<GodAgent>().HerbivorousPool = herbivorousPool;
            godChild.GetComponent<GodAgent>().ElementsToSpawn = WorkerCarniHerbi.AmountOfAgentsToAdd;
            godChild.SetActive(true);
        }

        /// <summary>
        /// Configures the agent to his curriculum, adjusting ground scale and amount of agents on academy reset
        /// </summary>
        private void ConfigureAgent()
        {
            /*
            GroundScale = (int) evolAcademy.resetParameters["ground_scale"];
            WorkerCarniHerbi.AmountOfAgentsToAdd = (int) evolAcademy.resetParameters["amount_of_agents"];
            WorkerCarniHerbi.AmountOfWorkers = (int) evolAcademy.resetParameters["amount_of_workers"];
            */
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



                // The last condition is only useful in reproduction mode
                if (workerObject.second.GetComponentsInChildren<CarnivorousAgent>().Length == 0
                    || workerObject.second.GetComponentsInChildren<HerbivorousAgent>().Length == 0
                    || workerObject.second.GetComponentsInChildren<LivingBeingAgent>().Length >
                    WorkerCarniHerbi.AmountOfAgentsToAdd * 10)
                {
                    // The god agent can tweak number of elements to add
                    WorkerCarniHerbi.AmountOfAgentsToAdd = godChild.GetComponent<GodAgent>().ElementsToSpawn;
                    
                    // Life length of the worker
                    workerObject.first.second = evolAcademy.GetStepCount() - workerObject.first.first;
                    
                    // Latest reset in term of steps
                    workerObject.first.first = evolAcademy.GetStepCount();

                    if (workerObject.second.GetComponentsInChildren<CarnivorousAgent>().Length == 0)
                    {
                        carnivorousSpecieLifeExpectancyGauge.Set(workerObject.first.second);
                        godChild.GetComponent<GodAgent>().CarnivorousSpeciesLifeExpectancy = workerObject.first.second;
                    }

                    if (workerObject.second.GetComponentsInChildren<HerbivorousAgent>().Length == 0)
                    {
                        herbivorousSpecieLifeExpectancyGauge.Set(workerObject.first.second);
                        godChild.GetComponent<GodAgent>().HerbivorousSpeciesLifeExpectancy = workerObject.first.second;
                    }


                    resetCounter.Inc(1.1);
                    ReleaseAgentsInWorker(workerObject.second);
                    for (var i = 0; i < WorkerCarniHerbi.AmountOfAgentsToAdd; i++)
                    {
                        var carnivorousChild = carnivorousPool.GetObject();
                        carnivorousChild.transform.parent = workerObject.second.transform;
                        carnivorousChild.SetActive(true);
                        carnivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.second.transform);
                        // carnivorousChild.GetComponent<LivingBeingAgent>().LivingBeing.Speed =
                        //    evolAcademy.resetParameters["speed"];

                        var herbivorousChild = herbivorousPool.GetObject();
                        herbivorousChild.transform.parent = workerObject.second.transform;
                        herbivorousChild.SetActive(true);
                        herbivorousChild.GetComponent<LivingBeingAgent>().ResetPosition(workerObject.second.transform);
                    }
                }
            }
        }

        private void ReleaseAgentsInWorker(GameObject currentWorker)
        {
            foreach (var agent in currentWorker.GetComponentsInChildren<HerbivorousAgent>())
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
            /*
            WorkerCarniHerbi.AmountOfAgentsToAdd = 5;
            WorkerCarniHerbi.AmountOfWorkers = 10;
            */
            Debug.Log("Application ending after " + Time.time + " seconds");
        }
    }
}