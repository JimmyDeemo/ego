using UnityEngine;
using System.Collections;

public class GameArea : MonoBehaviour
{
	void OnTriggerExit2D(Collider2D coll)
	{
		//Identify those that need destroying or 
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
