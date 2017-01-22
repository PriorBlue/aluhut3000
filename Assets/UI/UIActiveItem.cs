using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActiveItem : MonoBehaviour {
    public Data.ActiveItem ActiveItem;

    public Text Text;
    public Image Image;
	public Image Fill;

    // Use this for initialization
    void Start () {
        Refresh();
	}

    private void Refresh()
    {
        if (ActiveItem == null) return;

        if (ActiveItem.IsTemporary)
        {
            Text.text = string.Format("{0:0.0}", ActiveItem.LifetimeLeft.get);
			Fill.fillAmount = ActiveItem.LifetimeLeft.get / ActiveItem.LifetimeMax;
        } else
        {
            Text.text = "";
			Fill.fillAmount = 0f;
        }

        Image.sprite = SpriteFactory.Instance.Get(ActiveItem.Asset);
    }

    // Update is called once per frame
    void Update () {
        Refresh();
    }
}
