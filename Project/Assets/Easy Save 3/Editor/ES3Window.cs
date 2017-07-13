using UnityEngine;
using UnityEditor;

namespace ES3Editor
{
	public class ES3Window : EditorWindow
	{
		private SubWindow[] windows = null;

		public SubWindow currentWindow;

		// Add menu named "My Window" to the Window menu
		[MenuItem("Window/Easy Save 3...", false, 1000)]
		[MenuItem("Assets/Easy Save 3/Open Easy Save 3 Window...", false, 1000)]
		public static void Init()
		{
			// Get existing open window or if none, make a new one:
			ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
			window.Show();
		}
		public static void InitAndShowHome()
		{
			// Get existing open window or if none, make a new one:
			ES3Window window = (ES3Window)EditorWindow.GetWindow(typeof(ES3Window));
			window.Show();
			window.currentWindow = new HomeWindow(window);
		}

		public void InitSubWindows()
		{
			windows = new SubWindow[]{
				new HomeWindow(this),
				new SettingsWindow(this),
				new ToolsWindow(this),
				new TypesWindow(this)
			};
		}

		void OnEnable()
		{
			if(windows == null)
				InitSubWindows();
			// Set the window name and icon.
			var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(ES3EditorUtility.PathToEasySaveFolder()+"Editor/es3Logo16x16.png");
			titleContent = new GUIContent("Easy Save", icon);

			// Get the last opened window and open it.
			if(currentWindow == null)
			{
				var currentWindowName = EditorPrefs.GetString("ES3Editor.Window.currentWindow", windows[0].name);
				for(int i=0; i<windows.Length; i++)
				{
					if(windows[i].name == currentWindowName)
					{
						currentWindow = windows[i];
						break;
					}
				}
			}
		}

		void OnGUI()
		{
			var style = EditorStyle.Get;

			// Display the menu.
			EditorGUILayout.BeginHorizontal();

			for(int i=0; i<windows.Length; i++)
			{
				if(GUILayout.Button(windows[i].name, currentWindow == windows[i] ? style.menuButtonSelected : style.menuButton))
					SetCurrentWindow(windows[i]);
			}

			EditorGUILayout.EndHorizontal();

			if(currentWindow != null)
				currentWindow.OnGUI();
		}

		void SetCurrentWindow(SubWindow window)
		{
			currentWindow = window;
			EditorPrefs.SetString("ES3Editor.Window.currentWindow", window.name);
		}
	}

	public abstract class SubWindow
	{
		public string name;
		public EditorWindow parent;
		public abstract void OnGUI();

		public SubWindow(string name, EditorWindow parent)
		{
			this.name = name;
			this.parent = parent;
		}
	}
}