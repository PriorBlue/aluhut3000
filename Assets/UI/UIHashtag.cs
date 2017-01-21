using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHashtag : MonoBehaviour {
    public Data.HashtagInfo HashtagInfo;

    public Text Text;
    public Text TextUsagesLeft;
    public Button ButtonUse;
    public Image ImageUsed;

    void Refresh()
    {
        if (HashtagInfo == null) return;
        Text.text = string.Format("{0}", HashtagInfo.Text);
        TextUsagesLeft.text = string.Format("{0}", HashtagInfo.UsagesLeft.get);
        ImageUsed.gameObject.SetActive(HashtagInfo.IsGettingUsed.get);
    }

    // Use this for initialization
    void Start()
    {
        ButtonUse.onClick.AddListener(Use);
        Refresh();
    }

    private void Use()
    {
        HashtagInfo.IsGettingUsed.set = !HashtagInfo.IsGettingUsed.get;
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
