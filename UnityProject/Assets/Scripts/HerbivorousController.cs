using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivorousController : LivingBeingController
{
    // Use this for initialization
    protected override void Start()
    {
        livingBeing = livingBeingAgent.getLivingBeing<Herbivorous>();
    }

    protected override void Update()
    {
        base.Update();
    }
}
