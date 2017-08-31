using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WndLinePage : MonoBehaviour {
    public TMPro.TextMeshProUGUI Lbl_Title = null;
    public TMPro.TextMeshProUGUI Lbl_MoveCount = null;
    public TMPro.TextMeshProUGUI Lbl_Moves = null;
    public Button Btn_Save = null;
    public WndBoard Wnd_Board = null;
    public ScrollRect NotationTextScrollRect = null;

    public AudioClip MovePiece_Sound;
    public AudioClip Check_Sound;

    protected string lineName = "";
    protected ctLine line = null;

    protected int curMove = 0;
    protected bool isChanged = false;

    protected cgBoard board = null;
    protected List<cgSimpleMove> moveList = new List<cgSimpleMove>();


    // Update is called once per frame
	public void OnEscape() {
        if (WndManager.Singleton.IsOpenPromotionSelectWnd())
            WndManager.Singleton.ClosePromotionSelectWnd();
        else if (isChanged)
            WndManager.Singleton.OpenMsgBox("Do you want to save the changes?", CallBackSaveAndExit, WndManager.MSGBOX_BTN_TYPE.YesNo);
        else
            WndManager.Singleton.OpenLibraryPage(line.parent.name);
	}

    public void Init(string _libName, string _lineName)
    {
        lineName = _lineName;
        line = ctGameManager.Singleton.GetLine(_libName, _lineName);

        if (line == null)
            return;


        Lbl_Title.text = string.Format("{0}/{1}", line.parent.name, _lineName);

        curMove = 0;
        isChanged = false;

        moveList.Clear();
        for (int i = 0; i < line.moveList.Count; i++)
            moveList.Add(line.moveList[i]);
        board = new cgBoard();

        InitCtrls();
        UpdateInfos();
        Wnd_Board.Init(board, WndBoard.CanMove.All, OnPieceMove);
    }

    void InitCtrls()
    {
        Btn_Save.enabled = false;
    }

    public void OnFirstMove()
    {
        while (curMove > 0)
        {
            board.revert();
            curMove--;
        }
        UpdateInfos();
        Wnd_Board.UpdateBoard();
        AudioSource.PlayClipAtPoint(MovePiece_Sound, Vector3.zero, 1.0f);
    }

    public void OnPrevMove()
    {
        if (curMove == 0)
            return;

        board.revert();
        UpdateInfos();
        Wnd_Board.UpdateBoard();
        curMove--;
        AudioSource.PlayClipAtPoint(MovePiece_Sound, Vector3.zero, 1.0f);
    }

    public void OnNextMove()
    {
        if (curMove >= moveList.Count)
            return;

        board.move(moveList[curMove]);
        UpdateInfos();
        Wnd_Board.UpdateBoard();
        curMove++;
        AudioSource.PlayClipAtPoint(MovePiece_Sound, Vector3.zero, 1.0f);
    }

    public void OnLastMove()
    {
        while (curMove < moveList.Count)
        {
            board.move(moveList[curMove]);
            curMove++;
        }
        UpdateInfos();
        Wnd_Board.UpdateBoard();
        AudioSource.PlayClipAtPoint(MovePiece_Sound, Vector3.zero, 1.0f);
    }

    public void OnRotateBoard()
    {
        Wnd_Board.FlipBoard(!Wnd_Board.isFlipped());
    }

    public void OnSaveLine()
    {
        WndManager.Singleton.OpenMsgBox("Do you really want to save the changes?", CallBackSave, WndManager.MSGBOX_BTN_TYPE.YesNo);
    }

    void saveLine()
    {
        if (!isChanged)
            return;

        isChanged = false;
        line.moveList.Clear();
        for (int i = 0; i < moveList.Count; i++)
            line.moveList.Add(moveList[i]);
    }

    public void OnPieceMove(cgSimpleMove _move)
    {
        isChanged = true;
        Btn_Save.enabled = true;

        if (curMove < moveList.Count)
            moveList.RemoveRange(curMove, moveList.Count - curMove);

        moveList.Add(_move);
        board.move(_move);

        if (board.isChecked(board.whiteTurnToMove))
            AudioSource.PlayClipAtPoint(Check_Sound, Vector3.zero, 1.0f);
        else
            AudioSource.PlayClipAtPoint(MovePiece_Sound, Vector3.zero, 1.0f);

        curMove++;

        UpdateInfos();
        Wnd_Board.UpdateBoard();
    }

    void UpdateInfos()
    {
        // update notation
        cgNotation not = new cgNotation(board);
        Lbl_Moves.text = not.writeFullNotation(cgNotation.NotationType.Algebraic);

        // for change scroll position 
        float height = Lbl_Moves.preferredHeight + 20;
        NotationTextScrollRect.content.sizeDelta = new Vector2(NotationTextScrollRect.content.sizeDelta.x, height);
        if (NotationTextScrollRect.viewport.rect.height < height)
            NotationTextScrollRect.content.anchoredPosition = new Vector2(0, height - NotationTextScrollRect.viewport.rect.height);

        // update move count
        Lbl_MoveCount.text = string.Format("({0}) moves", moveList.Count);
 
    }

    bool CallBackSave(WndManager.MSGBOX_BTN _btn)
    {
        if (_btn == WndManager.MSGBOX_BTN.YES)
        {
            saveLine();
        }

        return true;
    }

    bool CallBackSaveAndExit(WndManager.MSGBOX_BTN _btn)
    {
        if (_btn == WndManager.MSGBOX_BTN.YES)
        {
            saveLine();
        }

        WndManager.Singleton.OpenLibraryPage(line.parent.name);
        return true;
    }
}
