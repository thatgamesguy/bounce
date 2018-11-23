using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bounce.StateMachine;
using Bounce.CoroutineManager;

namespace Bounce
{
    /// <summary>
    /// Shows all shapes.
    /// </summary>
	public class ShowShapesAction : FSMAction
	{
        private static readonly float DELAY_BETWEEN_SHAPES_SHOWN = 0.1f;

        private List<ShapeControl> m_ConsumableShapes;
		private CM_Job m_ShowShapesJob;

	    public ShowShapesAction( List<ShapeControl> shapes)
	    {
			m_ConsumableShapes = shapes;
			m_ShowShapesJob = CM_Job.Make( ShowShapes() ).Repeatable().Start();
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

		private IEnumerator ShowShapes()
		{
			var wait = new WaitForSeconds( DELAY_BETWEEN_SHAPES_SHOWN );
			
			for( int i = 0; i < m_ConsumableShapes.Count; i++ )
			{
				m_ConsumableShapes[ i ].Reset ();
				yield return wait;
			}
		}
	}
}
