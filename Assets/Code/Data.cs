using System.Collections;
using System.Collections.Generic;
using U3D.KVO;
using UnityEngine;

public class Data
{
    [System.Serializable]
    public class Player
    {
        /// <summary>
        /// money
        /// </summary>
        public ValueObserving<float> Likes = new ValueObserving<float>();
        /// <summary>
        /// multiplier
        /// </summary>
        public ValueObserving<float> Madness = new ValueObserving<float>();

        public ValueObserving<float> LikesPerSecond = new ValueObserving<float>();
        public ValueObserving<float> MadnessPerSecond = new ValueObserving<float>();

        public ListObserving<ActiveItem> ActiveItems = new ListObserving<ActiveItem>();

        public ListObserving<Posting> UnreadPostings = new ListObserving<Posting>();

        public ValueObserving<float> LikeMultiplier = new ValueObserving<float>();

        public ListObserving<ShopItem> ShopItems = new ListObserving<ShopItem>();

        public ListObserving<PossiblePostingOrEvent> PossiblePostingsOrEvents = new ListObserving<PossiblePostingOrEvent>();

        public ListObserving<Event> PlannedEvents = new ListObserving<Event>();

        public ListObserving<HashtagInfo> Hashtags = new ListObserving<HashtagInfo>();

        public void Update(float deltaT)
        {
            float likeMultiplier = 1f;
            ActiveItems.get.ForEach(it => likeMultiplier += it.LikeMultiplierAddition);
            LikeMultiplier.set = likeMultiplier;

            var lps = LikesPerSecond.get;
            var mps = MadnessPerSecond.get;

            ActiveItems.get.ForEach(it => lps += it.LikesPerSecond);
            ActiveItems.get.ForEach(it => mps += it.MadnessPerSecond);

            PlannedEvents.get.ForEach(it => it.Timeout -= Time.deltaTime);

            Hashtags.get.ForEach(it => it.UsagesLeft.set = it.UsagesLeft.get + Time.deltaTime * it.UsagesPerSeconds);

            Madness.set = Madness.get + mps * Time.deltaTime;
            Likes.set = Likes.get + lps * Time.deltaTime;

            ActiveItems.get.ForEach(it => it.LifetimeLeft.set = Mathf.Max(0f, it.LifetimeLeft.get - Time.deltaTime));

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
        
        public float MadnessAdd;
        public float MadnessPerSecond;

        public List<string> Tags = new List<string>();
        public List<string> EventTags = new List<string>();

        public float LikeMultiplierAddition;

        public float ActiveItemLifetime;

        public bool IsTemporary;

        public bool IsUnlimited;

        public ValueObserving<int> RemainingBuys = new ValueObserving<int>();

        public string Hashtag;
        public int HashtagUsages;
        public float HashtagUsagesPerSeconds;
    }
    
    [System.Serializable]
    public class ActiveItem
    {
        public string Name;
        public string Text;
        public string Asset;

        public string Type;

        public float LikeMultiplierAddition;

        public float LikesPerSecond;
        public float MadnessPerSecond;

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
