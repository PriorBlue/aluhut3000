using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text Likes;
    public Text Madness;
    public Text Unread;
    public Text LikeMultiplier;

    // Use this for initialization
    void Start()
    {
        Game.Instance.Player.Madness.RegisterObserverScoped(gameObject, (v) =>
        {
            Madness.text = string.Format("Madness: {0:0.0}", v);
        });

        Game.Instance.Player.Likes.RegisterObserverScoped(gameObject, (v) =>
        {
            Likes.text = string.Format("{0:0.0}", v);
        });

        Game.Instance.Player.UnreadPostings.RegisterObserverScoped(gameObject, (vOld, vNew) =>
        {
            Unread.text = string.Format("{0:0}", vNew.Count);
        });

        Game.Instance.Player.LikeMultiplier.RegisterObserverScoped(gameObject, (v) =>
        {
            LikeMultiplier.text = string.Format("x {0:0.0}", v);
        });
    }
}
