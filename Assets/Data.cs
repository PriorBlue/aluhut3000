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

        public void Update(float deltaT)
        {
            float likeMultiplier = 1f;
            ActiveItems.get.ForEach(it => likeMultiplier += it.LikeMultiplierAddition);
            LikeMultiplier.set = likeMultiplier;

            var lps = LikesPerSecond.get;
            var mps = MadnessPerSecond.get;

            ActiveItems.get.ForEach(it => lps += it.LikesPerSecond);
            ActiveItems.get.ForEach(it => mps += it.MadnessPerSecond);

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

        public float LikeMultiplierAddition;

        public float ActiveItemLifetime;

        public bool IsTemporary;

        public bool IsUnlimited;

        public ValueObserving<int> RemainingBuys = new ValueObserving<int>();
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

        public ValueObserving<float> LifetimeLeft;

        public bool IsTemporary;
    }

    [System.Serializable]
    public class HashtahInfo
    {
        public string Text;
        public float UsagesLeft;
        public float UsagesPerSeconds;
    }

    [System.Serializable]
    public class Posting
    {
        public List<string> Hashtags = new List<string>();
        public float LikeValue;
        public string Text;
    }

    public class Event
    {
        public string Text;
        public float Timeout;
        public List<string> Tags = new List<string>();
    }
}
