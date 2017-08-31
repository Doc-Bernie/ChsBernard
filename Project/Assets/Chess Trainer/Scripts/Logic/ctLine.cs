using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctLine  {
    public ctLibrary parent { get; private set; }

    public string name { get; set; }

    public string fullName
    {
        get
        {
            return parent.name + "/" + name;
        }
    }

    public List<cgSimpleMove> moveList = new List<cgSimpleMove>();

    public string moves { 
        get
        {
            cgNotation not = new cgNotation(new cgBoard());
            not.moves = moveList;
            return not.writeFullNotation(cgNotation.NotationType.Algebraic);
        }
        set
        {
            moveList.Clear();
            if (!value.Equals(""))
            {
                cgNotation not = new cgNotation(new cgBoard());
                not.Read(value);

                for (int i = 0; i < not.moves.Count; i++)
                    moveList.Add(not.moves[i]);
            }
        }
    }


    public ctLine(ctLibrary _parent, string _name, string _moves = "")
    {
        parent = _parent;
        name = _name;
        moves = _moves;
    }

    public void Clone(ctLine _line)
    {
        this.name = _line.name;
        this.moves = _line.moves;

        cgNotation not = new cgNotation(new cgBoard());
        not.Read(this.moves);
    }

}
