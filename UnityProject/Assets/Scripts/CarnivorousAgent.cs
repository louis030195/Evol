using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// This class handles the behaviour of the carnivorous agent
/// TODO : possibility that we can regroup the common code of multiple agents into a mother abstract class
/// </summary>
public class CarnivorousAgent : LivingBeingAgent
{

    public Carnivorous getLivingBeing()
    {
        return (Carnivorous)livingBeing;
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        livingBeing = new Carnivorous(99, 0, 20, 99, 0);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.01f);
        base.AgentAction(vectorAction, textAction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            print("Hit HerbivorousAgent");
            AddReward(50f);
            Done();
        }
    }

}
