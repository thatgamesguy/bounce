namespace Bounce
{
    /// <summary>
    /// Interface for any object that can update the menus view.
    /// </summary>
    public interface IMainMenuView 
	{		
		void Show();
		void Hide();

		void ShowAdditional();
		void HideAdditional();

		bool IsVisible();
		bool IsAdditionalVisible();
	}
}