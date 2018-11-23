namespace Bounce
{
	public enum PlayerStatus
	{
		None = 0,
		Fired = 100,
		OffScreen = 200,
		Placed = 300
	}

    /// <summary>
    /// Implement by any object that will be responsible for storing the players burrent status.
    /// </summary>
    public interface IPlayerStatus  
	{
		PlayerStatus currentPlayerStatus{ get; }
	}
}
