using System.Collections;
using System.Collections.Generic;
using U3D.KVO;
using UnityEngine;

public class Data
{
    [System.Serializable]
    public class Player
    {
        public ValueObserving<float> Follower = new ValueObserving<float>();
        public ValueObserving<float> FollowerPerSecond = new ValueObserving<float>();

        public ValueObserving<float> LikeMultiplier = new ValueObserving<float>();
        public ValueObserving<float> PostMultiplier = new ValueObserving<float>();

        public ListObserving<float> MultiplierBlockRemaingTimes = new ListObserving<float>();

        public ListObserving<ActiveItem> ActiveItems = new ListObserving<ActiveItem>();
        public ListObserving<Posting> UnreadPostings = new ListObserving<Posting>();
        public ListObserving<ShopItem> ShopItems = new ListObserving<ShopItem>();
        public ListObserving<PossiblePostingOrEvent> PossiblePostingsOrEvents = new ListObserving<PossiblePostingOrEvent>();
        public ListObserving<Event> PlannedEvents = new ListObserving<Event>();
        public ListObserving<HashtagInfo> Hashtags = new ListObserving<HashtagInfo>();

        public void Update(float deltaT)
        {
            float likeMultiplier = 1f;
            ActiveItems.get.ForEach(it => likeMultiplier += it.LikeMultiplierAddition);
            LikeMultiplier.set = likeMultiplier;

            float postMultiplier = 1f;
            ActiveItems.get.ForEach(it => postMultiplier += it.PostMultiplierAddition);
            PostMultiplier.set = postMultiplier;

            var fps = FollowerPerSecond.get;
            ActiveItems.get.ForEach(it => fps += it.LikesPerSecond);
            Follower.set = Follower.get + fps * Time.deltaTime;

            PlannedEvents.get.ForEach(it => it.Timeout -= Time.deltaTime);

            Hashtags.get.ForEach(it => it.UsagesLeft.set = it.UsagesLeft.get + Time.deltaTime * it.UsagesPerSeconds);

            ActiveItems.get.ForEach(it => it.LifetimeLeft.set = Mathf.Max(0f, it.LifetimeLeft.get - Time.deltaTime));

            // remove timeout items
            var l = ActiveItems.get;
            foreach(var it in l.ToArray())
            {
                if (it.IsTemporary && it.LifetimeLeft.get <= 0f)
                {
                    l.Remove(it);
                }
            }
            ActiveItems.set = l;
        }
    }

    [System.Serializable]
    public class ShopItem
    {
        public string Name;
        public string Text;
        public string Asset;

        public string Type;

        public float CostLikes;

        public float LikesAdd;
        public float LikesPerSecond;
        
        public List<string> Tags = new List<string>();
        public List<string> EventTags = new List<string>();

        public float LikeMultiplierAddition;
        public float PostMultiplierAddition;

        public float ActiveItemLifetime;

        public bool IsTemporary;

        public bool IsUnlimited;

        public ValueObserving<int> RemainingBuys = new ValueObserving<int>();

        public string Hashtag;
        public int HashtagUsages;
        public float HashtagUsagesPerSeconds;
        public float HashtagFollower;
    }
    
    [System.Serializable]
    public class ActiveItem
    {
        public string Name;
        public string Text;
        public string Asset;

        public string Type;

        public float LikeMultiplierAddition;
        public float PostMultiplierAddition;

        public float LikesPerSecond;

        public List<string> Tags = new List<string>();
        public List<string> EventTags = new List<string>();

        public ValueObserving<float> LifetimeLeft;

        public bool IsTemporary;
    }

    [System.Serializable]
    public class HashtagInfo
    {
        public string Text;
        public ValueObserving<float> UsagesLeft = new ValueObserving<float>();
        public float UsagesPerSeconds;
        public float Follower;

        public ValueObserving<bool> IsGettingUsed = new ValueObserving<bool>();
    }

    [System.Serializable]
    public class Posting
    {
        public List<string> Hashtags = new List<string>();
        public float LikeValue;
        public string Text;
    }

    [System.Serializable]
    public class Event
    {
        public string Text;
        public string Asset;
        public float Timeout;
        public List<string> Tags = new List<string>();
    }

    [System.Serializable]
    public class PossiblePostingOrEvent
    {
        public string Text;
        public float LikeValue;
        public List<string> Hashtags = new List<string>();
        public int RandomWeight;

        public bool IsEvent;

        public string EventText;
        public string EventAsset;
        public List<string> EventTags = new List<string>();
        public float EventTimeout;

        public Posting CreatePosting()
        {
            return new Posting()
            {
                Hashtags = Hashtags,
                LikeValue = LikeValue,
                Text = Text,
            };
        }

        public Event CreateEvent()
        {
            return new Event()
            {
                Asset = EventAsset,
                Tags = EventTags,
                Text = EventText,
                Timeout = EventTimeout,
            };
        }
    }
}
