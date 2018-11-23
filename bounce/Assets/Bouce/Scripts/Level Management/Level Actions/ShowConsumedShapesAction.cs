using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bounce.CoroutineManager;
using Bounce.StateMachine;

namespace Bounce
{
    /// <summary>
    /// Shows all shapes that were previously consumed.
    /// </summary>
	public class ShowConsumedShapesAction : FSMAction
	{
        private static readonly float DELAY_BETWEEN_SHAPES_SHOWN = 0.15f;

		private IConsumerListener m_Consumables;
		private CM_Job m_ShowShapesJob;

        public ShowConsumedShapesAction( IConsumerListener consumables )
        {
            m_Consumables = consumables;
            m_ShowShapesJob = CM_Job.Make( ShowShapes() ).Repeatable();
        }

		public override void Enter()
		{
			m_ShowShapesJob.Start();
		}

		public override void Exit()
		{

		}

		public override void PerformAction()
		{

		}

		private IEnumerator ShowShapes( )
		{
			var wait = new WaitForSeconds( DELAY_BETWEEN_SHAPES_SHOWN );

			List<ConsumableShapeControl> consumedShapes = new List<ConsumableShapeControl>( m_Consumables.m_ConsumedShapes );
			m_Consumables.ClearConsumedShapes();

			for( int i = 0; i < consumedShapes.Count; i++ )
			{
				consumedShapes[ i ].Reset();
				yield return wait;
			}

	
		}

	}
}