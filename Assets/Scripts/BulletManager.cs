using UnityEngine;
using UnityEngine.Serialization;

public class BulletManager : MonoBehaviour
{
	public enum BulletType
    {
		Enemy,
		Player
    };

	public Bullet BulletPrefab;
	public EnemyShot ShotPrefab;

	public Transform BulletParent;
	[FormerlySerializedAs("EnemyBulletPoolSize")]
	public int BulletPoolSize = 100;

    private static IManagedBullet[] m_EnemyBulletPool;

    // Use this for initialization
    private void Start()
    {
        m_EnemyBulletPool = new IManagedBullet[BulletPoolSize];
    }

	/// <summary>
	/// Update function used by Unity.
	/// </summary>
	private void Update()
	{
        for (int i = 0; i < BulletPoolSize; i++)
        {
			if (m_EnemyBulletPool[i] != null)
			{
				m_EnemyBulletPool[i].Poll();
			}
        }
	}

	#region Public API
	public void Reset()
    {
        //No need to destroy objects in the pool. Just set them as inactive.
        foreach (var enemy in m_EnemyBulletPool)
        {
            if (enemy != null)
            {
                enemy.Disable();
            }
        }
    }

	/// <summary>
	/// Function to find a number of requested inactive bullets from the pool and return them. Pools are used so that we don't create too many object; reusing older ones is more efficient.
	/// </summary>
	/// <returns>The bullet objects requested.</returns>
	/// <param name="numberOfBullets">Number of bullets needed.</param>
	public T[] RequestBulletsFromPool<T>(int numberOfBullets) where	T : Object, IManagedBullet
	{
		//Lets clamp so we know that we always need at least one.
		if (numberOfBullets <= 0)
		{
			numberOfBullets = 1;
		}

		T[] bulletAllocation = new T[numberOfBullets];
		int numberAllocated = 0;

		for (int bulletID = 0; bulletID < m_EnemyBulletPool.Length; bulletID++)
		{
			if (m_EnemyBulletPool[bulletID] != null &&
				m_EnemyBulletPool[bulletID] is T &&
				m_EnemyBulletPool[bulletID].IsAvailable())
			{
				bulletAllocation[numberAllocated++] = (T)m_EnemyBulletPool[bulletID];
				if (numberAllocated == numberOfBullets)
				{
					break;
				}
			}
		}

		//Enough?
		if (numberAllocated == numberOfBullets)
		{
			return bulletAllocation;
		}

		Object prefabToInsantiate;
        switch (typeof(T))
        {
			case var p when p == typeof(Bullet):
				prefabToInsantiate = BulletPrefab;
				break;
			case var p when p == typeof(EnemyShot):
				prefabToInsantiate = ShotPrefab;
				break;
            default:
				throw new System.NotSupportedException("This type is not supported within the pool.");
        }

        //Need to create the rest.
        for (int bulletID = 0; bulletID < m_EnemyBulletPool.Length; bulletID++)
		{
			if (m_EnemyBulletPool[bulletID] == null)
			{
				m_EnemyBulletPool[bulletID] = (IManagedBullet)Instantiate(prefabToInsantiate, transform.position, Quaternion.identity);
				m_EnemyBulletPool[bulletID].SetParent(BulletParent);
				bulletAllocation[numberAllocated++] = (T)m_EnemyBulletPool[bulletID];
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
