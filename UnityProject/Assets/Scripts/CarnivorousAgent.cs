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
    public override void InitializeAgent()
    {
        LivingBeing = new Carnivorous(99, 0, 20, 99, 0);
        base.InitializeAgent();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        action();
        AddReward(-0.01f);
        base.AgentAction(vectorAction, textAction);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<HerbivorousAgent>() != null)
        {
            print("Hit HerbivorousAgent");
            LivingBeing.Life += 10;
            collision.collider.GetComponent<HerbivorousAgent>().LivingBeing.Life -= 25;
            AddReward(50f);
            Done();
        }
    }

}
