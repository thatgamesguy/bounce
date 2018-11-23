using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Stores and updates the main menu model. Destroys and builds info and level selection models.
    /// </summary>
	public class MainMenuModel : MonoBehaviour, IMainMenuModel
	{
		public Transform levelSelectMenu;
		public Transform infoScreenMenu;

		public LevelManager levelManager;
		public GameObject buttonPrefab;

		private LevelSelectionButton[] m_Buttons;

		void Start()
		{
			m_Buttons = new LevelSelectionButton[ levelManager.levels.Length ];

			DestroyInfoScreen();
			DestroyLevelSelectionScreen();
		}

		public void BuildInfoScreen()
		{
			infoScreenMenu.gameObject.SetActive( true );
		}

		public void DestroyInfoScreen()
		{
			infoScreenMenu.gameObject.SetActive( false );
		}

        /// <summary>
        /// Creates a button for each level and adds to the level selection menu.
        /// </summary>
        /// <param name="menuControl">An object that can load a new level. Used when a level selection button is pressed.</param>
		public void BuildLevelSelectScreen( IMainMenuControl menuControl )
		{
			levelSelectMenu.gameObject.SetActive( true );
			
			var levels = levelManager.levels;

			for( int i = 0; i < levels.Length; i++ )
			{
				LevelSelectionButton button = null;
				
				if( m_Buttons[ i ] == null )
				{
					button = Instantiate( buttonPrefab ).GetComponent<LevelSelectionButton>();
					m_Buttons[ i ] = button;
				}
				else
				{
					button = m_Buttons[ i ];
					button.gameObject.SetActive( true );
				}


				button.Initialise(  menuControl, levelManager, levels[ i ] );
				button.transform.SetParent( levelSelectMenu, false );
			}
		}

		public void DestroyLevelSelectionScreen()
		{
			levelSelectMenu.gameObject.SetActive( false );

			for( int i = 0; i < m_Buttons.Length; i++ )
			{
				if( m_Buttons[ i ] == null )
				{
					break;
				}

				m_Buttons[ i ].gameObject.SetActive( false );
			}
		}

	}
}