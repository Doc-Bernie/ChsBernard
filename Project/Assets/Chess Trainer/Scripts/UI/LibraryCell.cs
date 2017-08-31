using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LibraryCell : EventTriggerEx, IPointerDownHandler, IPointerUpHandler,/* IPointerClickHandler,*/ IDragHandler
{
    public delegate bool CallbackTouch(int _id, bool _longTouch, PointerEventData _eventData);

    public TMPro.TextMeshProUGUI txtName = null;
    public GameObject imgBG = null;
  
    public string libName { get; set; }

    protected int m_ID = 0;
    protected CallbackTouch m_Callback = null;
    protected bool m_ToggleMode = false;

    bool m_isPressing = false;
    bool m_isLongPress = false;

    Coroutine m_Coroutine = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(int _id, string _text, MonoBehaviour _delegate, CallbackTouch _callback = null)
    {
        m_ID = _id;
        m_Callback = _callback;
        txtName.text = _text;// string.Format("{0} ({1})", lib.name, lib.lines.Count);

//         libName = _name;
//         ctLibrary lib = ctGameManager.Singleton.GetLibrary(_name);
//         if (lib == null)
//             return;
// 
//         txtName.text = _text;// string.Format("{0} ({1})", lib.name, lib.lines.Count);
//         callback = _callback;
        m_delegates.Add(_delegate);
        SetToggleMode(false);
    }

    public void SetToggleMode(bool _toggleMode)
    {
//         if (m_ToggleMode == _toggleMode)
//             return;

        m_ToggleMode = _toggleMode;
        if (m_ToggleMode)
        {
            imgBG.SetActive(true);
            gameObject.GetComponent<Toggle>().enabled = true;
        }
        else
        {
            imgBG.SetActive(false);
            gameObject.GetComponent<Toggle>().enabled = false;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (m_Callback == null)
            return;

//         if (eventData.clickCount < 2)
        {
            m_isPressing = true;
            m_isLongPress = false;
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }
            m_Coroutine = StartCoroutine(InvokeMethod(eventData));
        }
//         else
//         {
//             m_isLongPress = false;
//             if (m_Coroutine != null)
//             {
//                 StopCoroutine(m_Coroutine);
//                 m_Coroutine = null;
//             }
//             m_Callback(m_ID, false, eventData);
//             eventData.clickCount = 0;
//         }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        if (m_Callback == null)
            return;

        m_isPressing = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (m_Callback == null)
            return;

        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }

        if (!m_ToggleMode && !m_isLongPress && m_isPressing)
             m_Callback(m_ID, false, eventData);

        m_isPressing = false;


    }

    IEnumerator InvokeMethod(PointerEventData eventData)
    {
        yield return new WaitForSeconds(0.7f);

        if (m_isPressing)
            m_Callback(m_ID, true, eventData);

        m_isLongPress = true;
    }
}
