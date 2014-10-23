using UnityEngine;
using System.Collections;

/// <summary>
/// Static class used to play the sounds in the game.
/// </summary>
public class SoundManager : MonoBehaviour
{
	public AudioClip playerShoot;
	public AudioClip superShot;
	public AudioClip shieldDown;
	public AudioClip shieldUp;
	public AudioClip lose;

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

	private void Awake()
	{
		instance = this;

		audio.volume = 0.8f;
	}

	public void ShootSound()
	{
		audio.PlayOneShot(playerShoot);
	}

	public void SuperShot()
	{
		audio.PlayOneShot(superShot);
	}

	public void ShieldDown()
	{
		audio.PlayOneShot(shieldDown);
	}

	public void ShieldUp()
	{
		audio.PlayOneShot(shieldUp);
	}

	public void Lose()
	{
		audio.PlayOneShot(lose);
	}
}
