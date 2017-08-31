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
    public WndTrainPage trainPage = null;
    public WndTrainerSettingPage trainerSettingPage = null;
    public WndHelp helpWnd = null;
    public WndSetting settingWnd = null;
    
    public WndTrainSettingColor colorSettingWnd = null;
    public WndPromotionSelect promotionSelectWnd = null;
    public WndInputBox inputBox = null;
    public WndMsgBox msgBox = null;
    private uFileBrowser.FileBrowser fileBrowser = null;
    public GameObject fileBrowserPrefab;

    public delegate bool InputBoxCallback(string _name);
    public delegate bool MsgBoxCallback(MSGBOX_BTN _btnClicked);

    int defaultSleepTimeout = 0;

    protected void Awake()
    {
        mainPage.gameObject.SetActive(true);
        libPage.gameObject.SetActive(false);
        linePage.gameObject.SetActive(false);
        trainPage.gameObject.SetActive(false);
        trainerSettingPage.gameObject.SetActive(false);
        helpWnd.gameObject.SetActive(false);
        settingWnd.gameObject.SetActive(false);

        colorSettingWnd.gameObject.SetActive(false);
        promotionSelectWnd.gameObject.SetActive(false);
        inputBox.gameObject.SetActive(false);
        msgBox.gameObject.SetActive(false);

        GameObject fileBrowserObj = GameObject.Instantiate(fileBrowserPrefab);
        fileBrowserObj.transform.parent = gameObject.transform;
        fileBrowserObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        fileBrowser = fileBrowserObj.GetComponent<uFileBrowser.FileBrowser>();
        fileBrowser.gameObject.SetActive(false);
        fileBrowser.m_OnSubmit = new uFileBrowser.FileBrowser.FileBrowserSubmitEvent();
        fileBrowser.m_OnSubmit.AddListener(OnSelectPGN);
#if UNITY_ANDROID
        defaultSleepTimeout = Screen.sleepTimeout;
#endif
    }

    void Update()
    {
        if (!Input.GetKeyUp(KeyCode.Escape))
            return;

        if (msgBox.gameObject.activeInHierarchy)
        {
            CloseMsgBox();
            return;
        }

        if (inputBox.gameObject.activeInHierarchy)
        {
            CloseInputBox();
            return;
        }

        if (helpWnd.gameObject.activeInHierarchy)
        {
            helpWnd.OnEscape();
            return;
        }

        if (settingWnd.gameObject.activeInHierarchy)
        {
            settingWnd.OnEscape();
            return;
        }

        if (fileBrowser.gameObject.activeInHierarchy)
        {
            CloseFileBrowser();
            return;
        }

        if (mainPage.gameObject.activeInHierarchy)
        {
            mainPage.OnEscape();
            return;
        }

        if (libPage.gameObject.activeInHierarchy)
        {
            libPage.OnEscape();
            return;
        }

        if (linePage.gameObject.activeInHierarchy)
        {
            linePage.OnEscape();
            return;
        }

        if (trainPage.gameObject.activeInHierarchy)
        {
            trainPage.OnEscape();
            return;
        }

        if (trainerSettingPage.gameObject.activeInHierarchy)
        {
            trainerSettingPage.OnEscape();
            return;
        }


    }

    protected void OnDestroy()
    {
#if UNITY_ANDROID
        Screen.sleepTimeout = defaultSleepTimeout;
#endif
    }

    public void OpenMainPage()
    {
        libPage.gameObject.SetActive(false);
        linePage.gameObject.SetActive(false);
        trainPage.gameObject.SetActive(false);
        colorSettingWnd.gameObject.SetActive(false);
        promotionSelectWnd.gameObject.SetActive(false);
        trainerSettingPage.gameObject.SetActive(false);

        mainPage.gameObject.SetActive(true);
        mainPage.Init();

#if UNITY_ANDROID
        Screen.sleepTimeout = defaultSleepTimeout;
#endif
    }

    public void OpenLibraryPage(string _libName)
    {
        mainPage.gameObject.SetActive(false);
        linePage.gameObject.SetActive(false);
        trainPage.gameObject.SetActive(false);
        colorSettingWnd.gameObject.SetActive(false);
        promotionSelectWnd.gameObject.SetActive(false);
        trainerSettingPage.gameObject.SetActive(false);

        libPage.gameObject.SetActive(true);
        libPage.Init(_libName);

#if UNITY_ANDROID
        Screen.sleepTimeout = defaultSleepTimeout;
#endif
    }

    public void OpenLinePage(string _libName, string _lineName)
    {
        mainPage.gameObject.SetActive(false);
        libPage.gameObject.SetActive(false);
        trainPage.gameObject.SetActive(false);
        colorSettingWnd.gameObject.SetActive(false);
        promotionSelectWnd.gameObject.SetActive(false);
        trainerSettingPage.gameObject.SetActive(false);

        linePage.gameObject.SetActive(true);
        linePage.Init(_libName, _lineName);

#if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
    }

    public void OpenTrainPage(bool _isWhite, List<ctLine> _lineList, bool _fromMainPage)
    {
        mainPage.gameObject.SetActive(false);
        libPage.gameObject.SetActive(false);
        linePage.gameObject.SetActive(false);
        colorSettingWnd.gameObject.SetActive(false);
        promotionSelectWnd.gameObject.SetActive(false);
        trainerSettingPage.gameObject.SetActive(false);

        trainPage.gameObject.SetActive(true);
        trainPage.Init(_isWhite, _lineList, _fromMainPage);

#if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
    }

    public void OpenTrainerSettingPage(List<string> _libList)
    {
        mainPage.gameObject.SetActive(false);
        libPage.gameObject.SetActive(false);
        linePage.gameObject.SetActive(false);
        colorSettingWnd.gameObject.SetActive(false);
        promotionSelectWnd.gameObject.SetActive(false);
        trainPage.gameObject.SetActive(false);

        trainerSettingPage.gameObject.SetActive(true);
        trainerSettingPage.Init(_libList);

#if UNITY_ANDROID
        Screen.sleepTimeout = defaultSleepTimeout;
#endif
    }

    public void OpenColorSettingWnd(WndTrainSettingColor.CallBack _cb)
    {
        colorSettingWnd.gameObject.SetActive(true);
        colorSettingWnd.Init(_cb);
    }

    public bool IsOpenColorSettingWnd()
    {
        return colorSettingWnd.gameObject.activeInHierarchy;
    }

    public void CloseColorSettingWnd()
    {
        colorSettingWnd.gameObject.SetActive(false);
    }

    public void OpenInputBox(string _header, InputBoxCallback _callback, string _defaultStr = "")
    {
        inputBox.gameObject.SetActive(true);
        inputBox.Init(_header, _callback, _defaultStr);
    }

    public void CloseInputBox()
    {
        inputBox.gameObject.SetActive(false);
    }

    public bool IsOpenInputBox()
    {
        return inputBox.gameObject.activeInHierarchy;
    }

    public void OpenMsgBox(string _header, MsgBoxCallback _callback = null, MSGBOX_BTN_TYPE _type = MSGBOX_BTN_TYPE.OK)
    {
        StartCoroutine(openMsgBox(_header, _callback, _type));
    }

    public void CloseMsgBox()
    {
        msgBox.gameObject.SetActive(false);
    }

    IEnumerator openMsgBox(string _header, MsgBoxCallback _callback = null, MSGBOX_BTN_TYPE _type = MSGBOX_BTN_TYPE.OK)
    {
        while (msgBox.gameObject.activeInHierarchy)
            yield return null;

        msgBox.gameObject.SetActive(true);
        msgBox.Init(_header, _callback, _type);
    }

    public void OpenPromotionSelectWnd(bool _isWhite, WndPromotionSelect.CallbackType _callback)
    {
        promotionSelectWnd.gameObject.SetActive(true);
        promotionSelectWnd.Init(_isWhite, _callback);
    }

    public bool IsOpenPromotionSelectWnd()
    {
        return promotionSelectWnd.gameObject.activeInHierarchy;
    }

    public void ClosePromotionSelectWnd()
    {
        promotionSelectWnd.gameObject.SetActive(false);
    }

    public void OpenFileBrowser()
    {
        fileBrowser.gameObject.SetActive(true);
    }

    public bool IsOpenFileBrowser()
    {
        return fileBrowser.gameObject.activeInHierarchy;
    }

    public void CloseFileBrowser()
    {
        fileBrowser.gameObject.SetActive(false);
    }

    public void OnBtnSetting()
    {

    }

    public void OnBtnHelp()
    {

    }

    public void OnSelectPGN(string _path)
    {
        libPage.OnSelectPGN(_path);
    }
}
