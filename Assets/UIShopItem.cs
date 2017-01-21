using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour {
    public Data.ShopItem ShopItem;

    public Text Text;
    public Button ButtonBuy;
    public Image Image;

    void Refresh()
    {
        if (ShopItem == null) return;
        Text.text = string.Format("{0}", ShopItem.CostLikes);
        Image.sprite = SpriteFactory.Instance.Get(ShopItem.Asset);
    }

    // Use this for initialization
    void Start()
    {
        ButtonBuy.onClick.AddListener(Like);
        Refresh();
    }

    private void Like()
    {
        Game.Instance.Buy(ShopItem);
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
