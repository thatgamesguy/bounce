namespace Bounce
{
    /// <summary>
    /// Interface for main menu control. Implement in any class that should update that main menu view and model.
    /// </summary>
	public interface IMainMenuControl 
	{
		IMainMenuView GetView();
		IMainMenuModel GetModel();

        void BlockNextOpenAttempt();

        void RequestShowInfoScreen();
		void RequestShowLevelSelectScreen();
	}
}