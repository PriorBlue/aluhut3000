using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIPostings : MonoBehaviour
{
    public GameObject Prefab;

	// Use this for initialization
	void Start () {
        Game.Instance.Player.UnreadPostings.RegisterObserverScoped(gameObject, (lOld, lNew) =>
        {
            foreach (Transform it in transform) GameObject.Destroy(it.gameObject);
            foreach (var it in lNew.Take(10))
            {
                var go = GameObject.Instantiate(Prefab) as GameObject;
                go.GetComponent<UIPosting>().Posting = it;
                go.transform.SetParent(transform);
            }
        });
	}
}
