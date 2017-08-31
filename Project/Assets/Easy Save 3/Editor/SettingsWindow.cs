using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ES3Editor
{
	public class SettingsWindow : SubWindow
	{
		public ES3DefaultSettings editorSettings = null;
		public ES3Settings settings = null;
		private bool showAdvancedSettings = false;

		public SettingsWindow(EditorWindow window) : base("Settings", window){}

		public override void OnGUI()
		{
			if(settings == null || editorSettings == null)
				Init();

			var style = EditorStyle.Get;

			EditorGUILayout.BeginVertical(style.area);

			GUILayout.Label("Runtime Settings", style.heading);

			EditorGUILayout.BeginVertical(style.area);

			settings.location = (ES3.Location)EditorGUILayout.EnumPopup("Location", settings.location);
			// If the location is File, show the Directory.
			if(settings.location == ES3.Location.File)
				settings.directory = (ES3.Directory)EditorGUILayout.EnumPopup("Directory", settings.directory);

			settings.path = EditorGUILayout.TextField("Default File Path", settings.path);

			EditorGUILayout.Space();

			settings.encryptionType = (ES3.EncryptionType)EditorGUILayout.EnumPopup("Encryption Type", settings.encryptionType);
			settings.encryptionPassword = EditorGUILayout.TextField("Encryption Password", settings.encryptionPassword);

			EditorGUILayout.Space();

			if(showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Runtime Settings"))
			{
				EditorGUILayout.BeginVertical(style.area);

				settings.format = (ES3.Format)EditorGUILayout.EnumPopup("Format", settings.format);
				settings.bufferSize = EditorGUILayout.IntField("Buffer Size", settings.bufferSize);

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();

			GUILayout.Label("Editor Settings", style.heading);

			EditorGUILayout.BeginVertical(style.area);

			EditorGUILayout.BeginHorizontal();
			var wideLabel = new GUIStyle();
			wideLabel.fixedWidth = 400;
			EditorGUILayout.PrefixLabel("Auto Add Manager to Scene", wideLabel);
			editorSettings.addMgrToSceneAutomatically = EditorGUILayout.Toggle(editorSettings.addMgrToSceneAutomatically);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndVertical();
		}

		public void Init()
		{
			editorSettings = ES3EditorUtility.GetDefaultSettings();
			settings = editorSettings.settings;
		}
	}

}
