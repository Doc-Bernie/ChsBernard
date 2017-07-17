using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WndManager : MonoBehaviour {
    #region Singleton
    protected static WndManager _instance = null;

    public static WndManager Singleton
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType(typeof(WndManager)) as WndManager;
            return _instance;
        }
    }
    #endregion Singleton

    public enum MSGBOX_BTN_TYPE
    {
        OK,
        YesNo,
    };

    public enum MSGBOX_BTN
    {
        OK,
        YES,
        NO,
    };

    public WndMainPage mainPage = null;
    public WndLibraryPage libPage = null;
    public WndLinePage linePage = null;
    public WndInputBox inputBox = null;
    public WndMsgBox msgBox = null;

    public delegate bool InputBoxCallback(string _name);
    public delegate bool MsgBoxCallback(MSGBOX_BTN _btnClicked);

    protected void Awake()
    {
        mainPage.gameObject.SetActive(true);
        libPage.gameObject.SetActive(false);
        linePage.gameObject.SetActive(false);
        inputBox.gameObject.SetActive(false);
        msgBox.gameObject.SetActive(false);
    }

    public void OpenMainPage()
    {
        libPage.gameObject.SetActive(false);
        linePage.gameObject.SetActive(false);

        mainPage.gameObject.SetActive(true);
        mainPage.Init();
        
    }

    public void OpenLibraryPage(string _libName)
    {
        mainPage.gameObject.SetActive(false);
        linePage.gameObject.SetActive(false);

        libPage.gameObject.SetActive(true);
        libPage.Init(_libName);
    }

    public void OpenLinePage(string _lineName)
    {
        mainPage.gameObject.SetActive(false);
        libPage.gameObject.SetActive(false);

        linePage.gameObject.SetActive(true);
        linePage.Init(_lineName);
    }

    public void OpenInputBox(string _header, InputBoxCallback _callback, string _defaultStr = "")
    {
        inputBox.gameObject.SetActive(true);
        inputBox.Init(_header, _callback, _defaultStr);
    }

    public void OpenMsgBox(string _header, MsgBoxCallback _callback = null, MSGBOX_BTN_TYPE _type = MSGBOX_BTN_TYPE.OK)
    {
        msgBox.gameObject.SetActive(true);
        msgBox.Init(_header, _callback, _type);

    }
}
