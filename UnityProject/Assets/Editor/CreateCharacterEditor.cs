using Evol.Game.Player;
using Photon.Pun;
using UnityEngine;
using UnityEditor;
using PlayerManager = Photon.Pun.Demo.PunBasics.PlayerManager;

public class CreateCharacterEditor : EditorWindow
{
    private GUISkin skin;
    private GameObject charObj;
    private Animator charAnimator;
    private RuntimeAnimatorController controller;   
    private string characterName;
    private Vector2 rect = new Vector2(500, 630);
    private Vector2 scrool;
    private Editor humanoidpreview;
		
    /// <summary>
	/// 3rdPersonController Menu 
    /// </summary>    
    [MenuItem("Evol/Create Character", false, -1)]
    public static void CreateNewCharacter()
    {
        GetWindow<CreateCharacterEditor>();
    }
   
    private bool isHuman, isValidAvatar, charExist;

    private void OnGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        minSize = rect;
        titleContent = new GUIContent("Character", null, "Character Creator");
       
        GUILayout.BeginVertical("Character Creator Window", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("box");

        if (!charObj)
            EditorGUILayout.HelpBox("Make sure your FBX model is set as Humanoid!", MessageType.Info);
        else if (!charExist)
            EditorGUILayout.HelpBox("Missing a Animator Component", MessageType.Error);
        else if (!isHuman)
            EditorGUILayout.HelpBox("This is not a Humanoid", MessageType.Error);
        else if (!isValidAvatar)
            EditorGUILayout.HelpBox(charObj.name + " is a invalid Humanoid", MessageType.Info);

        charObj = EditorGUILayout.ObjectField("FBX Model", charObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;

        if (GUI.changed && charObj != null && charObj.GetComponent<PlayerManager>()==null)
            humanoidpreview = Editor.CreateEditor(charObj);
        if(charObj != null && charObj.GetComponent<PlayerManager>() != null)        
            EditorGUILayout.HelpBox("This gameObject already contains the component PlayerManager", MessageType.Warning);        

        controller = EditorGUILayout.ObjectField("Animator Controller: ", controller, typeof(RuntimeAnimatorController), false) as RuntimeAnimatorController;
      
        characterName = EditorGUILayout.TextField("New Character");
        
        GUILayout.EndVertical();

        if (charObj)
            charAnimator = charObj.GetComponent<Animator>();
        charExist = charAnimator != null;
        isHuman = charExist && charAnimator.isHuman;
        isValidAvatar = charExist && charAnimator.avatar.isValid;

        if (CanCreate())
        {
            DrawHumanoidPreview();
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

    private bool CanCreate()
    {
        return isValidAvatar && isHuman && charObj!=null && charObj.GetComponent<PlayerManager>()==null;
    }   

    /// <summary>
    /// Draw the Preview window
    /// </summary>
    private void DrawHumanoidPreview()
    {
        GUILayout.FlexibleSpace();

        if (humanoidpreview != null)
        {
            humanoidpreview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 400), "window");
        }
    }

    /// <summary>
    /// Created the Third Person Controller
    /// </summary>
    private void Create()
    {
        // base for the character
        var character = Instantiate(charObj, Vector3.zero, Quaternion.identity) as GameObject;
        if (!character)
            return;             

        character.name = characterName;     
        
        character.AddComponent<Mana>(); 
        character.AddComponent<Health>(); 
        character.AddComponent<BasicBehaviour>(); 
        character.AddComponent<MoveBehaviour>(); 
        character.AddComponent<CastBehaviour>(); 
        character.AddComponent<PlayerManager>(); 
        var photonView = character.AddComponent<PhotonView>();
        var photonTransformView = character.AddComponent<PhotonTransformView>();
        var photonRigidbodyView = character.AddComponent<PhotonRigidbodyView>();
        var photonAnimatorView = character.AddComponent<PhotonAnimatorView>();
        photonView.ObservedComponents.Add(photonTransformView);
        photonView.ObservedComponents.Add(photonRigidbodyView);
        photonView.ObservedComponents.Add(photonAnimatorView);

        character.AddComponent<Rigidbody>();
        var rigidbody = character.GetComponent<Rigidbody>();
        character.AddComponent<CapsuleCollider>();
        var collider = character.GetComponent<CapsuleCollider>();
         
        // camera
        if (Camera.main == null)
        {
            var cam = new GameObject("Camera");
            cam.AddComponent<ThirdPersonOrbitCamBasic>();           
            cam.AddComponent<AudioListener>();
            cam.tag = "MainCamera";
            cam.transform.SetParent(character.transform);
            // camera.GetComponent<Camera>().nearClipPlane = 0.01f;
        }
     
        character.tag = "Player";
      
        // rigidbody
        rigidbody.useGravity = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigidbody.mass = 50;

        // capsule collider 
        collider.height = ColliderHeight(character.GetComponent<Animator>());
        collider.center = new Vector3(0, (float)System.Math.Round(collider.height * 0.5f, 2), 0);
        collider.radius = (float)System.Math.Round(collider.height * 0.15f, 2);

        if (controller)
            character.GetComponent<Animator>().runtimeAnimatorController = controller;
        
        Close();
        
    }

    /// <summary>
    /// Capsule Collider height based on the Character height
    /// </summary>
    /// <param name="animator">animator humanoid</param>
    /// <returns></returns>
    float ColliderHeight(Animator animator)
    {
        var foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        return (float)System.Math.Round(Vector3.Distance(foot.position, hips.position) * 2f, 2);
    }  

}
