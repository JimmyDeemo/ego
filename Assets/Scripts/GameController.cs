using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController>
{
	public enum ClusterType { SHOTGUN, PULSE };

	#region Public member variables.
	public float ShotgunChancePercentage = 0.5f;

	public float ShotgunSpread = 3.0f;
	public float PulseSpread = 110.0f; //In degrees.

	public float MinEnemySpawnRate = 0.5f; //In seconds.
	public float MaxEnemySpawnRate = 1.0f; //In seconds.
	public float MinEnemySpeed = 2.0f;
	public float MaxEnemySpeed = 3.0f;

	public string PreviousScoreText = "Score last game: ";
	public string HighScoreText = "Highest Score: ";

	public Enemy[] EnemyPrefabs;

	public Rect SpawnArea;

	public Player PlayerRef;
	public GameObject HowToPlayRef;
	public GameObject LogoRef;
	public GameObject HighScoreRef;
	public GameObject ShieldMeterRef;

	public Vector3 shieldMeterFullSize;
	public Vector3 shieldMeterDefaultPosition;

	public HudUI HudRef;
	#endregion

	#region Private member variables.
	private int m_Score;
	private int m_HighScore;

	private Player m_PlayerScript;

	private float m_NextSpawnTime;
	#endregion

	private BulletManager m_BulletManager;

	public Player Player
	{
		get
		{
			return m_PlayerScript;
		}
	}

	private void Awake()
	{
		m_BulletManager = GetComponent<BulletManager>();
	}

	/// <summary>
	/// Initialisation function used by Unity.
	/// </summary>
	private void Start()
	{
#if UNITY_ANDROID
		HowToPlayRef.GetComponent<GUIText>().text = "Touch (and hold) screen to start.";
#endif

		m_BulletManager.enabled = false;

		m_NextSpawnTime = Time.time;

		m_PlayerScript = PlayerRef.GetComponent<Player>();
		m_PlayerScript.Init(m_BulletManager);

		//Start the player dead and the logo visible.
		PlayerRef.gameObject.SetActive(false);
		PlayerRef.HitRegisteredEventHandler += ScoreHit;
		PlayerRef.PlayerDeathEventHandler += OnPlayerDeath;



		shieldMeterFullSize = ShieldMeterRef.transform.localScale;
		shieldMeterDefaultPosition = ShieldMeterRef.transform.position;

		m_HighScore = -1;
		m_Score = -1;

		HudRef.Score.visible = false;
		HudRef.PrevScore.visible = false;
	}

	/// <summary>
	/// Reset all elements in the game. Generally used when we want to begin a new game.
	/// </summary>
	private void ResetGame()
	{
		m_Score = 0;
		PlayerRef.GetComponent<Player>().Reset();

		ShieldMeterRef.transform.localScale.Set(shieldMeterFullSize.x, shieldMeterFullSize.y, shieldMeterFullSize.z);
		ShieldMeterRef.transform.position = shieldMeterDefaultPosition;

		m_BulletManager.enabled = true;
		HudRef.Score.visible = true;
		HudRef.PrevScore.visible = false;
		LogoRef.GetComponent<Renderer>().enabled = false;
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
		if (PlayerRef.gameObject.activeSelf)
		{
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

			HudRef.Score.text = m_Score.ToString();

			SpawnEnemies();
		}
		else
		{
#if UNITY_ANDROID
			if ( Input.GetMouseButtonDown( 0 ) )
#else
			if (Input.GetKeyDown(KeyCode.Space))
#endif
			{
				ResetGame();
			}
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
			Enemy prefab = EnemyPrefabs[enemyIndex];
			Vector2 spawnCenter = new(Random.Range(SpawnArea.xMin, SpawnArea.xMax), Random.Range(SpawnArea.yMin, SpawnArea.yMax));
			Enemy enemy = Instantiate(prefab);
			enemy.Init(m_BulletManager);
			enemy.transform.position = spawnCenter;
			enemy.gameObject.SetActive(true);

			m_NextSpawnTime = Time.time + Random.Range(MinEnemySpawnRate, MaxEnemySpawnRate);
		}
	}

	/// <summary>
	/// Delagate function called from the player object when it gets a hit. Function is allocated to player on initialisation.
	/// </summary>
	private void ScoreHit()
	{
		//Check here because other wise player can score with bullets that are loose after death.
		if (PlayerRef.gameObject.activeSelf)
		{
			m_Score++;
		}
	}

	private void OnPlayerDeath()
	{
		SoundManager.Instance.Lose();
		LogoRef.GetComponent<Renderer>().enabled = true;

		if (m_Score > m_HighScore)
        {
			m_HighScore = m_Score;
			HudRef.PrevScore.text = HighScoreText + m_HighScore.ToString();
        }
		else
        {
			HudRef.PrevScore.text = PreviousScoreText + m_Score.ToString();
		}

		HudRef.PrevScore.visible = true;
	}


	public void OnDrawGizmos()
	{
		var prevColour = Gizmos.color;

		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(SpawnArea.center, SpawnArea.size);

		Gizmos.color = prevColour;
	}
}
