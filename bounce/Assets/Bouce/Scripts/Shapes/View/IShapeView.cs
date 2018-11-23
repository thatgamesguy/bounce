namespace Bounce
{
    /// <summary>
    /// Implement by any clas that is responsible for updating the shapes view.
    /// </summary>
    public interface IShapeView 
	{
	    void Show();
	    void Hide();

		void PlayHitAnimation();
		void PlayShowAnimation();
		void SetInteractable( bool interactable );
	}
}
