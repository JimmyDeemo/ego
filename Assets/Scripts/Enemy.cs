using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour
{
	public string ExitAnimatorParameterName = "Exit";

	protected Animator m_Animator;
	protected EnemyBulletManager m_BulletManager;

	private int m_ExitHash;

	public abstract void CommenceFiring();

	protected void FiringEnded()
	{
		m_Animator.SetBool(m_ExitHash, true);
	}

	public virtual void Awake()
	{
		m_Animator = GetComponent<Animator>();

		m_ExitHash = Animator.StringToHash(ExitAnimatorParameterName);
	}

	public void Init(EnemyBulletManager manager)
    {
		m_BulletManager = manager;
    }
}