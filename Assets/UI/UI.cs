using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI : MonoBehaviour
{
	public Text Follower;
    public Text Info;
    public Text Unread;
    public Text LikeMultiplier;
    public Text PostMultiplier;

    // Use this for initialization
    void Start()
    {
        Game.Instance.Player.Follower.RegisterObserverScoped(gameObject, (v) =>
        {
            if (Follower != null) Follower.text = string.Format("Follower: {0:0}", v);
        });

        Game.Instance.Player.UnreadPostings.RegisterObserverScoped(gameObject, (vOld, vNew) =>
        {
            if (Unread != null) Unread.text = string.Format("{0:0}", vNew.Count);
        });

        Game.Instance.Player.LikeMultiplier.RegisterObserverScoped(gameObject, (v) =>
        {
            if (LikeMultiplier != null) LikeMultiplier.text = string.Format("Madness: {0:0.0}", v);
        });

        Game.Instance.Player.PostMultiplier.RegisterObserverScoped(gameObject, (v) =>
        {
            if (PostMultiplier != null) PostMultiplier.text = string.Format("Madness: {0:0.0}", v);
        });

        Game.Instance.Player.EffectiveFollowerPerSecond.RegisterObserverScoped(gameObject, (v) =>
		{
		    Follower.text = string.Format("Follower:  {0:0}", v);
		});
    }

    void Update()
    {
        var player = Game.Instance.Player;

        Info.text = string.Format(@"
Follower: {0:0.0}
FollowerPerSecond: {1:0.0}
LikeMultiplier: x {2:0.0} 
PostMultiplier: x {3:0.0} 
Blocks: {4} 
Events: {5}
Hashtags: {6}
CreatePost: {7:0.0} / {8:0.0}", 
player.Follower.get,
player.EffectiveFollowerPerSecond.get,
player.LikeMultiplier.get,
player.PostMultiplier.get,
player.MultiplierBlockRemaingTimes.get.Count,
player.PlannedEvents.get.Count,
string.Join(",", player.Hashtags.get.Select(it => it.Text).ToArray()),
player.CreatePostLikeValueMultiplier.get,
player.CreatePostLikeValueMultiplierIncPerSecond.get
);
    }
}
