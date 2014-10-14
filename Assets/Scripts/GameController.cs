using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public GameObject enemyShot;
	public Vector2 spawnPostionMin;
	public Vector2 spawnPostionMax;

	public GameObject playerRef;
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
			SpawnCluster("shotgun");

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
		switch (typeID)
		{
			//Bunch of shots that spread outwards like a shot gun.
			case "shotgun":
				GameObject[] bulletsToSpawn = requestBulletsFromPool(10);
				Vector2 spawnCenter = new Vector2( Random.Range(spawnPostionMin.x,spawnPostionMax.x), Random.Range(spawnPostionMin.y, spawnPostionMax.y));
				Vector3 playerPosition = playerRef.transform.position;
				foreach (var spawn in bulletsToSpawn)
				{
					if (spawn != null)
					{
						//From center point, targetted slightly to the left or right of the player.
						spawn.GetComponent<EnemyShot>().Reset( spawnCenter,
						                                      new Vector2( Random.Range(playerPosition.x - GameSettings.SHOTGUN_SPREAD, playerPosition.x + GameSettings.SHOTGUN_SPREAD), playerPosition.y ),
						                                      Random.Range(GameSettings.ENEMY_SPEED_MIN, GameSettings.ENEMY_SPEED_MAX)
						            						);
					}
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
