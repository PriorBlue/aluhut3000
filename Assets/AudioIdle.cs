using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioIdle : MonoBehaviour {
    public float MinWait;
    public float MaxWait;
    public string FxName;

	// Use this for initialization
	IEnumerator Start () {
		while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinWait, MaxWait));
            Messenger.Broadcast<string>("play.sound.fx", FxName);
        }
	}	
}
