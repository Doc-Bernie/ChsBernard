using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WndMsgBox : MonoBehaviour {


    public TMPro.TextMeshProUGUI lblHeader = null;
    public Button btnOk = null;
    public Button btnYes = null;
    public Button btnNo = null;

    WndManager.MsgBoxCallback callback = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(string _header, WndManager.MsgBoxCallback _callback = null, WndManager.MSGBOX_BTN_TYPE _type = WndManager.MSGBOX_BTN_TYPE.OK)
    {
        lblHeader.text = _header;
        callback = _callback;

        if (_type == WndManager.MSGBOX_BTN_TYPE.OK)
        {
            btnOk.gameObject.SetActive(true);

            btnYes.gameObject.SetActive(false);
            btnNo.gameObject.SetActive(false);
        }
        else if (_type == WndManager.MSGBOX_BTN_TYPE.YesNo)
        {
            btnOk.gameObject.SetActive(false);

            btnYes.gameObject.SetActive(true);
            btnNo.gameObject.SetActive(true);
        }
    }

    public void OnBtnOK()
    {
        if (callback != null)
            callback(WndManager.MSGBOX_BTN.OK);
        gameObject.SetActive(false);
    }

    public void OnBtnYes()
    {
        if (callback != null)
            callback(WndManager.MSGBOX_BTN.YES);
        gameObject.SetActive(false);
    }

    public void OnBtnNo()
    {
        if (callback != null)
            callback(WndManager.MSGBOX_BTN.NO);
        gameObject.SetActive(false);
    }
}
