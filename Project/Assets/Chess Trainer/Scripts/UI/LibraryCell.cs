using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryCell : MonoBehaviour {
    public TMPro.TextMeshProUGUI txtName = null;
    public string name { get; set; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetName(string _name)
    {
        name = _name;
        ctLibrary lib = ctGameManager.Singleton.GetLibrary(_name);
        if (lib == null)
            return;

        txtName.text = string.Format("{0} ({1})", lib.name, lib.lines.Count);
    }
}
