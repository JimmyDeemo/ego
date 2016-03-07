using UnityEngine;
using System.Collections;

public class EnemyFireBehaviour : StateMachineBehaviour
{
	Enemy m_Enemy;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);

		m_Enemy = animator.GetComponent<Enemy>();

		if (m_Enemy != null)
		{
			m_Enemy.CommenceFiring();
		}
	}
}
