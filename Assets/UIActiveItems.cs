using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActiveItems : MonoBehaviour {
    public GameObject Prefab;

    // Use this for initialization
    void Start()
    {
        Game.Instance.Player.ActiveItems.RegisterObserverScoped(gameObject, (lOld, lNew) =>
        {
            foreach (Transform it in transform) GameObject.Destroy(it.gameObject);
            foreach (var it in lNew)
            {
                var go = GameObject.Instantiate(Prefab) as GameObject;
                go.GetComponent<UIActiveItem>().ActiveItem = it;
                go.transform.SetParent(transform);
            }
        });
    }
}
