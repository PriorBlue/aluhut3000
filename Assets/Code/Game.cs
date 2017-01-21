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
    public TextAsset GlobalsCsv;

    public Data.Player Player;

    public float TimeScale = 1f;

    private float timeoutPostingMin = 1f;
    private float timeoutPostingMax = 5f;

    private int maxUnreadPostings = 100;

    private float eventTimeoutMin = 1f;
    private float eventTimeoutMax = 5f;

    void Awake()
    {
        Instance = this;

        Player = new Data.Player();

        Player.BaseFollowerPerSecond.set = 0f;
        Player.LikeMultiplier.set = 1f;
        Player.PostMultiplier.set = 1f;

        LoadShopItems();
        LoadPostingsOrEvents();
        LoadGlobals();        
    }

    public void Post()
    {
        var l = Player.Hashtags.get;
        var hashtags = l.Where(it => it.IsGettingUsed.get).ToList();

        hashtags.ForEach(it => it.UsagesLeft.set = it.UsagesLeft.get - 1f);
        
        Player.Hashtags.set = l;

        float follower = 0f;
        hashtags.ForEach(it => follower += it.Follower);

        Player.Follower.set = Player.Follower.get + follower * Player.PostMultiplier.get;
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
                PostMultiplierAddition = row.GetFloat("PostMultiplierAddition"),
                FollowerAdd = row.GetFloat("FollowerAdd"),
                FollowerPerSecond = row.GetFloat("FollowerPerSecond"),
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
                HashtagFollower = row.GetFloat("HashtagFollower"),
            };
            shopItem.RemainingBuys.set = row.GetInt("RemainingBuys");

            l.Add(shopItem);
        }

        Player.ShopItems.set = l;
    }


    private void LoadGlobals()
    {
        var csvReader = new CsvReader();
        csvReader.Load('\n', ';', '"', GlobalsCsv.text);
        var row = csvReader.EnumDataRows().First();

        timeoutPostingMin = row.GetFloat("timeoutPostingMin");
        timeoutPostingMax = row.GetFloat("timeoutPostingMax");
        maxUnreadPostings = row.GetInt("maxUnreadPostings");
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
                EventBlockTime = row.GetFloat("EventBlockTime"),
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

    void Start()
    {
        Messenger.AddListener<string>("toast", gameObject, (text) => Debug.Log(text, gameObject));
        Messenger.AddListener<Data.ActiveItem>("new_item", gameObject, (it) => Debug.Log(it.Text, gameObject));
        Messenger.AddListener<Data.HashtagInfo>("new_hashtag", gameObject, (it) => Debug.Log(it.Text, gameObject));

        StartCoroutine(CoPosts());
        StartCoroutine(CoEvents());
    }

    private IEnumerator CoPosts()
    {
        while (true)
        {
            if (Player.UnreadPostings.get.Count < maxUnreadPostings) CreatePosting(it => !it.IsEvent);
            yield return new WaitForSeconds(UnityEngine.Random.Range(timeoutPostingMin, timeoutPostingMax));
        }
    }

    private IEnumerator CoEvents()
    {
        while (true)
        {
            if (Player.UnreadPostings.get.Count < maxUnreadPostings) CreatePosting(it => it.IsEvent);
            yield return new WaitForSeconds(UnityEngine.Random.Range(eventTimeoutMin, eventTimeoutMax));
        }
    }

    public bool CanBuy(Data.ShopItem shopItem)
    {
        return shopItem != null && 
            Player.Follower.get >= shopItem.CostLikes;
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
            Player.Follower.set = Player.Follower.get - shopItem.CostLikes;

            // hashtags
            if (!string.IsNullOrEmpty(shopItem.Hashtag))
            {
                var hi = new Data.HashtagInfo()
                {
                    Text = shopItem.Hashtag,
                    UsagesPerSeconds = shopItem.HashtagUsagesPerSeconds,
                    Follower = shopItem.HashtagFollower,
                };
                hi.UsagesLeft.set = shopItem.HashtagUsages;
                hi.IsGettingUsed.set = true;

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
                    FollowerPerSecond = shopItem.FollowerPerSecond,
                    Type = shopItem.Type,
                    //MadnessPerSecond = shopItem.MadnessPerSecond,
                    LikeMultiplierAddition = shopItem.LikeMultiplierAddition,
                    PostMultiplierAddition = shopItem.PostMultiplierAddition,
                    IsTemporary = shopItem.IsTemporary,
                    LifetimeLeft = new U3D.KVO.ValueObserving<float>(),
                    EventTags = shopItem.EventTags,
                };
                it.LifetimeLeft.set = shopItem.ActiveItemLifetime;
                l.Add(it);

                Messenger.Broadcast<Data.ActiveItem>("new_item", it);

                Player.ActiveItems.set = l;

                Player.Follower.set = Player.Follower.get + shopItem.FollowerAdd;
                Player.BaseFollowerPerSecond.set = Player.BaseFollowerPerSecond.get + shopItem.FollowerPerSecond;
            }
        }
    }

    public void CreatePosting(System.Func<Data.PossiblePostingOrEvent, bool> filter)
    {
        var l = Player.UnreadPostings.get;

        var possible = RandomHelper.PickWeightedRandom(
            Player.PossiblePostingsOrEvents.get.Where(it => filter == null || filter(it)), 
            (it => it.RandomWeight));

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
        if (Input.GetKeyDown(KeyCode.Alpha1)) TimeScale = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha2)) TimeScale = 2f;
        if (Input.GetKeyDown(KeyCode.Alpha3)) TimeScale = 4f;
        if (Input.GetKeyDown(KeyCode.Alpha4)) TimeScale = 8f;
        if (Input.GetKeyDown(KeyCode.Alpha5)) TimeScale = 16f;
        if (Input.GetKeyDown(KeyCode.Alpha6)) TimeScale = 32f;
        if (Input.GetKeyDown(KeyCode.Alpha7)) TimeScale = 64f;
        if (Input.GetKeyDown(KeyCode.Alpha8)) TimeScale = 100f;
        if (Input.GetKeyDown(KeyCode.Alpha9)) TimeScale = 100f;
        if (Input.GetKeyDown(KeyCode.Alpha0)) TimeScale = 0f;
        Time.timeScale = TimeScale;

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
        var l = player.MultiplierBlockRemaingTimes.get;
        l.Add(ev.BlockTime);
        player.MultiplierBlockRemaingTimes.set = l;
    }

    public void Like(Data.Posting posting)
    {
        Player.Follower.set = Player.Follower.get + posting.LikeValue * Player.LikeMultiplier.get;
        var l = Player.UnreadPostings.get;
        l.Remove(posting);
        Player.UnreadPostings.set = l;
    }
}