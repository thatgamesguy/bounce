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

		void OnCollisionEnter2D( Collision2D other )
		{
			if (m_MovementModel != null) 
			{
				var localVel = transform.InverseTransformDirection( m_MovementModel.GetVelocity() );

                Vector3[] dirs = new Vector3[9];

				dirs[0] = ((Vector3)localVel - transform.position).normalized;
				dirs[1] = Quaternion.AngleAxis(20.0f, Vector3.forward) * dirs[0];
				dirs[2] = Quaternion.AngleAxis(-20.0f, Vector3.forward) * dirs[0];
				dirs[3] = Quaternion.AngleAxis(40.0f, Vector3.forward) * dirs[0];
				dirs[4] = Quaternion.AngleAxis(-40.0f, Vector3.forward) * dirs[0];
				dirs[5] = Quaternion.AngleAxis(60.0f, Vector3.forward) * dirs[0];
				dirs[6] = Quaternion.AngleAxis(-60.0f, Vector3.forward) * dirs[0];
				dirs[7] = Quaternion.AngleAxis(70.0f, Vector3.forward) * dirs[0];
				dirs[8] = Quaternion.AngleAxis(-70.0f, Vector3.forward) * dirs[0];

                foreach (var dir in dirs)
                {
                    var hit = Physics2D.Raycast(transform.position, dir, 1.5f, reflectMask);

                    if (hit.collider != null)
                    {
                        m_Collision = other.gameObject;
                        break;
                    }
                }
			}


		}

		void OnCollisionExit2D( Collision2D other )
		{

			if( m_Collision != null && other.gameObject.GetInstanceID() == m_Collision.GetInstanceID () )
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
