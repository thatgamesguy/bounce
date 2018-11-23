namespace Bounce.StateMachine
{
    /// <summary>
    /// Responsible for logging FSM transitions for debug purposes.
    /// </summary>
	public class FSMStateSwitchLogger
	{
		private string _stateSwitchMessage;

		public FSMStateSwitchLogger ( FSMReason _state )
		{
			_stateSwitchMessage = _state + ": switching state to: " + _state.goToState;
		}

		public void Log ()
		{
		    DebugLog.instance.ApendLog( _stateSwitchMessage );
		}
	}
}