using DesignPattern.Objectpool;
using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used to update stats of the agent
/// </summary>
public abstract class LivingBeingController : MonoBehaviour {

    public bool evolve = true;

    protected LivingBeingAgent livingBeingAgent;
    protected LivingBeing livingBeing;
    protected float now;

    // Use this for initialization
    protected virtual void Start () {
        livingBeingAgent = GetComponent<LivingBeingAgent>();
        livingBeingAgent.Evolve = evolve;
        livingBeingAgent.action = DoAction;
        livingBeing = livingBeingAgent.LivingBeing;
	}

    // Update is called once per frame
    protected virtual void DoAction () {
        livingBeing.Life -= 0.01f;

        if (livingBeing.Life < 0)
        {
            // print($"{ transform.name } - I'm dead");
            livingBeingAgent.AddReward(-10f);
            if (evolve)
            {
                //Destroy(gameObject.GetComponentInParent<MeshFilter>());
                //Destroy(gameObject);
                Pool.ReleaseObject(gameObject);
            }
            else
                livingBeingAgent.ResetPosition();
            livingBeingAgent.Done();


        }

        livingBeing.Life = livingBeing.Life > 100 ?
            100 : livingBeing.Life;

        livingBeing.Satiety = livingBeing.Satiety > 100 ?
            100 : livingBeing.Satiety;
    }

    
    private void OnEnable()
    {
        //now = Time.fixedTime;
    }

    private void OnDisable()
    {
        /*
        if (Time.fixedTime > 100 && Time.fixedTime % 50 < 10)
        {
            System.IO.File.AppendAllText(@"positions.txt", $"\n Time : {Time.fixedTime} seconds" +
                $"\n Length of life : {Time.fixedTime - now} seconds");
        }*/
    }
}
