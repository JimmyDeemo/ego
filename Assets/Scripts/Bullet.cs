using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class script for a player bullet. Handles movement and collision.
/// </summary>
public class Bullet : MonoBehaviour, IManagedBullet
{
	new private Rigidbody2D rigidbody;
	public float StandardSpeed = 10.0f;
	public float SuperSpeed = 8.0f;

	public event Action OnHitEventHandler;

	public bool IsSuper;

	private float m_Speed;

    private void Awake()
    {
		rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start ()
	{
		if (IsSuper)
		{
			m_Speed = SuperSpeed;
		}
		else
		{
			m_Speed = StandardSpeed;
		}

	}

	/// <summary>
	/// Trigger function used by Unity. Called when this objects collides with
	/// another 2D object set as a 'Trigger'.
	/// </summary>
	/// <param name="coll">Collider2D reference of the object that has collided with this one.</param>
	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.CompareTag("EnemyBullet"))
		{
			//Kill both unless this is a super bullet.
			if (!IsSuper)
			{
				gameObject.SetActive(false);
			}

			coll.gameObject.SetActive(false);

			OnHitEventHandler?.Invoke();
		}
	}

	/// <summary>
	/// Reset this bullet to a new starting position and sets it to active.
	/// Primarily used by anything that pools bullet objects, e.g. Player.
	/// </summary>
	/// <param name="startPosition">The new position for this object to start at.</param>
	public void Reinit(Vector3 startPosition, Quaternion rotation)
	{
		transform.SetPositionAndRotation(startPosition, rotation);
		gameObject.SetActive(true);
	}

	public void Poll()
	{
		rigidbody.transform.position += Vector3.up * (m_Speed * Time.deltaTime);
	}

	public void Disable()
    {
		gameObject.SetActive(false);
    }

	public bool IsAvailable()
    {
		return !gameObject.activeSelf;
    }

	public void SetParent(Transform parent)
    {
		transform.parent = parent;
    }
}
