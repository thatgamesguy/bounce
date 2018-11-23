using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bounce.CoroutineManager
{
	/// <summary>
	/// The main job manager class. Encapsulates the behaviour for global and local job managers.
	/// Provides access to events and public access to stored jobs.
	/// </summary>
	public class CM_JobManager : CM_GlobalCoroutineManager<CM_JobManager>
	{
		#region PublicProperties

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_JobManager"/> is empty.
		/// </summary>
		/// <value><c>true</c> if is empty; otherwise, <c>false</c>.</value>
		public bool isEmpty{ get { return _ownedJobs.Count == 0; } }

		#endregion

		#region Events

		/// <summary>
		/// Raised when job added.
		/// </summary>
		protected event EventHandler<CM_JobManagerJobEditedEventArgs> jobAdded;

		/// <summary>
		/// Raised when job removed.
		/// </summary>
		protected event EventHandler<CM_JobManagerJobEditedEventArgs> jobRemoved;

		/// <summary>
		/// Raised when all jobs killed.
		/// </summary>
		protected event EventHandler<CM_JobManagerEventArgs> allJobsKilled;

		/// <summary>
		/// Raised when all jobs resumed.
		/// </summary>
		protected event EventHandler<CM_JobManagerEventArgs> allJobsResumed;

		/// <summary>
		/// Raised when all jobs paused.
		/// </summary>
		protected event EventHandler<CM_JobManagerEventArgs> allJobsPaused;

		/// <summary>
		/// Raised when all jobs cleared.
		/// </summary>
		protected event EventHandler<CM_JobManagerEventArgs> allJobsCleared;

		#endregion

		private Dictionary<string, CM_Job> _ownedJobs = new Dictionary<string, CM_Job> ();

		/// <summary>
		/// Returns an initialised <see cref="CM_JobManager"/> instance. Provides static access to class.
		/// </summary>
		public static CM_JobManager Make ()
		{
			var jobManager = new CM_JobManager ();
			return jobManager;
		}

		#region PublicOperations

		/// <summary>
		/// Adds a job to the job manager.
		/// </summary>
		/// <returns>The job.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager AddJob (CM_Job job)
		{
			if (job.id.IsNullOrEmpty ()) {
				AutoGenerateJobId (job);
			}

			if (!_ownedJobs.ContainsKey (job.id)) {
				_ownedJobs.Add (job.id, job);

				OnJobAdded (new CM_JobManagerJobEditedEventArgs (_ownedJobs, job));
			}
		
			return this;
		}

		/// <summary>
		/// Creates a job with the specified id and routine and adds to job manager.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="id">Identifier of job.</param>
		/// <param name="routine">Routine.</param>
		public CM_JobManager AddJob (string id, IEnumerator routine)
		{
			return AddJob (MakeJob (id, routine));
		}

		/// <summary>
		/// Adds the provided jobs to the job manager.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="jobs">Jobs to add to this instance.</param>
		public CM_JobManager AddJob (IList<CM_Job> jobs)
		{
			foreach (var j in jobs) {
				AddJob (j);
			}

			return this;
		}

		/// <summary>
		/// Adds the provided jobs to the job manager.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="jobs">Jobs to add to this instance.</param>
		public CM_JobManager AddJob (params CM_Job[] jobs)
		{
			foreach (var j in jobs) {
				AddJob (j);
			}
		
			return this;
		}

		/// <summary>
		/// Removes the job if owned by this instance of job manager.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager RemoveJob (CM_Job job)
		{
			return RemoveJob (job.id);
		}

		/// <summary>
		/// Removes the job if owned by this instance of job manager.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager RemoveJob (string id)
		{
			if (_ownedJobs.ContainsKey (id)) {
				var removedJob = _ownedJobs [id];
				_ownedJobs.Remove (id);
				OnJobRemoved (new CM_JobManagerJobEditedEventArgs (_ownedJobs, removedJob));
			}
		
			return this;
		}

		/// <summary>
		/// Starts the specified coroutine if owned by this instance of job manager. Job is searched using job id.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager StartCoroutine (CM_Job job)
		{
			StartCoroutine (job.id);

			return this;
		}

		/// <summary>
		/// Starts the specified coroutine if owned by this instance of job manager.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager StartCoroutine (string id)
		{
			if (_ownedJobs.ContainsKey (id)) {
				_ownedJobs [id].Start ();
			} 

			return this;
		}

		/// <summary>
		/// Starts all jobs owned by this instance.
		/// </summary>
		/// <returns>The all.</returns>
		public CM_JobManager StartAll ()
		{
			foreach (var job in _ownedJobs.Values) {
				job.Start ();
			}

			return this;
		}

		/// <summary>
		/// Starts all jobs owned by this instance after delay in seconds.
		/// </summary>
		/// <returns>The all.</returns>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobManager StartAll (float delayInSeconds)
		{
			foreach (var job in _ownedJobs.Values) {
				job.Start (delayInSeconds);
			}
		
			return this;
		}

		/// <summary>
		/// Stops the specified coroutine if owned by this instance of job manager. Job is searched using job id.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager StopCoroutine (CM_Job job)
		{
			StopCoroutine (job.id);

			return this;
		}

		/// <summary>
		/// Stops the specified coroutine if owned by this instance of job manager. 
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager StopCoroutine (string id)
		{
			if (_ownedJobs.ContainsKey (id)) {
				_ownedJobs [id].Kill ();
			}

			return this;
		}

		/// <summary>
		/// Pauses the specified coroutine if owned by this instance of job manager. Job is searched using job id.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager PauseCoroutine (CM_Job job)
		{
			PauseCoroutine (job.id);

			return this;
		}

		/// <summary>
		/// Stops the specified coroutine if owned by this instance of job manager.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager PauseCoroutine (string id)
		{
			if (_ownedJobs.ContainsKey (id)) {
				_ownedJobs [id].Pause ();
			}

			return this;
		}

		/// <summary>
		/// Resumes the specified coroutine if owned by this instance of job manager. Job is searched using job id.
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager ResumeCoroutine (CM_Job job)
		{
			ResumeCoroutine (job.id);

			return this;
		}

		/// <summary>
		/// Stops the specified coroutine if owned by this instance of job manager. 
		/// </summary>
		/// <returns>The job manager.</returns>
		/// <param name="job">Job.</param>
		public CM_JobManager ResumeCoroutine (string id)
		{
			if (_ownedJobs.ContainsKey (id)) {
				_ownedJobs [id].Resume ();
			}

			return this;
		}

		/// <summary>
		/// Pauses all jobs owned by this instance. Raises allJobsPaused event.
		/// </summary>
		/// <returns>The all.</returns>
		public CM_JobManager PauseAll ()
		{
			foreach (var j in _ownedJobs.Values) {
				j.Pause ();
			}
		
			OnAllJobsPaused (new CM_JobManagerEventArgs (_ownedJobs));
		
			return this;
		}

		/// <summary>
		/// Pauses all jobs owned by this instance after delay in seconds. Raises allJobsPaused event.
		/// </summary>
		/// <returns>The all.</returns>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobManager PauseAll (float delayInSeconds)
		{
			foreach (var j in _ownedJobs.Values) {
				j.Pause (delayInSeconds);
			}
		
			OnAllJobsPaused (new CM_JobManagerEventArgs (_ownedJobs));
		
			return this;
		}

		/// <summary>
		/// Resumes all jobs owned by this instance. Raises allJobsResumedEvent.
		/// </summary>
		/// <returns>The all.</returns>
		public CM_JobManager ResumeAll ()
		{
			foreach (var j in _ownedJobs.Values) {
				j.Resume ();
			}
		
			OnAllJobsResumed (new CM_JobManagerEventArgs (_ownedJobs));
		
			return this;
		}

		/// <summary>
		/// Resumes all jobs owned by this instance after delay in seconds. Raises allJobsResumedEvent.
		/// </summary>
		/// <returns>The all.</returns>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobManager ResumeAll (float delayInSeconds)
		{
			foreach (var j in _ownedJobs.Values) {
				j.Resume (delayInSeconds);
			}
		
			OnAllJobsResumed (new CM_JobManagerEventArgs (_ownedJobs));
		
			return this;
		}

		/// <summary>
		/// Kills all jobs owned by this instance. Raises allJobsKilled event.
		/// </summary>
		/// <returns>The all.</returns>
		public CM_JobManager KillAll ()
		{
			foreach (var j in _ownedJobs.Values) {
				j.Kill ();
			}

			OnAllJobsKilled (new CM_JobManagerEventArgs (_ownedJobs));

			return this;

		}

		/// <summary>
		/// Kills all jobs owned by this instance after delay in seconds. Raises allJobsKilled event.
		/// </summary>
		/// <returns>The all.</returns>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobManager KillAll (float delayInSeconds)
		{
			foreach (var j in _ownedJobs.Values) {
				j.Kill (delayInSeconds);
			}
		
			OnAllJobsKilled (new CM_JobManagerEventArgs (_ownedJobs));
		
			return this;
		
		}

		/// <summary>
		/// Clears the job list owned by this instance. It does not kill the jobs so they will continue to run. Raises
		/// allJobsCleared event.
		/// </summary>
		/// <returns>The job list.</returns>
		public CM_JobManager ClearJobList ()
		{
			_ownedJobs.Clear ();

			OnAllJobsCleared (new CM_JobManagerEventArgs (_ownedJobs));

			return this;
		}

		/// <summary>
		/// Determines whether this instance has the job with the specified id.
		/// </summary>
		/// <returns><c>true</c> if this instance has a job with the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public bool HasJob (string id)
		{
			return _ownedJobs.ContainsKey (id);
		}

		#endregion

		/// <summary>
		/// Determines whether the specified job is executing.
		/// </summary>
		/// <returns><c>true</c> if the job is currently running; otherwise, <c>false</c>.</returns>
		/// <param name="job">Job.</param>
		public bool IsRunning (CM_Job job)
		{
			return IsRunning (job.id);
		}

		/// <summary>
		/// Determines whether the specified job is executing.
		/// </summary>
		/// <returns><c>true</c> if the job is currently running; otherwise, <c>false</c>.</returns>
		/// <param name="job">Job ID.</param>
		public bool IsRunning (string id)
		{
			if (_ownedJobs.ContainsKey (id)) {
				return _ownedJobs [id].running;
			}

			return false;
		}

		#region EventSubscription

		/// <summary>
		/// Subscribes to the the jobAdded event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager NotifyOnJobAdded (EventHandler<CM_JobManagerJobEditedEventArgs> e)
		{
			jobAdded += e;

			return this;
		}

		/// <summary>
		/// Unsubscribes to the the jobAdded event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager RemoveNotifyOnJobAdded (EventHandler<CM_JobManagerJobEditedEventArgs> e)
		{
			jobAdded -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the the jobRemoved event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager NotifyOnJobRemoved (EventHandler<CM_JobManagerJobEditedEventArgs> e)
		{
			jobRemoved += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the jobRemoved event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager RemoveNotifyOnJobRemoved (EventHandler<CM_JobManagerJobEditedEventArgs> e)
		{
			jobRemoved -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the the jobPaused event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager NotifyOnAllJobsPaused (EventHandler<CM_JobManagerEventArgs> e)
		{
			allJobsPaused += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the jobPaused event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager RemoveNotifyOnAllJobsPaused (EventHandler<CM_JobManagerEventArgs> e)
		{
			allJobsPaused -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the the jobResumed event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager NotifyOnAllJobsResumed (EventHandler<CM_JobManagerEventArgs> e)
		{
			allJobsResumed += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the jobResumed event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager RemoveNotifyOnAllJobsResumed (EventHandler<CM_JobManagerEventArgs> e)
		{
			allJobsResumed -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the the allJobsKilled event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager NotifyOnAllJobsKilled (EventHandler<CM_JobManagerEventArgs> e)
		{
			allJobsKilled += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the allJobsKilled event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager RemoveNotifyOnAllJobsKilled (EventHandler<CM_JobManagerEventArgs> e)
		{
			allJobsKilled -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the the allJobsCleared event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager NotifyOnAllJobsCleared (EventHandler<CM_JobManagerEventArgs> e)
		{
			allJobsCleared += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the allJobsCleared event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_JobManager RemoveNotifyOnAllJobsCleared (EventHandler<CM_JobManagerEventArgs> e)
		{
			allJobsCleared -= e;
		
			return this;
		}

		#endregion

		#region EventHandlers

		/// <summary>
		/// Raises the job added event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnJobAdded (CM_JobManagerJobEditedEventArgs e)
		{
			if (jobAdded != null) {
				jobAdded (this, e);
			}
		}

		/// <summary>
		/// Raises the job removed event.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnJobRemoved (CM_JobManagerJobEditedEventArgs e)
		{
			if (jobRemoved != null) {
				jobRemoved (this, e);
			}
		}

		/// <summary>
		/// Raises the all jobs resumed event.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnAllJobsResumed (CM_JobManagerEventArgs e)
		{
			if (allJobsResumed != null) {
				allJobsResumed (this, e);
			}
		}

		/// <summary>
		/// Raises the all jobs paused event.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnAllJobsPaused (CM_JobManagerEventArgs e)
		{
			if (allJobsPaused != null) {
				allJobsPaused (this, e);
			}
		}

		/// <summary>
		/// Raises the all jobs killed event.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnAllJobsKilled (CM_JobManagerEventArgs e)
		{
			if (allJobsKilled != null) {
				allJobsKilled (this, e);
			}
		}

		/// <summary>
		/// Raises the all jobs cleared event.
		/// </summary>
		/// <param name="e">E.</param>
		public void OnAllJobsCleared (CM_JobManagerEventArgs e)
		{
			if (allJobsCleared != null) {
				allJobsCleared (this, e);
			}
		}

		#endregion

		protected override void HandlejobComplete (object sender, CM_JobEventArgs e)
		{
			if (e.job.repeating) {
				return;
			}

			e.job.RemoveNotifyOnJobComplete (HandlejobComplete);


			if (_ownedJobs.ContainsKey (e.job.id)) {
				_ownedJobs.Remove (e.job.id);

			}
		}

	}
}