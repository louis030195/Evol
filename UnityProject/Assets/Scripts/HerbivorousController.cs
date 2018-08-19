using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivorousController : LivingBeingController
{
    // Use this for initialization
    protected override void Start()
    {
        livingBeingAgent = GetComponent<HerbivorousAgent>();
        livingBeing = livingBeingAgent.LivingBeing;
    }
}
