using System;

namespace Bounce
{
    /// <summary>
    /// Responsible for interaction with player.
    /// </summary>
    public class NonConsumableShapeControl : ShapeControl, IInteractable
    {
        private Action<NonConsumableShapeControl> OnHitListener;

        /// <summary>
        /// Invoked when player collides with this shape.
        /// </summary>
        public void Interact()
        {
            if ( OnHitListener != null )
            {
                OnHitListener( this );
            }

            OnHit();
        }

        public void OnAnimationStarted()
        {
            OnHitAnimationStarted();
        }

        public void OnAnimationFinished()
        {
            OnHitAnimationFinished();
        }

        public void AddListener( Action<NonConsumableShapeControl> listener )
        {
            OnHitListener = listener;
        }
    }
}