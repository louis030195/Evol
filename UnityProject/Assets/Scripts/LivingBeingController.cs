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
    public Pool Pool { get; set; }

    protected LivingBeingAgent livingBeingAgent;
    protected LivingBeing livingBeing;
    protected float now;

    // Use this for initialization
    protected virtual void Start () {
        livingBeingAgent = GetComponent<LivingBeingAgent>();
        livingBeingAgent.Evolve = evolve;
        livingBeingAgent.action = DoAction;
        livingBeingAgent.Pool = Pool;
        livingBeing = livingBeingAgent.LivingBeing;
        now = Time.fixedTime;
    }

    // Update is called once per frame
    protected virtual void DoAction () {
        // Start losing life after 200 actions done
        
        //if(livingBeingAgent.AmountActions > 50)
        livingBeing.Life -= 0.1f;
        

        if (transform.position.y < 0)
            livingBeing.Life = -1;
        

        // To avoid dying instantly before having his stats resetted
        if (livingBeing.Life < 0)
        {
            ResetStats();
            livingBeingAgent.AddReward(-10f);
            
            if (evolve)
                Pool.ReleaseObject(gameObject);

            livingBeingAgent.Done();


        }

        livingBeing.Life = livingBeing.Life > 100 ?
            100 : livingBeing.Life;

        livingBeing.Satiety = livingBeing.Satiety > 100 ?
            100 : livingBeing.Satiety;
    }

    public void ResetStats()
    {
        livingBeing.Satiety = 50;
        livingBeing.Life = 50;
    }

    private void OnDisable()
    {

        if (Time.fixedTime > 100 && Time.fixedTime % 50 < 10)
        {
            if (!System.IO.File.Exists(@"disable.txt"))
                System.IO.File.Create(@"disable.txt");
            if (Time.fixedTime % 400 < 10)
                System.IO.File.WriteAllText(@"disable.txt", "");
            System.IO.File.AppendAllText(@"disable.txt", $"\n-------------" +
                $"\n Disabled {name} at {Time.fixedTime} seconds" +
                $"\nLength of life : {Time.fixedTime - now} seconds" +
                $"\nPosition|Rotation : {transform.position}|{transform.rotation}" +
                $"\nLivingBeing \n{livingBeing.ToString()}" +
                $"RigidBody velocity {GetComponent<Rigidbody>().velocity}");
        }
    }
}
