using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WndMainPage : MonoBehaviour {
    public GameObject LibraryList = null;
    public GameObject LibraryListCell = null;
    public TMPro.TextMeshProUGUI TxtLibCount = null;

    protected List<GameObject> cells = new List<GameObject>();

    int curLibIdx = 0;

	// Use this for initialization
	void Awake() {
        Init();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape))
        {
            WndManager.Singleton.OpenMsgBox("Do you really want exit?", CallBackExit, WndManager.MSGBOX_BTN_TYPE.YesNo);
        }
	}

    void ClearList()
    {
        for (int i = 0; i < LibraryList.transform.childCount; i++)
            Destroy(LibraryList.transform.GetChild(i).gameObject);

        cells.Clear();
    }

    void AddLibraries()
    {
        int countLib = ctGameManager.Singleton.m_Libraries.Count;
        if (countLib <= 0)
            return;

        LibraryList.GetComponent<ToggleGroup>().allowSwitchOff = true;

        curLibIdx = Mathf.Clamp(curLibIdx, 0, countLib - 1);
        for (int i = 0; i < ctGameManager.Singleton.m_Libraries.Count; i++)
        {
            ctLibrary lib = ctGameManager.Singleton.m_Libraries[i];

            GameObject childObj = GameObject.Instantiate(LibraryListCell);
            //childObj.transform.parent = LibraryList.transform;
            childObj.transform.SetParent(LibraryList.transform, false);
            childObj.GetComponent<LibraryCell>().SetName(lib.name);
            childObj.GetComponent<Toggle>().group = LibraryList.GetComponent<ToggleGroup>();
            childObj.GetComponent<Toggle>().isOn = curLibIdx == i ? true : false;
            childObj.GetComponent<Toggle>().onValueChanged.AddListener(OnLibCellChanged);

            cells.Add(childObj);
        }

        LibraryList.GetComponent<ToggleGroup>().allowSwitchOff = false;

        TxtLibCount.text = string.Format("({0}) libraries", ctGameManager.Singleton.m_Libraries.Count);
    }
    
    void UpdateWnd()
    {
        ClearList();

        AddLibraries();
    }

    public void Init()
    {
        curLibIdx = 0;
        UpdateWnd();
    }

    public void OnBtnView()
    {
        if (GetCurLibName().Equals(string.Empty))
            return;

        WndManager.Singleton.OpenLibraryPage(GetCurLibName());
    }

    public void OnBtnAdd()
    {
        WndManager.Singleton.OpenInputBox("Enter Library's name :", CallBackAdd);
    }

    public void OnBtnRemove()
    {
        if (GetCurLibName().Equals(string.Empty))
            return;

        string strPrompt = string.Format("Do you really want to remove {0} library?", GetCurLibName());
        WndManager.Singleton.OpenMsgBox(strPrompt, CallBackRemove, WndManager.MSGBOX_BTN_TYPE.YesNo);
    }

    public void OnBtnRename()
    {
        if (GetCurLibName().Equals(string.Empty))
            return;

        WndManager.Singleton.OpenInputBox("Enter Library's name :", CallBackRename);
    }

    public void OnBtnTrainer()
    {
        if (GetCurLibName().Equals(string.Empty))
            return;

        // to do
    }

    bool CallBackAdd(string _name)
    {
        if (ctGameManager.Singleton.GetLibrary(_name) != null)
        {
            WndManager.Singleton.OpenMsgBox("Duplicated name.");
            return false;
        }

        ctLibrary lib = new ctLibrary();
        lib.name = _name;
        ctGameManager.Singleton.m_Libraries.Add(lib);

        UpdateWnd();
        return true;
    }

    bool CallBackRename(string _name)
    {
        if (ctGameManager.Singleton.GetLibrary(_name) != null)
        {
            WndManager.Singleton.OpenMsgBox("Duplicated name.");
            return false;
        }

        ctGameManager.Singleton.RenameLibrary(GetCurLibName(), _name);
        UpdateWnd();
        return true;
    }

    bool CallBackRemove(WndManager.MSGBOX_BTN _btn)
    {
        if (_btn == WndManager.MSGBOX_BTN.YES)
        {
            ctGameManager.Singleton.RemoveLibrary(GetCurLibName());
            UpdateWnd();
        }
        return true;
    }

    bool CallBackExit(WndManager.MSGBOX_BTN _btn)
    {
        if (_btn == WndManager.MSGBOX_BTN.YES)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif         
        }
        return true;
    }
    public void OnLibCellChanged(bool check)
    {
        if (check)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<Toggle>().isOn)
                {
                    curLibIdx = i;
                    break;
                }
            }
        }
    }

    string GetCurLibName()
    {
        return ctGameManager.Singleton.m_Libraries[curLibIdx].name;
    }
}
