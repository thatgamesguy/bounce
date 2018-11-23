namespace Bounce
{
    /// <summary>
    /// Interface for any object that can interact with the player.
    /// </summary>
    public interface IInteractable 
    {
        void Interact();
        void OnAnimationStarted();
        void OnAnimationFinished();
    }
}