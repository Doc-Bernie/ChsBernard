using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineCell : MonoBehaviour
{
    public TMPro.TextMeshProUGUI txtName = null;
    public string name { get; set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetName(string _name)
    {
        name = _name;
        ctLine line = ctGameManager.Singleton.GetLine(_name);
        txtName.text = string.Format("{0} ({1})", line.name, line.moves.MoveCount());
    }
}
