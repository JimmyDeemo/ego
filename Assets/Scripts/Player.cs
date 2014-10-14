using UnityEngine;
using System.Collections;

/// <summary>
/// Script attached to the Player GameObject. Handles player movement and fireing.
/// </summary>
public class Player : MonoBehaviour
{
    private float speed;
    private float verticalMovement;
    private float horizontalMovement;

    public GameObject shotPrefab;
    private GameObject[] bulletPool;
	private float nextFireTime;

    private Vector3 spawnPosition;
    private Vector3 spawnScale;

	// Use this for initialization
	private void Start ()
    {
        speed = 5.0f;

        bulletPool = new GameObject[GameSettings.PLAYER_BULLET_POOL_SIZE];

		nextFireTime = Time.time + GameSettings.PLAYER_RATE_OF_FIRE;

        //Could hard code these but take them from the scene view in case they get
        //modified there.
		spawnPosition = transform.position;
        spawnScale = transform.localScale;
	}

	public void Reset()
	{
        transform.position = spawnPosition;
        transform.localScale = spawnScale;

        foreach (var bullet in bulletPool)
        {
            if (bullet != null)
            {
                bullet.SetActive(false);
            }
        }

        gameObject.SetActive(true);
	}
	
	// Update is called once per frame
    private void Update()
    {
        verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(horizontalMovement, verticalMovement, 0.0f);

		//Clamp to the screen.
		Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint( transform.position );
		playerScreenPosition.x = Mathf.Clamp(playerScreenPosition.x, 0, Screen.width);
		playerScreenPosition.y = Mathf.Clamp(playerScreenPosition.y, 0, Screen.height);
		transform.position = Camera.main.ScreenToWorldPoint(playerScreenPosition);

		if ( Input.GetButton("Fire1") && Time.time > nextFireTime )
        {
            FireBullet();
			nextFireTime = Time.time + GameSettings.PLAYER_RATE_OF_FIRE;
        }
	}

    private void FireBullet()
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
				bulletPool[bulletID].GetComponent<Bullet>().onHitEvent += registerHit;
                return;
            }
	    }

		Debug.LogWarning("Player bullet pool full!");
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "EnemyBullet")
        {
            gameObject.SetActive(false);
        }
    }

    private void registerHit()
	{
		gameObject.transform.localScale *= GameSettings.PLAYER_SCALE_FACTOR;
	}
}
