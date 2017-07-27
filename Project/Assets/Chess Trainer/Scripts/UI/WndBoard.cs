using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WndBoard : MonoBehaviour {

    public RectTransform[] m_SquarePoses;
    public GameObject m_PiecePrefab;
    public RectTransform m_ImgBoard;

    protected List<GameObject> m_PieceObjs = new List<GameObject>();

    bool m_IsFlip = false;

    protected void Awake()
    {
    }

    public void UpdateBoard(cgBoard _board)
    {
        if (_board == null)
            return;

        int i = 0;
        for (i = 0; i < m_PieceObjs.Count; i++)
            GameObject.Destroy(m_PieceObjs[i]);
        m_PieceObjs.Clear();

        for (i = 0; i < _board.squares.Count; i++)
        {
            if (_board.squares[i] != 0)
            {
                GameObject obj = GameObject.Instantiate(m_PiecePrefab);
                obj.GetComponent<ImgPiece>().SetType((ImgPiece.PieceType)_board.squares[i]);

                obj.transform.parent = m_SquarePoses[i];
                obj.transform.localPosition = Vector3.zero;

                m_PieceObjs.Add(obj);
            }
        }
    }

    public void FlipBoard()
    {
        m_IsFlip = !m_IsFlip;
        
        if (m_IsFlip)
            m_ImgBoard.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        else
            m_ImgBoard.localRotation = Quaternion.Euler(Vector3.zero);

        for (int i = 0; i < m_SquarePoses.Length; i++)
        {
            m_SquarePoses[i].localRotation = m_IsFlip ? Quaternion.Euler(new Vector3(0, 0, 180)) : Quaternion.Euler(Vector3.zero);
        }
    }
}
