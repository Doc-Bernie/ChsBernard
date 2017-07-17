﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WndInputBox : MonoBehaviour {
    public TMPro.TextMeshProUGUI lblHeader = null;
    public TMPro.TMP_InputField txtContent = null;

    protected WndManager.InputBoxCallback callBack = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(string _header, WndManager.InputBoxCallback _callback, string _default = "")
    {
        lblHeader.text = _header;
        callBack = _callback;
        txtContent.text = _default;
    }

    public void OnBtnOK()
    {
        string name = txtContent.textComponent.text;

        if (callBack(name))
        {
            gameObject.SetActive(false);
        }
    }
}