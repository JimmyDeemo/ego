using UnityEngine;
using System.Collections;

/// <summary>
/// Static class used to play the sounds in the game.
/// </summary>
public class SoundManager : MonoBehaviour
{
	public AudioClip playerShootSound;

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
	}

	public void PlayShootSound()
	{
		audio.PlayOneShot(playerShootSound);
	}
}
