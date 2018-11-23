namespace Bounce.CoroutineManager
{
	/// <summary>
	/// Arguements used in events raised by <see cref="CM_Job"/>.
	/// </summary>
	public class CM_JobEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets the current job.
		/// </summary>
		/// <value>The job.</value>
		public CM_Job job { get; private set; }

		/// <summary>
		/// Gets the child jobs (if any).
		/// </summary>
		/// <value>The child jobs.</value>
		public CM_Job[] childJobs { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_JobEventArgs"/> has child jobs.
		/// </summary>
		/// <value><c>true</c> if has child jobs; otherwise, <c>false</c>.</value>
		public bool hasChildJobs { get { return childJobs != null && childJobs.Length > 0; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="CM_JobEventArgs"/> class.
		/// </summary>
		/// <param name="job">Job.</param>
		/// <param name="childJobs">Child jobs.</param>
		public CM_JobEventArgs (CM_Job job, CM_Job[] childJobs)
		{
			this.job = job;
	
			this.childJobs = (childJobs != null) ? childJobs : new CM_Job[0];
		}
	
	}
}