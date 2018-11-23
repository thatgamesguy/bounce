using UnityEngine;
using System;

namespace Bounce
{
    /// <summary>
    /// Listens for touch events.
    /// </summary>
    public class TouchControl : MonoBehaviour
    {
        /// <summary>
        /// Raised when touch event begins.
        /// </summary>
        public Action<TouchData> OnTouchBegan;

        /// <summary>
        /// Raised when touch event ends.
        /// </summary>
        public Action<TouchData> OnTouchEnded;

        /// <summary>
        /// Raised when a player moves finger across screen.
        /// </summary>
        public Action<TouchData> OnTouchMoved;

        private bool m_TouchInProgress;

        void Update()
        {
            if ( Input.GetMouseButtonDown(0) )
            {
                TouchBegan( Input.mousePosition );
            }
            else if (Input.GetMouseButtonUp(0))
            {
                TouchEnded( Input.mousePosition );
            }
            else if( Input.GetMouseButton(0) )
            {
                TouchMoved(Input.mousePosition);
            }

         

         /*   foreach (var touch in Input.touches)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        TouchBegan( touch.position );
                        break;
                    case TouchPhase.Ended:
                        TouchEnded( touch.position );
                        break;
                    case TouchPhase.Moved:
                        TouchMoved( touch.position );
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Canceled:
                        break;
                }
            }*/
        }

        public Vector2 CalculateDirection( TouchData from, TouchData to )
        {
            var heading = to.worldPosition - from.worldPosition;
            var distance = heading.magnitude;
            return heading / distance;
        }

        /// <summary>
        /// Calculates squared distance between two touch points world position.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public float CalculateWorldDistanceSquared( TouchData from, TouchData to )
        {
            return (to.worldPosition - from.worldPosition).sqrMagnitude;
        }

        /// <summary>
        /// Calculates distance between two touch points screen position.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public float CalculateDistanceSquared(TouchData from, TouchData to)
        {
            return (to.screenPosition - from.screenPosition).sqrMagnitude;
        }

        private void TouchBegan( Vector2 touchPos )
        {
            if ( m_TouchInProgress )
            {
                return;
            }

            m_TouchInProgress = true;

            if ( OnTouchBegan != null )
            {
                var data = new TouchData( TouchType.Began, touchPos );

                OnTouchBegan( data );
            }
        }

        private void TouchEnded( Vector2 touchPos )
        {
            if ( OnTouchEnded != null )
            {
                var data = new TouchData( TouchType.Ended, touchPos );

                OnTouchEnded( data );
            }

            m_TouchInProgress = false;
        }

        private void TouchMoved( Vector2 touchPos )
        {
            if( OnTouchMoved != null )
            {
                var data = new TouchData( TouchType.Moved, touchPos );

                OnTouchMoved( data );
            }
        }
    }

        public enum TouchType
    {
        Began,
        Moved,
        Ended
    }

    /// <summary>
    /// Encapsulates data sent when a touch event is raised.
    /// </summary>
    public class TouchData
    {
        public Vector2 screenPosition { get; private set; }
        public Vector2 worldPosition { get; private set; }
        public TouchType touchType { get; private set; }

        public TouchData( TouchType touchType, Vector2 touchPosition )
        {
            screenPosition = touchPosition;
            worldPosition = ScreenToWorldPosition( touchPosition );
            this.touchType = touchType;
        }

        private Vector2 ScreenToWorldPosition( Vector2 screen )
        {
            return Camera.main.ScreenToWorldPoint( screen );
        }
    }

}