using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WndLibraryPage : MonoBehaviour {
    public GameObject LineList = null;
    public GameObject LineListCell = null;
    public TMPro.TextMeshProUGUI TxtLibName = null;
    public TMPro.TextMeshProUGUI TxtLineCount = null;

    protected string libName = "";
    protected ctLibrary library = null;
    protected List<Toggle> cells = new List<Toggle>();

    int curLineIdx = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            WndManager.Singleton.OpenMainPage();
        }
	}

    public void Init(string _libName)
    {
        libName = _libName;
        TxtLibName.text = libName;
        library = ctGameManager.Singleton.GetLibrary(libName);

        if (library == null)
        {
            library = null;

            Debug.Log("Cannot find library, name = " + libName);
            return;
        }

        curLineIdx = 0;
        UpdateWnd();
    }

    protected void ClearList()
    {
        cells.Clear();
        for (int i = 0; i < LineList.transform.childCount; i++)
            Destroy(LineList.transform.GetChild(i).gameObject);
    }

    protected void AddLines()
    {
        int linesCount = library.lines.Count;
        if (linesCount <= 0)
            return;

        ToggleGroup tglGrp = LineList.GetComponent<ToggleGroup>();
        tglGrp.allowSwitchOff = true;

        curLineIdx = Mathf.Clamp(curLineIdx, 0, linesCount - 1);
        for (int i = 0; i < linesCount; i++)
        {
            ctLine line = library.lines[i];


            GameObject childObj = GameObject.Instantiate(LineListCell);
            childObj.transform.SetParent(LineList.transform, false);
            childObj.GetComponent<LineCell>().SetName(line.name);
            Toggle tgl = childObj.GetComponent<Toggle>();
            tgl.isOn = (i == curLineIdx);
            tgl.onValueChanged.AddListener(OnListSelChanged);
            tgl.group = tglGrp;

            cells.Add(childObj.GetComponent<Toggle>());
        }

        tglGrp.allowSwitchOff = false;
        TxtLineCount.text = string.Format("({0}) lines", library.lines.Count);
    }

    void UpdateWnd()
    {
        ClearList();

        AddLines();
    }


    public void OnBtnTrain()
    {
        if (GetCurLine() == null)
            return;

        // to do
    }

    public void OnBtnAdd()
    {
        WndManager.Singleton.OpenInputBox("Enter Line's name :", CallBackAdd);
    }

    public void OnBtnRemove()
    {
        if (GetCurLine() == null)
            return;

        string strPrompt = string.Format("Do you really want to remove \"{0}\" library?", GetCurLine().name);
        WndManager.Singleton.OpenMsgBox(strPrompt, CallBackRemove, WndManager.MSGBOX_BTN_TYPE.YesNo);
    }

    public void OnBtnRename()
    {
        if (GetCurLine() == null)
            return;

        WndManager.Singleton.OpenInputBox("Enter Line's name :", CallBackRename);
    }

    public void OnBtnView()
    {
        if (GetCurLine() == null)
            return;

        WndManager.Singleton.OpenLinePage(GetCurLine().name);
    }

    ctLine GetCurLine()
    {
        return library.GetLine(curLineIdx);
    }

    public void OnListSelChanged(bool _check)
    {
        if (_check)
        {
            for (int i = 0; i < cells.Count; i++)
                if (cells[i].isOn)
                    curLineIdx = i;
        }
    }

    bool CallBackAdd(string _name)
    {
        if (library.GetLine(_name) != null)
        {
            WndManager.Singleton.OpenMsgBox("Duplicated name.");
            return false;
        }

        ctLine line = new ctLine(library);
        line.name = _name;
        library.lines.Add(line);

        UpdateWnd();

        return true;
    }

    bool CallBackRename(string _name)
    {
        if (library.GetLine(_name) != null)
        {
            WndManager.Singleton.OpenMsgBox("Duplicated name.");
            return false;
        }

        library.RenameLine(GetCurLine().name, _name);

        UpdateWnd();

        return true;
    }

    bool CallBackRemove(WndManager.MSGBOX_BTN _btn)
    {
        if (_btn == WndManager.MSGBOX_BTN.YES)
        {
            library.RemoveLine(GetCurLine().name);
            UpdateWnd();
        }
        return true;
    }

}
