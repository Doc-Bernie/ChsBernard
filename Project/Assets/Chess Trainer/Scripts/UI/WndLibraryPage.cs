using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using uFileBrowser;

public class WndLibraryPage : MonoBehaviour {
    public GameObject LineList = null;
    public GameObject LineListCell = null;
    public TMPro.TextMeshProUGUI TxtLibName = null;
    public TMPro.TextMeshProUGUI TxtLineCount = null;

    public MonoBehaviour ScrollRect = null;

    public GameObject BtnAdd;
    public GameObject BtnTrainer;
    public GameObject BtnRename;
    public GameObject BtnDelete;
    public GameObject BtnLoadFromPGN;

    protected string libName = "";
    protected ctLibrary library = null;
    protected List<Toggle> cells = new List<Toggle>();
    protected bool toggleMode = false;

    int curLineIdx = 0;
    string loadedNotation = "";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void OnEscape () {
        if (toggleMode)
            SetToggleMode(false);
        else
            WndManager.Singleton.OpenMainPage();
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
        SetToggleMode(false);
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
//            tgl.group = tglGrp;

            cells.Add(childObj.GetComponent<Toggle>());
        }
    }

    void UpdateWnd()
    {
        ClearList();

        AddLines();
    }


    public void OnBtnTrain()
    {
        if (toggleMode)
        {
            List<string> lineList = new List<string>();
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<Toggle>().isOn & library.lines[i].moveList.Count > 0)
                    lineList.Add(library.lines[i].name);
            }

            if (lineList.Count > 0)
            {
                WndManager.Singleton.OpenColorSettingWnd(CallbackColorSetting);
            }
            else
            {
                WndManager.Singleton.OpenMsgBox("Cannot train empty lines.");
            }
        }
        else
        {
            SetToggleMode(true);
        }
    }

    public void OnBtnAdd()
    {
        SetToggleMode(false); 
        WndManager.Singleton.OpenInputBox("Enter Line's name :", CallBackAdd);
    }

    public void OnBtnRemove()
    {
        WndManager.Singleton.OpenMsgBox("Do you really want to delete selected lines?", CallBackRemove, WndManager.MSGBOX_BTN_TYPE.YesNo);
    }

    public void OnBtnRename()
    {
        SetToggleMode(false);
        WndManager.Singleton.OpenInputBox("Enter Line's name :", CallBackRename, GetCurLine().name);
    }

    public void OnBtnLoadFromPGN()
    {
        WndManager.Singleton.OpenFileBrowser();
    }

    public void OnSelectPGN(string _path)
    {
        try
        {
            StreamReader sr = new StreamReader(_path);
            string line;
            string strNot = "";
            bool started = false;

            using (sr)
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (!started)
                    {
                        if (line.Equals("") || line[0] == '[')
                            continue;
                        else
                            started = true;
                    }

                    line = line.Trim();
                    strNot += line + " ";

                    if (strNot.Contains("1-0") || strNot.Contains("0-1") || strNot.Contains("1/2-1/2"))
                        break;
                }
            }

            int nCommentStart  = 0;
            while ((nCommentStart = strNot.IndexOf("{")) != -1)
            {
                int nCommentEnd = strNot.IndexOf("}");
                if (nCommentEnd != -1)
                    strNot = strNot.Remove(nCommentStart, nCommentEnd - nCommentStart + 1);
                else
                    strNot = strNot.Remove(nCommentStart, strNot.Length - nCommentStart);

                nCommentStart = strNot.IndexOf("{");
            }

            while ((nCommentStart = strNot.IndexOf("(")) != -1)
            {
                int nCommentEnd = strNot.IndexOf(")");
                if (nCommentEnd != -1)
                    strNot = strNot.Remove(nCommentStart, nCommentEnd - nCommentStart + 1);
                else
                    strNot = strNot.Remove(nCommentStart, strNot.Length - nCommentStart);
            }

            cgNotation not = new cgNotation(new cgBoard());
            not.Read(strNot);

            loadedNotation = not.writeFullNotation(cgNotation.NotationType.Algebraic);

            if (!loadedNotation.Equals(""))
                WndManager.Singleton.OpenInputBox("Enter line name:", CallBackAdd);
            else
                WndManager.Singleton.OpenMsgBox("Invalid file format");
        }
        catch (System.Exception e)
        {
            WndManager.Singleton.OpenMsgBox(string.Format("Failed to open \"{0}\"file.", _path));
        }
    }

    ctLine GetCurLine()
    {
        return library.GetLine(curLineIdx);
    }

    public void OnListSelChanged(bool _check)
    {
        UpdateButtonState();
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
        library.AddLine(line, true);
        if (!loadedNotation.Equals(""))
        {
            line.moves = loadedNotation;
            loadedNotation = "";
        }
        //UpdateWnd();

        // direct go to line page
        WndManager.Singleton.OpenLinePage(library.name, _name);

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
            List<string> lineList = new List<string>();
            int i;
            for (i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<Toggle>().isOn)
                    lineList.Add(library.lines[i].name);
            }

            for (i = 0; i < lineList.Count; i++)
                library.RemoveLine(lineList[i]);

            UpdateWnd();
            SetToggleMode(false);
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
            SetToggleMode(true);
        }
        else
        {
            WndManager.Singleton.OpenLinePage(library.name, line.name);
        }

        return true;
    }

    public void CallbackColorSetting(bool _isWhite)
    {
        List<ctLine> lineList = new List<ctLine>();
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].GetComponent<Toggle>().isOn & library.lines[i].moveList.Count > 0)
                lineList.Add(library.lines[i]);
        }

        WndManager.Singleton.OpenTrainPage(_isWhite, lineList, false);
    }

    void SetToggleMode(bool _mode)
    {
        toggleMode = _mode;

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].GetComponent<LibraryCell>().SetToggleMode(toggleMode);
            cells[i].GetComponent<Toggle>().isOn = false;
        }

        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        BtnAdd.SetActive(!toggleMode);
        BtnLoadFromPGN.SetActive(!toggleMode);

        if (toggleMode)
        {
            int selCount = 0;
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<Toggle>().isOn)
                {
                    selCount++;
                    curLineIdx = i;
                }
            }

            BtnTrainer.SetActive(selCount > 0);
            BtnRename.SetActive(selCount == 1);
            BtnDelete.SetActive(selCount > 0);
        }
        else
        {
            BtnTrainer.SetActive(true);
            BtnRename.SetActive(false);
            BtnDelete.SetActive(false);
        }
    }

}
