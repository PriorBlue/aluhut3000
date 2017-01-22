using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI : MonoBehaviour
{
    public Text Likes;
	public Text Follower;
    public Text Info;
    public Text Unread;
    public Text LikeMultiplier;

    // Use this for initialization
    void Start()
    {
        Game.Instance.Player.Follower.RegisterObserverScoped(gameObject, (v) =>
        {
            Likes.text = string.Format("Likes: {0:0}", v);
        });

        Game.Instance.Player.UnreadPostings.RegisterObserverScoped(gameObject, (vOld, vNew) =>
        {
            Unread.text = string.Format("{0:0}", vNew.Count);
        });

        Game.Instance.Player.LikeMultiplier.RegisterObserverScoped(gameObject, (v) =>
        {
            LikeMultiplier.text = string.Format("Madness: {0:0.0}", v);
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
FollowerPerSecond: {0:0.0}
LikeMultiplier: x {1:0.0} 
PostMultiplier: x {2:0.0} 
Blocks: {3} 
Events: {4}
Hashtags: {5}", 
player.EffectiveFollowerPerSecond.get,
player.LikeMultiplier.get,
player.PostMultiplier.get,
player.MultiplierBlockRemaingTimes.get.Count,
player.PlannedEvents.get.Count,
string.Join(",", player.Hashtags.get.Select(it => it.Text).ToArray()));
    }
}
