using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctLibrary {

    public string name { get; set; }

    public List<ctLine> lines = new List<ctLine>();


    public bool AddLine(ctLine _line, bool _sort = false)
    {
        lines.Add(_line);

        if (_sort)
        {
            lines.Sort(delegate(ctLine p1, ctLine p2)
            {
                return p1.name.CompareTo(p2.name);
            });
        }
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
            ctLine line = new ctLine(this, _lib.lines[i].name, _lib.lines[i].moves);
            AddLine(line);
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
        _name = _name.ToLower();
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].name.ToLower().Equals(_name))
                return i;
        }
        return -1;
    }
}
