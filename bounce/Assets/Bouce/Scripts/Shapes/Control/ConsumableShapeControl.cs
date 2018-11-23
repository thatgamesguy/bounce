using System;

namespace Bounce
{
    /// <summary>
    /// Responsible for interaction with player.
    /// </summary>
	public class ConsumableShapeControl : ShapeControl, IInteractable
	{
		private Action<ConsumableShapeControl> OnHitListener;
		
        /// <summary>
        /// Invoked when player collides with this shape.
        /// </summary>
	    public void Interact()
	    {
			if( OnHitListener != null )
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

		public void AddListener( Action<ConsumableShapeControl> listener )
		{
			OnHitListener = listener;
		}

        public void RemoveAllListeners()
        {
            OnHitListener = null;
        }
	}
}
