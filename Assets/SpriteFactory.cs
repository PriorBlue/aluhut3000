using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpriteFactory : MonoBehaviour {
    public static SpriteFactory Instance;

    public List<Sprite> Sprites;

	void Awake () {
        Instance = this;
	}

    public Sprite Get(string name)
    {
        return Sprites.FirstOrDefault(it => it.name == name);
    }
}
