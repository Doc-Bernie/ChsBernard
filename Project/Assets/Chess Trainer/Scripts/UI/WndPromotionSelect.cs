using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WndPromotionSelect : MonoBehaviour {
    public Image btnRook;
    public Image btnKnight;
    public Image btnBishop;
    public Image btnQueen;

    public Sprite spriteWhiteRook;
    public Sprite spriteBlackRook;
    public Sprite spriteWhiteKnight;
    public Sprite spriteBlackKnight;
    public Sprite spriteWhiteBishop;
    public Sprite spriteBlackBishop;
    public Sprite spriteWhiteQueen;
    public Sprite spriteBlackQueen;

    public delegate void CallbackType(byte _type);

    protected CallbackType callbackType = null;
    protected bool isWhite = true;

    public void Init(bool _isWhite, CallbackType _callback)
    {
        isWhite = _isWhite;
        callbackType = _callback;

        btnRook.gameObject.GetComponent<Image>().sprite = isWhite ? spriteWhiteRook : spriteBlackRook;
        btnKnight.gameObject.GetComponent<Image>().sprite = isWhite ? spriteWhiteKnight : spriteBlackKnight;
        btnBishop.gameObject.GetComponent<Image>().sprite = isWhite ? spriteWhiteBishop : spriteBlackBishop;
        btnQueen.gameObject.GetComponent<Image>().sprite = isWhite ? spriteWhiteQueen : spriteBlackQueen;
    }

    public void OnBtnRook()
    {
        callbackType(2);
        gameObject.SetActive(false);
    }

    public void OnBtnKnight()
    {
        callbackType(3);
        gameObject.SetActive(false);
    }

    public void OnBtnBishop()
    {
        callbackType(4);
        gameObject.SetActive(false);
    }

    public void OnBtnQueen()
    {
        callbackType(5);
        gameObject.SetActive(false);
    }	
}
