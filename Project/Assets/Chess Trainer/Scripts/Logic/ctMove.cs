using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctMove {
    public ctLine parent { get; set; }

    public List<string> moves = new List<string>();
    
    public ctMove(ctLine _parent)
    {
        parent = _parent;
    }

    public void SaveFile(string _key)
    {

    }

    public void ReadFile(string key)
    {

    }

    public int MoveCount()
    {
        return moves.Count;
    }

    public void Clone(ctMove _move)
    {
        for (int i = 0; i < _move.moves.Count; i ++)
        {
            moves.Add(_move.moves[i]);
        }
    }

    public override string ToString()
    {
        string strRet = "";

        for (int i = 0; i < moves.Count; i++)
            strRet += string.Format("{0}.{1}", (i + 1), moves[i]);
        
        return strRet;
    }
}
