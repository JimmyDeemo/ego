using UnityEngine;
using System.Collections;

public class GameArea : MonoBehaviour
{
	void OnTriggerExit2D(Collider2D other)
	{
		//Identify those that need destroying or 
		if (other.gameObject.tag == "PlayerBullet" || other.gameObject.tag == "EnemyBullet")
		{
			other.gameObject.SetActive(false);
		}
	}
}
