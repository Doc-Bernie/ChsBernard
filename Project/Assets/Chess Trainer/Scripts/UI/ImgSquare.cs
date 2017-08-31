using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent (typeof(Image))]
public class ImgSquare : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    int m_SquarePos = 0;

    public delegate void PointerUpHandler(int _pos);

    PointerUpHandler m_PointerUpHandler;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(int _squarePos, PointerUpHandler _pointerUpHandler)
    {
        m_SquarePos = _squarePos;
        m_PointerUpHandler = _pointerUpHandler;
    }

    public void OnPointerDown(PointerEventData _eventData)
    {

    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        if (m_PointerUpHandler != null)
            m_PointerUpHandler(m_SquarePos);
    }
}
