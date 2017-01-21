using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHashtags : MonoBehaviour
{
    public GameObject Prefab;

    // Use this for initialization
    void Start()
    {
        Game.Instance.Player.Hashtags.RegisterObserverScoped(gameObject, (lOld, lNew) =>
        {
            foreach (Transform it in transform) GameObject.Destroy(it.gameObject);
            foreach (var it in lNew)

            {
                // only show tags that have posts left
                if (it.UsagesLeft.get >= 1f)
                {
                    var go = GameObject.Instantiate(Prefab) as GameObject;
                    go.GetComponent<UIHashtag>().HashtagInfo = it;
                    go.transform.SetParent(transform);
                }
            }
        });
    }

    public void Post()
    {
        var l = Game.Instance.Player.Hashtags.get;
        Game.Instance.Post();
    }
}
