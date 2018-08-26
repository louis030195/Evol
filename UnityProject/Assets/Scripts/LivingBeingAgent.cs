using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// This class handles the behaviour of the agent
/// </summary>
public abstract class LivingBeingAgent : Agent
{

    protected int totalActions;

    protected RayPerception rayPer;
    public LivingBeing LivingBeing { get; protected set; }
    public System.Action action;
    public float OffsetX { get; set; }
    public float GroundSize {get; set; }

    protected int amountActions = 0;
    

    public enum RewardMode
    {
        Sparse, // very high level reward : harder
        Dense // hand written reward
    }

    [Header("Reward stuff")]
    [Space(10)]
    public RewardMode rewardMode = RewardMode.Dense;


    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void AgentReset()
    {
        transform.position = new Vector3(Random.Range(-GroundSize / 2, GroundSize / 2) + OffsetX, 0.5f, Random.Range(-GroundSize / 2, GroundSize / 2));
        transform.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);

        LivingBeing.Satiety = 49;
        LivingBeing.Life = 99;
    }
}
