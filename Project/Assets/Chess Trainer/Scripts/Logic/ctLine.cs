using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctLine  {
    public ctLibrary parent { get; private set; }

    public string name { get; set; }

    public string moves ;

    public List<cgSimpleMove> moveList = new List<cgSimpleMove>();

    public ctLine(ctLibrary _parent, string _name, string _moves = "")
    {
        parent = _parent;
        name = _name;
        moves = _moves;


        cgNotation not = new cgNotation(new cgBoard());
        not.Read(moves);

        for (int i = 0; i < not.moves.Count; i++)
            moveList.Add(not.moves[i]);
    }

    public void Clone(ctLine _line)
    {
        this.name = _line.name;
        this.moves = _line.moves;

        cgNotation not = new cgNotation(new cgBoard());
        not.Read(this.moves);
    }

}
