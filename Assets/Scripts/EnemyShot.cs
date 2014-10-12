using UnityEngine;
using System.Collections;

public class EnemyShot : MonoBehaviour
{
	private Vector3 direction;
	public Vector3 Direction
	{
		set
		{
			direction = value.normalized;
		}
	}

	private float speed;

	// Use this for initialization
	void Start ()
	{
			speed = 5.0f;
	}

	// Update is called once per frame
	void Update ()
	{
		transform.Translate (direction * (speed * Time.deltaTime));

		if (transform.position.y < -1.5)
		{
			gameObject.SetActive(false);
		}
	}

	public void Reset(Vector3 startPosition)
	{
		transform.position = startPosition;
		gameObject.SetActive(true);
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			return;
		}

		coll.gameObject.SetActive(false);
		Destroy(gameObject);
	}
}
