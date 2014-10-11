using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    private float speed;
    private float verticalMovement;
    private float horizontalMovement;

    public GameObject shotPrefab;
    private GameObject[] bulletPool;

    public const int MAX_BULLETS = 10;

	// Use this for initialization
	void Start ()
    {
        speed = 5.0f;

        bulletPool = new GameObject[MAX_BULLETS];
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
            FireBullet();
        }
	}
	
    void FireBullet()
    {
        bool fired = false;
        for (int bulletID = 0; bulletID < bulletPool.Length; bulletID++)
        {
            if (bulletPool[bulletID] != null)
            {
                if (bulletPool[bulletID].activeSelf == false)
                {
                    bulletPool[bulletID].GetComponent<Bullet>().Reset(transform.position);
                    fired = true;
                    break;
                }
            }
        }

		//No free bullet
        if (!fired)
        {
            bool added = false;
            for (int bulletID = 0; bulletID < bulletPool.Length; bulletID++)
		    {
		        if (bulletPool[bulletID] == null)
                {
                    print("Bullet added " + bulletID);
                    bulletPool[bulletID] = (GameObject)Instantiate(shotPrefab, transform.position, transform.rotation);
                    added = true;
                    break;
                }
		    }
        }
    }
}
