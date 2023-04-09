using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class script for a player bullet. Handles movement and collision.
/// </summary>
public class SuperBullet : MonoBehaviour, IManagedBullet
{
	public event Action OnHitEventHandler;

	[SerializeField]
	private float m_Speed;
	private Transform m_Transform;

	public Vector3 Position { get { return m_Transform.position; } }

    private void Awake()
    {
		m_Transform = GetComponent<Rigidbody2D>().transform;
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
		m_Transform.position += Vector3.up * (m_Speed * Time.deltaTime);
	}

	public void Disable()
    {
		gameObject.SetActive(false);
    }

	public bool IsActive()
    {
		return gameObject.activeSelf;
    }

	public void SetParent(Transform parent)
    {
		transform.parent = parent;
    }
}
