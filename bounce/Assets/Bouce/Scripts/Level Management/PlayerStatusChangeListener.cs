using UnityEngine;
using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Listens for PlayerStatusChangeEvent 
    /// </summary>
	public class PlayerStatusChangeListener  
	{
		private PlayerStatus m_PlayerStatus;
		private bool m_Listening;

		public void StartListening()
		{
			Events.instance.AddListener<PlayerStatusChangeEvent>( OnStatusChange );
			m_Listening = true;
		}

		public void StopListening()
		{
			Events.instance.RemoveListener<PlayerStatusChangeEvent>( OnStatusChange );
			m_Listening = false;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns>Returns true if the players current status matches parameter</returns>
		public bool MatchesStatus( PlayerStatus status )
		{
			if( !m_Listening )
			{
				Debug.LogWarning( "Status listener not activated" );
			}
			
			if ( m_PlayerStatus == status )
			{
				m_PlayerStatus = PlayerStatus.None;
				return true;
			}

			return false;
		}

		private void OnStatusChange( PlayerStatusChangeEvent e )
		{
			m_PlayerStatus = e.currentPlayerStatus;
		}

	}
}
