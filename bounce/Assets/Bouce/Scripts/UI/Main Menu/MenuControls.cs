using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Controls the menu view and model. Listens for touch events and updates the menu status accordingly.
    /// </summary>
    [RequireComponent( typeof( TouchControl ) )]
	public class MenuControls : MonoBehaviour, IMainMenuControl
    {
        public float maxDistanceBetweenTouch = 0.2f;

		private static readonly float MENU_SHOWN_Y_TOUCH_OFFSET = 0.145f;
		private static readonly float ADDITIONAL_MENU_SHOWN_Y_TOUCH_OFFSET = 0.76f;

        private TouchControl m_TouchControl;
        private TouchData m_StartPoint;
        private TouchData m_EndPoint;

		private IMainMenuView m_MainMenuView;
		private IMainMenuModel m_MainMenuModel;

        private bool m_ShouldBlockNextOpenAttempt;

        void Awake()
        {
            m_TouchControl = GetComponent<TouchControl>();
			m_MainMenuView = GetComponent<MainMenuView>();
			m_MainMenuModel = GetComponent<MainMenuModel>();
        }

		void OnEnable()
		{
			m_TouchControl.OnTouchBegan += OnTouchBegan;
			m_TouchControl.OnTouchEnded += OnTouchEnded;
		}

		void OnDisable()
		{
			m_TouchControl.OnTouchBegan -= OnTouchBegan;
			m_TouchControl.OnTouchEnded -= OnTouchEnded;
		}

		public IMainMenuView GetView()
		{
			return m_MainMenuView;
		}

		public IMainMenuModel GetModel()
		{
			return m_MainMenuModel;
		}

        public void BlockNextOpenAttempt()
        {
            m_ShouldBlockNextOpenAttempt = true;
        }

        public void RequestShowInfoScreen()
		{
			if( m_MainMenuModel != null )
			{
				m_MainMenuModel.DestroyLevelSelectionScreen();
				m_MainMenuModel.BuildInfoScreen();
			}

			if( m_MainMenuView != null )
			{
				if( !m_MainMenuView.IsAdditionalVisible() )
				{
					m_MainMenuView.ShowAdditional();
				}
			}
		}

		public void RequestShowLevelSelectScreen()
		{
			if( m_MainMenuModel != null )
			{
				m_MainMenuModel.DestroyInfoScreen();
				m_MainMenuModel.BuildLevelSelectScreen( this );
			}

			if( m_MainMenuView != null )
			{
				if( !m_MainMenuView.IsAdditionalVisible() )
				{
					m_MainMenuView.ShowAdditional();
				}
			}
		}
			
        private void OnTouchBegan( TouchData touchData )
        {
            if ( touchData.touchType != TouchType.Began )
            {
                Debug.LogWarning( "Incorrect data provided" );
                return;
            }

            m_StartPoint = touchData;
        }

        private void OnTouchEnded( TouchData touchData )
        {
            if ( touchData.touchType != TouchType.Ended )
            {
                Debug.LogWarning( "Incorrect data provided" );
                return;
            }

			DebugLog.instance.ApendLog ("Screen position norm y: " + (touchData.screenPosition.y / Screen.height));

			m_EndPoint = touchData;

            var distance = m_TouchControl.CalculateDistanceSquared( m_StartPoint, m_EndPoint );

            if( distance <= Mathf.Pow( maxDistanceBetweenTouch, 2 ) )
            {
				if( m_ShouldBlockNextOpenAttempt )
                {
                    m_ShouldBlockNextOpenAttempt = false;
                    return;
                }
				
				if( m_MainMenuView != null )
				{
					bool isVisible = m_MainMenuView.IsVisible();
					bool isAdditionalVisible = m_MainMenuView.IsAdditionalVisible();

					if( isVisible && !isAdditionalVisible && 
						( touchData.screenPosition.y / Screen.height ) > MENU_SHOWN_Y_TOUCH_OFFSET )
					{
						m_MainMenuView.Hide();
					}
					else if( isVisible && isAdditionalVisible && 
						( touchData.screenPosition.y / Screen.height ) > ADDITIONAL_MENU_SHOWN_Y_TOUCH_OFFSET )
					{
						m_MainMenuView.HideAdditional();
					}
					else if( !isVisible )
					{
						m_MainMenuView.Show();
					}

						
				}
					

            }
        }

      
    }
}