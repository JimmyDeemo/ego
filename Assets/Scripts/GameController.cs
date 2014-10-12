using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public GameObject enemyShot;
	public Vector3 spawnPostion;

	private float nextSpawnTime;
	private float spawnFrequency; //In seconds.

	private GameObject[] enemyBulletPool;

	// Use this for initialization
	void Start ()
	{
		enemyBulletPool = new GameObject[GameSettings.ENEMY_BULLET_POOL_SIZE];
		nextSpawnTime = Time.time;
		spawnFrequency = 1;
	}
	
	// Update is called once per frame
	void Update ()
	{
		SpawnEnemies();
	}

	void SpawnEnemies()
	{
		if (Time.time > nextSpawnTime)
		{
			GameObject newEnemy = getBulletFromPool();
			if (newEnemy != null)
			{
				newEnemy.GetComponent<EnemyShot>().Reset(spawnPostion);
				newEnemy.GetComponent<EnemyShot>().Direction = Vector3.down;
			}

			nextSpawnTime = Time.time + spawnFrequency;
		}
	}

	GameObject getBulletFromPool()
	{
		bool fired = false;
		for (int bulletID = 0; bulletID < enemyBulletPool.Length; bulletID++)
		{
			if (enemyBulletPool[bulletID] != null)
			{
				if (enemyBulletPool[bulletID].activeSelf == false)
				{
					return enemyBulletPool[bulletID];
				}
			}
		}
		
		//No free bullet
		if (!fired)
		{
			for (int bulletID = 0; bulletID < enemyBulletPool.Length; bulletID++)
			{
				if (enemyBulletPool[bulletID] == null)
				{
					enemyBulletPool[bulletID] = (GameObject)Instantiate(enemyShot, transform.position, Quaternion.identity);
					return enemyBulletPool[bulletID];
				}
			}
		}

		Debug.LogWarning("Enemy bullet pool full!");
		return null;
	}
}
