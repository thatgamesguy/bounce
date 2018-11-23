namespace Bounce.CoroutineManager
{
	/// <summary>
	/// Arguements used by events raised by <see cref="CM_JobQueue"/>.
	/// </summary>
	public class CM_QueueEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_QueueEventArgs"/> has jobs in queue.
		/// </summary>
		/// <value><c>true</c> if has jobs in queue; otherwise, <c>false</c>.</value>
		public bool hasJobsInQueue { get { return queuedJobs != null && queuedJobs.Length > 0; } }

		/// <summary>
		/// Gets the queued jobs.
		/// </summary>
		/// <value>The queued jobs.</value>
		public CM_Job[] queuedJobs { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_QueueEventArgs"/> has completed jobs.
		/// </summary>
		/// <value><c>true</c> if has completed jobs; otherwise, <c>false</c>.</value>
		public bool hasCompletedJobs { get { return completedJobs != null && completedJobs.Length > 0; } }

		/// <summary>
		/// Gets the completed jobs.
		/// </summary>
		/// <value>The completed jobs.</value>
		public CM_Job[] completedJobs { get; private set; }

		public CM_JobQueue jobQueue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CM_QueueEventArgs"/> class.
		/// </summary>
		/// <param name="queuedJobs">Queued jobs.</param>
		/// <param name="completedJobs">Completed jobs.</param>
		public CM_QueueEventArgs (CM_Job[] queuedJobs, CM_Job[] completedJobs, CM_JobQueue jobQueue)
		{
			this.queuedJobs = queuedJobs;
			this.completedJobs = completedJobs;
			this.jobQueue = jobQueue;
		}
	}
}