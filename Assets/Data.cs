using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    // [System.Serializable]
    public class Player
    {
        /// <summary>
        /// money
        /// </summary>
        public U3D.KVO.ValueObserving<float> Likes = new U3D.KVO.ValueObserving<float>();
        /// <summary>
        /// multiplier
        /// </summary>
        public U3D.KVO.ValueObserving<float> Madness = new U3D.KVO.ValueObserving<float>();

        public U3D.KVO.ValueObserving<float> LikesPerSecond = new U3D.KVO.ValueObserving<float>();
        public U3D.KVO.ValueObserving<float> MadnessPerSecond = new U3D.KVO.ValueObserving<float>();

        public U3D.KVO.ListObserving<ActiveItem> ActiveItems = new U3D.KVO.ListObserving<ActiveItem>();

        public U3D.KVO.ListObserving<Posting> UnreadPostings = new U3D.KVO.ListObserving<Posting>();

        public U3D.KVO.ValueObserving<float> LikeMultiplier = new U3D.KVO.ValueObserving<float>();
        
        public void Update(float deltaT)
        {
            float likeMultiplier = 1f;
            ActiveItems.get.ForEach(it => likeMultiplier += it.LikeMultiplierAddition);
            ActiveItems.get.ForEach(it => it.MadnessPerSecond.Add(deltaT, ref Madness));
            ActiveItems.get.ForEach(it => it.LikesPerSecond.Add(deltaT, ref Likes));

            Madness.set = Madness.get + MadnessPerSecond.get * Time.deltaTime;
            Likes.set = Likes.get + LikesPerSecond.get * Time.deltaTime;
        }
    }

    // [System.Serializable]
    public class ShopItem
    {
        public string Name;
        public string Text;
        public string Asset;

        public string Type;

        public float CostLikes;

        public float LikesAdd;
        public ValuePerSecond LikesPerSecond = new ValuePerSecond();

        public float MadnessAdd;
        public ValuePerSecond MadnessPerSecond = new ValuePerSecond();
    }

    // [System.Serializable]
    public class ValuePerSecond
    {
        public float PerSecond;
        public float DurationLeft;

        public void Add(float deltaT, ref U3D.KVO.ValueObserving<float> v)
        {
            var f = v.get;
            Add(deltaT, ref f);
            v.set = f;
        }

        public void Add(float deltaT, ref float v)
        {
            var dt = Mathf.Max(deltaT, DurationLeft);
            DurationLeft -= dt;
            v += PerSecond * dt;
        }
    }

    // [System.Serializable]
    public class ActiveItem
    {
        public string Name;
        public string Text;
        public string Asset;

        public string Type;

        public float LikeMultiplierAddition;

        public ValuePerSecond LikesPerSecond = new ValuePerSecond();
        public ValuePerSecond MadnessPerSecond = new ValuePerSecond();

        public List<string> Tags = new List<string>();
    }

    // [System.Serializable]
    public class HashtahInfo
    {
        public string Text;
        public float UsagesLeft;
        public float UsagesPerSeconds;
    }

    // [System.Serializable]
    public class Posting
    {
        public List<string> Hashtags = new List<string>();
        public float LikeValue;
    }

    public class Event
    {
        public string Text;
        public float Timeout;
        public List<string> Tags = new List<string>();
    }
}
