using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivorousController : LivingBeingController
{
    // Use this for initialization
    protected override void Start()
    {
        livingBeingAgent = GetComponent<CarnivorousAgent>();
        livingBeing = livingBeingAgent.LivingBeing;
    }

}
