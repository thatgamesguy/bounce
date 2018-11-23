using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Attached to the level selection buttons. Loads associated level on button press.
    /// </summary>
	public class LevelSelectionButton : MonoBehaviour 
	{
		public Text textContainer;

		public Color selectableFontColour;
		public Color completeTextColour;
		public Color notSelectableColour;

		private ILevelManager m_LevelManager;
		private IMainMenuControl m_MenuControl;
		private Level m_Level;
		private bool m_Initialised;

		private Button m_Button;

		void Awake()
		{
			m_Button = GetComponent<Button>();
		}

        /// <summary>
        /// Initialises button.
        /// </summary>
        /// <param name="mainMenuControl"></param>
        /// <param name="levelManager"></param>
        /// <param name="level"></param>
		public void Initialise( IMainMenuControl mainMenuControl, ILevelManager levelManager, Level level )
		{
			m_LevelManager = levelManager;
			m_Level = level;
			m_MenuControl = mainMenuControl;

			SetTextToLevelID();

			SetTextColour();

			if( ShouldLevelBeSelectable() )
			{
				EnableButtonInteraction();
			}
			else
			{
				DisableButtonInteraction();
			}		

			m_Initialised = true;
		}

        /// <summary>
        /// Loads level.
        /// </summary>
		public void OnButtonPressed()
		{
			if( !m_Initialised )
			{
				Debug.LogError( "Button not initialised" );
				return;
			}

	
			LoadLevel();
			HideMenu();
		}

		private void LoadLevel()
		{
			m_LevelManager.LoadLevel( m_Level.levelId - 1 );
		}

		private void HideMenu()
		{
            m_MenuControl.BlockNextOpenAttempt();
			m_MenuControl.GetView().HideAdditional();
		}

		private void DisableButtonInteraction()
		{
			m_Button.interactable = false;
		}

		private void EnableButtonInteraction()
		{
			m_Button.interactable = true;
		}

		private void SetTextToLevelID()
		{
			textContainer.text = m_Level.levelId.ToString( "D3" );
		}

		private void SetTextColour()
		{
			var colour = notSelectableColour;
			
			if( IsLevelCompleted() )
			{
				colour = completeTextColour;
			}
			else if( IsLevelCurrentLevelInProgress() )
			{
				colour = selectableFontColour;
			}

			textContainer.color = colour;
		}

		private bool ShouldLevelBeSelectable()
		{
			return IsLevelCompleted() || IsLevelCurrentLevelInProgress();
			
		}

		private bool IsLevelCurrentLevelInProgress()
		{
            if( m_Level.levelId == 1 )
            {
                return true;
            }

            if( m_Level.levelId == 2 && !m_LevelManager.IsLevelComplete(1) )
            {
                return false;
            }

           return m_Level.levelId <= m_LevelManager.GetHighestCompletedLevelID() + 1;
		}

		private bool IsLevelCompleted()
		{
			return m_Level.completed;
		}
	}
}