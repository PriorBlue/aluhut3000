using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour {
    public Data.ShopItem ShopItem;

    public Text TextCost;
    public Text TextBuys;
    public Button ButtonBuy;
    public Image Image;

    void Refresh()
    {
        if (ShopItem == null) return;
        TextCost.text = string.Format("{0}", ShopItem.CostLikes);
        TextBuys.text = string.Format("{0}", ShopItem.RemainingBuys.get);
        TextBuys.gameObject.SetActive(!ShopItem.IsUnlimited);
        Image.sprite = SpriteFactory.Instance.Get(ShopItem.Asset);
        TextCost.color = Game.Instance.CanBuy(ShopItem) ? Color.green : Color.red;
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
