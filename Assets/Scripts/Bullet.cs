using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public bool isSuper;

	public delegate void OnHit();
	public event OnHit onHitEvent;

    private float speed;

	// Use this for initialization
	void Start ()
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
	
	// Update is called once per frame
	void Update ()
    {
        if (gameObject.activeSelf)
        {
            transform.Translate(0.0f, speed * Time.deltaTime, 0.0f);
        }
	}

    public void Reset(Vector3 startPosition)
    {
        transform.position = startPosition;
        gameObject.SetActive(true);
    }

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "EnemyBullet")
		{
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
}
