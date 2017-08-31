using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WndTrainerSettingPage : MonoBehaviour {
    public Toggle Tgl_White;
    public Toggle Tgl_Black;
    public Toggle Tgl_Shuffle;
    public Toggle Tgl_AllLines;

    public GameObject List_Lines;
    public MonoBehaviour ScrollViewLineList;
    public Button Btn_Ok;

    public GameObject LineListCellPrefab;

    protected List<string> LibraryList = null;
    protected List<GameObject> LineCellObjs = new List<GameObject>();
    protected List<ctLine> LineList = new List<ctLine>();

    public void OnEscape()
    {
        WndManager.Singleton.OpenMainPage();
    }

    public void Init(List<string> _libList)
    {
        LibraryList = _libList;

        Tgl_White.isOn = true;
        Tgl_Black.isOn = false;

        Tgl_Shuffle.isOn = false;
        Tgl_AllLines.isOn = true;

        AddLines();
        UpdateWnd();
    }

	void UpdateWnd()
    {
        int selCount = 0;
        for (int i = 0; i < LineCellObjs.Count; i ++)
        {
            Toggle tglCell = LineCellObjs[i].GetComponent<Toggle>();
            if (tglCell.isOn)
                selCount++;

            tglCell.enabled = !Tgl_AllLines.isOn;
            if (Tgl_AllLines.isOn)
                tglCell.isOn = true;
        }

        Btn_Ok.enabled = selCount > 0;
    }

    void AddLines()
    {
        int i = 0;
        for (i = 0; i < List_Lines.transform.childCount; i++)
            Destroy(List_Lines.transform.GetChild(i).gameObject);

        LineCellObjs.Clear();
        LineList.Clear();
        
        i = 0;
        for (int idxLib = 0; idxLib < LibraryList.Count; idxLib++)
        {
            ctLibrary lib = ctGameManager.Singleton.GetLibrary(LibraryList[idxLib]);

            for (int idxLine = 0; idxLine < lib.lines.Count; idxLine ++)
            {
                if (lib.lines[idxLine].moveList.Count < 1)
                    continue;

                LineList.Add(lib.lines[idxLine]);

                GameObject childObj = GameObject.Instantiate(LineListCellPrefab);

                childObj.transform.SetParent(List_Lines.transform, false);

                string text = string.Format("{0} ({1})", lib.lines[idxLine].fullName, lib.lines[idxLine].moveList.Count);

                childObj.GetComponent<LibraryCell>().Init(i, text, ScrollViewLineList);
                childObj.GetComponent<LibraryCell>().SetToggleMode(true);
                //childObj.GetComponent<Toggle>().group = LibraryList.GetComponent<ToggleGroup>();
                childObj.GetComponent<Toggle>().isOn = true;
                childObj.GetComponent<Toggle>().onValueChanged.AddListener(OnListLineSellChange);

                if (Tgl_AllLines.isOn)
                    childObj.GetComponent<Toggle>().enabled = false;

                LineCellObjs.Add(childObj);
                i++;
            }
        }
    }

    public void OnCheckSide()
    {
        UpdateWnd();
    }

    public void OnCheckShuffle()
    {
        UpdateWnd();

    }

    public void OnCheckAllLines()
    {
        UpdateWnd();

    }

    public void OnListLineSellChange(bool _check)
    {
        UpdateWnd();
    }

    public void OnBtnOk()
    {
        // Gather selected line list
        List<ctLine> selLinList = new List<ctLine>();
        for (int i = 0; i < LineCellObjs.Count; i ++)
        {
            if (LineCellObjs[i].GetComponent<Toggle>().isOn)
                selLinList.Add(LineList[i]);
        }

        // Shuffle lines if shuffle is checked
        if (Tgl_Shuffle.isOn)
        {
            List<ctLine> shuffleList = new List<ctLine>();
            while (selLinList.Count > 0)
            {
                int randIdx = Random.Range(0, selLinList.Count);
                shuffleList.Add(selLinList[randIdx]);
                selLinList.RemoveAt(randIdx);
            }

            selLinList = shuffleList;
        }


        WndManager.Singleton.OpenTrainPage(Tgl_White.isOn, selLinList, true);
    }
}
