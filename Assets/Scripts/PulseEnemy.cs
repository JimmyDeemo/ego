using UnityEngine;
using System.Collections;
using System;

public class PulseEnemy : Enemy
{
	public int BulletsInPulse = 9;
	public float PulseSpread = 110.0f; //In degrees.
	public float Speed = 2f;

	private Coroutine m_Coroutine;

	public override void CommenceFiring()
	{
		m_Coroutine = StartCoroutine(FireCoroutine());
	}

	private IEnumerator FireCoroutine()
	{
		FirePulse();

		yield return null;

		FiringEnded();
	}

	private void FirePulse()
	{
		EnemyShot[] bulletsToSpawn;
		Vector2 spawnCenter = transform.position;
		Vector2 fireDirection;

		bulletsToSpawn = m_BulletManager.RequestBulletsFromPool<EnemyShot>(BulletsInPulse);
		fireDirection = -Vector2.up;

		//Calculate the angular separation of each bullet.
		float degreesPerShot = PulseSpread / (BulletsInPulse - 1);
		Quaternion tempRotation = Quaternion.AngleAxis(-PulseSpread * 0.5f, Vector3.forward); //Creates a rotation around the vector that is facing the camera.
		fireDirection = tempRotation * fireDirection; //Start point of the arc shaped pulse.
		tempRotation = Quaternion.AngleAxis(degreesPerShot, Vector3.forward); //Rotation to alter each time.

		foreach (var spawn in bulletsToSpawn)
		{
			spawn.Reinit(spawnCenter, fireDirection, Speed);

			fireDirection = tempRotation * fireDirection;
		}
	}

	public void OnDestroy()
	{
		if (m_Coroutine != null)
		{
			StopCoroutine(m_Coroutine);
		}
	}
}
