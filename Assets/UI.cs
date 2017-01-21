using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text Likes;
    public Text Info;
    public Text Unread;
    public Text LikeMultiplier;

    // Use this for initialization
    void Start()
    {
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

    void Update()
    {
        var player = Game.Instance.Player;

        Info.text = string.Format(@"Madness: {0:0.0}
LPS: {1:0.0} MPS: {2:0.0}", 
player.Madness.get, player.LikesPerSecond.get, player.MadnessPerSecond.get);
    }
}
