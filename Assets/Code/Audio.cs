using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class Audio : MonoBehaviour
{
    public AudioMixer Mixer;

    private List<FxGroup> fxGroups;

    void Start()
    {
        fxGroups = transform.GetComponentsInChildren<FxGroup>().ToList();
        Messenger.AddListener<string>("play.sound.fx", gameObject, PlayFx);
    }

    public void PlayFx(string name)
    {
        var fxGroup = fxGroups.FirstOrDefault(it => it.name == name);
        if (fxGroup != null && fxGroup.Sources != null)
        {
            try
            {
                var idleSource = RandomHelper.PickRandom(fxGroup.Sources.Where(it => !it.isPlaying));
                if (idleSource != null)
                {
                    idleSource.Play();
                }
            }
            catch
            {
                // no slot left -> silently ignore
            }
        }
    }
}
