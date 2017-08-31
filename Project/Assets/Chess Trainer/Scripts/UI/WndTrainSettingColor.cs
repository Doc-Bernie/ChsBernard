using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WndTrainSettingColor : MonoBehaviour {
    public delegate void CallBack(bool _isWhite);

    CallBack m_Callback;

    void Awake()
    {
    }

    public void Init(CallBack _cb)
    {
        m_Callback = _cb;
    }

	public void OnWhite()
    {
        m_Callback(true);
        gameObject.SetActive(false);
    }

    public void OnBlack()
    {
        m_Callback(false);
        gameObject.SetActive(false);
    }
}
