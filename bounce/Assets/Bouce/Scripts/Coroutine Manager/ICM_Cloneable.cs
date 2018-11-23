namespace Bounce.CoroutineManager
{
	/// <summary>
	/// An interface for all classes used by the coroutine manager that can be cloned.
	/// </summary>
	public interface ICM_Cloneable<T>
	{
		/// <summary>
		/// Clone this instance.
		/// </summary>
		T Clone ();

		/// <summary>
		/// Clone the specified numOfCopies.
		/// </summary>
		/// <param name="numOfCopies">Number of copies.</param>
		T[] Clone (int numOfCopies);
	}
}