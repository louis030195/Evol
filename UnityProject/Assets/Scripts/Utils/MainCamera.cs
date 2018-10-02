using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evol.Utils
{
    public class MainCamera : MonoBehaviour
    {

        //This is Main Camera in the scene
        Camera mainCamera;

        void Start()
        {
            //This gets the Main Camera from the scene
            mainCamera = Camera.main;
            //This enables Main Camera
            mainCamera.enabled = true;
        }
    }
}
