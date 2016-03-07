using UnityEngine;
using System.Collections;

/// <summary>
/// Singleton class used to play the sounds in the game.
/// </summary>
public class SoundManager : MonoBehaviour
{
	public AudioClip playerShoot;
	public AudioClip superShot;
	public AudioClip shieldDown;
	public AudioClip shieldUp;
	public AudioClip lose;

	public AudioSource AudioSource;

	private static SoundManager instance;
	public static SoundManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<SoundManager>();
			}
			return instance;
		}
	}

	/// <summary>
	/// Awake this instance. (Used by unity, effectivly initialisation.)
	/// </summary>
	private void Awake()
	{
		instance = this;

		AudioSource.volume = 0.8f;
	}

	/// <summary>
	/// Sound for player firing.
	/// </summary>
	public void ShootSound()
	{
		AudioSource.PlayOneShot(playerShoot);
	}

	/// <summary>
	/// Sound for super shot.
	/// </summary>
	public void SuperShot()
	{
		AudioSource.PlayOneShot(superShot);
	}

	/// <summary>
	/// Sound for losing shield
	/// </summary>
	public void ShieldDown()
	{
		AudioSource.PlayOneShot(shieldDown);
	}

	/// <summary>
	/// Shields available sound.
	/// </summary>
	public void ShieldUp()
	{
		AudioSource.PlayOneShot(shieldUp);
	}

	/// <summary>
	/// Game over man, game over.
	/// </summary>
	public void Lose()
	{
		AudioSource.PlayOneShot(lose);
	}
}
