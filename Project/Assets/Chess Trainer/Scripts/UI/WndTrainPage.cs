using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WndTrainPage : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Lbl_LineName = null;
    public TMPro.TextMeshProUGUI Lbl_LineScore = null;
    public TMPro.TextMeshProUGUI Lbl_TotalScore = null;

    public TMPro.TextMeshProUGUI Lbl_Notation = null;

    public WndBoard Wnd_Board = null;
    public ScrollRect NotationTextScrollRect = null;

    protected cgBoard m_Board = null;
    bool m_PlayAsWhite = true;
    bool m_WhiteTurn = true;

    public AudioClip MovePiece_Sound;
    public AudioClip Check_Sound;


    List<ctLine> m_LineList = new List<ctLine>();
    int m_CurLineIdx = 0;
    int m_CurMoveIdx = 0;
    ctLine m_CurLine = null;
    int m_Score = 0;
    int m_TotalScore = 0;
    int m_MaxScore = 0;
    int m_TotalMaxScore = 0;

    bool m_Missed = false;
    bool m_FromMainPage = true;
    bool m_MissedThisLine = false;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void OnEscape()
    {
        if (WndManager.Singleton.IsOpenPromotionSelectWnd())
            WndManager.Singleton.ClosePromotionSelectWnd();
        else
            WndManager.Singleton.OpenMsgBox("Do you want to leave training?", CallBackExit, WndManager.MSGBOX_BTN_TYPE.YesNo);
    }

    public void Init(bool _isWhite, List<ctLine> _lineList, bool _fromMainPage)
    {
        m_PlayAsWhite = _isWhite;
        m_WhiteTurn = true;

        m_FromMainPage = _fromMainPage;
        
        m_LineList.Clear();
        m_TotalMaxScore = 0;
        for (int i = 0; i < _lineList.Count; i++)
        {
            m_LineList.Add(_lineList[i]);
            m_TotalMaxScore += CalcMaxScore(_lineList[i]);
        }

        m_CurLineIdx = 0;
        m_TotalScore = 0;
        
        startNewLine();
    }

    void InitCtrls()
    {
        
    }

    public void OnPieceMove(cgSimpleMove _move)
    {
        if (isRightMove(_move))
        {
            if (!m_MissedThisLine)
            {
                m_Score++;
                m_TotalScore++;
            }

            moveNextMove();

        }
        else
        {
            m_MissedThisLine = true;

            if (m_Missed)
            {
                WndManager.Singleton.OpenMsgBox("Incorrect move. The Right move is " + cgNotation.NotationFromMove(m_Board, m_CurLine.moveList[m_CurMoveIdx])/*, CallBackIncorrectMove*/);
            }
            else
            {
                WndManager.Singleton.OpenMsgBox("Incorrect move.");
                //revertMove();
                m_Missed = true;
            }
        }

        UpdateInfos();
        
    }

    void moveNextMove(bool _moveOneSide = false)
    {
        m_Missed = false;

        m_WhiteTurn = !m_WhiteTurn;
        m_Board.move(m_CurLine.moveList[m_CurMoveIdx ++]);

        UpdateInfos();

        if (m_Board.isChecked(m_Board.whiteTurnToMove))
            AudioSource.PlayClipAtPoint(Check_Sound, Vector3.zero, 1.0f);
        else
            AudioSource.PlayClipAtPoint(MovePiece_Sound, Vector3.zero, 1.0f);

        if (checkEndLine())
            return;

        Invoke("moveComputerMove", 1.5f);
    }

    void moveComputerMove()
    {
        m_WhiteTurn = !m_WhiteTurn;

        m_Board.move(m_CurLine.moveList[m_CurMoveIdx++]);
        UpdateInfos();

        checkEndLine();

        if (m_Board.isChecked(m_Board.whiteTurnToMove))
            AudioSource.PlayClipAtPoint(Check_Sound, Vector3.zero, 1.0f);
        else
            AudioSource.PlayClipAtPoint(MovePiece_Sound, Vector3.zero, 1.0f);
    }

    void revertMove()
    {
        m_Board.revert();
    }

    bool checkEndLine()
    {
        if (m_CurMoveIdx >= m_CurLine.moveList.Count)
        {
            if (m_CurLineIdx >= m_LineList.Count - 1)
            {
                // end training
                string msg = string.Format("Congratulation. You finished your training.\nYour total score is {0} out of {1}.", m_TotalScore, m_TotalMaxScore);
                WndManager.Singleton.OpenMsgBox(msg, CallBackEnd);
            }
            else
            {
                // start new line
                m_CurLineIdx ++;
                string msg;
                if (m_Score == m_MaxScore)
                    msg = string.Format("Well done! You will now start \"{0}\"", m_LineList[m_CurLineIdx].fullName);
                else
                    msg = string.Format("You will now start \"{0}\"", m_LineList[m_CurLineIdx].fullName);

                WndManager.Singleton.OpenMsgBox(msg, CallBackNewLine);
            }

            return true;
        }
        return false;
    }


    bool isRightMove(cgSimpleMove _move)
    {
        cgSimpleMove curMove = m_CurLine.moveList[m_CurMoveIdx];
        if (curMove.from == _move.from && curMove.to == _move.to)
            return true;

        return false;
    }

    void startNewLine()
    {
        m_CurMoveIdx = 0;
        m_CurLine = m_LineList[m_CurLineIdx];
        m_Board = new cgBoard();
        Wnd_Board.Init(m_Board, m_PlayAsWhite ? WndBoard.CanMove.White : WndBoard.CanMove.Black, OnPieceMove);
        if (!m_PlayAsWhite)
            Wnd_Board.FlipBoard(true);

        m_Score = 0;
        m_MaxScore = CalcMaxScore(m_CurLine);
        m_MissedThisLine = false;

        m_Missed = false;

        Lbl_LineName.text = string.Format("{0}/{1} ({2})",m_CurLine.parent.name, m_CurLine.name, m_CurLine.moveList.Count);

        if (!m_PlayAsWhite)
            Invoke("moveComputerMove", 1.5f);
        else
            UpdateInfos();
    }

    void UpdateInfos()
    {
        Lbl_LineScore.text = string.Format("Score : {0} out of {1}", m_Score, m_MaxScore);
        Lbl_TotalScore.text = string.Format("Total Score : {0} out of {1}", m_TotalScore, m_TotalMaxScore);

        cgNotation not = new cgNotation(m_Board);
        Lbl_Notation.text = not.writeFullNotation(cgNotation.NotationType.Algebraic);

        // for change scroll position 
        float height = Lbl_Notation.preferredHeight + 20;
        NotationTextScrollRect.content.sizeDelta = new Vector2(NotationTextScrollRect.content.sizeDelta.x, height);
        if (NotationTextScrollRect.viewport.rect.height < height)
            NotationTextScrollRect.content.anchoredPosition = new Vector2(0, height - NotationTextScrollRect.viewport.rect.height);

        Wnd_Board.UpdateBoard();
    }

