using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WndBoard : MonoBehaviour {
    public enum CanMove
    {
        White,
        Black,
        All,
    };

    public RectTransform[] m_SquarePoses;
    public GameObject m_PiecePrefab;
    public RectTransform m_ImgBoard;
//     public AudioClip m_MoveSound;
//     public AudioClip m_CaptureSound;
//     public AudioClip m_CheckSound;

    static Color Col_LastMove = new Color(0.74f, 0.74f, 0.34f);
    static Color Col_None = new Color(1, 1, 1, 0);
    static Color Col_Select = new Color(0.5f, 0.78f, 0.5f);

    cgBoard m_Board = null;
    int m_SelectedPiece = -1;
    CanMove m_CanMove = CanMove.All;

    protected List<GameObject> m_PieceObjs = new List<GameObject>();

    public delegate void PieceMoveHandler(cgSimpleMove _move);

    protected PieceMoveHandler m_PieceMoveHandler = null;

    bool m_IsFlip = false;

    cgSimpleMove m_CurMove = null;

    protected void Awake()
    {
        for (int i = 0; i < m_SquarePoses.Length; i++)
        {
            ImgSquare comp = m_SquarePoses[i].gameObject.AddComponent<ImgSquare>();
            comp.Init(i, OnSquareSelected);
        }
    }

    public void Init(cgBoard _board, CanMove _canMove, PieceMoveHandler _handler)
    {
        m_Board = _board;
        m_CanMove = _canMove;
        m_PieceMoveHandler = _handler;

        FlipBoard(false);

        UpdateBoard();
    }

    public void UpdateBoard()
    {
        if (m_Board == null)
            return;

        int i = 0;
        for (i = 0; i < m_PieceObjs.Count; i++)
            GameObject.Destroy(m_PieceObjs[i]);
        m_PieceObjs.Clear();

        UnselectAllSquare();

        for (i = 0; i < m_Board.squares.Count; i++)
        {
            if (m_Board.squares[i] != 0)
            {
                GameObject obj = GameObject.Instantiate(m_PiecePrefab);
                obj.GetComponent<ImgPiece>().Init((ImgPiece.PieceType)m_Board.squares[i], i, OnPieceSelected);

                obj.transform.parent = m_SquarePoses[i];
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = new Vector3(1, 1, 1);
                m_PieceObjs.Add(obj);
            }
        }
    }

    public bool isFlipped()
    {
        return m_IsFlip;
    }

    public void FlipBoard(bool _isFlip)
    {
        if (m_IsFlip == _isFlip)
            return;

        m_IsFlip = _isFlip;
        
        if (m_IsFlip)
            m_ImgBoard.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        else
            m_ImgBoard.localRotation = Quaternion.Euler(Vector3.zero);

        for (int i = 0; i < m_SquarePoses.Length; i++)
        {
            m_SquarePoses[i].localRotation = m_IsFlip ? Quaternion.Euler(new Vector3(0, 0, 180)) : Quaternion.Euler(Vector3.zero);
        }
    }

    public void OnPieceSelected(int _pos)
    {
        bool canMove = false;
        Debug.Log("source pos = " + m_SelectedPiece.ToString() + ", target pos = " + _pos.ToString());

        if ((m_SelectedPiece != -1) && (m_SelectedPiece != _pos))
        {
            List<cgSimpleMove> moveSet = m_Board.findStrictLegalMoves(m_Board.whiteTurnToMove);
            foreach (cgSimpleMove move in moveSet)
                if (move.from == m_SelectedPiece && move.to == _pos)
                {
                    if (IsPromotionMove(move))
                    {
                        m_CurMove = move;
                        WndManager.Singleton.OpenPromotionSelectWnd(m_Board.whiteTurnToMove, CallbackPromotionSelect);
                    }
                    else
                    {
                        OnMovePiece(move);
                        canMove = true;
                    }
                    break;
                }

            UnselectAllSquare();
            m_SelectedPiece = -1;
        }

        if (m_SelectedPiece == -1 || !canMove)
        {
            UnselectAllSquare();
//             List<cgSimpleMove> moveSet = m_Board.findStrictLegalMoves(m_Board.whiteTurnToMove);
//             foreach (cgSimpleMove move in moveSet)
//                 if (move.from == _pos)
//                     SelectSquare(move.to);
            if (m_Board.squares[_pos] != 0 && 
                ((m_Board.squares[_pos] > 0 && m_Board.whiteTurnToMove && (m_CanMove == CanMove.All || (m_CanMove == CanMove.White))) 
                || (m_Board.squares[_pos] < 0 && !m_Board.whiteTurnToMove && (m_CanMove == CanMove.All || (m_CanMove == CanMove.Black)))))
            {
                SelectSquare(_pos);
                m_SelectedPiece = _pos;
            }
            else
                m_SelectedPiece = -1;
        }

    }

    public void OnSquareSelected(int _pos)
    {
        if (m_SelectedPiece == -1)
            return;

        List<cgSimpleMove> moveSet = m_Board.findStrictLegalMoves(m_Board.whiteTurnToMove);
        foreach (cgSimpleMove move in moveSet)
            if (move.from == m_SelectedPiece && move.to == _pos)
            {
                if (IsPromotionMove(move))
                {
                    m_CurMove = move;
                    WndManager.Singleton.OpenPromotionSelectWnd(m_Board.whiteTurnToMove, CallbackPromotionSelect);
                }
                else
                {
                    OnMovePiece(move);
                }

                break;
            }

        UnselectAllSquare();
        m_SelectedPiece = -1;
    }

    void SelectSquare(int _pos)
    {
         Image img = m_SquarePoses[_pos].GetComponent<Image>();
         img.color = Col_Select;
    }

    void UnselectAllSquare()
    {
        for (int i = 0; i < m_SquarePoses.Length; i++)
        {
            Image img = m_SquarePoses[i].GetComponent<Image>();
            img.color = Col_None;
        }

        if (m_Board.moves.Count > 0)
        {
            cgSimpleMove lastMove = m_Board.moves[m_Board.moves.Count - 1];
            Image img = m_SquarePoses[lastMove.to].GetComponent<Image>();
            img.color = Col_LastMove;
        }
    }

    void OnMovePiece(cgSimpleMove _move)
    {
        m_PieceMoveHandler(_move);
//         if (m_Board.isChecked(m_Board.whiteTurnToMove))
//             AudioSource.PlayClipAtPoint(m_CheckSound, Vector3.zero, 1.0f);
//         else
//             AudioSource.PlayClipAtPoint(m_MoveSound, Vector3.zero, 1.0f);
    }

    bool IsPromotionMove(cgSimpleMove _move)
    {
        return (_move.to < 8 && m_Board.squares[_move.from] == 1) || (_move.to > 55 && m_Board.squares[_move.from] == -1);
    }

    void CallbackPromotionSelect(byte _promotionType)
    {
        if (m_CurMove == null)
            return;

        m_CurMove.promoted = true;
        m_CurMove.promotionType = _promotionType;
        OnMovePiece(m_CurMove);
    }
}
