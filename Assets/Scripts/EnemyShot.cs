using UnityEngine;
using System.Collections;

/// <summary>
/// Class script for an enemy bullet. Handles movement and collision.
/// </summary>
public class EnemyShot : MonoBehaviour, IManagedBullet
{
	new private Rigidbody2D rigidbody;
	private Vector3 direction;
	private float speed;

	private Quaternion rotationToDirection = new Quaternion();

    private void Awake()
    {
		rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Poll()
	{
		rigidbody.transform.position += direction * (speed * Time.deltaTime);
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
