using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bounce.CoroutineManager
{
	/// <summary>
	/// The main job queue class. Encapsulates all behaviour related to queueing a job. Provides access to events, and status (i.e. running, repeating).
	/// </summary>
	public class CM_JobQueue : CM_GlobalCoroutineManager<CM_JobQueue>, ICM_Cloneable<CM_JobQueue>
	{
		#region PublicProperties

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_JobQueue"/> is repeating.
		/// </summary>
		/// <value><c>true</c> if repeating; otherwise, <c>false</c>.</value>
		public bool repeating { get; private set; }

		/// <summary>
		/// Gets the number of times this queue executed (used if repeating).
		/// </summary>
		/// <value>The number of times executed.</value>
		public int numOfTimesExecuted { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_JobQueue"/> is running.
		/// </summary>
		/// <value><c>true</c> if running; otherwise, <c>false</c>.</value>
		public bool running { get { return _jobQueueRunning; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_JobQueue"/> is running continously i.e. will not stop running until StopContinousRunning is called.
		/// </summary>
		/// <value><c>true</c> if continous running; otherwise, <c>false</c>.</value>
		public bool continousRunning { get { return _continousRunning; } }

		#endregion

		#region Events

		/// <summary>
		/// Raised when queue started.
		/// </summary>
		protected event EventHandler<CM_QueueEventArgs> queueStarted;

		/// <summary>
		/// Raised when queue complete.
		/// </summary>
		protected event EventHandler<CM_QueueEventArgs> queueComplete;

		/// <summary>
		/// Raised when a job in the queue has finished.
		/// </summary>
		protected event EventHandler<CM_QueueEventArgs> jobProcessed;

		#endregion

		/// <summary>
		/// Gets a value indicating whether this <see cref="CM_JobQueue"/> has difficulty starting.
		/// THis may be because the queue is already running or that there are no jobs in the queue.
		/// </summary>
		/// <value><c>true</c> if error starting; otherwise, <c>false</c>.</value>
		private bool errorStarting {
			get {

				if (_jobQueueRunning) {
					return true;
				} 

				return false;
			}
		}

		/// <summary>
		/// Used to repeat the routine a set number of times, if
		/// <see cref="CM_JobQueue.repeating"/> is true.
		/// </summary>
		private int? _numOfTimesToRepeat;

		private Queue<CM_Job> _jobQueue = new Queue<CM_Job> ();
		private bool _jobQueueRunning = false;
		private bool _continousRunning = false;
		private List<CM_Job> _completedJobs = new List<CM_Job> ();

		/// <summary>
		/// Returns an initialised <see cref="CM_JobQueue"/> instance. Provides static access to class.
		/// </summary>
		public static CM_JobQueue Make ()
		{
			return new CM_JobQueue ();
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public CM_JobQueue Clone ()
		{
			var clonedQueue = Make ();

			var queue = _jobQueue.ToArray ();

			for (int i = 0; i <= queue.Length - 1; i++) {
				clonedQueue.Enqueue (queue [i].Clone ().RemoveNotifyOnJobComplete (HandlejobComplete));
			}

			if (queueStarted != null) {
				clonedQueue.queueStarted += queueStarted;
			}

			if (queueComplete != null) {
				clonedQueue.queueComplete += queueComplete;
			}

			if (jobProcessed != null) {
				clonedQueue.jobProcessed += jobProcessed;
			}

			foreach (var job in _completedJobs) {
				clonedQueue._completedJobs.Add (job);
			}

			clonedQueue.repeating = repeating;
			clonedQueue._numOfTimesToRepeat = _numOfTimesToRepeat;

			return clonedQueue;
		}

		/// <summary>
		/// Clone this instance the specified numOfCopies.
		/// </summary>
		/// <param name="numOfCopies">Number of copies.</param>
		public CM_JobQueue[] Clone (int numOfCopies)
		{
			var queue = new CM_JobQueue[numOfCopies];

			for (int i = 0; i < numOfCopies; i++) {
				queue [i] = Clone ();
			}

			return queue;
		}

		#region Enqueue

		/// <summary>
		/// Enqueues the specified other queue. Adds the jobs from one queue to this queue and also adds the other queues event subscriptions.
		/// </summary>
		/// <param name="other">Other.</param>
		public CM_JobQueue Enqueue (CM_JobQueue other)
		{
			foreach (var job in other._jobQueue) {
				Enqueue (job);
			}

			if (other.jobProcessed != null) {
				this.jobProcessed += other.jobProcessed;
			}

			if (other.queueStarted != null) {
				this.queueStarted += other.queueStarted;
			}

			if (other.queueComplete != null) {
				this.queueComplete += other.queueComplete;
			}

			return this;
		}

		/// <summary>
		/// Enqueues the specified jobs.
		/// </summary>
		/// <param name="jobs">Jobs.</param>
		public CM_JobQueue Enqueue (params CM_Job[] jobs)
		{
			foreach (var job in jobs) {
				Enqueue (job);
			}
		
			return this;
		}

		/// <summary>
		/// Enqueues the specified jobs.
		/// </summary>
		/// <param name="jobs">Jobs.</param>
		public CM_JobQueue Enqueue (IList<CM_Job> jobs)
		{
			foreach (var job in jobs) {
				Enqueue (job);
			}
		
			return this;
		}

		/// <summary>
		/// Creates a new job with specified id and coroutine and adds job to queue.
		/// </summary>
		/// <param name="id">Job Identifier.</param>
		/// <param name="routine">Routine.</param>
		public CM_JobQueue Enqueue (string id, IEnumerator routine)
		{
			return Enqueue (MakeJob (id, routine, false));
		}

		/// <summary>
		/// Creates a new job with specified id and coroutine and adds job to queue.
		/// </summary>
		/// <param name="id">Job Identifier.</param>
		/// <param name="routine">Routine.</param>
		public CM_JobQueue Enqueue (IEnumerator routine)
		{
			return Enqueue (MakeJob (null, routine, false));
		}


		/// <summary>
		/// Enqueues the specified job.
		/// </summary>
		/// <param name="job">Job.</param>
		public CM_JobQueue Enqueue (CM_Job job)
		{
			job.NotifyOnJobComplete (HandlejobComplete);

			AddJobToQueue (job);

			if (_continousRunning && _jobQueue.Count == 1) {
				_jobQueue.Peek ().Start ();
			}
		
			return this;
		}

		#endregion

		#region PublicOperations

		/// <summary>
		/// Start this instance of the queue immediately.
		/// </summary>
		public CM_JobQueue Start ()
		{

			if (!errorStarting) {
				OnQueueStarted (new CM_QueueEventArgs (_jobQueue.ToArray (), _completedJobs.ToArray (), this));

				if (_jobQueue.Count > 0) {
					_jobQueue.Peek ().Start ();
				}

				_jobQueueRunning = true;
			}


			return this;
		}

		/// <summary>
		/// Start the specified instance after delayInSeconds.
		/// </summary>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobQueue Start (float delayInSeconds)
		{
			if (errorStarting) {
				return this;
			}

			int delay = (int)delayInSeconds * 1000;
		
			new System.Threading.Timer (obj => {
				lock (this) {
					OnQueueStarted (new CM_QueueEventArgs (_jobQueue.ToArray (), _completedJobs.ToArray (), this));

					if (_jobQueue.Count > 0) {
						_jobQueue.Peek ().Start (0f);
					}

					_jobQueueRunning = true;
				}
			}, null, delay, System.Threading.Timeout.Infinite);
		
			return this;
		}

		/// <summary>
		/// Sets this instance to repeat. The job is repeated when it has finished processing.
		/// </summary>
		public CM_JobQueue Repeat ()
		{
			repeating = true;

			return this;
		}

		/// <summary>
		/// Sets this instance to repeat a number of times. The job is repeated when it has finished processing.
		/// </summary>
		public CM_JobQueue Repeat (int numOfTimes)
		{
			repeating = true;

			numOfTimesExecuted = 0;
			_numOfTimesToRepeat = numOfTimes;

			return this;
		}

		/// <summary>
		/// Stops the repeat.
		/// </summary>
		/// <returns>The repeat.</returns>
		public CM_JobQueue StopRepeat ()
		{
			repeating = false;

			return this;
		}

		/// <summary>
		/// Stops the repeat after a specified delay in seconds.
		/// </summary>
		/// <returns>The repeat.</returns>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobQueue StopRepeat (float delayInSeconds)
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
		/// Pauses this instance.
		/// </summary>
		public CM_JobQueue Pause ()
		{
			if (_jobQueueRunning) {
				_jobQueue.Peek ().Pause ();
			}

			return this;
		}

		/// <summary>
		/// Pause this instance after the specified delayInSeconds.
		/// </summary>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobQueue Pause (float delayInSeconds)
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
		/// Resume this instance immediately.
		/// </summary>
		public CM_JobQueue Resume ()
		{
			if (_jobQueueRunning && _jobQueue.Count > 0) {
				_jobQueue.Peek ().Resume ();
			}

			return this;
		}

		/// <summary>
		/// Resume the instance after the specified delayInSeconds.
		/// </summary>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobQueue Resume (float delayInSeconds)
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
		/// Set the queue to run continously.
		/// </summary>
		public CM_JobQueue ContinousRunning ()
		{
			_continousRunning = true;

			return this;
		}

		/// <summary>
		/// Stops the continous running of this queue.
		/// </summary>
		public CM_JobQueue StopContinousRunning ()
		{
			_continousRunning = false;

			return this;
		}

		/// <summary>
		/// Kill all currently queued jobs immediately. Clears queue list.
		/// </summary>
		public CM_JobQueue KillAll ()
		{
			if (_jobQueue.Count == 0) {
				return this;
			}

			var jobs = _jobQueue.ToArray ();

			foreach (var job in jobs) {
				job.Kill ();
			}

			return this;
		}

		/// <summary>
		/// Kill all currently queued jobs after the specified delayInSeconds.
		/// </summary>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobQueue KillAll (float delayInSeconds)
		{
			int delay = (int)delayInSeconds * 1000;
		
			new System.Threading.Timer (obj => {
				lock (this) {
					KillAll ();
				}
			}, null, delay, System.Threading.Timeout.Infinite);
		
			return this;
		}

		/// <summary>
		/// Kills the current running job immediately.
		/// </summary>
		/// <returns>The current.</returns>
		public CM_JobQueue KillCurrent ()
		{
			if (_jobQueue.Count > 0) {
				_jobQueue.Peek ().Kill ();
			}

			return this;
		}

		/// <summary>
		/// Kills the current running job after the specified delayInSeconds.
		/// </summary>
		/// <returns>The current.</returns>
		/// <param name="delayInSeconds">Delay in seconds.</param>
		public CM_JobQueue KillCurrent (float delayInSeconds)
		{
			if (_jobQueue.Count > 0) {
				int delay = (int)delayInSeconds * 1000;
			
				new System.Threading.Timer (obj => {
					lock (this) {
						KillCurrent ();
					}
				}, null, delay, System.Threading.Timeout.Infinite);
			}
		
			return this;
		}

		#endregion

		#region EventSubscription

		/// <summary>
		/// Subscribes to the queue started event.
		/// </summary>
		/// <param name="e">The event handler to be invoked on event.</param>
		public CM_JobQueue NotifyOnQueueStarted (EventHandler<CM_QueueEventArgs> e)
		{
			queueStarted += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the queue started event.
		/// </summary>
		/// <param name="e">The event handler to be invoked on event.</param>
		public CM_JobQueue RemoveNotifyOnQueueStarted (EventHandler<CM_QueueEventArgs> e)
		{
			queueStarted -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the queue completed event.
		/// </summary>
		/// <param name="e">The event handler to be invoked on event.</param>
		public CM_JobQueue NotifyOnQueueComplete (EventHandler<CM_QueueEventArgs> e)
		{
			queueComplete += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the queue completed event.
		/// </summary>
		/// <param name="e">The event handler to be invoked on event.</param>
		public CM_JobQueue RemoveNotifyOnQueueComplete (EventHandler<CM_QueueEventArgs> e)
		{
			queueComplete -= e;
		
			return this;
		}

		/// <summary>
		/// Subscribes to the the job processed event.
		/// </summary>
		/// <param name="e">The event handler to be invoked on event.</param>
		public CM_JobQueue NotifyOnJobProcessed (EventHandler<CM_QueueEventArgs> e)
		{
			jobProcessed += e;
		
			return this;
		}

		/// <summary>
		/// Unsubscribes to the the job processed event. This event is invoked every time a job in the queue has finished running.
		/// </summary>
		/// <param name="e">The event handler to be invoked on event.</param>
		public CM_JobQueue RemoveNotifyOnJobProcessed (EventHandler<CM_QueueEventArgs> e)
		{
			jobProcessed -= e;
		
			return this;
		}

		#endregion

		#region EventHandlers

		/// <summary>
		/// Raises the queue started event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnQueueStarted (CM_QueueEventArgs e)
		{
			if (queueStarted != null) {
				queueStarted (this, e);
			}
		}

		/// <summary>
		/// Raises the queue complete event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnQueueComplete (CM_QueueEventArgs e)
		{
			if (queueComplete != null) {
				queueComplete (this, e);
			}
		}

		/// <summary>
		/// Raises the job processed event.
		/// </summary>
		/// <param name="e">E.</param>
		protected void OnJobProcessed (CM_QueueEventArgs e)
		{
			if (jobProcessed != null) {
				jobProcessed (this, e);
			}
		}

		#endregion

		/// <summary>
		/// Invoked whenever a queued job has finished processing. Handles maintenance of queue and raising 
		/// OnJobProcessed and OnQueueComplete events.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected override void HandlejobComplete (object sender, CM_JobEventArgs e)
		{
			if (e.job.repeating) {
				return;
			}

			RemoveJobFromQueue (e.job);

			var jobQueue = _jobQueue.ToArray ();
			var completedJobs = _completedJobs.ToArray ();

			OnJobProcessed (new CM_QueueEventArgs (jobQueue, completedJobs, this));


			if (_jobQueue.Count > 0 && _jobQueueRunning) {
				_jobQueue.Peek ().Start ();
			} else if (_jobQueue.Count == 0 && _jobQueueRunning) {
				numOfTimesExecuted++;

				OnQueueComplete (new CM_QueueEventArgs (jobQueue, completedJobs, this));

				if (repeating) {

					if (!_numOfTimesToRepeat.HasValue) {
						RequeueAll ();
						_jobQueue.Peek ().Start ();
					} else {
						
						if (numOfTimesExecuted < _numOfTimesToRepeat) {
							RequeueAll ();
							_jobQueue.Peek ().Start ();
						} else {
							repeating = false;
							_numOfTimesToRepeat = null;
							_jobQueueRunning = false;
						}
					}


				} else if (!_continousRunning) {
					_jobQueueRunning = false;
				}

				_completedJobs.Clear ();

			}
			

		}

		private void RequeueAll ()
		{
			_jobQueue.Clear ();

			foreach (var job in _completedJobs) {
				var jobClone = job.Clone ().NotifyOnJobComplete (HandlejobComplete);
				_jobQueue.Enqueue (jobClone);
			}
		}

		private void AddJobToQueue (CM_Job job)
		{
			_jobQueue.Enqueue (job);
		}

		private void RemoveJobFromQueue (CM_Job job)
		{
			job.RemoveNotifyOnJobComplete (HandlejobComplete);
			_completedJobs.Add (job);
			_jobQueue.Dequeue ();
		}


	}
}