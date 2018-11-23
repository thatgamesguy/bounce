namespace Bounce
{
    /// <summary>
    /// Interface for any object that can update the menus model.
    /// </summary>
    public interface IMainMenuModel  
	{
		void BuildInfoScreen();
		void DestroyInfoScreen();

		void BuildLevelSelectScreen( IMainMenuControl menuControl );
		void DestroyLevelSelectionScreen();
	}
}
