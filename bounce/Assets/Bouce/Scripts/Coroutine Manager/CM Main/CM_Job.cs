using System;
using System.Collections;
using System.Collections.Generic;

namespace Bounce.CoroutineManager
{

	/// <summary>
	/// The main coroutine job class. Encapsulates the behaviour for a single coroutine job.
	/// Provides access to status (i.e. running, paused, killed etc),
	/// </summary>
	public class CM_Job : ICM_Cloneable<CM_Job>
	{
		#region PublicProperties

		/// <summary>
		/// Gets or sets the identifier. The identifier is a unique key used by <see cref="CM_JobManager"/> to reference 
		/// individual jobs.
		/// </summary>
		/// <value>The identifier.</value>
		public string id { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_Job"/> is running.
		/// </summary>
		/// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
		public bool running { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_Job"/> is paused.
		/// </summary>
		/// <value><c>true</c> if paused; otherwise, <c>false</c>.</value>
		public bool paused { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_Job"/> job was killed or was allowed to complete.
		/// </summary>
		/// <value><c>true</c> if job killed; otherwise, <c>false</c>.</value>
		public bool jobKilled { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_Job"/> is repeating.
		/// </summary>
		/// <value><c>true</c> if repeating; otherwise, <c>false</c>.</value>
		public bool repeating { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="TacticalShooter.CoroutineManager.CM_Job"/> is repeatable.
		/// </summary>
		/// <value><c>true</c> if repeatable; otherwise, <c>false</c>.</value>
		public bool repeatable { get; private set; }

		/// <summary>
		/// Gets the number of times this job has been executed.
		/// </summary>
		/// <value>The number of times executed.</value>
		public int numOfTimesExecuted { get; private set; }

		/// <summary>
		/// Gets the coroutine of this job.
		/// </summary>
		/// <value>The coroutine.</value>
		public IEnumerator coroutine { get; private set; }

		#endregion

		#region Events

		/// <summary>
		/// Occurs when job started.
		/// </summary>
		private event EventHandler<CM_JobEventArgs> jobStarted;

		/// <summary>
		/// Occurs when job paused.
		/// </summary>
		private event EventHandler<CM_JobEventArgs> jobPaused;

		/// <summary>
		/// Occurs when job resumed.
		/// </summary>
		private event EventHandler<CM_JobEventArgs> jobResumed;

		/// <summary>
		/// Occurs when job complete.
		/// </summary>
		private event EventHandler<CM_JobEventArgs> jobComplete;

		/// <summary>
		/// Occurs when child jobs started.
		/// </summary>
		private event EventHandler<CM_JobEventArgs> childJobsStarted;

		/// <summary>
		/// Occurs when child jobs complete.
		/// </summary>
		private event EventHandler<CM_JobEventArgs> childJobsComplete;

		/// <summary>
		/// Occurs when job finished running. This is useful when a job has been set to repeat and you would like to be informed when
		/// all repeat jobs have finished.
		/// </summary>
		private event EventHandler<CM_JobEventArgs> jobFinishedRunning;

		#endregion

		/// <summary>
		/// Gets the child jobs as an array.
		/// </summary>
		/// <value>The child jobs.</value>
		private CM_Job[] childJobs { get { return _childJobs == null ? null : _childJobs.ToArray (); } }

		/// <summary>
		/// The of child jobs.
		/// </summary>
		private List<CM_Job> _childJobs;

		private Dictionary<CM_Job, IEnumerator> _childJobRoutineClones = new Dictionary<CM_Job, IEnumerator> ();

		/// <summary>
		/// Holds a clone of the coroutine. Used to repeat the routine if
		/// <see cref="CM_Job.repeating"/> is true.
		/// </summary>
		private IEnumerator _routineClone;

		/// <summary>
		/// Used to repeat the routine a set number of times, if
		/// <see cref="CM_Job.repeating"/> is true.
		/// </summary>
		private int? _numOfTimesToRepeat;

		/// <summary>
		/// Initializes a new instance of the <see cref="CM_Job"/> class.
		/// </summary>
		/// <param name="coroutine">Coroutine.</param>
		private CM_Job (IEnumerator coroutine) : this (coroutine, "")
		{
		}

		private CM_Job (IEnumerator coroutine, string id)
		{
			this.coroutine = coroutine;
			this.id = id;
			_routineClone = coroutine.Clone ();
		}

		/// <summary>
		/// Returns an initialised <see cref="CM_Job"/> instance. Provides static access to class.
		/// </summary>
		/// <param name="coroutine">Coroutine.</param>
		public static CM_Job Make (IEnumerator coroutine)
		{
			return new CM_Job (coroutine);
		}

		/// <summary>
		/// Returns an initialised <see cref="CM_Job"/> instance with the specified id. Provides static access to class.
		/// </summary>
		/// <param name="coroutine">Coroutine.</param>
		/// <param name="id">Identifier.</param>
		public static CM_Job Make (IEnumerator coroutine, string id)
		{
			return new CM_Job (coroutine, id);
		}

		/// <summary>
		/// Builds the specified coroutines into <see cref="CM_Job"/> instances.
		/// </summary>
		/// <param name="coroutines">The built jobs.</param>
		public static CM_Job[] Builder (params IEnumerator[] coroutines)
		{
			var retJobs = new CM_Job[coroutines.Length];

			int i = 0;
			foreach (var c in coroutines) {
				retJobs [i++] = new CM_Job (c);
			}

			return retJobs;
		}

		#region Clone

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public CM_Job Clone ()
		{
			var job = new CM_Job (_routineClone.Clone (), id);

			if (jobStarted != null) {
				job.jobStarted += jobStarted;
			}

			if (jobPaused != null) {
				job.jobPaused += jobPaused;
			}

			if (jobResumed != null) {
				job.jobResumed += jobResumed;
			}

			if (jobComplete != null) {
				job.jobComplete += jobComplete;
			}

			if (childJobsComplete != null) {
				job.childJobsComplete += childJobsComplete;
			}

			if (childJobsStarted != null) {
				job.childJobsStarted += childJobsStarted;
			}

			if (jobFinishedRunning != null) {
				job.jobFinishedRunning += jobFinishedRunning;
			}

			job.repeating = repeating;
			job._numOfTimesToRepeat = _numOfTimesToRepeat;

			if (childJobs != null) {

				job._childJobs = new List<CM_Job> ();

				foreach (var cj in _childJobs) {
					job._childJobs.Add (new CM_Job (_childJobRoutineClones [cj].Clone (), cj.id));
				}
			}

			return job;
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		/// <param name="numOfCopies">Number of copies to create.</param>
		public CM_Job[] Clone (int numOfCopies)
		{
			var retJobs = new CM_Job[numOfCopies];
		
			for (int i = 0; i < numOfCopies; i++) {
				retJobs [i] = Clone ();
			}
		
			return retJobs;
		}

		#endregion

		#region PublicOperations

		/// <summary>
		/// Start this instance. Runs the coroutine immediately.
		/// </summary>
		public CM_Job Start ()
		{
			running = true;
			CM_Dispatcher.instance.StartCoroutine (RunJob ());
		
			return this;
		}

		/// <summary>
		/// Start the specified instance after delayInSeconds. The coroutine is added to
		/// <see cref="CM_Dispatcher"/> job queue to be executed in the next timestep as a coroutine cannot be started in
		/// a seperate thread.
		/// </summary>
		/// <param name="delayInSecods">Delay in secods until instance is processed.</param>
		public CM_Job Start (float delayInSeconds)
		{
			int delay = (int)delayInSeconds * 1000;

			new System.Threading.Timer (obj => {
				lock (this) {
					running = true;
					CM_Dispatcher.instance.AddToExecuteQueue (RunJob ());
				}
			}, null, delay, System.Threading.Timeout.Infinite);

			return this;
		}

		/// <summary>
		/// Sets this instance to repeat. The job is repeated when it has finished processing.
		/// </summary>
		public CM_Job Repeat ()
		{
			repeating = true;

			return this;
		}

		public CM_Job Repeatable ()
		{
			repeatable = true;

			return this;
		}

		/// <summary>
		/// Sets this instance to repeat. The job is repeated a set number of times.
		/// </summary>
		public CM_Job Repeat (int numOfTimes)
		{
			repeating = true;
			_numOfTimesToRepeat = numOfTimes;
		
			return this;
		}

		public CM_Job StopRepeat ()
		{
			repeating = false;

			return this;
		}

		/// <summary>
		/// Stops the repeat after a specified delay in seconds.
		/// </summary>
		/// <returns>The repeat.</returns>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_Job StopRepeat (float delayInSeconds)
		{
			int delay = (int)delayInSeconds * 1000;
		
			new System.Threading.Timer (obj => {
				lock (this) {
					repeating = false;
					_numOfTimesToRepeat = null;
				}
			}, null, delay, System.Threading.Timeout.Infinite);
		
			return this;
		
		}

		/// <summary>
		/// Pause this instance.
		/// </summary>
		public CM_Job Pause ()
		{
			paused = true;

			OnJobPaused (new CM_JobEventArgs (this, childJobs));

			return this;
		}

		/// <summary>
		/// Pause the specified instance after delayInSeconds.
		/// </summary>
		/// <param name="delayInSecods">Delay in secods until instance is paused.</param>
		public CM_Job Pause (float delayInSeconds)
		{
			int delay = (int)delayInSeconds * 1000;
		
			new System.Threading.Timer (obj => {
				lock (this) {
					Pause ();
				}
			}, null, delay, System.Threading.Timeout.Infinite);
		
			return this;
		}

		/// <summary>
		/// Resume this instance.
		/// </summary>
		public CM_Job Resume ()
		{
			paused = false;

			OnJobResumed (new CM_JobEventArgs (this, childJobs));

			return this;
		}

		/// <summary>
		/// Resume the specified instance after delayInSeconds.
		/// </summary>
		/// <param name="delayInSecods">Delay in secods until instance is resumed.</param>
		public CM_Job Resume (float delayInSeconds)
		{
			int delay = (int)delayInSeconds * 1000;
		
			new System.Threading.Timer (obj => {
				lock (this) {
					Resume ();
				}
			}, null, delay, System.Threading.Timeout.Infinite);
		
			return this;
		}

		/// <summary>
		/// Kill this instance. Stops the running coroutine.
		/// </summary>
		public void Kill ()
		{
			jobKilled = true;
			running = false;
			paused = false;
		}

		/// <summary>
		/// Kill this instance. Stops the running coroutine after delayInSeconds.
		/// </summary>
		/// <param name="delayInSeconds">Delay in seconds until instance killed.</param>
		public void Kill (float delayInSeconds)
		{
			var delay = (int)delayInSeconds * 1000;
			new System.Threading.Timer (obj => {
				lock (this) {
					Kill ();
				}
			}, null, delay, System.Threading.Timeout.Infinite);
		}

		#endregion

		#region ChildJobs

		/// <summary>
		/// Adds a child job.
		/// </summary>
		/// <returns>The child.</returns>
		/// <param name="childJob">Child job.</param>
		public CM_Job AddChild (CM_Job childJob)
		{
			if (_childJobs == null) {
				_childJobs = new List<CM_Job> ();
			}
		
			_childJobs.Add (childJob);
			_childJobRoutineClones.Add (childJob, childJob.coroutine.Clone ());
		
			return this;
		}

		/// <summary>
		/// Create a new job using the provided Enumerator and adds as a child job.
		/// </summary>
		/// <returns>The child.</returns>
		/// <param name="childJob">Child job.</param>
		public CM_Job AddChild (IEnumerator childJob)
		{
			return AddChild (new CM_Job (childJob));
		}

		/// <summary>
		/// Removes a child job if present.
		/// </summary>
		/// <returns>The child job.</returns>
		/// <param name="childJob">Child job.</param>
		public CM_Job RemoveChildJob (CM_Job childJob)
		{
			if (_childJobs == null)
				return this;
		
			if (_childJobs.Contains (childJob)) {
				_childJobs.Remove (childJob);
				_childJobRoutineClones.Remove (childJob);
			}
		
			return this;
		}

		#endregion

		#region EventSubscription

		/// <summary>
		/// Subscribes to the jobFinishedRunning event
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_Job NotifyOnJobFinishedRunning (EventHandler<CM_JobEventArgs> e)
		{
			jobFinishedRunning += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the jobFinishedRunning event
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_Job RemoveNotifyOnJobFinishedRunning (EventHandler<CM_JobEventArgs> e)
		{
			jobFinishedRunning -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the jobStarted event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_Job NotifyOnJobStarted (EventHandler<CM_JobEventArgs> e)
		{
			jobStarted += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the jobStarted event.
		/// </summary>
		/// <param name="e">The eventhandler to be unsubscribed.</param>
		public CM_Job RemoveNotifyOnJobStarted (EventHandler<CM_JobEventArgs> e)
		{
			jobStarted -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the job paused event.
		/// </summary>
		/// <returns>The on job paused.</returns>
		/// <param name="e">E.</param>
		public CM_Job NotifyOnJobPaused (EventHandler<CM_JobEventArgs> e)
		{
			jobPaused += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the job paused event.
		/// </summary>
		/// <param name="e">The eventhandler to be unsubscribed.</param>
		public CM_Job RemoveNotifyOnJobPaused (EventHandler<CM_JobEventArgs> e)
		{
			jobPaused -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the job resumed event.
		/// </summary>
		/// <returns>The on job paused.</returns>
		/// <param name="e">E.</param>
		public CM_Job NotifyOnJobResumed (EventHandler<CM_JobEventArgs> e)
		{
			jobResumed += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the job resumed event.
		/// </summary>
		/// <param name="e">The eventhandler to be unsubscribed.</param>
		public CM_Job RemoveNotifyOnJobResumed (EventHandler<CM_JobEventArgs> e)
		{
			jobResumed -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the the jobComplete event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_Job NotifyOnJobComplete (EventHandler<CM_JobEventArgs> e)
		{
			jobComplete += e;

			return this;
		}

		/// <summary>
		/// Unsubscribes to the the jobComplete event.
		/// </summary>
		/// <param name="e">The eventhandler to be unsubscribed.</param>
		public CM_Job RemoveNotifyOnJobComplete (EventHandler<CM_JobEventArgs> e)
		{
			jobComplete -= e;

			return this;
		}

		/// <summary>
		/// Subscribes to the the childJobsStarted event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_Job NotifyOnChildJobStarted (EventHandler<CM_JobEventArgs> e)
		{
			childJobsStarted += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the childJobsStarted event.
		/// </summary>
		/// <param name="e">The eventhandler to be unsubscribed.</param>
		public CM_Job RemoveNotifyOnChildJobStarted (EventHandler<CM_JobEventArgs> e)
		{
			childJobsStarted -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the the childJobsComplete event.
		/// </summary>
		/// <param name="e">The eventhandler to be invoked on event.</param>
		public CM_Job NotifyOnChildJobComplete (EventHandler<CM_JobEventArgs> e)
		{
			childJobsComplete += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the childJobsComplete event.
		/// </summary>
		/// <param name="e">The eventhandler to be unsubscribed.</param>
		public CM_Job RemoveNotifyOnChildJobComplete (EventHandler<CM_JobEventArgs> e)
		{
			childJobsComplete -= e;
		
			return this;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Raises the job finished running event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnJobFinishedRunning (CM_JobEventArgs e)
		{
			if (jobFinishedRunning != null) {
				jobFinishedRunning (this, e);
			}
		}

		/// <summary>
		/// Raises the job started event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnJobStarted (CM_JobEventArgs e)
		{
			if (jobStarted != null) {
				jobStarted (this, e);
			}
		
		}

		/// <summary>
		/// Raises the job complete event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnJobComplete (CM_JobEventArgs e)
		{
			if (jobComplete != null) {
				jobComplete (this, e);
			}

		}

		/// <summary>
		/// Raises the job paused event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnJobPaused (CM_JobEventArgs e)
		{
			if (jobPaused != null) {
				jobPaused (this, e);
			}
		
		}

		/// <summary>
		/// Raises the job resumed event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnJobResumed (CM_JobEventArgs e)
		{
			if (jobResumed != null) {
				jobResumed (this, e);
			}
		
		}

		/// <summary>
		/// Raises the child jobs started event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnChildJobsStarted (CM_JobEventArgs e)
		{
			if (childJobsStarted != null) {
				childJobsStarted (this, e);
			}
		
		}

		/// <summary>
		/// Raises the child jobs complete event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnChildJobsComplete (CM_JobEventArgs e)
		{
			if (childJobsComplete != null) {
				childJobsComplete (this, e);
			}
		
		}

		#endregion

		private IEnumerator StartAsCoroutine ()
		{
			running = true;
			yield return CM_Dispatcher.instance.StartCoroutine (RunJob ());
		}

		private IEnumerator RunJob ()
		{
			yield return null;

			OnJobStarted (new CM_JobEventArgs (this, childJobs));
		
			while (running) {

				if (paused) {
					yield return null;
				} else {

					if (coroutine.MoveNext ()) {
						yield return coroutine.Current;
					} else {
						if (_childJobs != null) {
							yield return CM_Dispatcher.instance.StartCoroutine (RunChildJobs ());
						}

			
						running = false;

					}
				}
			}

			numOfTimesExecuted++;



			if (repeating) {

				if (!_numOfTimesToRepeat.HasValue) {
					ResetJob ();
				} else {
			
					if (numOfTimesExecuted < _numOfTimesToRepeat) {
						ResetJob ();
					} else {
						repeating = false;
						_numOfTimesToRepeat = null;
					}
				}

			}

			if (repeatable) {
				ResetJob ();
			}

			OnJobComplete (new CM_JobEventArgs (this, childJobs));

			if (repeating) {
				Start ();
			} else {
				OnJobFinishedRunning (new CM_JobEventArgs (this, childJobs));
			}
		}

		private void ResetJob ()
		{
			coroutine = _routineClone.Clone ();

			if (_childJobs != null) {
				foreach (var cj in _childJobs) {
					cj.coroutine = _childJobRoutineClones [cj].Clone ();
				}
			}
		}

		private IEnumerator RunChildJobs ()
		{
			if (_childJobs != null && _childJobs.Count > 0) {

				var eventArgs = new CM_JobEventArgs (this, _childJobs.ToArray ());

				OnChildJobsStarted (eventArgs);

				foreach (var job in _childJobs) {
					yield return CM_Dispatcher.instance.StartCoroutine (job.StartAsCoroutine ());
				}

				OnChildJobsComplete (eventArgs);

			}
		}


	}
}