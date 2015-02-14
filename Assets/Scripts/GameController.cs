using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{

#region Public member variables.
	public GameObject enemyShot;
	public Vector2 spawnPostionMin;
	public Vector2 spawnPostionMax;

	public GameObject playerRef;
	public GameObject howToPlayRef;
    public GameObject logoRef;
	public GameObject scoreRef;
	public GameObject prevScoreRef;
	public GameObject highScoreRef;
	public GameObject shieldMeterRef;

	public Vector3 shieldMeterFullSize;
	public Vector3 shieldMeterDefaultPosition;
#endregion

#region Public member variables.
	private int score;
	private int highScore;

	private Transform playerTransform;
	private Player playerScript;

	private float nextSpawnTime;

	private GameObject[] enemyBulletPool;
#endregion

	/// <summary>
	/// Initialisation function used by Unity.
	/// </summary>
	private void Start ()
	{
#if UNITY_ANDROID
		howToPlayRef.guiText.text = "Touch (and hold) screen to start.";
#endif

		enemyBulletPool = new GameObject[GameSettings.ENEMY_BULLET_POOL_SIZE];
		nextSpawnTime = Time.time;

		playerTransform = playerRef.transform;
		playerScript = playerRef.GetComponent<Player>();

        //Start the player dead and the logo visible.
        playerRef.SetActive(false);
		playerRef.GetComponent<Player>().onRegisterHit += ScoreHit;

		shieldMeterFullSize = shieldMeterRef.transform.localScale;
		shieldMeterDefaultPosition = shieldMeterRef.transform.position;

		highScore = -1;
		score = -1;
	}

	/// <summary>
	/// Reset all elements in the game. Genarally used when we want to begin a new game.
	/// </summary>
   	private void ResetGame()
    {
		score = 0;
        playerRef.GetComponent<Player>().Reset();

        //No need to destroy objects in the pool. Just set them as inactive.
        foreach (var enemy in enemyBulletPool)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
            }
        }

		shieldMeterRef.transform.localScale.Set(shieldMeterFullSize.x, shieldMeterFullSize.y, shieldMeterFullSize.z);
		shieldMeterRef.transform.position = shieldMeterDefaultPosition;
    }
	
	/// <summary>
	/// Update function used by Unity.
	/// </summary>
	private void Update ()
	{
        //Quit?
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			Application.Quit();
			return;
		}

		//Is player alive?
        if (playerRef.activeSelf)
        {
			SetOverlayVisibility(false);

			//Shield animation and status.
			if (!playerScript.ShieldActive)
			{
				float ratio = (Time.time - playerScript.ShieldDeactivateTime) / (playerScript.ShieldReactivateTime - playerScript.ShieldDeactivateTime);
				shieldMeterRef.transform.localScale =  new Vector3( shieldMeterFullSize.x * ratio, shieldMeterFullSize.y, shieldMeterFullSize.z );
				shieldMeterRef.transform.position = new Vector3(shieldMeterDefaultPosition.x + (shieldMeterRef.transform.localScale.x * 0.5f) - (shieldMeterFullSize.x * 0.5f),
				                                                shieldMeterDefaultPosition.y,
				                                                shieldMeterDefaultPosition.z
				                                                );
				shieldMeterRef.renderer.material.color = new Color( 1.0f, 0.0f, 0.0f);
			}
			else
			{
				shieldMeterRef.transform.localScale = shieldMeterFullSize;
				shieldMeterRef.renderer.material.color = new Color( 0.0f, 1.0f, 0.0f);
			}

			scoreRef.GetComponent<GUIText>().text = score.ToString();

			SpawnEnemies();
        }
		else
        {
			//Show the start screen.
			highScore = Mathf.Max( score, highScore );
			SetOverlayVisibility(true);

#if UNITY_ANDROID
			if ( Input.GetMouseButtonDown( 0 ) )
#else
			if ( Input.GetKeyDown(KeyCode.R) )
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
	private void  SetOverlayVisibility( bool isVisable )
	{
		//Start screen asssets and game UI should be mutually exclusive.
		logoRef.renderer.enabled = isVisable;
		howToPlayRef.SetActive(isVisable);

		scoreRef.SetActive(!isVisable);
		shieldMeterRef.SetActive(!isVisable);

		//Only show the rest of the detils we we have them.
		if (score != -1)
		{
			prevScoreRef.GetComponent<GUIText>().text = GameSettings.PREVIOUS_SCORE_TEXT + score.ToString();
			prevScoreRef.GetComponent<GUIText>().enabled = isVisable;
		}
		else
		{
			prevScoreRef.GetComponent<GUIText>().enabled = false;
		}

		if (highScore != -1)
		{
			highScoreRef.GetComponent<GUIText>().text = GameSettings.HIGH_SCORE_TEXT + highScore.ToString();
			highScoreRef.GetComponent<GUIText>().enabled = isVisable;
		}
		else
		{
			highScoreRef.GetComponent<GUIText>().enabled = false;
		}
	}

	/// <summary>
	/// Spawns enemy clusters based upon a random chance.
	/// </summary>
	private void SpawnEnemies()
	{
		if (Time.time > nextSpawnTime)
		{
			if (Random.value <= GameSettings.SHOTGUN_CHANCE)
			{
				SpawnCluster("shotgun");
			}
			else
			{
				SpawnCluster("pulse");
			}

			nextSpawnTime = Time.time + Random.Range(GameSettings.ENEMY_RATE_OF_SPAWN_MIN, GameSettings.ENEMY_RATE_OF_SPAWN_MAX);
		}
	}

	/// <summary>
	/// Spawns a cluster of enemies dependant on type.
	/// 	/// </summary>
	/// <param name="typeID"></param>
	private void SpawnCluster( string typeID )
	{
		GameObject[] bulletsToSpawn;
		Vector2 spawnCenter = new Vector2( Random.Range(spawnPostionMin.x,spawnPostionMax.x), Random.Range(spawnPostionMin.y, spawnPostionMax.y));
		Vector2 fireDirection;

		switch (typeID)
		{
			//Bunch of shots that spread outwards like a shot gun.
			case "shotgun":
				bulletsToSpawn = RequestBulletsFromPool(10);

				foreach (var spawn in bulletsToSpawn)
				{
					if (spawn != null)
					{
						//From center point, targetted slightly to the left or right of the player.
						fireDirection = new Vector2( Random.Range(playerTransform.position.x - GameSettings.SHOTGUN_SPREAD, playerTransform.position.x + GameSettings.SHOTGUN_SPREAD),
						                             playerTransform.position.y
						                           );
						fireDirection = fireDirection - spawnCenter;
						fireDirection.Normalize();

						spawn.GetComponent<EnemyShot>().Reset( spawnCenter, fireDirection, Random.Range(GameSettings.ENEMY_SPEED_MIN, GameSettings.ENEMY_SPEED_MAX) );
					}
				}
				break;
			
			//Shots fly from center point out in an evenly distributed arc.
			case "pulse":
				int numShotsInPulse = 9;
				bulletsToSpawn = RequestBulletsFromPool(numShotsInPulse);
				fireDirection = -Vector2.up;

				//Calculate the angular separation of each bullet.
				float degreesPerShot = GameSettings.PULSE_SPREAD / (numShotsInPulse - 1);
				Quaternion tempRotation = Quaternion.AngleAxis( -GameSettings.PULSE_SPREAD * 0.5f, Vector3.forward ); //Creates a rotation around the vector that is facing the camera.
				fireDirection = tempRotation * fireDirection; //Start point of the arc shaped pulse.
				tempRotation = Quaternion.AngleAxis( degreesPerShot, Vector3.forward ); //Rotation to alter each time.

				foreach (var spawn in bulletsToSpawn)
				{
					if (spawn != null)
					{
						spawn.GetComponent<EnemyShot>().Reset( spawnCenter, fireDirection, GameSettings.ENEMY_SPEED_MIN );
					}

					fireDirection = tempRotation * fireDirection;
				}
				break;

			default:
				break;
		}
	}

	/// <summary>
	/// Function to find a number of requested inactive bullets from the pool and return them. Pools are used so that we don't create too many object; reusing older ones is more efficient.
	/// </summary>
	/// <returns>The bullet objects requested.</returns>
	/// <param name="numberOfBullets">Number of bullets needed.</param>
	private GameObject[] RequestBulletsFromPool(int numberOfBullets)
	{
		//Lets clamp so we know that we always need at least one.
		if (numberOfBullets <= 0)
		{
			numberOfBullets = 1;
		}

		GameObject[] bulletAllocation = new GameObject[numberOfBullets];
		int numberAllocated = 0;

		for (int bulletID = 0; bulletID < enemyBulletPool.Length; bulletID++)
		{
			if (enemyBulletPool[bulletID] != null)
			{
				if (enemyBulletPool[bulletID].activeSelf == false) //Inactive bullets are available for reuse.
				{
					bulletAllocation[numberAllocated++] = enemyBulletPool[bulletID];
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
		for (int bulletID = 0; bulletID < enemyBulletPool.Length; bulletID++)
		{
			if (enemyBulletPool[bulletID] == null)
			{
				enemyBulletPool[bulletID] = (GameObject)Instantiate(enemyShot, transform.position, Quaternion.identity);
				bulletAllocation[numberAllocated++] = enemyBulletPool[bulletID];
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
		if (playerRef.activeSelf)
		{
			score++;
		}
	}
}
