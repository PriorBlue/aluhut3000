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

	// Use this for initialization
	void Start () {
        Game.Instance.Player.UnreadPostings.RegisterObserverScoped(gameObject, (lOld, lNew) =>
        {
            foreach (Transform it in transform) GameObject.Destroy(it.gameObject);
            int idx = 0;
            foreach (var it in lNew.Take(Take))
            {
                if (idx % OnlyShowNth == OnlyShowNthSelector)
                {
                    var go = GameObject.Instantiate(Prefab) as GameObject;
                    go.GetComponent<UIPosting>().Posting = it;
                    go.transform.SetParent(transform);
					go.transform.localPosition = Vector3.zero;
					go.transform.localScale = Vector3.one;
					go.transform.localEulerAngles = Vector3.zero;
                }
                ++idx;
            }
        });
	}
}
