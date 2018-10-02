using MLAgents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Evol.Agents;

namespace Evol.Utils
{
    // The Pool class is the most important class in the object pool design pattern. It controls access to the
    // pooled objects, maintaining a list of available objects and a collection of objects that have already been
    // requested from the pool and are still in use. The pool also ensures that objects that have been released
    // are returned to a suitable state, ready for the next time they are requested. 
    public class Pool
    {
        private List<GameObject> available;
        private List<GameObject> inUse;
        private GameObject prefab;
        private GameObject parent;

        public Brain Brain { get; set; } // Auto because not all items are agents

        public Pool(GameObject prefab)
        {
            available = new List<GameObject>();
            inUse = new List<GameObject>();
            this.prefab = prefab;

            parent = new GameObject($"Pool_{prefab.name}");
        }

        /// <summary>
        /// Return the object asked or null if it doesn't exist or the pool is empty
        /// </summary>
        /// <param name="name">Name of the GameObject we want to use</param>
        /// <returns></returns>
        public GameObject GetObject()
        {
            if (available.Count != 0)
            {
                GameObject go = available[0];
                inUse.Add(go);
                available.RemoveAt(0);
                //go.transform.parent = null;
                    
                return go;
            }
            else
            {
                GameObject go = UnityEngine.Object.Instantiate(prefab);
                if (go.GetComponent<LivingBeingController>() != null)
                    go.GetComponent<LivingBeingController>().Pool = this;
                if (go.GetComponent<LivingBeingAgent>() != null)
                {
                    go.GetComponent<Agent>().GiveBrain(Brain);
                }
                inUse.Add(go);
                return go;
            }
        }

        /// <summary>
        /// Deactivate the GameObject and add it to the pool
        /// </summary>
        /// <param name="go"></param>
        public void ReleaseObject(GameObject go)
        {
            available.Add(go);
            inUse.Remove(go);
            go.transform.parent = parent.transform;
            go.SetActive(false);
        }


        public override string ToString()
        {
            return ($"\nAvailable objects : {available.Count}" +
                $"\nIn use objects : {inUse.Count}");
        }
    }
}