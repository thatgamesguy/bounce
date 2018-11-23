namespace Bounce
{
    /// <summary>
    /// Implemented by any class that interacts with shapes.
    /// </summary>
	public interface ICharacterInteractionModel
	{
        void EnableInteraction( ICharacterMovementModel movementModel );
        void DisableInteraction();
	}
}