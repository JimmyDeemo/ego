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

	public void Reset(Vector2 startPosition, Vector2 targetPosition, float newSpeed)
	{
		speed = newSpeed;
		transform.position = startPosition;
		direction = targetPosition - startPosition;
		direction.Normalize();
		gameObject.SetActive(true);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "PlayerBullet")
		{
			Destroy(gameObject);
		}
	}
}
