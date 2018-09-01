using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/// <summary>
/// This class handles the behaviour of the herbivorous agent
/// </summary>
[RequireComponent(typeof(JointDriveController))] // Required to set joint forces
public class JointAgent : LivingBeingAgent
{
    /*
    [Header("Body Parts")] [Space(10)] public Transform body;
    public Transform leg0Upper;
    public Transform leg0Lower;
    public Transform leg1Upper;
    public Transform leg1Lower;
    public Transform leg2Upper;
    public Transform leg2Lower;
    public Transform leg3Upper;
    public Transform leg3Lower;

    [Header("Joint Settings")]
    [Space(10)]
    JointDriveController jdController;


    bool isNewDecisionStep;
    int currentDecisionStep;
    
    
    Vector3 prevPosition;



    public override void InitializeAgent()
    {
        prevPosition = transform.GetChild(0).position;

        jdController = GetComponent<JointDriveController>();
        currentDecisionStep = 1;

        //Setup each body part
        jdController.SetupBodyPart(body);
        jdController.SetupBodyPart(leg0Upper);
        jdController.SetupBodyPart(leg0Lower);
        jdController.SetupBodyPart(leg1Upper);
        jdController.SetupBodyPart(leg1Lower);
        jdController.SetupBodyPart(leg2Upper);
        jdController.SetupBodyPart(leg2Lower);
        jdController.SetupBodyPart(leg3Upper);
        jdController.SetupBodyPart(leg3Lower);

        LivingBeing = new Herbivorous(99, 0, 20, 99, 0);
        rayPer = GetComponent<RayPerception>();
    }

    /// <summary>
    /// We only need to change the joint settings based on decision freq.
    /// </summary>
    public void IncrementDecisionTimer()
    {
        if (currentDecisionStep == agentParameters.numberOfActionsBetweenDecisions
            || agentParameters.numberOfActionsBetweenDecisions == 1)
        {
            currentDecisionStep = 1;
            isNewDecisionStep = true;
        }
        else
        {
            currentDecisionStep++;
            isNewDecisionStep = false;
        }
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(BodyPart bp)
    {
        var rb = bp.rb;
        AddVectorObs(bp.groundContact.touchingGround ? 1 : 0); // Whether the bp touching the ground
        AddVectorObs(rb.velocity);
        AddVectorObs(rb.angularVelocity);

        if (bp.rb.transform != body)
        {
            Vector3 localPosRelToBody = body.InverseTransformPoint(rb.position);
            AddVectorObs(localPosRelToBody);
            AddVectorObs(bp.currentXNormalizedRot); // Current x rot
            AddVectorObs(bp.currentYNormalizedRot); // Current y rot
            AddVectorObs(bp.currentZNormalizedRot); // Current z rot
            AddVectorObs(bp.currentStrength / jdController.maxJointForceLimit);
        }
    }

    public override void CollectObservations()
    {
        jdController.GetCurrentJointForces();

        // Forward & up to help with orientation
        AddVectorObs(body.transform.position.y);
        AddVectorObs(body.forward);
        AddVectorObs(body.up);
        foreach (var bodyPart in jdController.bodyPartsDict.Values)
        {
            CollectObservationBodyPart(bodyPart);
        }
    }


    /// <summary>
    /// Agent touched the target
    /// </summary>
    public void TouchedTarget()
    {
        AddReward(1f);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        foreach (var bodyPart in jdController.bodyPartsDict.Values)
        {
            if (bodyPart.touchingSomething != null && !IsDone() && bodyPart.touchingSomething.touchedGameObject != null)
            {
                if (bodyPart.touchingSomething.touchedGameObject.GetComponentInParent<Herb>() != null)
                {
                    LivingBeing.Satiety += 100;
                    if (rewardMode == RewardMode.Dense)
                    {
                        print("I ate something");
                        AddReward(20f);
                        Done();
                    }
                }
                else if (bodyPart.touchingSomething.touchedGameObject.GetComponentInParent<CarnivorousAgent>() != null)
                {
                    if (rewardMode == RewardMode.Dense)
                    {
                        print("hit carnivorous");
                        AddReward(-1f);
                    }
                }

            }
        }


        action();
        if (rewardMode == RewardMode.Sparse)
        {

            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 999)
            {
                AddReward(10f);
                amountActions = 0;
                Done();
            }

            if (LivingBeing.Life == 0 || transform.GetChild(0).position.y < -10) // Strange, the parent y position doesn't change but the child, so we check child pos
            {
                AddReward(-10f);
                amountActions = 0;
                Done();
            }

        }

        else if (rewardMode == RewardMode.Dense)
        {
            AddReward(-0.01f);
            // Reset every 1000 actions or when the agent fell
            if (amountActions >= 1000)
            {
                //AddReward(-10f);
                //print("I finished after " + amountActions + " actions");
                //amountActions = 0;
                //Done();
            }

            if (transform.GetChild(0).position.y < -2f)
            {
                print("I jumped from the board after " + amountActions + " actions");
                AddReward(-10f);
                amountActions = 0;
                Done();
            }

            if (Vector3.Distance(transform.GetChild(0).position, prevPosition) > 0)
            {
                AddReward(0.01f);
                prevPosition = transform.GetChild(0).position;
            }
        }

        // Joint update logic only needs to happen when a new decision is made
        if (isNewDecisionStep)
        {
            // The dictionary with all the body parts in it are in the jdController
            var bpDict = jdController.bodyPartsDict;

            int i = -1;
            // Pick a new target joint rotation
            bpDict[leg0Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
            bpDict[leg1Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
            bpDict[leg2Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
            bpDict[leg3Upper].SetJointTargetRotation(vectorAction[++i], vectorAction[++i], 0);
            bpDict[leg0Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);
            bpDict[leg1Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);
            bpDict[leg2Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);
            bpDict[leg3Lower].SetJointTargetRotation(vectorAction[++i], 0, 0);

            // Update joint strength
            bpDict[leg0Upper].SetJointStrength(vectorAction[++i]);
            bpDict[leg1Upper].SetJointStrength(vectorAction[++i]);
            bpDict[leg2Upper].SetJointStrength(vectorAction[++i]);
            bpDict[leg3Upper].SetJointStrength(vectorAction[++i]);
            bpDict[leg0Lower].SetJointStrength(vectorAction[++i]);
            bpDict[leg1Lower].SetJointStrength(vectorAction[++i]);
            bpDict[leg2Lower].SetJointStrength(vectorAction[++i]);
            bpDict[leg3Lower].SetJointStrength(vectorAction[++i]);
        }


        IncrementDecisionTimer();
    }



    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    public override void AgentReset()
    {
        foreach (var bodyPart in jdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }

        isNewDecisionStep = true;
        currentDecisionStep = 1;
        
        LivingBeing.Satiety = 49;
        LivingBeing.Life = 99;
    }*/
}
