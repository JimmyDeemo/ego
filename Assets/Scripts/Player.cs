using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private float speed;
    private float verticalMovement;
    private float horizontalMovement;

    public GameObject shotPrefab;
    private GameObject[] bulletPool;

	// Use this for initialization
	void Start ()
    {
        speed = 5.0f;

        bulletPool = new GameObject[GameSettings.PLAYER_BULLET_POOL_SIZE];
	}
	
	// Update is called once per frame
	void Update ()
    {
        verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(horizontalMovement, verticalMovement, 0.0f);

        if ( Input.GetButtonDown("Fire1") )
        {
            FireBullet();
        }
	}
	
    void FireBullet()
    {
        for (int bulletID = 0; bulletID < bulletPool.Length; bulletID++)
        {
            if (bulletPool[bulletID] != null)
            {
                if (bulletPool[bulletID].activeSelf == false)
                {
                    bulletPool[bulletID].GetComponent<Bullet>().Reset(transform.position);
                    return;
                }
            }
        }

		//No free bullet, try to make one.
        for (int bulletID = 0; bulletID < bulletPool.Length; bulletID++)
	    {
	        if (bulletPool[bulletID] == null)
            {
                bulletPool[bulletID] = (GameObject)Instantiate(shotPrefab, transform.position, transform.rotation);
                return;
            }
	    }

		Debug.LogWarning("Player bullet pool full!");
    }
}
