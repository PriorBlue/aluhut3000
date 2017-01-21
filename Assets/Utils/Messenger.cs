// uncomment this for logging
//#define LOG_ADD_LISTENER
//#define LOG_BROADCAST_MESSAGE
//#define LOG_BROADCAST_WITHOUT_RECIPIENT
//#define PROFILE_BROADCAST_MESSAGE

using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Global message bus system. Each listener is linked to a GameObject and gets removed if the gameobject dies.
 **/
public static class Messenger {

	struct ListenerLifecyclePair {
		public Delegate Listener;
		public GameObject LifecycleObject;
	}

	class EventListeners {
		public List<ListenerLifecyclePair> Listeners = new List<ListenerLifecyclePair>();
	}

	private static Dictionary<string, EventListeners> eventTable = new Dictionary<string, EventListeners>();

	// sends a message
	public static void Broadcast(string messageName) {
#if PROFILE_BROADCAST_MESSAGE
        Profiler.BeginSample("UKMessager_Broadcast0_" + messageName);
#endif
#if LOG_BROADCAST_MESSAGE
		Debug.Log(string.Format("MESSENGER send message {0} ()", messageName));
#endif

        int callsDelivered = 0;

		EventListeners l;
		if (eventTable.TryGetValue(messageName, out l)) {
			int countDead = 0;

			var listeners = l.Listeners;

			for (int i = 0; i < listeners.Count; ++i) {
				if (listeners[i].LifecycleObject != null) {
					// listener still alive -> try to call
					Callback callback = listeners[i].Listener as Callback;
					
					if (callback != null) {
						callback();
                        ++callsDelivered;
					}
				} else {
					++countDead;
				}
			}

			if (countDead > 0) {
				PurgeDeadListeners(l);
			}
        }

#if PROFILE_BROADCAST_MESSAGE
        Profiler.EndSample();
#endif

#if LOG_BROADCAST_WITHOUT_RECIPIENT
        if (callsDelivered == 0) Debug.LogWarning(string.Format("MESSENGER send message {0} () without recipient", messageName));
#endif
    }

    // sends a message
    public static void Broadcast<T0>(string messageName, T0 p0) {
#if PROFILE_BROADCAST_MESSAGE
        Profiler.BeginSample("UKMessager_Broadcast1_" + messageName);
#endif
#if LOG_BROADCAST_MESSAGE
        Debug.Log(string.Format("MESSENGER send message {0} ({1})", messageName, p0));
#endif

        int callsDelivered = 0;

		EventListeners l;
		if (eventTable.TryGetValue(messageName, out l)) {
			int countDead = 0;
			
			var listeners = l.Listeners;

			for (int i = 0; i < listeners.Count; ++i) {
				if (listeners[i].LifecycleObject != null) {
					// listener still alive -> try to call
					Callback<T0> callback = listeners[i].Listener as Callback<T0>;

					if (callback != null) {
						callback(p0);
                        ++callsDelivered;
					}
				} else {
					++countDead;
				}
			}
			
			if (countDead > 0) {
				PurgeDeadListeners(l);
			}
		}

#if PROFILE_BROADCAST_MESSAGE
        Profiler.EndSample();
#endif

#if LOG_BROADCAST_WITHOUT_RECIPIENT
        if (callsDelivered == 0) Debug.LogWarning(string.Format("MESSENGER send message {0} () without recipient", messageName));
#endif
    }

    // sends a message
    public static void Broadcast<T0, T1>(string messageName, T0 p0, T1 p1) {
#if PROFILE_BROADCAST_MESSAGE
        Profiler.BeginSample("UKMessager_Broadcast2_" + messageName);
#endif
#if LOG_BROADCAST_MESSAGE
		Debug.Log(string.Format("MESSENGER send message {0} ({1}, {2})", messageName, p0, p1));
#endif

        int callsDelivered = 0;

		EventListeners l;
		if (eventTable.TryGetValue(messageName, out l)) {
			int countDead = 0;
			
			var listeners = l.Listeners;
     
			for (int i = 0; i < listeners.Count; ++i) {
				if (listeners[i].LifecycleObject != null) {
					// listener still alive -> try to call
					Callback<T0, T1> callback = listeners[i].Listener as Callback<T0, T1>;
					
					if (callback != null) {
						callback(p0, p1);
                        ++callsDelivered;
					}
				} else {
					++countDead;
				}
			}
			
			if (countDead > 0) {
				PurgeDeadListeners(l);
			}
        }

#if PROFILE_BROADCAST_MESSAGE
        Profiler.EndSample();
#endif

#if LOG_BROADCAST_WITHOUT_RECIPIENT
        if (callsDelivered == 0) Debug.LogWarning(string.Format("MESSENGER send message {0} () without recipient", messageName));
#endif
    }

