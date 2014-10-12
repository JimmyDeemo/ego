using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public GameObject enemyShot;
	public Vector3 spawnPostion;

	private float nextSpawnTime;
	private float spawnFrequency; //In seconds.

	// Use this for initialization
	void Start ()
	{
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
			GameObject newEnemy = (GameObject)Instantiate(enemyShot, spawnPostion, Quaternion.identity);
			newEnemy.GetComponent<EnemyShot>().Direction = Vector3.down;

			nextSpawnTime = Time.time + spawnFrequency;
		}
	}
}
