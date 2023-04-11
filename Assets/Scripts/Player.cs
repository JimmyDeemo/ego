using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Script attached to the Player GameObject. Handles player movement and firing.
/// </summary>
public class Player : MonoBehaviour
{
#region Public member variables.
	[Header("Settings")]
	[Tooltip("How fast in seconds can the player fire.")]
	public float RateOfFire = 0.2f; //In seconds.
	[Range(1f, 1.5f), Tooltip("How much should the player grow with each hit?.")]
	public float ScaleFactor = 1.0015f;
	[Tooltip("Distance between the shot origins.")]
	public float GunSeparation = 0.3f;
	public float ShieldRechargeTime = 6.0f; //In seconds.
	[Tooltip("Per multiples of scale, how many shots shall we spawn for super shots?")]
	public int SuperShotRatio = 4; //Per multiples of scale.

	public float Speed = 5f;

	[Header("References")]
	public Bullet SuperShotPrefab;

	public event Action HitRegisteredEventHandler;
	public event Action PlayerDeathEventHandler;
	#endregion

	private BulletManager m_BulletManager;
	private float m_NextFireTime;

	private Vector3 m_SpawnPosition;
	private Vector3 m_SpawnScale;

	private bool m_ShieldActive;
	private float m_ShieldReactivateTime;

#region Properties.
	public bool ShieldActive
	{
		get
		{
			return m_ShieldActive;
		}
	}

	public float ShieldReactivateTime
	{
		get
		{
			return m_ShieldReactivateTime;
		}
	}

	public float ShieldDeactivateTime
	{
		get
		{
			return m_ShieldReactivateTime - ShieldRechargeTime;
		}
	}

    internal void Init(BulletManager manager)
    {
		m_BulletManager = manager;
    }
    #endregion

    #region Private member functions.
    /// <summary>
    /// Initialisation function used by Unity.
    /// </summary>
    private void Start ()
	{
		m_NextFireTime = Time.timeSinceLevelLoad + RateOfFire;

		SetShieldActive(true);

		//Could hard code these but take them from the scene view in case they get
		//modified there.
		m_SpawnPosition = transform.position;
		m_SpawnScale = transform.localScale;
	}

	/// <summary>
	/// Update function used by Unity.
	/// </summary>
	private void Update()
	{
		//Shield status
		if (!m_ShieldActive)
		{
			if (Time.time >= m_ShieldReactivateTime)
			{
				SetShieldActive(true);
			}
		}

		//Movement
#if UNITY_ANDROID
		//TODO: Make this smarter but positioning player just above finger position.
		Vector3 pos = Input.mousePosition;
		pos.z = transform.position.z - Camera.main.transform.position.z;
		transform.position = Camera.main.ScreenToWorldPoint( pos ); 
#else
		float verticalMovement = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
		float horizontalMovement = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;

		transform.Translate(horizontalMovement, verticalMovement, 0.0f);
#endif
		
		//Clamp to the screen.
		Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint( transform.position );
		playerScreenPosition.x = Mathf.Clamp(playerScreenPosition.x, 0, Screen.width);
		playerScreenPosition.y = Mathf.Clamp(playerScreenPosition.y, 0, Screen.height);
		transform.position = Camera.main.ScreenToWorldPoint(playerScreenPosition);

		//Firing.
		if ( Input.GetButton("Fire1") && Time.time > m_NextFireTime )
		{
			Bullet[] bullets = m_BulletManager.RequestBulletsFromPool<Bullet>(2);
			FireBullet(bullets[0], GunSeparation);
			FireBullet(bullets[1], -GunSeparation);
			m_NextFireTime = Time.time + RateOfFire;
		}
		
		//If we have released the fire button with a shield up, that means the players
		//needs a 'super shot'.
		if ( Input.GetButtonUp("Fire1") && m_ShieldActive )
		{
			SetShieldActive(false);
			SpawnSuperShot();
		}
	}

	/// <summary>
	/// When activated, will create a set of special bullets that clear a path for the player.
	/// </summary>
	private void SpawnSuperShot()
	{
		//Super shot number and width is proportional to the players current size.
		int numToSpawn = Mathf.CeilToInt( SuperShotRatio * ( transform.localScale.x / m_SpawnScale.x ) );
		float playerWidth = GetComponent<SpriteRenderer>().bounds.size.x;
		float xSeparation = playerWidth / numToSpawn;
		float halfSeparation = xSeparation / 2.0f;
		float halfWidth = playerWidth / 2.0f;

		Vector3 basePosition = transform.position;
		basePosition.x -= halfWidth + halfSeparation;
		SuperBullet[] superBullets = m_BulletManager.RequestBulletsFromPool<SuperBullet>(numToSpawn);
		foreach(var bullet in superBullets)
		{
			basePosition.x += xSeparation;
			bullet.Reinit(basePosition, transform.rotation);
		}
		
		SoundManager.Instance.SuperShot();
	}

	/// <summary>
	/// Shooty bangs!
	/// </summary>
	/// <param name="positionOffset">Horizontal distance from the central firing point.</param>
	private void FireBullet(Bullet bullet, float positionOffset)
	{
		Vector3 firePosition = transform.position;
		firePosition.x += positionOffset;

		bullet.Reinit(firePosition, transform.rotation);
		bullet.OnHitEventHandler -= RegisterHit;
		bullet.OnHitEventHandler += RegisterHit;
		SoundManager.Instance.ShootSound();
	}

	/// <summary>
	/// Trigger function used by Unity. Called when this objects collides with another object
	/// </summary>
	/// <param name="coll">The collider object of the game object that the player has collided with.</param>
	private void OnTriggerEnter2D(Collider2D coll)
	{
        if (coll.CompareTag("EnemyBullet"))
        {
            if (m_ShieldActive)
            {
                SetShieldActive(false);
                coll.gameObject.SetActive(false);
            }
            else
            {
				PlayerDeathEventHandler?.Invoke();
                gameObject.SetActive(false);
			}
        }
    }

	/// <summary>
	/// Function to set the status of the players shields.
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> is active.</param>
	private void SetShieldActive( bool isActive )
	{
		m_ShieldActive = isActive;
		transform.Find("Shield").gameObject.SetActive(isActive);
		
		if (!isActive)
		{
			SoundManager.Instance.ShieldDown();
			m_ShieldReactivateTime = Time.time + ShieldRechargeTime;
		}
		else
		{
			SoundManager.Instance.ShieldUp();
		}
	}

	/// <summary>
	/// Delegate function given to bullet objects to register a hit when they collide with an enebmy shot.
	/// </summary>
	private void RegisterHit()
	{
		//Grow the players 'ego'.
		gameObject.transform.localScale *= ScaleFactor;
		
		HitRegisteredEventHandler();
	}
#endregion

#region Public member functions
	/// <summary>
	/// Resets the player ready for a new game.
	/// Setting the game object to active at the end effectively begins the new games.
	/// </summary>
	public void Reset()
	{
		transform.position = m_SpawnPosition;
		transform.localScale = m_SpawnScale;

		SetShieldActive(true);

		gameObject.SetActive(true);
	}
#endregion
}
