using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WndLibraryPage : MonoBehaviour {
    public GameObject LineList = null;
    public GameObject LineListCell = null;
    public TMPro.TextMeshProUGUI TxtLibName = null;
    public TMPro.TextMeshProUGUI TxtLineCount = null;
    public GameObject PopupMenu = null;
    public RectTransform RTMenu = null;
    public MonoBehaviour ScrollRect = null;

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
        showPopupMenu(false);
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
        TxtLineCount.text = string.Format("({0}) lines", linesCount);
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

            string text = string.Format("{0} ({1})", line.name, line.moveList.Count);
            childObj.GetComponent<LibraryCell>().Init(i, text, ScrollRect, CallbackTouchCell);
            Toggle tgl = childObj.GetComponent<Toggle>();
            tgl.isOn = (i == curLineIdx);
            tgl.onValueChanged.AddListener(OnListSelChanged);
            tgl.group = tglGrp;

            cells.Add(childObj.GetComponent<Toggle>());
        }

        tglGrp.allowSwitchOff = false;
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
        showPopupMenu(false);

    }

    public void OnBtnRename()
    {
        if (GetCurLine() == null)
            return;

        WndManager.Singleton.OpenInputBox("Enter Line's name :", CallBackRename, GetCurLine().name);
        showPopupMenu(false);

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
        if (_name.Equals(""))
        {
            WndManager.Singleton.OpenMsgBox("The name cannot be empty.");
            return false;
        }

        if (library.GetLine(_name) != null)
        {
            WndManager.Singleton.OpenMsgBox("Duplicated name.");
            return false;
        }

        ctLine line = new ctLine(library, _name);
        library.lines.Add(line);

        //UpdateWnd();

        // direct go to line page
        WndManager.Singleton.OpenLinePage(_name);

        return true;
    }

    bool CallBackRename(string _name)
    {
        if (_name.Equals(""))
        {
            WndManager.Singleton.OpenMsgBox("The name cannot be empty.");
            return false;
        }

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

    public bool CallbackTouchCell(int _id, bool _longTouch, PointerEventData _eventData)
    {
        ctLine line = library.GetLine(_id);
        if (line == null)
            return true;

        curLineIdx  = _id;
        if (_longTouch)
        {
            showPopupMenu(true);
            RTMenu.position = _eventData.position;
        }
        else
        {
            WndManager.Singleton.OpenLinePage(line.name);
        }

        return true;
    }

    void showPopupMenu(bool _vis = true)
    {
        PopupMenu.SetActive(_vis);
    }
}
