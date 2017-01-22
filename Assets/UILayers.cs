using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class UILayers : MonoBehaviour
{
    public GameObject Prefab;

    // Use this for initialization
    void Start()
    {
        Game.Instance.Player.ActiveItems.RegisterObserverScoped(gameObject, (lOld, lNew) =>
        {
            foreach (var it in lNew)
            {
                if (!string.IsNullOrEmpty(it.AssetLayer))
                {
                    var go = GameObject.Instantiate(Prefab) as GameObject;
                    go.GetComponent<Image>().sprite = SpriteFactory.Instance.Get(it.AssetLayer);
                    go.transform.SetParent(transform);
                }
            }
        });
    }
}
