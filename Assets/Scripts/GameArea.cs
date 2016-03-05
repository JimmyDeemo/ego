using UnityEngine;
using System.Collections;

/// <summary>
/// Simple Class script for the Game Area object.
/// The primary role of this object is to detect when game objects have
/// left the playable area.
/// </summary>
public class GameArea : MonoBehaviour
{
	/// <summary>
	/// Trigger function called by Unity when the game area detects an object
	/// has exited it's bounds.
	/// </summary>
	/// <param name="coll">Collision object of the 2D game object that has left the area.</param>
	private void OnTriggerExit2D(Collider2D coll)
	{
		//Identify those that need destroying.
		if (coll.gameObject.tag == "PlayerBullet" || coll.gameObject.tag == "EnemyBullet")
		{
			if (coll.gameObject.tag == "PlayerBullet" && coll.gameObject.GetComponent<Bullet>().isSuper)
			{
				//Super shots are not pooled.
				Destroy(coll.gameObject);
			}
			else
			{
				coll.gameObject.SetActive(false);
			}
		}
	}
}
