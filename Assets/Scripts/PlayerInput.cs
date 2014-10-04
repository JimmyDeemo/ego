using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    private float speed;
    private float verticalMovement;
    private float horizontalMovement;

    private GameObject shotPrefab;
    private Bullet[] bullets;

	// Use this for initialization
	void Start ()
    {
        speed = 5.0f;

        shotPrefab = (GameObject)Resources.Load("shotPrefab");

        /*
        bullets = new Bullet[20];
        for (int bulletID = 0; bulletID < bullets.Length; bulletID++)
        {
            bullets[bulletID] = new Bullet();
        }
         */
	}
	
	// Update is called once per frame
	void Update ()
    {
        verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.Translate(horizontalMovement, verticalMovement, 0.0f);

        if ( Input.GetButtonDown("Fire1") )
        {
            print("FIRE!");
            Instantiate(shotPrefab, transform.position, transform.rotation);
        }
	}

}
