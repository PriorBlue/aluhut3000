using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LivesAndHealthController : MonoBehaviour 
{
	public RectTransform livesPrefab;
	public RectTransform livesContainer;
	public RectTransform healthBar;
	public float healthBarMaxWidth;
	
	public LivesAndHealthModel model;
	
	void Start () 
	{
		model.lives.RegisterObserverScoped(gameObject, UpdateLives);
        model.health.RegisterObserverScoped(gameObject, (health) => healthBar.sizeDelta = new Vector2(health * healthBarMaxWidth, healthBar.sizeDelta.y));
	}
	void UpdateLives(int lives)
	{
        Debug.LogFormat("UPDATE {0}", lives);

		while(livesContainer.childCount < lives)
		{
			RectTransform live= Instantiate(livesPrefab) as RectTransform;
			live.SetParent(livesContainer);
		}
		while(livesContainer.childCount > lives)
		{
			Transform live = livesContainer.GetChild(0);
			DestroyImmediate(live.gameObject);
		}
	}
}
