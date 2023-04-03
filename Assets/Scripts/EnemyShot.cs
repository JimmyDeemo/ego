using UnityEngine;
using System.Collections;

/// <summary>
/// Class script for an enemy bullet. Handles movement and collision.
/// </summary>
public class EnemyShot : MonoBehaviour
{
	private Vector3 direction;
	private float speed;

	private Quaternion rotationToDirection = new Quaternion();

#region Private member functions.W
	/// <summary>
	/// Update function used by Unity.
	/// </summary>
	private void Update ()
	{
		transform.position += (direction * (speed * Time.deltaTime));
	}
#endregion

	/// <summary>
	/// Resets the object with a new start position, direction and speed.
	/// </summary>
	/// <param name="startPosition">Start position.</param>
	/// <param name="newDirection">New direction. Note: Function assumes that this is a unit vector.</param>
	/// <param name="newSpeed">New speed.</param>
	public void Reinit(Vector2 startPosition, Vector2 newDirection, float newSpeed)
	{
		speed = newSpeed;
		transform.position = startPosition;
		direction = newDirection;

		rotationToDirection.SetFromToRotation(Vector2.down, direction);
		transform.rotation = rotationToDirection;

		gameObject.SetActive(true);
	}
}
