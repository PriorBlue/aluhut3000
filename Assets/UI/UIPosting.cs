using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPosting : MonoBehaviour {
    public Data.Posting Posting;

    public Text Text;
    public Button ButtonLike; 

    void Refresh()
    {
        if (Posting == null) return;
        Text.text = string.Format("{0}", Posting.Text);
    }

	// Use this for initialization
	void Start () {
        Refresh();
	}

    private void Like()
    {
        Game.Instance.Like(Posting);
    }

    // Update is called once per frame
    void Update () {
        Refresh();
	}
}
