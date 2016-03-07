using UnityEngine;
using System.Collections;

public class EnemyExitBehaviour : StateMachineBehaviour
{
	GameObject m_GameObject;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);

		m_GameObject = animator.gameObject;
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit(animator, stateInfo, layerIndex);

		Destroy(m_GameObject);
	}
}
