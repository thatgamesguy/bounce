using System;
using Bounce.StateMachine;

namespace Bounce
{
    /// <summary>
    /// Provides easy way to transition between states regardles of other criteria.
    /// </summary>
	public class LevelEntryReason : FSMReason
	{
		private bool m_ShouldTransition = true;

		public LevelEntryReason( FSMTransistion identifier, IStateTransitioner controller, bool shouldTransition, FSMStateID goToState ) 
			: base( identifier, goToState, controller )
	    {
			m_ShouldTransition = shouldTransition;
	    }

		public override void Enter ()
		{
			m_ShouldTransition = false;
		}

	    public override bool ChangeState()
	    {
	        if( m_ShouldTransition )
	        {
	            PerformTransition();

	            return true;
	        }

	        return false;
	    }
	}
}

