using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public GameObject enemyShot;
	public Vector2 spawnPostionMin;
	public Vector2 spawnPostionMax;

	public GameObject playerRef;
	public Transform playerTransform;
    public GameObject logoRef;
	public GameObject scoreRef;

	private int score;

	private float nextSpawnTime;

	private GameObject[] enemyBulletPool;


	// Use this for initialization
	void Start ()
	{
		enemyBulletPool = new GameObject[GameSettings.ENEMY_BULLET_POOL_SIZE];
		nextSpawnTime = Time.time;

		playerTransform = playerRef.transform;

        //Start the player dead and the logo visible.
        playerRef.SetActive(false);
		playerRef.GetComponent<Player>().onRegisterHit += ScoreHit;
	}

    void ResetGame()
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
    }
	
	// Update is called once per frame
	void Update ()
	{
        //Quit?
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			Application.Quit();
			return;
		}

        if (playerRef.activeSelf)
        {

            logoRef.renderer.enabled = false;
			scoreRef.SetActive(true);

			scoreRef.GetComponent<GUIText>().text = score.ToString();
        }
		else
        {
            logoRef.renderer.enabled = true;
			scoreRef.SetActive(false);

            if ( Input.GetKeyDown(KeyCode.R) )
            {
                ResetGame();
            }
        }

		SpawnEnemies();
	}

	void SpawnEnemies()
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
	/// 
	/// </summary>
	/// <param name="typeID"></param>
	void SpawnCluster( string typeID )
	{
		GameObject[] bulletsToSpawn;
		Vector2 spawnCenter = new Vector2( Random.Range(spawnPostionMin.x,spawnPostionMax.x), Random.Range(spawnPostionMin.y, spawnPostionMax.y));
		Vector2 fireDirection;

		switch (typeID)
		{
			//Bunch of shots that spread outwards like a shot gun.
			case "shotgun":
				bulletsToSpawn = requestBulletsFromPool(10);

				foreach (var spawn in bulletsToSpawn)
				{
					if (spawn != null)
					{
						fireDirection = new Vector2( Random.Range(playerTransform.position.x - GameSettings.SHOTGUN_SPREAD, playerTransform.position.x + GameSettings.SHOTGUN_SPREAD), playerTransform.position.y );
						fireDirection = fireDirection - spawnCenter;
						fireDirection.Normalize();
						//From center point, targetted slightly to the left or right of the player.
						spawn.GetComponent<EnemyShot>().Reset( spawnCenter,
						                                      fireDirection,
						                                      Random.Range(GameSettings.ENEMY_SPEED_MIN, GameSettings.ENEMY_SPEED_MAX)
						            						);
					}
				}
				break;

			case "pulse":
				int numShotsInPulse = 9;
				bulletsToSpawn = requestBulletsFromPool(numShotsInPulse);
				fireDirection = -Vector2.up;
				float degreesPerShot = GameSettings.PULSE_SPREAD / (numShotsInPulse - 1);
				Quaternion tempRotation = Quaternion.AngleAxis( -GameSettings.PULSE_SPREAD * 0.5f, Vector3.forward ); //Creates a rotation around the vector that is facing the camera.
				fireDirection = tempRotation * fireDirection; //Star point of the arc shaped pulse.
				tempRotation = Quaternion.AngleAxis( degreesPerShot, Vector3.forward ); //Rotation to alter each time.

				foreach (var spawn in bulletsToSpawn)
				{
					if (spawn != null)
					{
						//From center point, targetted slightly to the left or right of the player.
						spawn.GetComponent<EnemyShot>().Reset( spawnCenter,
						                                      fireDirection,
						                                      GameSettings.ENEMY_SPEED_MIN
						                                      );
					}

					fireDirection = tempRotation * fireDirection;
				}
				break;

			default:
				break;
		}
	}

	GameObject[] requestBulletsFromPool(int numberOfBullets)
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
				if (enemyBulletPool[bulletID].activeSelf == false)
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

	void ScoreHit()
	{
		score++;
	}
}
