using System.Collections.Generic;

namespace Bounce.CoroutineManager
{
	/// <summary>
	/// Arguements used in events raised by <see cref="CM_JobManager"/>.
	/// </summary>
	public class CM_JobManagerJobEditedEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the other arguments.
		/// </summary>
		/// <value>The other arguments.</value>
		public CM_JobManagerEventArgs otherArgs { get; private set; }

		/// <summary>
		/// Gets the job currently changed that caused the event to be raised.
		/// </summary>
		/// <value>The job edited.</value>
		public CM_Job jobEdited { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CM_JobManagerJobEditedEventArgs"/> class.
		/// </summary>
		/// <param name="ownedJobs">Owned jobs.</param>
		/// <param name="jobEdited">Job edited.</param>
		public CM_JobManagerJobEditedEventArgs (Dictionary<string, CM_Job> ownedJobs, CM_Job jobEdited)
		{
			this.otherArgs = new CM_JobManagerEventArgs (ownedJobs);
			this.jobEdited = jobEdited;
		}
	}
}