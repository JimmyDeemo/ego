using UnityEngine;
using System.Collections;

public class GameArea : MonoBehaviour
{
	void OnTriggerExit2D(Collider2D coll)
	{
		//Identify those that need destroying or 
		if (coll.gameObject.tag == "PlayerBullet" || coll.gameObject.tag == "EnemyBullet")
		{
			coll.gameObject.SetActive(false);
		}
	}
}
