using UnityEngine;
using System.Collections;

using UnityEditor;

#region menu
public static class Util_PoolingLoadingMenu
{
	[MenuItem("Tools/Pooling/Create")]
	public static void CreatePooling()
	{
		GameObject root = new GameObject("<Pooling>");
		Undo.RegisterCreatedObjectUndo(root, "ubik.pooling.create");

		Util_PoolLoading pooling = root.AddComponent<Util_PoolLoading>();
		pooling._keys = new System.Collections.Generic.List<string>();
		pooling._keys.Add("kEverytime");
		pooling._database = new System.Collections.Generic.List<Util_PoolManagerDatabase>();

		GameObject containers = new GameObject("Containers");
		containers.transform.parent = root.transform;

		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty tagsProp = tagManager.FindProperty("tags");
		bool found = false;
		for (int i = 0; i < tagsProp.arraySize; i++)
		{
			SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
			if (t.stringValue.Equals("PoolingContainer")) { found = true; break; }
		}
		// if not found, add it
		if (!found)
		{
			tagsProp.InsertArrayElementAtIndex(0);
			SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
			n.stringValue = "PoolingContainer";
			tagManager.ApplyModifiedProperties();
		}

		containers.tag = "PoolingContainer";

		pooling._containeres = containers;


		Selection.activeObject = root;
	}


	[MenuItem("Tools/Pooling/Create DataBase", validate = true)]
	private static bool CanCreateDatabase()
	{
		GameObject selected = Selection.activeObject as GameObject;
		if (selected == null)
		{
			return false;
		}

		return selected.GetComponent<Util_PoolLoading>();
	}

	[MenuItem("Tools/Pooling/Create DataBase")]
	private static void CreateDatabase()
	{
		GameObject selected = Selection.activeObject as GameObject;


		GameObject db = new GameObject("<database>");
		db.transform.parent = selected.transform;
		db.AddComponent<Util_PoolManagerDatabase>();

		Undo.RecordObject(db, "ubik.pooling.create.db");

		EditorUtility.SetDirty(db);
		Selection.activeGameObject = db;
	}
}

#endregion



[CustomEditor(typeof(Util_PoolLoading))]
public class Util_PoolLoadingEditor : Editor {

	Util_PoolLoading myScript;

	void OnEnable() {

		myScript = (Util_PoolLoading)target;
	}

	public void AddDatabase() {

		GameObject db = new GameObject("<database>");
		db.transform.parent = myScript.transform;
		db.AddComponent<Util_PoolManagerDatabase>();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		Color c = GUI.backgroundColor; // store value

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Auto play. Start initialization whe the object is acivated.", MessageType.Info);
		GUI.backgroundColor = c;

		EditorGUILayout.Space();

		myScript._autoplay = EditorGUILayout.Toggle("Play at Start: ", myScript._autoplay);

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Don't destroy this gameObject!", MessageType.Info);
		GUI.backgroundColor = c;

		EditorGUILayout.Space();

		myScript._dontDestroy = EditorGUILayout.Toggle("Don't Destroy: ", myScript._dontDestroy);

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Key list for load and unload a database. Default is kEverytime.", MessageType.Info);
		GUI.backgroundColor = c;

		EditorGUILayout.Space();

		// DrawDefaultInspector();
		for(int i = 1; i < myScript._keys.Count; ++i) {

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.TextArea(myScript._keys[i], GUILayout.MaxWidth(150));
			GUI.backgroundColor = Color.red;
			if(GUILayout.Button("-", GUILayout.MaxWidth(30))) {

				myScript._keys.RemoveAt(i);
				return;
			}
			GUI.backgroundColor = c;
			EditorGUILayout.EndHorizontal();
		}

		GUI.backgroundColor = Color.green;
		if(GUILayout.Button("Add Key")) {

			myScript._keys.Add("");
		}
		GUI.backgroundColor = c;

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Database list:", MessageType.Info);
		GUI.backgroundColor = c;

		for(int i = 0; i < myScript.transform.childCount; ++i) {
			
			Transform t = myScript.transform.GetChild(i);

			Util_PoolManagerDatabase o = t.GetComponent<Util_PoolManagerDatabase>();
			if(o != null) {

				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(t.name, GUILayout.MaxWidth(150));
				GUI.backgroundColor = Color.red;
				if(GUILayout.Button("-", GUILayout.MaxWidth(30))) {

					DestroyImmediate(t.gameObject);
					myScript._database.RemoveAt(i);
					return;
				}

				GUI.backgroundColor = c;
				if(GUILayout.Button(">", GUILayout.MaxWidth(30))) {

					Selection.activeGameObject = t.gameObject;
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Space();


		GUI.backgroundColor = Color.green;
		if(GUILayout.Button("Add Database")) {

			AddDatabase();
		}
		GUI.backgroundColor = c;

		Util_PoolLoading.Instance = myScript;

		serializedObject.ApplyModifiedProperties();
	}

	void OnSceneGUI() {

		Util_PoolLoading.Instance = myScript;
	}
}
