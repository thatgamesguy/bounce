using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Raised when a players status changes. Stores the current status.
    /// </summary>
	public class PlayerStatusChangeEvent : GameEvent, IPlayerStatus
	{
		public PlayerStatus currentPlayerStatus{ get; private set; }

		public PlayerStatusChangeEvent( PlayerStatus status )
		{
			currentPlayerStatus = status;
		}
	}
}