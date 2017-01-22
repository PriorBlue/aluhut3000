using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectRecycler : MonoBehaviour {
	public static ObjectRecycler Instance;
    
    /// <summary>
    /// If this is true this instance does not touch the global recycler singleton instance
    /// </summary>
    public bool IsNotSingleton;

    public static void DespositAllChilds(GameObject o, bool rootRecyclerOnDeposit)
    {
        o.BroadcastMessage("OnDepositAllChilds", rootRecyclerOnDeposit, SendMessageOptions.DontRequireReceiver);
    }

    public static void DepositAllChilds(GameObject o, bool rootRecyclerOnDeposit = false)
    {
        var t = o.transform;
        
        if (rootRecyclerOnDeposit)
        {

            while (t.childCount > 0)
            {
                DepositObject(t.GetChild(0).gameObject, true);
            }
        }
        else
        {
            for(int i = 0; i < t.childCount; ++i)
            {
                DepositObject(t.GetChild(i).gameObject, true);
            }
        }
    }

    public static void DepositObject(GameObject o, bool rootRecyclerOnDeposit = false) 
	{
        var c = o.GetComponent<ObjectRecyclerDepositMe>();
        c.RootRecyclerOnDeposit = rootRecyclerOnDeposit;
		c.Deposit();
	}

    public static void DepositObject(GameObject o, float timeout, bool rootRecyclerOnDeposit = false) 
	{
        var c = o.GetComponent<ObjectRecyclerDepositMe>();
        c.RootRecyclerOnDeposit = rootRecyclerOnDeposit;
        c.Deposit(timeout); 
	}

	public Dictionary<string, Queue<GameObject>> _cachedObjects = new Dictionary<string, Queue<GameObject>>();
	
	void Awake()
	{
        if (IsNotSingleton) return;
		Instance = this;
	}
	
	private Queue<GameObject> GetQueueByGroup(string recycleGroup)
	{
		if (!_cachedObjects.ContainsKey (recycleGroup)) {
			_cachedObjects [recycleGroup] = new Queue<GameObject> ();
		}
		
		return _cachedObjects[recycleGroup];
	}
	
	private GameObject PopObjectFromQueueByGroup(string recycleGroup)
	{
		var queue = GetQueueByGroup(recycleGroup);
		
		while(queue.Count > 0)
		{
			GameObject o = queue.Dequeue();
            if (o != null)
            {
                var dm = o.GetComponent<ObjectRecyclerDepositMe>();
                dm.IsDeposited = false;

                return o;
            }
		}
		
		return null;
	}

	// uses the prefab name as recycle group
	public GameObject GetObject(GameObject prefab)
	{
		return GetObject(prefab.name, prefab);
	}

	public GameObject GetObject(string recycleGroup, GameObject prefab)
	{
		GameObject o = PopObjectFromQueueByGroup(recycleGroup);
		if (o != null)
		{
			// recycle
			o.SetActive(true);
			o.SendMessage("OnRestart", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			// create new
			o = (GameObject)GameObject.Instantiate(prefab);
			if (o.GetComponent<ObjectRecyclerDepositMe>() == null) {
				var dm = o.AddComponent<ObjectRecyclerDepositMe>();
                dm.IsDeposited = false;
				dm.Recycler = this;
				dm.RecyclerGroup = recycleGroup;
			}
		}

		return o;
	}
	
	public GameObject GetObject(string recycleGroup, System.Func<GameObject> factory)
	{
		GameObject o = PopObjectFromQueueByGroup(recycleGroup);
		if (o != null)
		{
			// recycle
			o.SetActive(true);
			o.SendMessage("OnRestart", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			// create new
			o = factory();			
			if (o.GetComponent<ObjectRecyclerDepositMe>() == null) {
				var dm = o.AddComponent<ObjectRecyclerDepositMe>();
				dm.Recycler = this;
				dm.RecyclerGroup = recycleGroup;
			}
		}

		return o;
	}
	
	public GameObject GetObjectAt(string recycleGroup, Vector3 pos, Quaternion rot, System.Func<GameObject> factory)
	{
		GameObject o = GetObject(recycleGroup, factory);
		o.transform.position = pos;
		o.transform.rotation = rot;
		return o;
	}

	public GameObject GetObjectAt(string recycleGroup, Vector3 pos, Quaternion rot, GameObject prefab)
	{
		return GetObjectAt(recycleGroup, pos, rot, () => {
			return (GameObject)GameObject.Instantiate(prefab);
		});
	}
	
	public void DepositObject(string recycleGroup, GameObject o, bool rootRecycler)
	{
        var me = o.GetComponent<ObjectRecyclerDepositMe>();
        if (me.IsDeposited) return;
        me.IsDeposited = true;

        o.SendMessage("OnDeposit", SendMessageOptions.DontRequireReceiver);
		o.SetActive(false);
        
        var group = GetQueueByGroup(recycleGroup);
#if UNITY_EDITOR
        // already deposited?
        foreach (var it in group)
        {
            if (it == o) 
            {
                Debug.LogError("double deposit");
            }
        }
#endif
        group.Enqueue(o);
        if (rootRecycler) o.transform.SetParent(transform);
	}
	
	public IEnumerable<GameObject> EnumAllByGroup(string recycleGroup)
	{
		foreach(var o in GetQueueByGroup(recycleGroup))
		{
			yield return o;
		}
	}
	
	public IEnumerable<GameObject> EnumAll()
	{
		foreach(var t in _cachedObjects.Keys)
		foreach(var o in GetQueueByGroup(t))
		{
			yield return o;
		}
	}
}
