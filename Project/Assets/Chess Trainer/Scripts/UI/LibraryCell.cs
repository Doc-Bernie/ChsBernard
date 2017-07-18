using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LibraryCell : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    public delegate bool CallbackTouch(int _id, bool _longTouch, PointerEventData _eventData);

    public TMPro.TextMeshProUGUI txtName = null;
  
    public string libName { get; set; }

    protected int m_ID = 0;
    protected CallbackTouch m_Callback = null;

    bool m_isLongPress = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(int _id, string _text, CallbackTouch _callback = null)
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
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_Callback == null)
            return;

        m_isLongPress = false;
        StartCoroutine(InvokeMethod(eventData));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_Callback == null)
            return;

        StopCoroutine("InvokeMethod");

        if (!m_isLongPress)
            m_Callback(m_ID, false, eventData);
    }

    IEnumerator InvokeMethod(PointerEventData eventData)
    {
        yield return new WaitForSeconds(1.0f);

        m_Callback(m_ID, true, eventData);
        m_isLongPress = true;
    }
}
