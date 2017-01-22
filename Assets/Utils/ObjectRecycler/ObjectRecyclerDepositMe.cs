// #define SHOW_DELETE_ERROR_MESSAGES

using UnityEngine;
using System.Collections;

public class ObjectRecyclerDepositMe : MonoBehaviour {
	public ObjectRecycler Recycler;
	public string RecyclerGroup;
    public bool RootRecyclerOnDeposit;
    public bool IsDeposited;
    
#if SHOW_DELETE_ERROR_MESSAGES
    private bool quitting = false;

    void OnApplicationQuit()
    {
        quitting = true;
    }
#endif

	public void Deposit()
	{
        // if the recycler and the objects get destroyed during scene switch in
        // the wrong order the recycler could be null
        // in this case we just ignore the deposit command because it
        // won't be possible to "restore" the object from the recycler anyway
        if (Recycler != null)
        {
            Recycler.DepositObject(RecyclerGroup, gameObject, RootRecyclerOnDeposit);
        }
	}

	public void Deposit(float timeout)
	{
		Invoke("Deposit", timeout);
	}

    void OnDepositAllChilds(bool rootRecyclerOnDeposit)
    {
        this.RootRecyclerOnDeposit = rootRecyclerOnDeposit;
        Deposit();
    }

#if SHOW_DELETE_ERROR_MESSAGES
    void OnDestroy()
    {
        if (!quitting) Debug.LogErrorFormat(gameObject, "Don't delete pooled objects. Use Deposit to put them back into the pool: {0}", gameObject.name);
    }
#endif
}
