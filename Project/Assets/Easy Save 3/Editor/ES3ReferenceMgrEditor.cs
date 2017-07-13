using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

[CustomEditor(typeof(ES3ReferenceMgr))]
[System.Serializable]
public class ES3ReferenceMgrEditor : Editor
{
	public bool isDraggingOver = false;

	public override void OnInspectorGUI() 
	{
		var mgr = (ES3ReferenceMgr)serializedObject.targetObject;

		mgr.openReferences = EditorGUILayout.Foldout(mgr.openReferences, "References");
		// Make foldout drag-and-drop enabled for objects.
		if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
		{
			Event evt = Event.current;

			switch (evt.type) 
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					isDraggingOver = true;
					break;
				case EventType.DragExited:
					isDraggingOver = false;
					break;
			}

			if(isDraggingOver)
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (evt.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					Undo.RecordObject(mgr, "Add References to Easy Save 3 Reference List");
					foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
						mgr.Add(obj);
					// Return now because otherwise we'll change the GUI during an event which doesn't allow it.
					return;
				}
			}
		}
			
		if(mgr.openReferences)
		{
			EditorGUI.indentLevel++;

			var keys = mgr.idRef.Keys;
			var values = mgr.idRef.Values;

			for(int i=0; i<keys.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				var value = EditorGUILayout.ObjectField(values[i], typeof(UnityEngine.Object), true);
				var key = EditorGUILayout.LongField(keys[i]);

				EditorGUILayout.EndHorizontal();

				if(value != values[i] || key != keys[i])
				{
					Undo.RecordObject(mgr, "Change Easy Save 3 References");

					// If we're deleting a value, delete it.
					if(value == null)
						mgr.Remove(key);
					// Else, update the ID.
					else
						mgr.idRef.ChangeKey(keys[i], key);
				}
			}


			EditorGUI.indentLevel--;
		}

		if(GUILayout.Button("Refresh References"))
		{
			//ES3Postprocessor.GenerateReferences();
			//ES3Postprocessor.GeneratePrefabReferences();
		}
	}

	[MenuItem("GameObject/Easy Save 3/Enable Easy Save for Scene", false, 1002)]
	[MenuItem("Assets/Easy Save 3/Enable Easy Save for Scene", false, 1002)]
	public static void EnableForScene()
	{
		var scene = SceneManager.GetActiveScene();
		if(!scene.isLoaded)
			EditorUtility.DisplayDialog("Could not enable Easy Save", "Could not enable Easy Save because there is not currently a scene open.", "Ok");
		var es3RefMgr = AddManagerToScene();
		Selection.activeObject = es3RefMgr.gameObject;
	}

	[MenuItem("GameObject/Easy Save 3/Enable Easy Save for Scene", true, 1002)]
	[MenuItem("Assets/Easy Save 3/Enable Easy Save for Scene", true, 1002)]
	private static bool CanEnableForScene()
	{
		var scene = SceneManager.GetActiveScene();
		if(!scene.isLoaded)
			return false;
		if(UnityEngine.Object.FindObjectOfType<ES3ReferenceMgr>() != null)
			return false;
		return true;
	}

	public static ES3ReferenceMgr AddManagerToScene()
	{
		var go = GameObject.Find("Easy Save 3 Manager");
		ES3ReferenceMgr es3RefMgr;
		if(go == null)
		{
			go = new GameObject("Easy Save 3 Manager");
			es3RefMgr = go.AddComponent<ES3ReferenceMgr>();
			Undo.RegisterCreatedObjectUndo(go, "Enabled Easy Save for Scene");
		}
		else
		{
			es3RefMgr = go.GetComponent<ES3ReferenceMgr>();
			if(es3RefMgr == null)
			{
				es3RefMgr = go.AddComponent<ES3ReferenceMgr>();
				Undo.RegisterCreatedObjectUndo(es3RefMgr, "Enabled Easy Save for Scene");
			}
		}
		return es3RefMgr;
	}
}
