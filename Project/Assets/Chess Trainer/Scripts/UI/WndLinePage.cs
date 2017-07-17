using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WndLinePage : MonoBehaviour {
    public TMPro.TextMeshProUGUI Lbl_Title = null;
    public TMPro.TextMeshProUGUI Lbl_MoveCount = null;
    public TMPro.TextMeshProUGUI Lbl_Moves = null;

    protected string lineName = "";
    protected ctLine line = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape))
        {
            WndManager.Singleton.OpenLibraryPage(line.parent.name);
        }
	}

    public void Init(string _lineName)
    {
        lineName = _lineName;
        line = ctGameManager.Singleton.GetLine(_lineName);

        if (line == null)
            return;


        Lbl_Title.text = _lineName;
        Lbl_MoveCount.text = string.Format("({0}) moves", line.moves.MoveCount());
        Lbl_Moves.text = line.moves.ToString();
    }
}
