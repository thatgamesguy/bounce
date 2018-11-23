using System.Collections;
using System.Collections.Generic;

namespace Bounce.CoroutineManager
{
	/// <summary>
	/// Used to start coroutines in the main thread from seperate threads.
	/// </summary>
	public class CM_Dispatcher : Singleton<CM_Dispatcher>
	{
		private List<IEnumerator> _queue = new List<IEnumerator> ();

		/// <summary>
		/// Adds to execute queue. Coroutines in the queue are executed during the next timestep in the update method.
		/// </summary>
		/// <param name="job">Job.</param>
		public void AddToExecuteQueue( IEnumerator job )
		{
			_queue.Add( job );
		}

		void Update()
		{
			if ( _queue.Count > 0 ) {

				foreach( var j in _queue ) {
					StartCoroutine( j );
				}

				_queue.Clear();
			}
		}
	}
}