using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour
{
    public static Game Instance = null;
    public TextAsset ShopItemsCsv;

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

        var csvReader = new CsvReader();
        csvReader.Load('\n', ';', '"', ShopItemsCsv.text);

        {
            var l = new List<Data.ShopItem>();

            foreach (var row in csvReader.EnumDataRows())
            {
                l.Add(new Data.ShopItem()
                {
                    Asset = row.GetString("Asset"),
                    ActiveItemLifetime = row.GetFloat("ActiveItemLifetime"),
                    CostLikes = row.GetFloat("CostLikes"),
                    IsTemporary= row.GetBool("IsTemporary"),
                    LikeMultiplierAddition = row.GetFloat("LikeMultiplierAddition"),
                    LikesAdd = row.GetFloat("LikesAdd"),
                    LikesPerSecond = row.GetFloat("LikesPerSecond"),
                    MadnessAdd = row.GetFloat("MadnessAdd"),
                    MadnessPerSecond = row.GetFloat("MadnessPerSecond"),
                    Name = row.GetString("Name"),
                    Tags = row.GetString("Tags").Split(new char[] { ',', ' '}).Select(it => it.Trim()).ToList(),
                    Text = row.GetString("Text"),
                    Type = row.GetString("Type"),
                });
            }

            Player.ShopItems.set = l;
        }
        /*
        Player.ShopItems.set =
        {
            new Data.ShopItem()
            {
                Asset = "crap",
                CostLikes = 10,
                LikesAdd = 0,
                LikesPerSecond = 0.1f,
                MadnessPerSecond = 0.2f,
                IsTemporary = true,
                ActiveItemLifetime = 10f,
                Name = "madness",
                Type = "blub",
                LikeMultiplierAddition = 0.1f,
                MadnessAdd = 1f,
                Tags = new List<string>() {"maddddness" },
                Text = "lorem tet lala",                                
            },
            new Data.ShopItem()
            {
                Asset = "element_reach_001",
                CostLikes = 20,
                LikesAdd = 0,
                LikesPerSecond = 0.1f,
                MadnessPerSecond = 0.2f,
                IsTemporary = true,
                ActiveItemLifetime = 10f,
                Name = "flat earth",
                Type = "blub",
                LikeMultiplierAddition = 0.1f,
                MadnessAdd = 10f,
                Tags = new List<string>() {"maddddness" },
                Text = "lorem tet lala",
            },
            new Data.ShopItem()
            {
                Asset = "element_reach_001",
                CostLikes = 50,
                LikesAdd = 0,
                LikesPerSecond = 10f,
                MadnessPerSecond = 1f,
                IsTemporary = false,
                ActiveItemLifetime = 0f,
                Name = "flat earth",
                Type = "blub",
                LikeMultiplierAddition = 0.1f,
                MadnessAdd = 10f,
                Tags = new List<string>() {"maddddness" },
                Text = "lorem tet lala",
            },
        };
        */
    }

    IEnumerator Start()
    {
        while (true) {
            if (Player.UnreadPostings.get.Count < MaxUnreadPostings) CreatePosting();
            yield return new WaitForSeconds(TimeoutPosting);
        }
    }

    public void Buy(Data.ShopItem shopItem)
    {
        if (Player.Likes.get >= shopItem.CostLikes)
        {
            // remove from shop
            {
                var l = Player.ShopItems.get;
                l.Remove(shopItem);
                Player.ShopItems.set = l;
            }

            // pay
            Player.Likes.set = Player.Likes.get - shopItem.CostLikes;

            // add active items
            {
                var l = Player.ActiveItems.get;
                var it = new Data.ActiveItem()
                {
                    Asset = shopItem.Asset,
                    Name = shopItem.Name,
                    Text = shopItem.Text,
                    Tags = shopItem.Tags,
                    LikesPerSecond = shopItem.LikesPerSecond,
                    Type = shopItem.Type,
                    MadnessPerSecond = shopItem.MadnessPerSecond,
                    LikeMultiplierAddition = shopItem.LikeMultiplierAddition,
                    IsTemporary = shopItem.IsTemporary,
                    LifetimeLeft = new U3D.KVO.ValueObserving<float>(),
                };
                it.LifetimeLeft.set = shopItem.ActiveItemLifetime;
                l.Add(it);

                Player.ActiveItems.set = l;

                Player.Likes.set = Player.Likes.get + shopItem.LikesAdd;
                Player.Madness.set = Player.Madness.get + shopItem.MadnessAdd;
            }
        }
    }

    public void CreatePosting()
    {
        var l = Player.UnreadPostings.get;
        l.Add(new Data.Posting()
        {
            LikeValue = UnityEngine.Random.Range(PostingLikeValueMin, PostingLikeValueMax),
            Hashtags = new List<string>() { "lala" },
            Text = "lorem ipsum",
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