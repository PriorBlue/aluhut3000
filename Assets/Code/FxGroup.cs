using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxGroup : MonoBehaviour
{
    public List<AudioSource> Sources;

	// Use this for initialization
	void Start () {
        if (Sources == null) Sources = new List<AudioSource>();

		foreach(var it in transform.GetComponentsInChildren<AudioSource>())
        {
            if (!Sources.Contains(it)) Sources.Add(it);
        }
	}
}
