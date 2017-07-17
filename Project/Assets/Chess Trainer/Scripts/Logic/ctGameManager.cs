using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctGameManager : MonoBehaviour {

    #region Singleton
    protected static ctGameManager _instance = null;

    public static ctGameManager Singleton
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType(typeof(ctGameManager)) as ctGameManager;
            return _instance;
        }
    }
    #endregion Singleton

    public List<ctLibrary> m_Libraries = new List<ctLibrary>();
    public ctTrainer m_Trainer = new ctTrainer();

    string LIBRARY_COUNT_KEY = "LCK";
    string LIBRARY_NAME_KEY = "LNK_";
    string LINE_COUNT_KEY = "LineCountKey_";
    string LINE_NAME_KEY = "LineNameKey_";
    string LINE_ORDER_KEY = "LineOrderKey_";

    string TRAINER_NAME = "TrainerName";
    string TRAINER_SCORE = "TrainerScore";

	// Use this for initialization
	void Awake () {
        loadData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDestroy()
    {
        saveData();
    }

    public void loadData()
    {
        m_Libraries.Clear();

        int libCount = PlayerPrefs.GetInt(LIBRARY_COUNT_KEY, 0);
        if (libCount > 0)
        {
            for (int idxLib = 0; idxLib < libCount; idxLib++)
            {
                ctLibrary lib = new ctLibrary();
                lib.name = PlayerPrefs.GetString(LIBRARY_NAME_KEY + idxLib.ToString(), "");
                int lineCount = PlayerPrefs.GetInt(LINE_COUNT_KEY + idxLib.ToString(), 0);

                for (int idxLine = 0; idxLine < lineCount; idxLine++)
                {
                    ctLine line = new ctLine(lib);
                    line.name = PlayerPrefs.GetString(LINE_NAME_KEY + idxLib.ToString() + "_" + idxLine.ToString(), "");
                    line.moves.ReadFile(LINE_ORDER_KEY + idxLib.ToString() + "_" + idxLine.ToString());

                    if (lib.GetLineIdx(line.name) > -1)
                    {
                        Debug.Log("Duplicated line name found, cannot add line");
                    }
                    else
                        lib.lines.Add(line);
                }

                if (GetLibrary(lib.name) != null)
                {
                    // 
                    Debug.Log("Duplicated library name found, cannot add library");
                }
                else
                    m_Libraries.Add(lib);
            }
        }
        else
            _createDefaultLibrary();

        m_Trainer.name = PlayerPrefs.GetString(TRAINER_NAME, "Michael");
        m_Trainer.score = PlayerPrefs.GetFloat(TRAINER_SCORE, 0);
    }

    public void saveData()
    {
        PlayerPrefs.SetInt(LIBRARY_COUNT_KEY, m_Libraries.Count);
        if (m_Libraries.Count > 0)
        {
            for (int idxLib = 0; idxLib < m_Libraries.Count; idxLib++)
            {
                ctLibrary lib = m_Libraries[idxLib];
                PlayerPrefs.SetString(LIBRARY_NAME_KEY + idxLib.ToString(), lib.name);
                PlayerPrefs.SetInt(LINE_COUNT_KEY + idxLib.ToString(), lib.lines.Count);

                for (int idxLine = 0; idxLine < lib.lines.Count; idxLine ++ )
                {
                    ctLine line = lib.lines[idxLine];
                    PlayerPrefs.SetString(LINE_NAME_KEY + idxLib.ToString() + "_" + idxLine.ToString(), line.name);
                    line.moves.SaveFile(LINE_ORDER_KEY + idxLib.ToString() + "_" + idxLine.ToString());
               }
            }
        }

        PlayerPrefs.SetString(TRAINER_NAME, m_Trainer.name);
        PlayerPrefs.SetFloat(TRAINER_SCORE, m_Trainer.score);
    }

    void _createDefaultLibrary()
    {
        ctLibrary lib = new ctLibrary();
        lib.name = "Spanish Openings";

        ctLine line = new ctLine(lib);
        line.name = "Morphy Defense";

        lib.lines.Add(line);

        m_Libraries.Add(lib);
    }

    public ctLine GetLine(string _name)
    {
        for (int i = 0; i < m_Libraries.Count; i ++)
        {
            if (m_Libraries[i].GetLine(_name) != null)
                return m_Libraries[i].GetLine(_name);
        }
        return null;
    }

    public bool AddLibrary(string _name, ctLibrary _lib)
    {
        if (GetLibrary(_name) != null)
            return false;

        m_Libraries.Add(_lib);
        return true;
    }

    public void RemoveLibrary(string _name)
    {
        ctLibrary lib = GetLibrary(_name);
        if (lib == null)
            return;

        m_Libraries.Remove(lib);
    }

    public bool RenameLibrary(string _oldName, string _newName)
    {
        ctLibrary lib = GetLibrary(_oldName);
        if (lib == null)
            return false;

        if (GetLibrary(_newName) != null)
            return false;

        lib.name = _newName;
        return true;
    }

    public ctLibrary GetLibrary(string _name)
    {
        int idx = GetLibraryIdx(_name);
        if (idx < 0)
            return null;

        return m_Libraries[idx];
    }

    public int GetLibraryIdx(string _name)
    {
        for (int i = 0; i < m_Libraries.Count; i ++ )
        {
            if (m_Libraries[i].name.Equals(_name))
                return i;
        }
        return -1;
    }

    public ctLibrary GetLibrary(int idx)
    {
        return idx < m_Libraries.Count ? m_Libraries[idx] : null;
    }
}
