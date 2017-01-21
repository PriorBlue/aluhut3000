using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using U3D.KVO;

public class Game : MonoBehaviour
{
    public static Game Instance = null;
    public TextAsset ShopItemsCsv;
    public TextAsset PostingsOrEventsCsv;

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

        LoadShopItems();
        LoadPostingsOrEvents();

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

    public void Post()
    {
        
    }

    private void LoadShopItems()
    {
        var csvReader = new CsvReader();
        csvReader.Load('\n', ';', '"', ShopItemsCsv.text);

        var l = new List<Data.ShopItem>();

        foreach (var row in csvReader.EnumDataRows())
        {
            var shopItem = new Data.ShopItem()
            {
                Asset = row.GetString("Asset"),
                ActiveItemLifetime = row.GetFloat("ActiveItemLifetime"),
                CostLikes = row.GetFloat("CostLikes"),
                IsTemporary = row.GetBool("IsTemporary"),
                LikeMultiplierAddition = row.GetFloat("LikeMultiplierAddition"),
                LikesAdd = row.GetFloat("LikesAdd"),
                LikesPerSecond = row.GetFloat("LikesPerSecond"),
                MadnessAdd = row.GetFloat("MadnessAdd"),
                MadnessPerSecond = row.GetFloat("MadnessPerSecond"),
                Name = row.GetString("Name"),
                Tags = ParseTags(row.GetString("Tags")),
                Text = row.GetString("Text"),
                Type = row.GetString("Type"),
                IsUnlimited = row.GetBool("IsUnlimited"),
                RemainingBuys = new ValueObserving<int>(),
                EventTags = ParseTags(row.GetString("EventTags")),
                Hashtag = row.GetString("Hashtag"),
                HashtagUsages = row.GetInt("HashtagUsages"),
                HashtagUsagesPerSeconds = row.GetFloat("HashtagUsagesPerSeconds"),
            };
            shopItem.RemainingBuys.set = row.GetInt("RemainingBuys");

            l.Add(shopItem);
        }

        Player.ShopItems.set = l;
    }

    private void LoadPostingsOrEvents()
    {
        var csvReader = new CsvReader();
        csvReader.Load('\n', ';', '"', PostingsOrEventsCsv.text);

        var l = new List<Data.PossiblePostingOrEvent>();

        foreach (var row in csvReader.EnumDataRows())
        {
            var item = new Data.PossiblePostingOrEvent()
            {
                Text = row.GetString("Text"),
                EventAsset = row.GetString("EventAsset"),
                EventTags = ParseTags(row.GetString("EventTags")),
                EventText = row.GetString("EventText"),
                EventTimeout = row.GetFloat("EventTimeout"),
                Hashtags = ParseTags(row.GetString("Hashtags")),
                IsEvent = row.GetBool("IsEvent"),
                LikeValue = row.GetFloat("LikeValue"),
                RandomWeight = row.GetInt("RandomWeight"),
            };

            l.Add(item);
        }

        Player.PossiblePostingsOrEvents.set = l;
    }

    public bool AnyMatchingTag(List<string> tagsA, List<string> tagsB)
    {
        if (tagsA != null && tagsB != null)
        {
            foreach(var it in tagsA)
            {
                if (tagsB.Contains(it)) return true;
            }
        }

        return false;
    }

    private List<string> ParseTags(string tags)
    {
        return tags.Split(new char[] { ',', ' ' }).Select(it => it.Trim()).ToList();
    }

    IEnumerator Start()
    {
        Messenger.AddListener<string>("toast", gameObject, (text) => Debug.Log(text, gameObject));
        Messenger.AddListener<Data.ActiveItem>("new_item", gameObject, (it) => Debug.Log(it.Text, gameObject));
        Messenger.AddListener<Data.HashtagInfo>("new_hashtag", gameObject, (it) => Debug.Log(it.Text, gameObject));

        while (true) {
            if (Player.UnreadPostings.get.Count < MaxUnreadPostings) CreatePosting();
            yield return new WaitForSeconds(TimeoutPosting);
        }
    }

