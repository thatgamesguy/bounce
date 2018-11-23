using UnityEngine;
using System.Collections;
using Bounce.StateMachine;
using Bounce.CoroutineManager;

namespace Bounce
{
    /// <summary>
    /// Changes state is all shapes that can be consumed are consumed.
    /// </summary>
	public class ConsumptionShapesClearedReason : FSMReason 
	{
		private CM_Job m_MarkCompleteJob;
		private IConsumerListener m_Consumed;
		private bool m_ShouldTransition;

		public ConsumptionShapesClearedReason( IStateTransitioner controller, 
            IConsumerListener consumed, FSMStateID goToState )
			: base( FSMTransistion.AllConsumableShapesConsumed, goToState, controller )
		{
			m_Consumed = consumed;

			m_MarkCompleteJob = CM_Job.Make( MarkShouldTransition() ).Repeatable();
		}

		public override void Enter ()
		{
			m_ShouldTransition = false;
		}

		public override void Exit ()
		{
			m_MarkCompleteJob.Kill();
		}

		public override bool ChangeState ()
		{
			if( m_Consumed.AllConsumed () && !m_MarkCompleteJob.running )
			{
				m_MarkCompleteJob.Start();
			}

			if( m_ShouldTransition )
			{
				Debug.Log ("Should transition");
				PerformTransition();
				return true;
			}

			return false;
		}

		private IEnumerator MarkShouldTransition()
		{
			yield return new WaitForSeconds(2f);
			m_ShouldTransition = true;
		}
	}
}
