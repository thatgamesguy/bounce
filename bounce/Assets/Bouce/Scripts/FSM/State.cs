using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bounce.StateMachine
{
	public class State : FSMState
	{

		private List<FSMAction> actions;
		private List<FSMReason> reasons;
		private int currentAction = 0;

        private bool _parallelExecution;

        public State(FSMStateID stateid, FSMAction action, FSMReason reason, bool parallelExecution = false)
        {
            var actionList = new List<FSMAction>();
            actionList.Add(action);

            var reasonList = new List<FSMReason>();
            reasonList.Add(reason);

            this.stateID = stateid;
            this.actions = actionList;
            this.reasons = reasonList;

            foreach (var r in reasons)
            {
                AddTransition(r.transition, r.goToState);
            }

            _parallelExecution = parallelExecution;
        }

        public State (FSMStateID stateid, FSMAction action, List<FSMReason> reasons, bool parallelExecution = false)
		{
			var actionList = new List<FSMAction> ();
			actionList.Add (action);
			
			this.stateID = stateid;
			this.actions = actionList;
			this.reasons = reasons;
			
			foreach (var reason in reasons) {
				AddTransition (reason.transition, reason.goToState);
			}

            _parallelExecution = parallelExecution;
        }

        public State(FSMStateID stateid, List<FSMAction> actions, FSMReason reason, bool parallelExecution = false)
        {
            var reasonList = new List<FSMReason>();
            reasonList.Add(reason);

            this.stateID = stateid;
            this.actions = actions;
            this.reasons = reasonList;

            foreach (var r in reasonList)
            {
                AddTransition(r.transition, r.goToState);
            }

            _parallelExecution = parallelExecution;
        }

        public State (FSMStateID stateid, List<FSMAction> actions, List<FSMReason> reasons, bool parallelExecution = false)
		{
			this.stateID = stateid;
			this.actions = actions;
			this.reasons = reasons;
			
			foreach (var reason in reasons) {
				AddTransition (reason.transition, reason.goToState);
			}

            _parallelExecution = parallelExecution;
        }

		public override void Enter ()
		{
			currentAction = 0;

			foreach (var action in actions) {
				action.Enter ();
			}

			foreach (var reason in reasons) {
				reason.Enter ();
			}
						
		}

		public override void Exit ()
		{
			foreach (var action in actions) {
				action.Exit ();
			}

			foreach (var reason in reasons) {
				reason.Exit ();
			}
		}

		public override void Reason ()
		{
			foreach (var reason in reasons) {
				if (reason.ChangeState ()) {
					break;
				}
			}
		}

		public override void Act ()
		{
            if (_parallelExecution)
            {
                foreach (var action in actions)
                {
                    action.PerformAction();
                }
            }
            else
            {
                var action = actions[currentAction];
                currentAction = (currentAction + 1) % actions.Count;

                action.PerformAction();
            }
	
		}

		protected override bool OkToAct ()
		{
			return true;
		}

	}
}
