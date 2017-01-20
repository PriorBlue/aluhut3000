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
        Text.text = string.Format("LikeValue: {0}", Posting.LikeValue);
    }

	// Use this for initialization
	void Start () {
        ButtonLike.onClick.AddListener(Like);
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
