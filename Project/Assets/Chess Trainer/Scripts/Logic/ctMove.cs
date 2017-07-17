using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctMove {
    public ctLine parent { get; set; }

    public List<string> moves = new List<string>();
    
    public ctMove(ctLine _parent)
    {
        parent = _parent;

        moves.Add("e4 e5");
        moves.Add("Nf3 Nc6");
        moves.Add("Bb5 a6");
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

    public string ToString()
    {
        return "1.e4 e5 2.Nf3 Nc6 3.Bb5 a6";
    }
}
