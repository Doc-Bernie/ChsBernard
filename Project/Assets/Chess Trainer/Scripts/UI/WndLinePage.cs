using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WndLinePage : MonoBehaviour {
    public TMPro.TextMeshProUGUI Lbl_Title = null;
    public TMPro.TextMeshProUGUI Lbl_MoveCount = null;
    public TMPro.TextMeshProUGUI Lbl_Moves = null;
    public WndBoard Wnd_Board = null;

    protected string lineName = "";
    protected ctLine line = null;

    protected int curMove = 0;
    protected bool isChanged = false;

    protected cgBoard board = new cgBoard();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Escape))
        {
            WndManager.Singleton.OpenLibraryPage(line.parent.name);
        }
	}

    public void Init(string _lineName)
    {
        lineName = _lineName;
        line = ctGameManager.Singleton.GetLine(_lineName);

        if (line == null)
            return;


        Lbl_Title.text = _lineName;

        Lbl_MoveCount.text = string.Format("({0}) moves", line.moveList.Count);
        Lbl_Moves.text = line.moves.ToString();
        curMove = 0;
        isChanged = false;

        Wnd_Board.UpdateBoard(board);
    }

    public void OnFirstMove()
    {
        while (curMove > 0)
        {
            board.revert();
            curMove--;
        }
        Wnd_Board.UpdateBoard(board);
    }

    public void OnPrevMove()
    {
        if (curMove == 0)
            return;

        board.revert();
        Wnd_Board.UpdateBoard(board);
        curMove--;
    }

    public void OnNextMove()
    {
        if (curMove >= line.moveList.Count)
            return;

        board.move(line.moveList[curMove]);
        Wnd_Board.UpdateBoard(board);
        curMove++;
    }

    public void OnLastMove()
    {
        while (curMove < line.moveList.Count)
        {
            board.move(line.moveList[curMove]);
            curMove++;
        }
        Wnd_Board.UpdateBoard(board);
    }

    public void OnRotateBoard()
    {
        Wnd_Board.FlipBoard();
    }

    public void OnSaveLine()
    {
        isChanged = true;
    }
}
