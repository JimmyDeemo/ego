using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    float speed;

    bool alive;

	// Use this for initialization
	void Start ()
    {
        alive = true;
        speed = 10.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (alive)
        {
            transform.Translate(0.0f, speed * Time.deltaTime, 0.0f);

            //Kill if out of bounds.
            if (transform.position.y > 1000)
            {
                alive = false;
            }
        }


	}
}
