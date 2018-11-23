using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bounce.StateMachine;

namespace Bounce
{
    /// <summary>
    /// Disables all non consumable shapes and calles LevelManager::OnLevelComplete
    /// </summary>
	public class LevelCompleteAction : FSMAction 
	{
		private ILevelManager m_LevelManager;
		private NonConsumableShapeControl[] m_NonConsumableShapes;
		
		public LevelCompleteAction( ILevelManager levelManager, 
			NonConsumableShapeControl[] nonConsumableShapes )
		{
			m_LevelManager = levelManager;

			m_NonConsumableShapes = nonConsumableShapes;
		}

		public override void Enter ()
		{
			if(m_NonConsumableShapes != null ) 
			{
				foreach ( NonConsumableShapeControl shape in m_NonConsumableShapes ) 
				{
					shape.Disable();
				}
			}

			m_LevelManager.OnLevelComplete();
		}

		public override void Exit ()
		{	
		}

		public override void PerformAction ()
		{
		}
	}
}
