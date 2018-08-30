using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivorousController : LivingBeingController
{
    // Use this for initialization
    protected override void Start()
    {
        livingBeingAgent = GetComponent<LivingBeingAgent>();
        livingBeing = livingBeingAgent.LivingBeing;
        livingBeingAgent.action = DoAction;
    }
}
