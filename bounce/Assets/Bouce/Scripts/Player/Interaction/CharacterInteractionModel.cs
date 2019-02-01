using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Responsible for interacting with shapes.
    /// </summary>
	public class CharacterInteractionModel : MonoBehaviour, ICharacterInteractionModel
	{
		public LayerMask reflectMask;

		private bool m_InteractionEnabled;
	    private ICharacterMovementModel m_MovementModel;
		private GameObject m_Collision;
        private Vector2 m_PreviousVelocity;

        /// <summary>
        /// Enables interaction with shapes.
        /// </summary>
        /// <param name="movementModel"></param>
		public void EnableInteraction( ICharacterMovementModel movementModel )
		{
			this.m_MovementModel = movementModel;    
			m_InteractionEnabled = true;
		}

		/// <summary>
        /// Disables interaction with shapes.
        /// </summary>
		public void DisableInteraction()
		{
			m_InteractionEnabled = false;
		}
			
		private IEnumerator DoInteractionAfterNFrames( int frames, IInteractable interactable )
		{
			int i = 0;

			while( i++ < frames )
			{
				yield return new WaitForEndOfFrame();
			}

			interactable.Interact();
		}

        void FixedUpdate()
        {
            if (m_MovementModel != null)
            {
                m_PreviousVelocity = m_MovementModel.GetVelocity();
            }
        }

        void OnCollisionEnter2D( Collision2D other )
		{
			if (m_MovementModel != null) 
			{
                Vector2 vec1 = m_PreviousVelocity;
                Vector2 vec2 = m_MovementModel.GetVelocity();
                float angle = Vector2.Angle(vec1, vec2);

                if(Mathf.Abs(angle) > 2f)
                {
                    m_Collision = other.gameObject;
                }

            }


		}

		void OnCollisionExit2D( Collision2D other )
		{
            if ( m_Collision != null && other.gameObject.GetInstanceID() == m_Collision.GetInstanceID () )
			{
				m_Collision = null;

				if( ShouldInteract() )
				{
                    InteractIfPossible( other.gameObject );
				}
			}
		}

	    void OnTriggerEnter2D( Collider2D other )
		{
			if ( ShouldInteract() ) 
			{
				InteractIfPossible( other.gameObject );
			}
		}

	   	private bool ShouldInteract()
	   	{
	   		return m_InteractionEnabled;
	  	}

	  	private void InteractIfPossible( GameObject other )
		{
			var interactable = other.GetComponent<IInteractable> ();

			if ( interactable != null ) 
			{
				StartCoroutine( DoInteractionAfterNFrames( 4, interactable ) );
			}
		}
				
	}
}
