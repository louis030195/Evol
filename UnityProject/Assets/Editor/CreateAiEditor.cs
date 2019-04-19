using Evol.Game.Item;
using Evol.Game.Player;
using Evol.Heuristic;
using Evol.Heuristic.StateMachine;
using Photon.Pun;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class CreateAiEditor : EditorWindow
{
    private GUISkin skin;
    private GameObject charObj;
    private Animator charAnimator;
    private RuntimeAnimatorController controller;
    private string aiName;
    private Vector2 rect = new Vector2(500, 630);
    private Vector2 scrool;
    private Editor humanoidpreview;
		
    [MenuItem("Evol/Create Ai", false, -1)]
    public static void CreateNewAi()
    {
        GetWindow<CreateAiEditor>();
    }
   
    private bool isHuman, isValidAvatar, charExist;

    private void OnGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        minSize = rect;
        titleContent = new GUIContent("AI", null, "Ai Creator");
       
        GUILayout.BeginVertical("Ai creator Window", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("box");

        if (!charExist)
            EditorGUILayout.HelpBox("Missing a Animator Component", MessageType.Error);

        charObj = EditorGUILayout.ObjectField("FBX Model", charObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;

        if (GUI.changed && charObj != null && charObj.GetComponent<StateController>()==null)
            humanoidpreview = Editor.CreateEditor(charObj);
        if(charObj != null && charObj.GetComponent<StateController>() != null)        
            EditorGUILayout.HelpBox("This gameObject already contains the component StateController", MessageType.Warning);        

        controller = EditorGUILayout.ObjectField("Animator Controller: ", controller, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;

        aiName = EditorGUILayout.TextField("New Ai");
        
        GUILayout.EndVertical();

        if (charObj)
            charAnimator = charObj.GetComponent<Animator>();
        charExist = charAnimator != null;
        isValidAvatar = charExist ? charAnimator.avatar.isValid : false;

        if (CanCreate())
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (controller != null)
            {
                if (GUILayout.Button("Create"))
                    Create();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    bool CanCreate()
    {
        return isValidAvatar && charObj!=null && charObj.GetComponent<StateController>()==null;
    }   

    /// <summary>
    /// Created the Third Person Controller
    /// </summary>
    private void Create()
    {
        // base for the character
        var ai = Instantiate(charObj, Vector3.zero, Quaternion.identity);
        if (!ai)
            return;             

        ai.name = aiName;     
        
        ai.AddComponent<StateController>();

        var rigidbody = ai.AddComponent<Rigidbody>();
        var collider = ai.AddComponent<CapsuleCollider>();
        ai.AddComponent<Movement>();
        ai.AddComponent<Health>();
        ai.AddComponent<Attack>();
        ai.AddComponent<NavMeshAgent>();
        ai.AddComponent<Loot>();
        var photonView = ai.AddComponent<PhotonView>();
        var photonTransformView = ai.AddComponent<PhotonTransformView>();
        var photonAnimatorView = ai.AddComponent<PhotonAnimatorView>();
        photonView.ObservedComponents.Add(photonTransformView);
        photonView.ObservedComponents.Add(photonAnimatorView);
        
        // rigidbody
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigidbody.mass = 50;

        // capsule collider 
        // Won't work on non humanoid animator
        /*
        collider.height = ColliderHeight(ai.GetComponent<Animator>());
        collider.center = new Vector3(0, (float)System.Math.Round(collider.height * 0.5f, 2), 0);
        collider.radius = (float)System.Math.Round(collider.height * 0.15f, 2);
        */

        var eyes = new GameObject("Eyes");
        eyes.transform.SetParent(ai.transform);
        
        if (controller)
            ai.GetComponent<Animator>().runtimeAnimatorController = controller;
        
        Close();
        
    }

    /// <summary>
    /// Capsule Collider height based on the Character height
    /// </summary>
    /// <param name="animator">animator humanoid</param>
    /// <returns></returns>
    private float ColliderHeight(Animator animator)
    {
        var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        return (float)System.Math.Round(Vector3.Distance(foot.position, hips.position) * 2f, 2);
    }  

}
