using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections.Generic;
using System.Reflection;

[InitializeOnLoad]
public class ES3Postprocessor : AssetPostprocessor
{
	public static ES3ReferenceMgr _refMgr;
	public static bool didGenerateReferences = false;
	public static ES3DefaultSettings settings;


	// This constructor is called once when playmode is activated.
	static ES3Postprocessor()
	{
		EditorApplication.update += GenerateReferences;
	}

	/*[PostProcessSceneAttribute (0)]
	public static void OnPostprocessScene() 
	{
		// Generate prefab references first as we'll need to get it's dependencies in GenerateReferences()
		//GeneratePrefabReferences();
		//GenerateReferences();
	}

	[PostProcessBuildAttribute (0)]
	public static void OnPostprocessBuild() 
	{
	}*/

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		GeneratePrefabReferences();
	}

	public static void GenerateReferences()
	{
		var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		if(didGenerateReferences || !scene.isDirty || !scene.isLoaded || EditorApplication.isPlaying)
		{
			didGenerateReferences = false;
			return;
		}

		var refMgr = GetReferenceMgr();
		if(refMgr == null)
			return;

		bool undoRecorded = false;

		Undo.RecordObject(refMgr, "Generate Reference List");
		didGenerateReferences = true;

		// Remove NULL values from Dictionary.
		if(refMgr.idRef.RemoveNullValues() > 0)
		{
			Undo.RecordObject(refMgr, "Update Easy Save 3 Reference List");
			undoRecorded = true;
		}

		var sceneObjects = scene.GetRootGameObjects();

		foreach(var go in sceneObjects)
		{
			var dependencies = EditorUtility.CollectDependencies(sceneObjects);

			for(int i=0; i<dependencies.Length; i++)
			{
				var obj = (UnityEngine.Object)dependencies[i];

				// If we're adding a new item to the type list, make sure we've recorded an undo for the object.
				if(refMgr.Get(obj) == -1)
				{
					if(!undoRecorded)
					{
						Undo.RecordObject(refMgr, "Update Easy Save 3 Reference List");
						undoRecorded = true;
					}
					refMgr.Add(obj);
				}
			}
		}
	}

	public static void GeneratePrefabReferences()
	{
		var refMgr = GetReferenceMgr();
		if(refMgr == null)
			return;

		bool undoRecorded = false;

		// Remove null values from prefab array.
		if(refMgr != null)
		{
			if(refMgr.prefabs.RemoveAll(item => item == null) > 0)
			{
				Undo.RecordObject(refMgr, "Update Easy Save 3 Reference List");
				undoRecorded = true;
			}
		}

		var es3Prefabs = Resources.FindObjectsOfTypeAll<ES3Prefab>();

		if(es3Prefabs.Length == 0)
			return;

		foreach(var es3Prefab in es3Prefabs)
		{
			Debug.Log(es3Prefab);
			if(PrefabUtility.GetPrefabType(es3Prefab.gameObject) != PrefabType.Prefab)
				continue;

			if(refMgr != null)
			{
				if(refMgr.GetPrefab(es3Prefab) != -1)
				{
					refMgr.AddPrefab(es3Prefab);
					if(!undoRecorded)
					{
						Undo.RecordObject(refMgr, "Update Easy Save 3 Reference List");
						undoRecorded = true;
					}
				}
			}

			bool prefabUndoRecorded = false;

			if(es3Prefab.localRefs.RemoveNullKeys() > 0)
			{
				Undo.RecordObject(es3Prefab, "Update Easy Save 3 Prefab");
				prefabUndoRecorded = true;
			}

			// Get GameObject and it's children and add them to the reference list.
			foreach(var obj in EditorUtility.CollectDependencies(new UnityEngine.Object[]{es3Prefab}))
			{
				if(es3Prefab.Get(obj) != -1)
				{
					es3Prefab.Add(obj);
					if(!prefabUndoRecorded)
					{
						Undo.RecordObject(es3Prefab, "Update Easy Save 3 Prefab");
						prefabUndoRecorded = true;
					}
				}
			}
		}
	}

	private static ES3ReferenceMgr GetReferenceMgr()
	{
		if(_refMgr == null)
		{
			var refMgrs = GameObject.FindObjectsOfType<ES3ReferenceMgr>();

			if(refMgrs.Length > 1)
				Debug.LogError("More than one ES3ReferenceMgr in scene.");
			else if(refMgrs.Length == 1)
				_refMgr = refMgrs[0];
			else
			{
				if(ES3EditorUtility.GetDefaultSettings().addMgrToSceneAutomatically)
					_refMgr = ES3ReferenceMgrEditor.AddManagerToScene();
			}
		}
		return _refMgr;
	}
}
