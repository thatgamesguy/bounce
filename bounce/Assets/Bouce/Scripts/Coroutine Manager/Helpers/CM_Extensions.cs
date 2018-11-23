using System.Reflection;
using System.Collections;

namespace Bounce.CoroutineManager
{
	/// <summary>
	/// A number of extensions used by the Coroutine Manager.
	/// </summary>
	public static class CM_Extensions
	{
		/// <summary>
		/// Determines if string is null or empty.
		/// </summary>
		/// <returns><c>true</c> if is null or empty; otherwise, <c>false</c>.</returns>
		/// <param name="s">S.</param>
		public static bool IsNullOrEmpty (this string s)
		{
			return s == null || s.Equals ("");
		}

		/// <summary>
		/// Clone the specified IEnumerator.
		/// </summary>
		/// <param name="source">Source.</param>
		public static IEnumerator Clone (this IEnumerator source)
		{
			var sourceType = source.GetType ().UnderlyingSystemType;
			var sourceTypeConstructor = sourceType.GetConstructors () [0]; 
			var newInstance = sourceTypeConstructor.Invoke (null) as IEnumerator;
		
			var nonPublicFields = source.GetType ().GetFields (BindingFlags.NonPublic | BindingFlags.Instance);
			var publicFields = source.GetType ().GetFields (BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in nonPublicFields) {
				var value = field.GetValue (source);
				field.SetValue (newInstance, value);
			}
			foreach (var field in publicFields) {
				var value = field.GetValue (source);
				field.SetValue (newInstance, value);
			}
		
			return newInstance;
		}
	}
}