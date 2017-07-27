using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class ImgPiece : MonoBehaviour
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

    public Sprite[] m_Sprites;
    public PieceType m_PieceType = PieceType.WhitePawn;

    protected void Awake()
    {
        SetType(m_PieceType);
    }

    public void SetType(PieceType _type)
    {
        m_PieceType = _type;
        Debug.Log("SetType = " + _type.ToString());
        Sprite[] sprites = Resources.LoadAll<Sprite>(ctGameManager.Singleton.GetSpriteNameFromType(_type));

        GetComponent<Image>().sprite = sprites[0];
    }


}
