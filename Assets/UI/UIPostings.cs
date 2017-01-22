using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIPostings : MonoBehaviour
{
    public GameObject Prefab;

    public int OnlyShowNth = 2;
    public int OnlyShowNthSelector = 0;
    public int Take = 50;
    public int Limit = 20;

	// Use this for initialization
	void Start () {
        Game.Instance.Player.UnreadPostings.RegisterObserverScoped(gameObject, (lOld, lNew) =>
        {
            foreach (Transform it in transform) GameObject.Destroy(it.gameObject);
            int idx = 0;
            int limit = Limit;
            foreach (var it in lNew.Take(Take))
            {
                if (idx % OnlyShowNth == OnlyShowNthSelector)
                {
                    var go = GameObject.Instantiate(Prefab) as GameObject;
                    go.GetComponent<UIPosting>().Posting = it;
                    go.transform.SetParent(transform);
                    --limit;
                    if (limit <= 0) break;
                }
                ++idx;
            }
        });
	}
}
