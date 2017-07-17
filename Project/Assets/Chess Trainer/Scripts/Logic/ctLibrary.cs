using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctLibrary {

    public string name { get; set; }

    public List<ctLine> lines = new List<ctLine>();


    public bool AddLine(ctLine _line, int idx = -1)
    {
        if (idx == -1)
            lines.Add(_line);
        else
            lines.Insert(idx, _line);

        return true;
    }

    public bool RemoveLine(string _name)
    {
        ctLine line = GetLine(_name);
        if (line != null)
            lines.Remove(line);

        return true;
    }

    public bool RenameLine(string _srcName, string _dstName)
    {
        ctLine line = GetLine(_srcName);
        if (line == null)
            return false;

        if (GetLine(_dstName) != null)
            return false;

        line.name = _dstName;
        return true;
    }

    public void Clone(ctLibrary _lib)
    {
        this.name = _lib.name;

        for (int i = 0; i < _lib.lines.Count; i ++)
        {
            ctLine line = new ctLine(this);
            line.Clone(_lib.lines[i]);
            this.lines.Add(line);
        }
    }

    public ctLine GetLine(string _name)
    {
        int idx = GetLineIdx(_name);
        if (idx < 0)
            return null;

        return lines[idx];
    }

    public ctLine GetLine(int idx)
    {
        return idx < lines.Count ? lines[idx] : null;
    }

    public int GetLineIdx(string _name)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].name.Equals(_name))
                return i;
        }
        return -1;
    }
}