//     bool CallBackSave(WndManager.MSGBOX_BTN _btn)
//     {
//         if (_btn == WndManager.MSGBOX_BTN.YES)
//         {
//             saveLine();
//         }
// 
//         return true;
//     }
// 
    bool CallBackExit(WndManager.MSGBOX_BTN _btn)
    {
        if (_btn == WndManager.MSGBOX_BTN.YES)
        {
            backToPrevPage();
        }
        
        return true;
    }

    bool CallBackEnd(WndManager.MSGBOX_BTN _btn)
    {
        backToPrevPage();
        return true;
    }

    void backToPrevPage()
    {
        if (m_FromMainPage)
        {
            WndManager.Singleton.OpenMainPage();
        }
        else
        {
            WndManager.Singleton.OpenLibraryPage(m_LineList[0].parent.name);
        }
    }

    bool CallBackNewLine(WndManager.MSGBOX_BTN _btn)
    {
        startNewLine();
        return true;
    }

    bool CallBackIncorrectMove(WndManager.MSGBOX_BTN _btn)
    {
        moveNextMove();
        return true;
    }

    bool CallBackSkipLine(WndManager.MSGBOX_BTN _btn)
    {
        if (_btn == WndManager.MSGBOX_BTN.YES)
        {
            m_CurMoveIdx = m_CurLine.moveList.Count;
            checkEndLine();
        }
        return true;
    }

    int CalcMaxScore (ctLine _line)
    {
        if (m_PlayAsWhite)
        {
            return (int)Mathf.Ceil((float)_line.moveList.Count / 2);
        }
        else
        {
            return _line.moveList.Count / 2;
        }
    }

    public void OnBtnSkip()
    {
        WndManager.Singleton.OpenMsgBox("Do you want to really skip this line?", CallBackSkipLine, WndManager.MSGBOX_BTN_TYPE.YesNo);
    }
}
