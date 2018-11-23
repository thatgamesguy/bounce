using System.Collections.Generic;

namespace Bounce.CoroutineManager
{
	/// <summary>
	/// Arguements used in events raised by <see cref="CM_JobManager"/>.
	/// </summary>
	public class CM_JobManagerEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the owned jobs of the job manager at the time of the event being raised.
		/// </summary>
		/// <value>The owned jobs.</value>
		public CM_Job[] ownedJobs { get; private set; }

		/// <summary>
		/// Gets the currently running jobs of the job manager at the time of the event being raised.
		/// </summary>
		/// <value>The running jobs.</value>
		public CM_Job[] runningJobs { get; private set; }

		/// <summary>
		/// Gets the paused jobs of the job manager at the time of the event being raised.
		/// </summary>
		/// <value>The paused jobs.</value>
		public CM_Job[] pausedJobs { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CM_JobManagerEventArgs"/> class.
		/// </summary>
		/// <param name="ownedJobs">Owned jobs.</param>
		public CM_JobManagerEventArgs (Dictionary<string, CM_Job> ownedJobs)
		{
			this.ownedJobs = new CM_Job[ownedJobs.Values.Count];
			ownedJobs.Values.CopyTo (this.ownedJobs, 0);

			var runningJobs = new List<CM_Job> ();
			var pausedJobs = new List<CM_Job> ();

			foreach (var j in this.ownedJobs) {
				if (j.running) {
					runningJobs.Add (j);
				} else if (j.paused) {
					pausedJobs.Add (j);
				}
			}

			this.runningJobs = runningJobs.ToArray ();
			this.pausedJobs = pausedJobs.ToArray ();
		}
	}
}