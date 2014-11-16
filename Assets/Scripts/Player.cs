using UnityEngine;
using System.Collections;

/// <summary>
/// Script attached to the Player GameObject. Handles player movement and fireing.
/// </summary>
public class Player : MonoBehaviour
{
	public delegate void OnHit();
	public event OnHit onRegisterHit;

    private float speed;
    private float verticalMovement;
    private float horizontalMovement;

    public GameObject shotPrefab;
	public GameObject superShotPrefab;
    private GameObject[] bulletPool;
	private float nextFireTime;

    private Vector3 spawnPosition;
    private Vector3 spawnScale;

	private bool shieldActive;
	public bool ShieldActive
	{
		get
		{
			return this.shieldActive;
		}
	}

	private float shieldReactivateTime;
	public float ShieldReactivateTime
	{
		get
		{
			return this.shieldReactivateTime;
		}
	}

	public float ShieldDeactivateTime
	{
		get
		{
			return this.shieldReactivateTime - GameSettings.SHIELD_RECHARGE_TIME;
		}
	}


	// Use this for initialization
	private void Start ()
    {
        speed = 5.0f;

        bulletPool = new GameObject[GameSettings.PLAYER_BULLET_POOL_SIZE];

		nextFireTime = Time.time + GameSettings.PLAYER_RATE_OF_FIRE;

		SetShieldActive(true);

        //Could hard code these but take them from the scene view in case they get
        //modified there.
		spawnPosition = transform.position;
        spawnScale = transform.localScale;
	}

	/// <summary>
	/// Resets the player ready for a new game.
	/// Sets the game object to active at the end effectively begins the new games.
	/// </summary>
	public void Reset()
	{
        transform.position = spawnPosition;
        transform.localScale = spawnScale;

		//Unlikely that bullets would still be active but just in case.
        foreach (var bullet in bulletPool)
        {
            if (bullet != null)
            {
                bullet.SetActive(false);
            }
        }

		SetShieldActive(true);

        gameObject.SetActive(true);
	}
	
	// Update is called once per frame
    private void Update()
    {
		if (!shieldActive)
		{
			if (Time.time >= shieldReactivateTime)
			{
				SetShieldActive(true);
			}
		}

#if UNITY_ANDROID
		Vector3 pos = Input.mousePosition;
		pos.z = transform.position.z - Camera.main.transform.position.z;
		transform.position = Camera.main.ScreenToWorldPoint( pos ); 
#else
        verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;


        transform.Translate(horizontalMovement, verticalMovement, 0.0f);
#endif

		//Clamp to the screen.
		Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint( transform.position );
		playerScreenPosition.x = Mathf.Clamp(playerScreenPosition.x, 0, Screen.width);
		playerScreenPosition.y = Mathf.Clamp(playerScreenPosition.y, 0, Screen.height);
		transform.position = Camera.main.ScreenToWorldPoint(playerScreenPosition);

		if ( Input.GetButton("Fire1") && Time.time > nextFireTime )
        {
            FireBullet(GameSettings.GUN_SEPARATION);
			FireBullet(-GameSettings.GUN_SEPARATION);
			nextFireTime = Time.time + GameSettings.PLAYER_RATE_OF_FIRE;
        }

		//If we have released the fire button with a shield up, that means the players
		//needs a 'super shot'.
		if ( Input.GetButtonUp("Fire1") && shieldActive )
		{
			SetShieldActive(false);
			SpawnSuperShot();
		}
	}

	private void SpawnSuperShot()
	{
		int numToSpawn = Mathf.CeilToInt( GameSettings.SUPER_SHOT_RATIO * ( transform.localScale.x / spawnScale.x ) );
		float playerWidth = GetComponent<SpriteRenderer>().bounds.size.x;
		float xSeparation = playerWidth / numToSpawn;

		for (int i = 0; i < numToSpawn; i++)
		{
			Instantiate(superShotPrefab, new Vector3( transform.position.x - (playerWidth * 0.5f) + (i * xSeparation), transform.position.y, transform.position.z), transform.rotation);	
		}

		SoundManager.Instance.SuperShot();
	}

    private void FireBullet(float positionOffset)
    {
		Vector3 firePosition = transform.position;
		firePosition.x += positionOffset;
        for (int bulletID = 0; bulletID < bulletPool.Length; bulletID++)
        {
            if (bulletPool[bulletID] != null)
            {
                if (bulletPool[bulletID].activeSelf == false)
                {
					bulletPool[bulletID].GetComponent<Bullet>().Reset(firePosition);
					SoundManager.Instance.ShootSound();
                    return;
                }
            }
        }

		//No free bullet, try to make one.
        for (int bulletID = 0; bulletID < bulletPool.Length; bulletID++)
	    {
	        if (bulletPool[bulletID] == null)
            {
				bulletPool[bulletID] = (GameObject)Instantiate(shotPrefab, firePosition, transform.rotation);
				bulletPool[bulletID].GetComponent<Bullet>().onHitEvent += RegisterHit;
				SoundManager.Instance.ShootSound();
                return;
            }
	    }

		Debug.LogWarning("Player bullet pool full!");
    }

	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "EnemyBullet")
		{
			if (shieldActive)
			{
				SetShieldActive(false);
				coll.gameObject.SetActive(false);
			}
			else
			{
				SoundManager.Instance.Lose();
				gameObject.SetActive(false);
			}
		}
	}

	private void SetShieldActive( bool isActive )
	{
		shieldActive = isActive;
		transform.Find("Shield").gameObject.SetActive(isActive);

		if (!isActive)
		{
			SoundManager.Instance.ShieldDown();
			shieldReactivateTime = Time.time + GameSettings.SHIELD_RECHARGE_TIME;
		}
		else
		{
			SoundManager.Instance.ShieldUp();
		}
	}

    private void RegisterHit()
	{
		gameObject.transform.localScale *= GameSettings.PLAYER_SCALE_FACTOR;

		onRegisterHit();
	}
}
