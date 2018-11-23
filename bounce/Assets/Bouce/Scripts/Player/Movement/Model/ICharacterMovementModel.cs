using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Implement by any object that will be responsible for player movement.
    /// </summary>
	public interface ICharacterMovementModel
	{
	    void Place( Vector2 position );
	    void SetDirection( Vector2 dir );
	    void StopMovement();
		bool IsMoving();
        Vector2 GetVelocity();
	}
}