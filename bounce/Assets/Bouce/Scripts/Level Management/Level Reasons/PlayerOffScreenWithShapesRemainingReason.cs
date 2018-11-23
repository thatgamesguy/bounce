using Bounce.StateMachine;

namespace Bounce
{
    /// <summary>
    /// Transitions to desingated state if player is off screen and there are either shapes remaining or no shapes remaining on screen.
    /// </summary>
	public class PlayerOffScreenWithShapesRemainingReason : FSMReason 
	{
		private PlayerStatusChangeListener m_StatusListener;
		private IConsumerListener m_Consumed;
		private bool m_ShouldShapesBeRemaining;

		public PlayerOffScreenWithShapesRemainingReason( FSMTransistion identifier, IStateTransitioner controller, 
			IConsumerListener consumed, bool shapesRemaining, FSMStateID goToState )
			: base( identifier, goToState, controller )
		{
			m_Consumed = consumed;
			m_ShouldShapesBeRemaining = shapesRemaining;
			m_StatusListener = new PlayerStatusChangeListener();
		}

		public override void Enter ()
		{
			m_StatusListener.StartListening();
		}

		public override void Exit ()
		{
			m_StatusListener.StopListening();
		}

		public override bool ChangeState ()
		{
			bool shapesConsumed = false;

			if(( m_ShouldShapesBeRemaining && !m_Consumed.AllConsumed() ) ||
			   ( !m_ShouldShapesBeRemaining && m_Consumed.AllConsumed() ) )
			{
				shapesConsumed = true;
			}

			if( m_StatusListener.MatchesStatus( PlayerStatus.OffScreen ) && shapesConsumed )
			{
				PerformTransition();
				return true;
			}

			return false;
		}
	}
}
