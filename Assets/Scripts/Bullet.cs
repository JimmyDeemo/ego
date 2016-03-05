using UnityEngine;
using System.Collections;

/// <summary>
/// Class script for a player bullet. Handles movement and collision.
/// </summary>
public class Bullet : MonoBehaviour
{
	public delegate void OnHit();
	public event OnHit onHitEvent;

	public bool isSuper;

	private float speed;

#region Private member functions.
	/// <summary>
	/// Initialisation function used by Unity.
	/// </summary>
	private void Start ()
	{
		if (isSuper)
		{
			speed = 8.0f;
		}
		else
		{
			speed = 10.0f;
		}

	}
	
	/// <summary>
	/// Update function used by Unity.
	/// </summary>
	private void Update()
	{
		if (gameObject.activeSelf)
		{
			transform.Translate(0.0f, speed * Time.deltaTime, 0.0f);
		}
	}

	/// <summary>
	/// Trigger function used by Unity. Called when this objects collides with
	/// another 2D object set as a 'Trigger'.
	/// </summary>
	/// <param name="coll">Collider2D reference of the object that has collided with this one.</param>
	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "EnemyBullet")
		{
			//Kill kill both unless this is a super bullet.
			if (!isSuper)
			{
				gameObject.SetActive(false);
			}

			coll.gameObject.SetActive(false);

			if (onHitEvent != null)
			{
				onHitEvent();
			}
		}
	}
#endregion

#region Public member functions
	/// <summary>
	/// Reset this bullet to a new starting position and sets it to active.
	/// Primarily used by anything that pools bullet objects, e.g. Player.
	/// </summary>
	/// <param name="startPosition">The new position for this object to start at.</param>
	public void Reset(Vector3 startPosition)
	{
		transform.position = startPosition;
		gameObject.SetActive(true);
	}
#endregion
}
