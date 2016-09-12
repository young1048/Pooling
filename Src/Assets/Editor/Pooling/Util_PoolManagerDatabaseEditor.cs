using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(Util_PoolManagerDatabase))]
public class Util_PoolManagerDatabaseEditor : Editor {

	Util_PoolManagerDatabase myScript;

	string [] _keys = null;

	bool _lastCento = true;

	private void OnEnable()
	{
		myScript = (Util_PoolManagerDatabase) (target);

		__refresh();
	}

	private void __refresh() {

		if(Util_PoolLoading.Instance != null) {

			_keys = new string[Util_PoolLoading.Instance._keys.Count];
			int index = 0;
			foreach(string s in Util_PoolLoading.Instance._keys) {

				_keys[index++] = s;
			}
		}
	}

	public static void RemoveAt<T>(ref T[] arr, int index)
	{
		for (int a = index; a < arr.Length - 1; a++)
		{
			// moving elements downwards, to fill the gap at [index]
			arr[a] = arr[a + 1];
		}
		// finally, let's decrement Array's size by one
		Array.Resize(ref arr, arr.Length - 1);
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		__refresh();

		// DrawDefaultInspector();

		Color c = GUI.backgroundColor;

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Database parent.", MessageType.Info);
		GUI.backgroundColor = c;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(myScript.transform.parent.name, GUILayout.MaxWidth(150));
			
		if(GUILayout.Button(">", GUILayout.MaxWidth(30))) {

			Selection.activeGameObject = myScript.transform.parent.gameObject;
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Async Pooling. Load everythig in 1 frames (sync) or bit for each frame (async)", MessageType.Info);
		GUI.backgroundColor = c;

		myScript._async = EditorGUILayout.Toggle("Async: ", myScript._async);

		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Ceate pooling when the gameobject is activated.", MessageType.Info);
		GUI.backgroundColor = c;

		myScript._loadAtStart = EditorGUILayout.Toggle("Load At Start: ", myScript._loadAtStart);

		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Enable pooling. If disable the object is create and destroy from memory.", MessageType.Warning);
		GUI.backgroundColor = c;

		myScript._noPooling = EditorGUILayout.Toggle("No pooling: ", myScript._noPooling);

		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("TAG Pooling for key database key search. ( es: Util_PoolManagerDatabase.GetInstance(tag) )", MessageType.Info);
		GUI.backgroundColor = c;

		string tag = EditorGUILayout.TextField("TAG:", myScript._tag);
		if(tag != myScript._tag) {
			myScript._tag = tag;
			myScript.name = myScript._tag;
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Objects:", MessageType.Info);
		GUI.backgroundColor = c;

		bool lastCento = EditorGUILayout.Toggle("Last 100", _lastCento);

		int count = 0;
		if(lastCento)
			count = myScript._poolingDefinitions.Length - 100;
		if(count < 0)
			count = 0;

		if(lastCento != _lastCento)
			_lastCento = lastCento;
		
		if(lastCento && count != myScript._poolingDefinitions.Length) {

			if(GUILayout.Button("...")) {

				_lastCento = false;
			}
		}

		for(int i = count; i < myScript._poolingDefinitions.Length; ++i) {

			PoolingDefinition def = myScript._poolingDefinitions[i];

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(def._object.name, GUILayout.MaxWidth(150));
			EditorGUILayout.LabelField("amount:", GUILayout.MaxWidth(64));
			int amount = EditorGUILayout.IntField(def._amount, GUILayout.MaxWidth(30));
			if(amount != def._amount) {
				def._amount = amount;
				myScript._poolingDefinitions[i] = def;
			}

			GUI.backgroundColor = Color.red;
			if(GUILayout.Button("-", GUILayout.MaxWidth(30))) {

				RemoveAt<PoolingDefinition>(ref myScript._poolingDefinitions, i);
				return;
			}
			GUI.backgroundColor = c;

			if(GUILayout.Button(">", GUILayout.MaxWidth(30))) {

				Selection.activeGameObject = def._object;
			}

			GUI.backgroundColor = c;

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Total: " + myScript._poolingDefinitions.Length);

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		//
		{
			Event evt = Event.current;
			Rect drop_area = GUILayoutUtility.GetRect (0.0f, 50.0f, GUILayout.ExpandWidth (true));
			GUI.Box (drop_area, "Drag & Drop a object");

			switch (evt.type) {
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!drop_area.Contains (evt.mousePosition))
					return;

				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag ();

					foreach (UnityEngine.Object dragged_object in DragAndDrop.objectReferences) {
						// Do On Drag Stuff here
						//Debug.Log("Drag " + dragged_object.ToString());

						PoolingDefinition [] newBuffer = new PoolingDefinition[myScript._poolingDefinitions.Length+1];
						System.Array.Copy(myScript._poolingDefinitions, newBuffer, myScript._poolingDefinitions.Length);

						if(dragged_object != null) {

							newBuffer[myScript._poolingDefinitions.Length] = new PoolingDefinition();
							newBuffer[myScript._poolingDefinitions.Length]._object = (GameObject)dragged_object;
							newBuffer[myScript._poolingDefinitions.Length]._amount = 1;
						}

						myScript._poolingDefinitions = newBuffer;
					}
				}
				break;
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		// mostra lista chiavi dentro Util_PoolLoading
		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Settare la chiave. Poi volendo durante il gioco si possono scaricare/caricare il pool che corrisponde a questa chiave", MessageType.Info);
		GUI.backgroundColor = c;

		if(_keys == null) {

			EditorGUILayout.Space();
			GUI.backgroundColor = Color.red;
			EditorGUILayout.HelpBox("Selezionare l'oggetto che ha lo script Util_PoolLoading per riempire la lista chiavi!", MessageType.Warning);
			GUI.backgroundColor = c;

			__refresh();
		}
		else {

			if(_keys.Length == 0) {

				__refresh();
			}
				
			myScript._key = EditorGUILayout.Popup("Select Key:", myScript._key, _keys); 
		}

		EditorGUILayout.Space();

		GUI.backgroundColor = Color.cyan;
		EditorGUILayout.HelpBox("Aggiungi oggetto al database!", MessageType.Info);
		GUI.backgroundColor = c;

		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("Add Object")) {

			PoolingDefinition [] newBuffer = new PoolingDefinition[myScript._poolingDefinitions.Length+1];
			System.Array.Copy(myScript._poolingDefinitions, newBuffer, myScript._poolingDefinitions.Length);
			myScript._poolingDefinitions = newBuffer;
			if(myScript._poolingDefinitions[myScript._poolingDefinitions.Length-1] != null) {
				
				myScript._poolingDefinitions[myScript._poolingDefinitions.Length-1]._amount = 1;
			}
		}
		GUI.backgroundColor = c;

		serializedObject.ApplyModifiedProperties();
	}
}
