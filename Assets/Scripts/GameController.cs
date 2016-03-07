using UnityEngine;
using System.Collections;
using UnityEditor;

public class GameController : Singleton<GameController>
{
	public enum ClusterType { SHOTGUN, PULSE };

	#region Public member variables.
	public float ShotgunChancePercentage = 0.5f;

	public float ShotgunSpread = 3.0f;
	public float PulseSpread = 110.0f; //In degrees.

	public int EnemyBulletPoolSize = 100;
	public float MinEnemySpawmRate = 0.5f; //In seconds.
	public float MaxEnemySpawnRate = 1.0f; //In seconds.
	public float MinEnemySpeed = 2.0f;
	public float MaxEnemySpeed = 3.0f;

	public string PreviousScoreText = "Score last game: ";
	public string HighScoreText = "Highest Score: ";

	public GameObject[] EnemyPrefabs;

	public GameObject EnemyShot;
	public Rect SpawnArea;

	//TODO: Dear lord the GO references!? This stinks of too much being in the controller; these need pulling out.
	public GameObject PlayerRef;
	public GameObject HowToPlayRef;
	public GameObject LogoRef;
	public GameObject ScoreRef;
	public GameObject PrevScoreRef;
	public GameObject HighScoreRef;
	public GameObject ShieldMeterRef;

	public Vector3 shieldMeterFullSize;
	public Vector3 shieldMeterDefaultPosition;
	#endregion

	#region Private member variables.
	private int m_Score;
	private int m_HighScore;

	private Transform m_PlayerTransform;
	private Player m_PlayerScript;

	private float m_NextSpawnTime;

	private static GameObject[] m_EnemyBulletPool;
	#endregion

	public Player Player
	{
		get
		{
			return m_PlayerScript;
		}
	}

	/// <summary>
	/// Initialisation function used by Unity.
	/// </summary>
	private void Start()
	{
#if UNITY_ANDROID
		//TODO: Remove this GetComponent.
		HowToPlayRef.GetComponent<GUIText>().text = "Touch (and hold) screen to start.";
#endif

		m_EnemyBulletPool = new GameObject[EnemyBulletPoolSize];
		m_NextSpawnTime = Time.time;

		m_PlayerTransform = PlayerRef.transform;
		m_PlayerScript = PlayerRef.GetComponent<Player>();

		//Start the player dead and the logo visible.
		PlayerRef.SetActive(false);
		PlayerRef.GetComponent<Player>().HitRegisteredEventHandler += ScoreHit;

		shieldMeterFullSize = ShieldMeterRef.transform.localScale;
		shieldMeterDefaultPosition = ShieldMeterRef.transform.position;

		m_HighScore = -1;
		m_Score = -1;
	}

	/// <summary>
	/// Reset all elements in the game. Genarally used when we want to begin a new game.
	/// </summary>
	private void ResetGame()
	{
		m_Score = 0;
		PlayerRef.GetComponent<Player>().Reset();

		//No need to destroy objects in the pool. Just set them as inactive.
		foreach (var enemy in m_EnemyBulletPool)
		{
			if (enemy != null)
			{
				enemy.SetActive(false);
			}
		}

		ShieldMeterRef.transform.localScale.Set(shieldMeterFullSize.x, shieldMeterFullSize.y, shieldMeterFullSize.z);
		ShieldMeterRef.transform.position = shieldMeterDefaultPosition;
	}

	/// <summary>
	/// Update function used by Unity.
	/// </summary>
	private void Update()
	{
		//Quit?
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
			return;
		}

