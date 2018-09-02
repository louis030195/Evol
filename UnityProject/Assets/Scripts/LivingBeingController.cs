using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to update stats of the agent
/// </summary>
public abstract class LivingBeingController : MonoBehaviour {


    protected LivingBeingAgent livingBeingAgent;
    protected LivingBeing livingBeing;

	// Use this for initialization
	protected virtual void Start () {
        livingBeingAgent = GetComponent<LivingBeingAgent>();
        livingBeingAgent.action = DoAction;
        livingBeing = livingBeingAgent.LivingBeing;
	}

    // Update is called once per frame
    protected virtual void DoAction () {
        //livingBeing.Life += livingBeing.Satiety >= 50 ? 0.1f : -0.1f;
        //livingBeing.Satiety -= 0.1f;
        livingBeing.Life -= 0.001f;
        //print(livingBeing.Life);

        if (livingBeing.Life < 0)
        {
            livingBeingAgent.Done();
            Destroy(gameObject);
        }

        livingBeing.Life = livingBeing.Life > 100 ?
            100 : livingBeing.Life;

        livingBeing.Satiety = livingBeing.Satiety > 100 ?
            100 : livingBeing.Satiety;
    }

}
