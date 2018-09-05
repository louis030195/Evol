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
    protected Rigidbody rigidBody;

    protected RayPerception rayPer;
    public LivingBeing LivingBeing { get; protected set; }
    public bool Evolve { get; set; }
    public System.Action action;
    public float moveSpeed = 20;

    protected int amountActions = 0;
    

    public enum RewardMode
    {
        Sparse, // very high level reward : harder
        Dense // hand written reward
    }

    [Header("Reward stuff")]
    [Space(10)]
    public RewardMode rewardMode = RewardMode.Dense;


    public void ResetPosition()
    {
        float groundSize = transform.parent.Find("Ground").GetComponent<MeshRenderer>().bounds.size.x / 2;
        float offsetX = transform.parent.position.x;
        transform.position = new Vector3(Random.Range(-groundSize, groundSize) + offsetX, 0.5f, Random.Range(-groundSize, groundSize));
        transform.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
        LivingBeing.Satiety = 50;
        LivingBeing.Life = 50;
    }
}
