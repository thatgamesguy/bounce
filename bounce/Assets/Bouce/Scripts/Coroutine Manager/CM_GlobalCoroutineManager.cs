using System.Collections;

namespace Bounce.CoroutineManager
{
	/// <summary>
	/// The base class of Global Coroutine Managers. Provides access to a singular global copy of the class.
	/// </summary>
	public abstract class CM_GlobalCoroutineManager<T> where T : new()
	{
		private static int id = 0;

		private static int idIncrement { get { return ++id; } }

		private string uniqueGeneratedID { get { return "auto_generated_id: " + idIncrement.ToString (); } }

		private static T _global;

		/// <summary>
		/// Access to a global instance.
		/// </summary>
		public static T Global {
			get {
				if (_global == null) {
					_global = new T ();
				}
			
				return _global;
			}
		}

		protected virtual CM_Job MakeJob (string id, IEnumerator routine, bool addListenerToOnComplete = true)
		{
			var job = CM_Job.Make (routine);
			job.id = (id.IsNullOrEmpty ()) ? uniqueGeneratedID : id;

			if (addListenerToOnComplete) {
				job.NotifyOnJobComplete (HandlejobComplete);
			}
		
			return job;
		}

		protected virtual CM_Job MakeJob (CM_Job job)
		{
			job.NotifyOnJobComplete (HandlejobComplete);
		
			return job;
		}

		protected void AutoGenerateJobId (CM_Job job)
		{
			job.id = uniqueGeneratedID;
		}

		protected abstract void HandlejobComplete (object sender, CM_JobEventArgs e);
	}
}