using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WndMainPage : MonoBehaviour {
    public GameObject LibraryList = null;
    public GameObject LibraryListCell = null;
    public TMPro.TextMeshProUGUI TxtLibCount = null;
    public MonoBehaviour ScrollRect = null;

    public GameObject BtnAdd;
    public GameObject BtnTrainer;
    public GameObject BtnRename;
    public GameObject BtnDelete;


    protected List<GameObject> cells = new List<GameObject>();

    int curLibIdx = 0;
    bool toggleMode = false;

	// Use this for initialization
	void Awake() {
        Init();
	}
	
	// Update is called once per frame
	public void OnEscape () {
        if (toggleMode)
            SetToggleMode(false);
        else
            WndManager.Singleton.OpenMsgBox("Do you really want exit?", CallBackExit, WndManager.MSGBOX_BTN_TYPE.YesNo);
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
        TxtLibCount.text = string.Format("({0}) libraries", countLib);
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

            string text = string.Format("{0} ({1})", lib.name, lib.lines.Count);

            childObj.GetComponent<LibraryCell>().Init(i, text, ScrollRect, CallbackTouchCell);
            //childObj.GetComponent<Toggle>().group = LibraryList.GetComponent<ToggleGroup>();
            childObj.GetComponent<Toggle>().isOn = curLibIdx == i ? true : false;
            childObj.GetComponent<Toggle>().onValueChanged.AddListener(OnLibCellChanged);

            cells.Add(childObj);
        }

        //LibraryList.GetComponent<ToggleGroup>().allowSwitchOff = false;

    }
    
    void UpdateWnd()
    {
        ClearList();

        AddLibraries();
    }

    public void Init()
    {
        curLibIdx = 0;

        SetToggleMode(false);
        UpdateWnd();
    }

//     public void OnBtnView()
//     {
//         if (GetCurLibName().Equals(string.Empty))
//             return;
// 
//         WndManager.Singleton.OpenLibraryPage(GetCurLibName());
//     }

    public void OnBtnAdd()
    {
        SetToggleMode(false);

        WndManager.Singleton.OpenInputBox("Enter Library's name :", CallBackAdd);
    }

    public void OnBtnRemove()
    {
        if (GetCurLibName().Equals(string.Empty))
            return;

        string strPrompt = string.Format("Do you really want to delete selected libraries?"/* \"{0}\" library?", GetCurLibName()*/);
        WndManager.Singleton.OpenMsgBox(strPrompt, CallBackRemove, WndManager.MSGBOX_BTN_TYPE.YesNo);
    }

    public void OnBtnRename()
    {
        SetToggleMode(false);

        if (GetCurLibName().Equals(string.Empty))
            return;

        WndManager.Singleton.OpenInputBox("Enter Library's name :", CallBackRename, GetCurLibName());
    }

    public void OnBtnTrainer()
    {
        if (toggleMode)
        {
            List<string> libList = new List<string>();
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<Toggle>().isOn)
                {
                    libList.Add(ctGameManager.Singleton.GetLibrary(i).name);
                }
            }

            if (libList.Count < 1)
                return;

            WndManager.Singleton.OpenTrainerSettingPage(libList);

            SetToggleMode(false);
        }
        else
        {
            SetToggleMode(true);
        }
    }

    bool CallBackAdd(string _name)
    {
        if (_name.Equals(""))
        {
            WndManager.Singleton.OpenMsgBox("The name cannot be empty.");
            return false;
        }

        if (ctGameManager.Singleton.GetLibrary(_name) != null)
        {
            WndManager.Singleton.OpenMsgBox("Duplicated name.");
            return false;
        }

        ctLibrary lib = new ctLibrary();
        lib.name = _name;
        ctGameManager.Singleton.AddLibrary(lib, true);

        UpdateWnd();
        return true;
    }

    bool CallBackRename(string _name)
    {
        if (_name.Equals(string.Empty))
        {
            WndManager.Singleton.OpenMsgBox("The name cannot be empty.");
            return false;
        }

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
            List<string> libList = new List<string>();
            int i;
            for (i = 0; i < cells.Count; i ++ )
            {
                if (cells[i].GetComponent<Toggle>().isOn)
                    libList.Add(ctGameManager.Singleton.GetLibrary(i).name);
            }

            for (i = 0; i < libList.Count; i++)
                ctGameManager.Singleton.RemoveLibrary(libList[i]);

            UpdateWnd();
            SetToggleMode(false);
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
        UpdateButtonState();
    }

    string GetCurLibName()
    {
        return ctGameManager.Singleton.m_Libraries[curLibIdx].name;
    }

    public bool CallbackTouchCell(int _id, bool _longTouch, PointerEventData _eventData)
    {
        ctLibrary lib = ctGameManager.Singleton.GetLibrary(_id);
        if (lib == null)
            return true;

        curLibIdx = _id;
        if (_longTouch)
        {
            SetToggleMode(true);
        }
        else
        {
            WndManager.Singleton.OpenLibraryPage(lib.name);
        }

        return true;
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

        if (toggleMode)
        {
            int selCount = 0;
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].GetComponent<Toggle>().isOn)
                {
                    selCount++;
                    curLibIdx = i;
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
