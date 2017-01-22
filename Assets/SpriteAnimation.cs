using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public List<Sprite> Frames;
    public float FrameTime = 1f;
    public Image Image;


    IEnumerator Start()
    {
        int idx = 0;

        while (true)
        {
            Image.sprite = Frames[idx % Frames.Count];
            yield return new WaitForSeconds(FrameTime);
            ++idx;
        }
    }
}
