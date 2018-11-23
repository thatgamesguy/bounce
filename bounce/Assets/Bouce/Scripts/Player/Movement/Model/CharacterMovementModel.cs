using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Responsible for moving the player.
    /// </summary>
	[RequireComponent( typeof( Rigidbody2D ) )]
	public class CharacterMovementModel : MonoBehaviour, ICharacterMovementModel
	{
	    public float force;

	    private Rigidbody2D m_Rigidbody2D;

	    void Awake()
	    {
	        m_Rigidbody2D = GetComponent<Rigidbody2D>();
	    }

	    public void Place( Vector2 position )
	    {
	        DebugLog.instance.ApendLog("Showing at position: " + position);

	        transform.position = position;
	    }

	    public void SetDirection( Vector2 dir )
	    {
	        m_Rigidbody2D.AddForce( dir * force );
	    }

	    public void StopMovement()
	    {
	        m_Rigidbody2D.velocity = Vector2.zero;
	    }

		public bool IsMoving()
		{
			return m_Rigidbody2D.velocity != Vector2.zero;
		}

        public Vector2 GetVelocity()
        {
            return m_Rigidbody2D.velocity;
        }
	}
}