using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent (typeof(Image))]
public class ImgPiece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum PieceType
    {
        WhitePawn = 1,
        WhiteRook = 2,
        WhiteKnight = 3,
        WhiteBishop = 4,
        WhiteQueen = 5,
        WhiteKing = 6,

        BlackPawn = -1,
        BlackRook = -2,
        BlackKnight = -3,
        BlackBishop = -4,
        BlackQueen = -5,
        BlackKing = -6
    };

    public Image m_ImgPiece;
    public PieceType m_PieceType = PieceType.WhitePawn;
    public int m_SquarePos;

    public delegate void PointerUpHandler(int _pos);

    PointerUpHandler m_PointerUpHandler;

    protected void Awake()
    {
    }

    public void Init(PieceType _type, int _pos, PointerUpHandler _pointerUpHandler)
    {
        m_PieceType = _type;
        m_SquarePos = _pos;
        m_PointerUpHandler = _pointerUpHandler;

        Debug.Log("SetType = " + _type.ToString());
        Sprite[] sprites = Resources.LoadAll<Sprite>(ctGameManager.Singleton.GetSpriteNameFromType(_type));

        m_ImgPiece.sprite = sprites[0];
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