		//Is player alive?
		if (PlayerRef.activeSelf)
		{
			SetOverlayVisibility(false);

			//Shield animation and status.
			if (!m_PlayerScript.ShieldActive)
			{
				float ratio = (Time.time - m_PlayerScript.ShieldDeactivateTime) / (m_PlayerScript.ShieldReactivateTime - m_PlayerScript.ShieldDeactivateTime);
				ShieldMeterRef.transform.localScale = new Vector3(shieldMeterFullSize.x * ratio, shieldMeterFullSize.y, shieldMeterFullSize.z);
				ShieldMeterRef.transform.position = new Vector3(shieldMeterDefaultPosition.x + (ShieldMeterRef.transform.localScale.x * 0.5f) - (shieldMeterFullSize.x * 0.5f),
																shieldMeterDefaultPosition.y,
																shieldMeterDefaultPosition.z
																);
				ShieldMeterRef.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
			}
			else
			{
				ShieldMeterRef.transform.localScale = shieldMeterFullSize;
				ShieldMeterRef.GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
			}

			ScoreRef.GetComponent<GUIText>().text = m_Score.ToString();

			SpawnEnemies();
		}
		else
		{
			//Show the start screen.
			m_HighScore = Mathf.Max(m_Score, m_HighScore);
			SetOverlayVisibility(true);

#if UNITY_ANDROID
			if ( Input.GetMouseButtonDown( 0 ) )
#else
			if (Input.GetKeyDown(KeyCode.R))
#endif
			{
				ResetGame();
			}
		}
	}

	/// <summary>
	/// Sets whether or not we can see the start screen assets or not. Shows/Hides the game logo as well as the last game's score and the current high score.
	/// </summary>
	/// <param name="isVisable">If set to <c>true</c> is visable.</param>
	private void SetOverlayVisibility(bool isVisable)
	{
		//Start screen asssets and game UI should be mutually exclusive.
		LogoRef.GetComponent<Renderer>().enabled = isVisable;
		HowToPlayRef.SetActive(isVisable);

		ScoreRef.SetActive(!isVisable);
		ShieldMeterRef.SetActive(!isVisable);

		//Only show the rest of the detils we we have them.
		if (m_Score != -1)
		{
			PrevScoreRef.GetComponent<GUIText>().text = PreviousScoreText + m_Score.ToString();
			PrevScoreRef.GetComponent<GUIText>().enabled = isVisable;
		}
		else
		{
			PrevScoreRef.GetComponent<GUIText>().enabled = false;
		}

		if (m_HighScore != -1)
		{
			HighScoreRef.GetComponent<GUIText>().text = HighScoreText + m_HighScore.ToString();
			HighScoreRef.GetComponent<GUIText>().enabled = isVisable;
		}
		else
		{
			HighScoreRef.GetComponent<GUIText>().enabled = false;
		}
	}

	/// <summary>
	/// Spawns enemy clusters based upon a random chance.
	/// </summary>
	private void SpawnEnemies()
	{
		if (Time.time > m_NextSpawnTime)
		{
			int enemyIndex = Random.Range(0, EnemyPrefabs.Length);
			GameObject prefab = EnemyPrefabs[enemyIndex];
			Vector2 spawnCenter = new Vector2(Random.Range(SpawnArea.xMin, SpawnArea.xMax), Random.Range(SpawnArea.yMin, SpawnArea.yMax));
			GameObject enemyGO = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
			enemyGO.transform.position = spawnCenter;
			enemyGO.SetActive(true);

			m_NextSpawnTime = Time.time + Random.Range(MinEnemySpawmRate, MaxEnemySpawnRate);
		}
	}

	/// <summary>
	/// Function to find a number of requested inactive bullets from the pool and return them. Pools are used so that we don't create too many object; reusing older ones is more efficient.
	/// </summary>
	/// <returns>The bullet objects requested.</returns>
	/// <param name="numberOfBullets">Number of bullets needed.</param>
	public GameObject[] RequestBulletsFromPool(int numberOfBullets)
	{
		//Lets clamp so we know that we always need at least one.
		if (numberOfBullets <= 0)
		{
			numberOfBullets = 1;
		}

		GameObject[] bulletAllocation = new GameObject[numberOfBullets];
		int numberAllocated = 0;

		for (int bulletID = 0; bulletID < m_EnemyBulletPool.Length; bulletID++)
		{
			if (m_EnemyBulletPool[bulletID] != null)
			{
				if (m_EnemyBulletPool[bulletID].activeSelf == false) //Inactive bullets are available for reuse.
				{
					bulletAllocation[numberAllocated++] = m_EnemyBulletPool[bulletID];
					if (numberAllocated == numberOfBullets)
					{
						break;
					}
				}
			}
		}

		//Enough?
		if (numberAllocated == numberOfBullets)
		{
			return bulletAllocation;
		}

		//Need to create the rest.
		for (int bulletID = 0; bulletID < m_EnemyBulletPool.Length; bulletID++)
		{
			if (m_EnemyBulletPool[bulletID] == null)
			{
				m_EnemyBulletPool[bulletID] = (GameObject)Instantiate(EnemyShot, transform.position, Quaternion.identity);
				bulletAllocation[numberAllocated++] = m_EnemyBulletPool[bulletID];
				if (numberAllocated == numberOfBullets)
				{
					return bulletAllocation;
				}
			}
		}

		Debug.LogWarning("Enemy bullet pool full!");
		//Return what we've got.
		return bulletAllocation;
	}

	/// <summary>
	/// Delagate function called from the player object when it gets a hit. Function is allocated to player on initialisation.
	/// </summary>
	private void ScoreHit()
	{
		//Check here because other wise player can score with bullets that are loose after death.
		if (PlayerRef.activeSelf)
		{
			m_Score++;
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(SpawnArea.center, SpawnArea.size);
	}
}
