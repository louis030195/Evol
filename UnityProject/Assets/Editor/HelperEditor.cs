using UnityEngine;
using UnityEditor;
using System.Collections;

class HelperEditor : EditorWindow
{    
    GUISkin skin;
    private Texture2D m_Logo = null;
    Vector2 rect = new Vector2(380, 460);

    void OnEnable()
    {
        m_Logo = (Texture2D)Resources.Load("logo", typeof(Texture2D));
    }

    void OnGUI()
    {        
        this.titleContent = new GUIContent("About");
        this.minSize = rect;

        GUILayout.Label(m_Logo, GUILayout.MaxHeight(240));

        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;        

        GUILayout.BeginVertical("window");       

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();        
	    GUILayout.Label("Basic Locomotion FREE VERSION: 1.0c", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Check for Update"))
        {
            UnityEditorInternal.AssetStore.Open("/content/82048");
            this.Close();
        }
        GUILayout.EndHorizontal();        
        
        EditorGUILayout.Space();
      

        EditorGUILayout.Space();        
        EditorGUILayout.HelpBox("UPDATE INSTRUCTIONS: \n\n *ALWAYS BACKUP YOUR PROJECT BEFORE UPDATE!* \n\n Delete the Invector's Folder from the Project before import the new version", MessageType.Info);        
        
        GUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}
