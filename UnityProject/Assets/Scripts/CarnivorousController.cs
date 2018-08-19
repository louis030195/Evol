using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivorousController : LivingBeingController
{
    // Use this for initialization
    protected override void Start()
    {
        livingBeing = livingBeingAgent.getLivingBeing<Carnivorous>();
    }

    protected override void Update()
    {
        base.Update();
    }

}
