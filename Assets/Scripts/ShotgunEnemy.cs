using UnityEngine;
using System.Collections;

public class ShotgunEnemy : Enemy
{
	public int BulletsInBurst = 10;
	public float ShotgunSpread = 3.0f;
	public float BulletSpeedMin = 2.0f;
	public float BulletSpeedMax = 3.0f;

	private Coroutine m_Coroutine;
	private Transform m_PlayerTransform;

	public override void CommenceFiring()
	{
		m_Coroutine = StartCoroutine(FireCoroutine());
	}

	private IEnumerator FireCoroutine()
	{
		FireBurst();

		yield return null;

		FiringEnded();
	}

	private void FireBurst()
	{
		EnemyShot[] bulletsToSpawn;
		Vector2 spawnCenter = transform.position;
		Vector2 fireDirection;

		bulletsToSpawn = m_BulletManager.RequestBulletsFromPool<EnemyShot>(BulletsInBurst);

		foreach (var spawn in bulletsToSpawn)
		{
			if (spawn != null)
			{
				//From center point, targetted slightly to the left or right of the player.
				fireDirection = new Vector2(Random.Range(m_PlayerTransform.position.x - ShotgunSpread, m_PlayerTransform.position.x + ShotgunSpread),
											 m_PlayerTransform.position.y
										   );
				fireDirection -= spawnCenter;
				fireDirection.Normalize();

				spawn.Reinit(spawnCenter, fireDirection, Random.Range(BulletSpeedMin, BulletSpeedMax));
			}
		}
	}

	public override void Awake()
	{
		base.Awake();

		m_PlayerTransform = GameController.Instance.Player.transform;
	}

	public void OnDestroy()
	{
		if (m_Coroutine != null)
		{
			StopCoroutine(m_Coroutine);
		}
	}
}
