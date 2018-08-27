
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// Used to store relevant information for acting and learning for each body part in agent.
    /// </summary>
    [System.Serializable]
    public class ItemPart
    {
        [Header("Body Part Info")] [Space(10)] public ConfigurableJoint joint;
        public Rigidbody rb;
        [HideInInspector] public Vector3 startingPos;
        [HideInInspector] public Quaternion startingRot;

        [Header("Ground & Target Contact")] [Space(10)]
        public MLAgents.GroundContact groundContact;

        public MLAgents.TargetContact targetContact;

        [HideInInspector] public JointController thisJDController;

        [Header("Current Joint Settings")] [Space(10)]
        public Vector3 currentEularJointRotation;

        [HideInInspector] public float currentStrength;
        public float currentXNormalizedRot;
        public float currentYNormalizedRot;
        public float currentZNormalizedRot;

        [Header("Other Debug Info")] [Space(10)]
        public Vector3 currentJointForce;

        public float currentJointForceSqrMag;
        public Vector3 currentJointTorque;
        public float currentJointTorqueSqrMag;
        public AnimationCurve jointForceCurve = new AnimationCurve();
        public AnimationCurve jointTorqueCurve = new AnimationCurve();

        /// <summary>
        /// Reset body part to initial configuration.
        /// </summary>
        public void Reset(ItemPart bp)
        {
            bp.rb.transform.position = bp.startingPos;
            bp.rb.transform.rotation = bp.startingRot;
            bp.rb.velocity = Vector3.zero;
            bp.rb.angularVelocity = Vector3.zero;
            if (bp.groundContact)
            {
                bp.groundContact.touchingGround = false;
            }

            if (bp.targetContact)
            {
                bp.targetContact.touchingTarget = false;
            }
        }

        /// <summary>
        /// Apply torque according to defined goal `x, y, z` angle and force `strength`.
        /// </summary>
        public void SetJointTargetRotation(float x, float y, float z)
        {
            x = (x + 1f) * 0.5f;
            y = (y + 1f) * 0.5f;
            z = (z + 1f) * 0.5f;

            var xRot = Mathf.Lerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, x);
            var yRot = Mathf.Lerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, y);
            var zRot = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);

            currentXNormalizedRot =
                Mathf.InverseLerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, xRot);
            currentYNormalizedRot = Mathf.InverseLerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, yRot);
            currentZNormalizedRot = Mathf.InverseLerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, zRot);

            joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
            currentEularJointRotation = new Vector3(xRot, yRot, zRot);
        }

        public void SetJointStrength(float strength)
        {
            var rawVal = (strength + 1f) * 0.5f * thisJDController.maxJointForceLimit;
            var jd = new JointDrive
            {
                positionSpring = thisJDController.maxJointSpring,
                positionDamper = thisJDController.jointDampen,
                maximumForce = rawVal
            };
            joint.slerpDrive = jd;
            currentStrength = jd.maximumForce;
        }
    }

    public class JointController : MonoBehaviour
    {
        [Header("Joint Drive Settings")] [Space(10)]
        public float maxJointSpring;

        public float jointDampen;
        public float maxJointForceLimit;
        float facingDot;

        [HideInInspector] public Dictionary<Transform, ItemPart> ItemPartsDict = new Dictionary<Transform, ItemPart>();

        [HideInInspector] public List<ItemPart> ItemPartsList = new List<ItemPart>();

        /// <summary>
        /// Create ItemPart object and add it to dictionary.
        /// </summary>
        public void SetupItemPart(Transform t)
        {
            ItemPart bp = new ItemPart
            {
                rb = t.GetComponent<Rigidbody>(),
                joint = t.GetComponent<ConfigurableJoint>(),
                startingPos = t.position,
                startingRot = t.rotation
            };
            bp.rb.maxAngularVelocity = 100;

            // Add & setup the ground contact script
            bp.groundContact = t.GetComponent<MLAgents.GroundContact>();
            if (!bp.groundContact)
            {
                bp.groundContact = t.gameObject.AddComponent<MLAgents.GroundContact>();
                bp.groundContact.agent = gameObject.GetComponent<MLAgents.Agent>();
            }
            else
            {
                bp.groundContact.agent = gameObject.GetComponent<MLAgents.Agent>();
            }

            // Add & setup the target contact script
            bp.targetContact = t.GetComponent<MLAgents.TargetContact>();
            if (!bp.targetContact)
            {
                bp.targetContact = t.gameObject.AddComponent<MLAgents.TargetContact>();
            }

            bp.thisJDController = this;
            ItemPartsDict.Add(t, bp);
            ItemPartsList.Add(bp);
        }

        public void GetCurrentJointForces()
        {
            foreach (var ItemPart in ItemPartsDict.Values)
            {
                if (ItemPart.joint)
                {
                    ItemPart.currentJointForce = ItemPart.joint.currentForce;
                    ItemPart.currentJointForceSqrMag = ItemPart.joint.currentForce.magnitude;
                    ItemPart.currentJointTorque = ItemPart.joint.currentTorque;
                    ItemPart.currentJointTorqueSqrMag = ItemPart.joint.currentTorque.magnitude;
                    if (Application.isEditor)
                    {
                        if (ItemPart.jointForceCurve.length > 1000)
                        {
                            ItemPart.jointForceCurve = new AnimationCurve();
                        }

                        if (ItemPart.jointTorqueCurve.length > 1000)
                        {
                            ItemPart.jointTorqueCurve = new AnimationCurve();
                        }

                        ItemPart.jointForceCurve.AddKey(Time.time, ItemPart.currentJointForceSqrMag);
                        ItemPart.jointTorqueCurve.AddKey(Time.time, ItemPart.currentJointTorqueSqrMag);
                    }
                }
            }
        }
    }

