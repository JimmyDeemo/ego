using UnityEngine;
using System.Collections;

public class EnemyShot : MonoBehaviour
{
		private Vector3 direction;
		Vector3 Direction
		{
			set
			{
				direction = value.normalized;
			}
		}

		private float speed;

		// Use this for initialization
		void Start ()
		{
				speed = 10.0f;
		}
	
		// Update is called once per frame
		void Update ()
		{
				transform.Translate (direction * speed);
		}
}
