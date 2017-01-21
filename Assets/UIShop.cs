using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShop : MonoBehaviour {
    public GameObject Prefab;

	// Use this for initialization
	void Start () {
        Game.Instance.Player.ShopItems.RegisterObserverScoped(gameObject, (lOld, lNew) =>
        {
            foreach (Transform it in transform) GameObject.Destroy(it.gameObject);
            foreach (var it in lNew)
            {
                var go = GameObject.Instantiate(Prefab) as GameObject;
                go.GetComponent<UIShopItem>().ShopItem = it;
                go.transform.SetParent(transform);
            }
        });
    }
}
