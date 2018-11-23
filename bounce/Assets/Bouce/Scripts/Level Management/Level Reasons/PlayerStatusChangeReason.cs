using Bounce.StateMachine;

namespace Bounce
{
    /// <summary>
    /// Changes state when players status matches desired status.
    /// </summary>
	public class PlayerStatusChangeReason : FSMReason
	{
		private PlayerStatus m_DesiredStatus;
		private PlayerStatusChangeListener m_StatusListener;

		public PlayerStatusChangeReason( IStateTransitioner controller, PlayerStatus desiredStatus, FSMStateID goToState ) 
			: base( FSMTransistion.PlayerStatusChanged, goToState, controller )
		{
			m_DesiredStatus = desiredStatus;
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

		public override bool ChangeState()
		{
			if( m_StatusListener.MatchesStatus( m_DesiredStatus ) )
			{
				PerformTransition();

				return true;
			}

			return false;
		}



	}
}
