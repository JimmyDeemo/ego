using UnityEngine;
using System.Collections;

public class EnemyShot : MonoBehaviour
{
	private Vector3 direction;
	private float speed;

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		transform.Translate (direction * (speed * Time.deltaTime));
	}

	/// <summary>
	/// Resets the object with a new start position, direction and speed.
	/// </summary>
	/// <param name="startPosition">Start position.</param>
	/// <param name="newDirection">New direction. Note: Function assumes that this is a unit vector.</param>
	/// <param name="newSpeed">New speed.</param>
	public void Reset(Vector2 startPosition, Vector2 newDirection, float newSpeed)
	{
		speed = newSpeed;
		transform.position = startPosition;
		direction = newDirection;
		gameObject.SetActive(true);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "Player")
		{
			coll.gameObject.SetActive(false);
		}
	}
}
