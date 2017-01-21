using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonActicateChild : MonoBehaviour
{
    public GameObject Child;

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(Click);
	}

    private void Click()
    {
        var parent = Child.transform.parent;
        for (int i = 0; i < parent.childCount; ++i)
        {
            var child = parent.GetChild(i).gameObject;
            child.SetActive(child == Child);
        }
    }
}
