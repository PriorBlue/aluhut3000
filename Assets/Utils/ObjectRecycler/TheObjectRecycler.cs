using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The singletone like global auto created easy to use recycler.
/// If in doubt use this one.
/// </summary>
public class TheObjectRecycler : MonoBehaviour 
{
	public static ObjectRecycler Instance;

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

    private static ObjectRecycler GetOrCreateGlobalInstanceOnDemand()
    {
        if (Instance == null)
        {
            var go = new GameObject("UKTheObjectRecycler");
            Instance = go.AddComponent<ObjectRecycler>();
        }

        return Instance;
    }

    // uses the prefab name as recycle group
    public static GameObject GetObject(GameObject prefab)
    {
        var self = GetOrCreateGlobalInstanceOnDemand();
        return self.GetObject(prefab);
    }

    public static GameObject GetObject(string recycleGroup, GameObject prefab)
    {
        var self = GetOrCreateGlobalInstanceOnDemand();
        return self.GetObject(recycleGroup, prefab);
    }

    public static GameObject GetObject(string recycleGroup, System.Func<GameObject> factory)
    {
        var self = GetOrCreateGlobalInstanceOnDemand();
        return self.GetObject(recycleGroup, factory);
    }

    public static GameObject GetObjectAt(string recycleGroup, Vector3 pos, Quaternion rot, System.Func<GameObject> factory)
    {
        var self = GetOrCreateGlobalInstanceOnDemand();
        return self.GetObjectAt(recycleGroup, pos, rot, factory);
    }

    public static GameObject GetObjectAt(string recycleGroup, Vector3 pos, Quaternion rot, GameObject prefab)
    {
        var self = GetOrCreateGlobalInstanceOnDemand();
        return self.GetObjectAt(recycleGroup, pos, rot, prefab);
    }
}
