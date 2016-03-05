using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

/// <summary>
/// Script attached to the Player GameObject. Handles player movement and fireing.
/// </summary>
public class Player : MonoBehaviour
{
#region Public member variables.
	[Header("Settings")]
	[Tooltip("How fast in seconds can the player fire.")]
	public float RateOfFire = 0.2f; //In seconds.
	[Tooltip("Max size of the bullet pool.")]
	public int BulletPoolSize = 40;
	[Range(1f, 1.5f), Tooltip("How much should the player grow with each hit?.")]
	public float ScaleFactor = 1.0015f;
	[Tooltip("Distance between the shot origins.")]
	public float GunSeparation = 0.3f;
	public float ShieldRechargeTime = 6.0f; //In seconds.
	[Tooltip("Per multiples of scale, how many shots shall we spawn for super shots?")]
	public int SuperShotRatio = 4; //Per multiples of scale.

	[Header("References")]
	public GameObject ShotPrefab;
	public GameObject SuperShotPrefab;

	public event Action HitRegisteredEventHandler;
#endregion

#region Public member variables.
	private Bullet[] m_BulletPool;
	private float m_NextFireTime;

	private Vector3 m_SpawnPosition;
	private Vector3 m_SpawnScale;

	private bool m_ShieldActive;
	private float m_ShieldReactivateTime;
#endregion

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
#endregion

#region Private member functions.
	/// <summary>
	/// Initialisation function used by Unity.
	/// </summary>
	private void Start ()
	{
		m_BulletPool = new Bullet[BulletPoolSize];

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
		verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
		horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

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
			FireBullet(GunSeparation);
			FireBullet(-GunSeparation);
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
		
		for (int i = 0; i < numToSpawn; i++)
		{
			GameObject superShotGO = PrefabUtility.InstantiatePrefab(SuperShotPrefab) as GameObject;
			Vector3 position = transform.position;
			position.x = position.x - (playerWidth * 0.5f) + (i * xSeparation);
			superShotGO.transform.position = position;
			superShotGO.transform.rotation = transform.rotation;
		}
		
		SoundManager.Instance.SuperShot();
	}

	/// <summary>
	/// Shooty bangs!
	/// </summary>
	/// <param name="positionOffset">Horizontal distance from the central firing point.</param>
	private void FireBullet(float positionOffset)
	{
		Vector3 firePosition = transform.position;
		firePosition.x += positionOffset;

		//Use a free bullet from the pool.
		for (int bulletID = 0; bulletID < m_BulletPool.Length; bulletID++)
		{
			Bullet bullet = m_BulletPool[bulletID];
			if (bullet != null)
			{
				if (!bullet.gameObject.activeSelf)
				{
					bullet.Reset(firePosition);
					SoundManager.Instance.ShootSound();
					return;
				}
			}
		}
		
		//No free bullet, try to make one.
		for (int bulletID = 0; bulletID < m_BulletPool.Length; bulletID++)
		{
			Bullet bullet = m_BulletPool[bulletID];
			if (bullet == null)
			{
				GameObject bulletGO = PrefabUtility.InstantiatePrefab(ShotPrefab) as GameObject;
				bulletGO.transform.position = firePosition;
				bulletGO.transform.rotation = transform.rotation;
				bullet = bulletGO.GetComponent<Bullet>();
				bullet.OnHitEventHandler += RegisterHit;
				SoundManager.Instance.ShootSound();
				return;
			}
		}
		
		Debug.LogWarning("Player bullet pool full!");
	}

	/// <summary>
	/// Trigger function used by Unity. Called when this objects collides with another object
	/// </summary>
	/// <param name="coll">The collider object of the game object that the player has collided with.</param>
	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "EnemyBullet")
		{
			if (m_ShieldActive)
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

		//Unlikely that bullets would still be active but just in case.
		for (int bulletID = 0; bulletID < m_BulletPool.Length; bulletID++)
		{
			Bullet bullet = m_BulletPool[bulletID];
			if (bullet != null)
			{
				bullet.gameObject.SetActive(false);
			}
		}

		SetShieldActive(true);

		gameObject.SetActive(true);
	}
#endregion
}
