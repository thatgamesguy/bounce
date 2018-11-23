namespace Bounce
{
    /// <summary>
    /// Impmented by any object that will be resposible for managing levels.
    /// </summary>
    public interface ILevelManager
    {
        void OnLevelComplete();
        void LoadLevel(int levelId);
        int GetHighestCompletedLevelID();
        bool IsLevelComplete(int levelId);
    }
}
