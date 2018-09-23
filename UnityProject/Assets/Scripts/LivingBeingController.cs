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
    protected float now;

    // Use this for initialization
    protected virtual void Start () {
        livingBeingAgent = GetComponent<LivingBeingAgent>();
        livingBeingAgent.Evolve = evolve;
        livingBeingAgent.action = DoAction;
        livingBeingAgent.Pool = Pool;
        now = Time.fixedTime;
    }

    // Update is called once per frame
    protected virtual void DoAction ()
    {
        //print("act" + now + "\n" + livingBeing.ToString());
        livingBeingAgent.LivingBeing.Life -= 0.1f;
        

        if (transform.position.y < 0)
            livingBeingAgent.LivingBeing.Life = -1;
        

        // To avoid dying instantly before having his stats resetted
        if (livingBeingAgent.LivingBeing.Life < 0)
        {
            //print("dying " + livingBeingAgent.LivingBeing.Life);
            livingBeingAgent.AddReward(-10f);
            livingBeingAgent.Done();
            if (evolve)
                Pool.ReleaseObject(gameObject);
        }

        livingBeingAgent.LivingBeing.Life = livingBeingAgent.LivingBeing.Life > 100 ?
            100 : livingBeingAgent.LivingBeing.Life;

        livingBeingAgent.LivingBeing.Satiety = livingBeingAgent.LivingBeing.Satiety > 100 ?
            100 : livingBeingAgent.LivingBeing.Satiety;
    }

    public void ResetStats()
    {
        livingBeingAgent.LivingBeing.Satiety = 50;
        livingBeingAgent.LivingBeing.Life = 50;
    }

    private void OnDisable()
    {

        ResetStats();
        if (Time.fixedTime > 100 && Time.fixedTime % 50 < 10)
        {
            if (!System.IO.File.Exists(@"disable.txt"))
                System.IO.File.Create(@"disable.txt");
            if (Time.fixedTime % 400 < 10)
                System.IO.File.WriteAllText(@"disable.txt", "");
            System.IO.File.AppendAllText(@"disable.txt", $"\n-------------" +
                $"\n Disabled {name} at {Time.fixedTime} seconds" +
                $"\nLength of life : {livingBeingAgent.AmountActions} actions" +
                $"\nPosition|Rotation : {transform.position}|{transform.rotation}" +
                $"\nLivingBeing \n{livingBeingAgent.LivingBeing.ToString()}" +
                $"RigidBody velocity {GetComponent<Rigidbody>().velocity}");
        }
    }
}
