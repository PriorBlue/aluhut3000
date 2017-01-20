using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardRow : MonoBehaviour 
{
	public Text positionText;
	public Text nameText;
	public Text scoreText;
	public Image background;
	public Color evenColor, oddColor;
	
	public void Set(int position, string name, int score)
	{
		positionText.text= position.ToString();
		nameText.text= name;
		scoreText.text= score.ToString();
		
		background.color = position % 2 == 0 ? evenColor : oddColor;
	}
}
