using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AudioButtonClick : MonoBehaviour {
    public string FxName = "click";

    // Use this for initialization
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Messenger.Broadcast<string>("play.sound.fx", FxName);
        });
    }
}
