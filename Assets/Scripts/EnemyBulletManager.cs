using UnityEngine;
using System.Collections;

public class EnemyBulletManager : MonoBehaviour
{
	public GameObject BulletPrefab;
	public Transform BulletParent;
    public int EnemyBulletPoolSize = 100;

    private static GameObject[] m_EnemyBulletPool;

    // Use this for initialization
    void Start()
    {
        m_EnemyBulletPool = new GameObject[EnemyBulletPoolSize];
    }

#region Public API
    public void Reset()
    {
        //No need to destroy objects in the pool. Just set them as inactive.
        foreach (var enemy in m_EnemyBulletPool)
        {
            if (enemy != null)
            {
                enemy.SetActive(false);
            }
        }
    }

	/// <summary>
	/// Function to find a number of requested inactive bullets from the pool and return them. Pools are used so that we don't create too many object; reusing older ones is more efficient.
	/// </summary>
	/// <returns>The bullet objects requested.</returns>
	/// <param name="numberOfBullets">Number of bullets needed.</param>
	public GameObject[] RequestBulletsFromPool(int numberOfBullets)
	{
		//Lets clamp so we know that we always need at least one.
		if (numberOfBullets <= 0)
		{
			numberOfBullets = 1;
		}

		GameObject[] bulletAllocation = new GameObject[numberOfBullets];
		int numberAllocated = 0;

		for (int bulletID = 0; bulletID < m_EnemyBulletPool.Length; bulletID++)
		{
			if (m_EnemyBulletPool[bulletID] != null)
			{
				if (m_EnemyBulletPool[bulletID].activeSelf == false) //Inactive bullets are available for reuse.
				{
					bulletAllocation[numberAllocated++] = m_EnemyBulletPool[bulletID];
					if (numberAllocated == numberOfBullets)
					{
						break;
					}
				}
			}
		}

		//Enough?
		if (numberAllocated == numberOfBullets)
		{
			return bulletAllocation;
		}

		//Need to create the rest.
		for (int bulletID = 0; bulletID < m_EnemyBulletPool.Length; bulletID++)
		{
			if (m_EnemyBulletPool[bulletID] == null)
			{
				m_EnemyBulletPool[bulletID] = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
				m_EnemyBulletPool[bulletID].transform.SetParent(BulletParent);
				bulletAllocation[numberAllocated++] = m_EnemyBulletPool[bulletID];
				if (numberAllocated == numberOfBullets)
				{
					return bulletAllocation;
				}
			}
		}

		Debug.LogWarning("Enemy bullet pool full!");
		//Return what we've got.
		return bulletAllocation;
	}
#endregion

}
