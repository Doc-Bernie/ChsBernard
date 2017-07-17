using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctLine  {
    public ctLibrary parent { get; set; }

    public string name { get; set; }

    public ctMove moves { get; set; }

    public ctLine(ctLibrary _parent)
    {
        parent = _parent;
        moves = new ctMove(this);
    }

    public void Clone(ctLine _line)
    {
        this.name = _line.name;
        this.moves.Clone(_line.moves);
    }

}
