using UnityEngine;
using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Provides touch controls for player.
    /// </summary>
	public class CharacterTouchControl : CharacterControl
	{
        /// <summary>
        /// The minimum distance between start and end points for it to count as a drag.
        /// </summary>
        public float minTouchDistance = 1f;

        private TouchControl m_TouchControl;
        private TouchData m_TouchStartPoint;
        private TouchData m_TouchEndPoint;
        private ICharacterLineView m_LineView;

        protected override void Awake()
        {
            base.Awake();

            m_LineView = GetComponent<ICharacterLineView>();

            m_TouchControl = GetComponent<TouchControl>();

            if( m_TouchControl == null )
            {
                m_TouchControl = gameObject.AddComponent<TouchControl>();
            }

            FindObjectOfType<LevelManager>().OnNewLevelLoaded += ListenForTouchEvents;
        }
	
		void OnEnable()
		{
			ListenForTouchEvents();

			Events.instance.AddListener<MenuStatusChangedEvent>( OnMenuStatusChanged );
			Events.instance.AddListener<AllShapesConsumedEvent>( OnAllShapesConsumed );
		}

		void OnDisable()
		{
			StopListeningForTouchEvents();

			Events.instance.RemoveListener<MenuStatusChangedEvent>( OnMenuStatusChanged );
			Events.instance.AddListener<AllShapesConsumedEvent>( OnAllShapesConsumed );
		}

		private void OnAllShapesConsumed( AllShapesConsumedEvent e )
		{
			StopListeningForTouchEvents();
		}

		private void OnMenuStatusChanged( MenuStatusChangedEvent e )
		{
			if( !e.isVisible )
			{
				ListenForTouchEvents();
			}
			else 
			{
				StopListeningForTouchEvents();

				Hide();
			}
		}

		private void ListenForTouchEvents()
		{
			m_TouchControl.OnTouchBegan += OnTouchBegan;
			m_TouchControl.OnTouchEnded += OnTouchEnded;
            m_TouchControl.OnTouchMoved += OnTouchMoved;
       }

		private void StopListeningForTouchEvents()
		{
			m_TouchControl.OnTouchBegan -= OnTouchBegan;
			m_TouchControl.OnTouchEnded -= OnTouchEnded;
            m_TouchControl.OnTouchMoved -= OnTouchMoved;
        }

	    private void OnTouchBegan( TouchData touchData )
	    {
            if ( touchData.touchType != TouchType.Began )
            {
                Debug.LogWarning( "Incorrect data provided" );
                return;
            }
   
            m_TouchStartPoint = touchData;

            SetLocation( m_TouchStartPoint.worldPosition );
			RaisePlayerStatusChangeEvent( PlayerStatus.Placed );

            if( m_LineView != null )
            {
                m_LineView.SetStart( m_TouchStartPoint.worldPosition );
            }
	    }

	    private void OnTouchEnded( TouchData touchData )
	    {
            if( touchData.touchType != TouchType.Ended )
            {
                Debug.LogWarning( "Incorrect data provided" );
                return;
            }

            m_TouchEndPoint = touchData;

            float distance = m_TouchControl.CalculateWorldDistanceSquared( m_TouchEndPoint, m_TouchStartPoint );

			DebugLog.instance.ApendLog( "Distance sqr: " + distance );

	        if( distance > Mathf.Pow( minTouchDistance, 2 ) )
	        {
                Vector2 direction = m_TouchControl.CalculateDirection( m_TouchStartPoint, m_TouchEndPoint );
				RaisePlayerStatusChangeEvent( PlayerStatus.Fired );
                SetDirection( direction );

	        }
	        else
	        {
	            Hide();
	        }

            if ( m_LineView != null)
            {
                m_LineView.Hide();
            }
        }

        private void OnTouchMoved( TouchData touchData )
        {
            DebugLog.instance.ApendLog("Touch Moved");

            if ( touchData.touchType != TouchType.Moved )
            {
                Debug.LogWarning( "Incorrect data provided" );
                return;
            }

            if ( m_LineView != null )
            {
				m_LineView.SetEnd( touchData.worldPosition );
                m_LineView.Show();
            }
        }
	
	}
}