    // sends a message
    public static void Broadcast<T0, T1, T2>(string messageName, T0 p0, T1 p1, T2 p2) {
#if PROFILE_BROADCAST_MESSAGE
        Profiler.BeginSample("UKMessager_Broadcast3_" + messageName);
#endif
#if LOG_BROADCAST_MESSAGE
		Debug.Log(string.Format("MESSENGER send message {0} ({1}, {2}, {3})", messageName, p0, p1, p2));
#endif

        int callsDelivered = 0;

		EventListeners l;
		if (eventTable.TryGetValue(messageName, out l)) {
			int countDead = 0;
			
			var listeners = l.Listeners; 
            
            for (int i = 0; i < listeners.Count; ++i) {
				if (listeners[i].LifecycleObject != null) {
					// listener still alive -> try to call					
					Callback<T0, T1, T2> callback = listeners[i].Listener as Callback<T0, T1, T2>;
					
					if (callback != null) {
						callback(p0, p1, p2);
                        ++callsDelivered;
					}
				} else {
					++countDead;
				}
			}
			
			if (countDead > 0) {
				PurgeDeadListeners(l);
			}
        }

#if PROFILE_BROADCAST_MESSAGE
        Profiler.EndSample();
#endif

#if LOG_BROADCAST_WITHOUT_RECIPIENT
        if (callsDelivered == 0) Debug.LogWarning(string.Format("MESSENGER send message {0} () without recipient", messageName));
#endif
    }


    private static void PurgeDeadListeners(EventListeners l) {
		var listeners = l.Listeners;
		for (int i = 0; i < listeners.Count; ++i) {
			if (listeners[i].LifecycleObject == null) {
				// dead listener found -> remove
				listeners.RemoveAt(i);
				--i;
			}
		}
	}

	private static void AddListenerInternal(string messageName, GameObject lifecycleObject, Delegate handler) {
#if LOG_ADD_LISTENER
		Debug.Log(string.Format("MESSENGER add listener {0} at {1}", handler, messageName), lifecycleObject);
#endif

		// don't add dead listeners
		if (lifecycleObject == null) return;

		// prepare
		if (!eventTable.ContainsKey(messageName)) {
			eventTable[messageName] = new EventListeners();
		}
		
		// add
		eventTable[messageName].Listeners.Add(new ListenerLifecyclePair(){
			Listener = handler,
			LifecycleObject = lifecycleObject,
		});
	}

	// adds an listener linked to the lifetime of a gameobject
	public static void AddListener(string messageName, GameObject lifecycleObject, Callback handler) {
		AddListenerInternal(messageName, lifecycleObject, handler);
	}

	public static void AddListener<T0>(string messageName, GameObject lifecycleObject, Callback<T0> handler) {
		AddListenerInternal(messageName, lifecycleObject, handler);
	}

	public static void AddListener<T0, T1>(string messageName, GameObject lifecycleObject, Callback<T0, T1> handler) {
		AddListenerInternal(messageName, lifecycleObject, handler);
	}
	
	public static void AddListener<T0, T1, T2>(string messageName, GameObject lifecycleObject, Callback<T0, T1, T2> handler) {
		AddListenerInternal(messageName, lifecycleObject, handler);
	}

    public static void Link(string srcMessage, GameObject lifecycleObject, string dstMessage)
    {
        AddListener(srcMessage, lifecycleObject, () => Broadcast(dstMessage));
    }

    public static void Link<T0>(string srcMessage, GameObject lifecycleObject, string dstMessage)
    {
        AddListener<T0>(srcMessage, lifecycleObject, (t0) => Broadcast<T0>(dstMessage, t0));
    }

    public static void Link<T0, T1>(string srcMessage, GameObject lifecycleObject, string dstMessage)
    {
        AddListener<T0, T1>(srcMessage, lifecycleObject, (t0, t1) => Broadcast<T0, T1>(dstMessage, t0, t1));
    }

    public static void Link<T0, T1, T2>(string srcMessage, GameObject lifecycleObject, string dstMessage)
    {
        AddListener<T0, T1, T2>(srcMessage, lifecycleObject, (t0, t1, t2) => Broadcast<T0, T1, T2>(dstMessage, t0, t1, t2));
    }
    
    // removes all listener
	public static void Cleanup() {
		eventTable.Clear();
	}
}
