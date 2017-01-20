using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance = null;

    public Data.Player Player;

    public float TimeoutPosting = 1f;
    public int MaxUnreadPostings = 100;
    public float PostingLikeValueMin = 1f;
    public float PostingLikeValueMax = 5f;

    void Awake()
    {
        Instance = this;

        Player = new Data.Player();
        Player.LikesPerSecond.set = 0.1f;
        Player.MadnessPerSecond.set = 0.01f;
        Player.LikeMultiplier.set = 1f;
    }

    IEnumerator Start()
    {
        while (true) {
            if (Player.UnreadPostings.get.Count < MaxUnreadPostings) CreatePosting();
            yield return new WaitForSeconds(TimeoutPosting);
        }
    }

    public void CreatePosting()
    {
        var l = Player.UnreadPostings.get;
        l.Add(new Data.Posting()
        {
            LikeValue = UnityEngine.Random.Range(PostingLikeValueMin, PostingLikeValueMax),
            Hashtags = new List<string>() { "lala" },
        });
        Player.UnreadPostings.set = l;
    }

    void Update()
    {
        Player.Update(Time.deltaTime);
    }

    public void Like(Data.Posting posting)
    {
        Player.Likes.set = Player.Likes.get + posting.LikeValue * Player.LikeMultiplier.get;
        var l = Player.UnreadPostings.get;
        l.Remove(posting);
        Player.UnreadPostings.set = l;
    }
}