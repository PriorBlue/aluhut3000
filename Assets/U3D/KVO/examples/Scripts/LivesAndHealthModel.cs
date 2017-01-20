using UnityEngine;

public class LivesAndHealthModel : MonoBehaviour 
{
	public int maxLives = 4;
	[RangeAttribute(1, 10)]
	public float loseLifeEverySeconds = 5;
	
	public U3D.KVO.ReadOnlyValueObserving<int> lives { get { return m_lives; } } 
	U3D.KVO.ValueObserving<int> m_lives = new U3D.KVO.ValueObserving<int>();  
	public U3D.KVO.ReadOnlyValueObserving<float> health { get { return m_health; } } 
	U3D.KVO.ValueObserving<float> m_health = new U3D.KVO.ValueObserving<float>();  
	
	void Start () 
	{
		m_lives.set = maxLives;
		m_health.set = 1;
	}
	
	float m_thisLifeDuration = 0;
	void Update () 
	{
		m_thisLifeDuration+= Time.deltaTime;
		float health= (loseLifeEverySeconds - m_thisLifeDuration) / loseLifeEverySeconds;
		
		if(health< 0)
		{
			int lives= m_lives.get -1;
			if(lives< 0)
			{
				lives= maxLives;
			}
			m_lives.set= lives;
			m_health.set= 1;
			m_thisLifeDuration= 0;
		}
		else
		{
			m_health.set= health;
		}
	}
}
