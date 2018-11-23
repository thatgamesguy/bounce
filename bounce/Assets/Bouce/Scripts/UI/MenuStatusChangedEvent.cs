using Bounce.EventManagement;

namespace Bounce
{
	/// <summary>
    /// Raised when the menus status changes. Used to disable/enable gameplay.
    /// </summary>
	public class MenuStatusChangedEvent : GameEvent 
	{
        /// <summary>
        /// Returns true if the menu is visible.
        /// </summary>
		public bool isVisible { get; private set; }

        /// <summary>
        /// Returns true if the additional menu is visible.
        /// </summary>
		public bool isAdditionalVisible { get; private set; }

		public MenuStatusChangedEvent( bool isVisible, bool isAdditionalVisible )
		{
			this.isVisible = isVisible;
			this.isAdditionalVisible = isAdditionalVisible;
		}
	}
}
