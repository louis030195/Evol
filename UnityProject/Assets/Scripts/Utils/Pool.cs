using MLAgents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DesignPattern.Objectpool
{
    // The Pool class is the most important class in the object pool design pattern. It controls access to the
    // pooled objects, maintaining a list of available objects and a collection of objects that have already been
    // requested from the pool and are still in use. The pool also ensures that objects that have been released
    // are returned to a suitable state, ready for the next time they are requested. 
    public static class Pool
    {
        private static List<GameObject> _available = new List<GameObject>();
        private static List<GameObject> _inUse = new List<GameObject>();
        private static GameObject _parent;

        public static int GetAvailableCount()
        {
            return _available.Count;
        }

        public static int GetInUseCount()
        {
            return _inUse.Count;
        }

        public static void Initialize(int poolSize, List<GameObject> gameObjects, List<Brain> brains)
        {
            _parent = new GameObject("Pool");
            for(int i = 0; i < poolSize; i++)
            {
                foreach(GameObject go in gameObjects)
                {
                    GameObject goToAdd = UnityEngine.Object.Instantiate(go);
                    goToAdd.SetActive(false);
                    _available.Add(goToAdd);
                    goToAdd.transform.parent = _parent.transform;
                }
            }
            foreach (GameObject agent in _available)
                if(agent.GetComponent<Agent>() != null)
                    foreach (Brain brain in brains.Where(brain => agent.GetComponent<Agent>().GetType().Name.Contains(Regex.Split(brain.name, @"(?<!^)(?=[A-Z])")[1])))
                        agent.GetComponent<Agent>().GiveBrain(brain);
        }

        /// <summary>
        /// Return the object asked or null if it doesn't exist or the pool is empty
        /// </summary>
        /// <param name="name">Name of the GameObject we want to use</param>
        /// <returns></returns>
        public static GameObject GetObject(string name)
        {
            lock (_available)
            {
                if (_available.Count != 0)
                {
                    // We need to use "Contains" because Unity add (Clone) to the name
                    GameObject go = _available.Find(tmpGo => tmpGo.name.Contains(name));
                    if (go)
                    {
                        go.SetActive(true);
                        _inUse.Add(go);
                        _available.Remove(go);
                        go.transform.parent = null;
                    }
                    return go;
                } // Else ?
                return null;
            }
        }

        /// <summary>
        /// Deactivate the GameObject and add it to the pool
        /// </summary>
        /// <param name="go"></param>
        public static void ReleaseObject(GameObject go)
        {
            lock (_available)
            {
                go.SetActive(false);
                _available.Add(go);
                _inUse.Remove(go);
                go.transform.parent = _parent.transform;
            }
        }
    }
}