    public bool CanBuy(Data.ShopItem shopItem)
    {
        return shopItem != null && 
            Player.Likes.get >= shopItem.CostLikes;
    }

    public void Buy(Data.ShopItem shopItem)
    {
        if (CanBuy(shopItem))
        {
            // remove from shop
            if (!shopItem.IsUnlimited)
            {
                // consume
                shopItem.RemainingBuys.set = shopItem.RemainingBuys.get - 1;
                if (shopItem.RemainingBuys.get <= 0)
                {
                    var l = Player.ShopItems.get;
                    l.Remove(shopItem);
                    Player.ShopItems.set = l;
                }
            }

            // pay
            Player.Likes.set = Player.Likes.get - shopItem.CostLikes;

            // hashtags
            if (!string.IsNullOrEmpty(shopItem.Hashtag))
            {
                var hi = new Data.HashtagInfo()
                {
                    Text = shopItem.Hashtag,
                    UsagesPerSeconds = shopItem.HashtagUsagesPerSeconds,
                };
                hi.UsagesLeft.set = shopItem.HashtagUsages;

                var l = Player.Hashtags.get;
                var existing = l.FirstOrDefault(it => it.Text == hi.Text);
                if (existing == null)
                {
                    // add hashtag info
                    l.Add(hi);
                }
                else
                {
                    // merge hashtag info
                    existing.UsagesLeft.set = existing.UsagesLeft.get + hi.UsagesLeft.get;
                    existing.UsagesPerSeconds = (existing.UsagesPerSeconds + hi.UsagesPerSeconds) / 2f;
                }
                Player.Hashtags.set = l;

                Messenger.Broadcast<Data.HashtagInfo>("new_hashtag", hi);
            }

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
                    EventTags = shopItem.EventTags,
                };
                it.LifetimeLeft.set = shopItem.ActiveItemLifetime;
                l.Add(it);

                Messenger.Broadcast<Data.ActiveItem>("new_item", it);

                Player.ActiveItems.set = l;

                Player.Likes.set = Player.Likes.get + shopItem.LikesAdd;
                Player.Madness.set = Player.Madness.get + shopItem.MadnessAdd;
            }
        }
    }

    public void CreatePosting()
    {
        var l = Player.UnreadPostings.get;

        var possible = RandomHelper.PickWeightedRandom(Player.PossiblePostingsOrEvents.get, (it => it.RandomWeight));

        l.Add(possible.CreatePosting());
        if (possible.IsEvent)
        {
            var el = Player.PlannedEvents.get;
            el.Add(possible.CreateEvent());
            Player.PlannedEvents.set = el;
        }
        
        Player.UnreadPostings.set = l;
    }

    void Update()
    {
        Player.Update(Time.deltaTime);

        var events = Player.PlannedEvents.get;
        foreach(var ev in events.ToArray())
        {
            if (ev.Timeout <= 0f)
            {
                var preventedBy = Player.ActiveItems.get.FirstOrDefault(it => AnyMatchingTag(it.EventTags, ev.Tags));
                if (preventedBy != null)
                {
                    PreventEvent(Player, ev, preventedBy);
                }
                else
                {
                    ExecuteEvent(Player, ev);
                }

                events.Remove(ev);
            }
        }

        Player.PlannedEvents.set = events;
    }

    private void PreventEvent(Data.Player player, Data.Event ev, Data.ActiveItem preventedBy)
    {
        Messenger.Broadcast<string>("toast", "PREVENTED: " + ev.Text);
    }

    private void ExecuteEvent(Data.Player player, Data.Event ev)
    {
        Messenger.Broadcast<string>("toast", ev.Text);
        player.LikeMultiplier.set = 1f;
    }

    public void Like(Data.Posting posting)
    {
        Player.Likes.set = Player.Likes.get + posting.LikeValue * Player.LikeMultiplier.get;
        var l = Player.UnreadPostings.get;
        l.Remove(posting);
        Player.UnreadPostings.set = l;
    }
}