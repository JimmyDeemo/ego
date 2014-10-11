using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    float speed;

	// Use this for initialization
	void Start ()
    {
        speed = 10.0f;

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (gameObject.activeSelf)
        {
            transform.Translate(0.0f, speed * Time.deltaTime, 0.0f);

            //Kill if out of bounds.
            if (transform.position.y > 9)
            {
                gameObject.SetActive(false);
            }
        }
	}

    public void Reset(Vector3 startPosition)
    {
        transform.position = startPosition;
        gameObject.SetActive(true);
    }
}
