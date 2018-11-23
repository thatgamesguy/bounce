using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bounce.StateMachine
{
	/// <summary>
	/// FSM transistion.
	/// </summary>
	public enum FSMTransistion
	{
		None = 0,
        Entry,
		PlayerStatusChanged,
        BallInPlay,
        BallOutOfPlay,
        AllConsumableShapesConsumed,
		Exit
	}


	/// <summary>
	/// Eash state requires a unique id.
	/// </summary>
	public enum FSMStateID
	{
		None = 0,
        Entry,
        Start,
        InProgress,
        Complete
	}

	public class FSM : MonoBehaviour
	{
		private List <FSMState> fsmStates = new List<FSMState> ();

		//private FSMStateID previousStateID;
		public FSMStateID PreviousStateID {
			get {	
				return previousState.ID;
			}
		}

		//private FSMStateID currentStateID;
		public FSMStateID CurrentStateID {
			get {
				return currentState.ID;
			}
		}

		private FSMState previousState;

		public FSMState PreviousState {
			get {
				return previousState;
			}
		}

		private FSMState currentState;

		public FSMState CurrentState {
			get {
				return currentState;
			}
		}

		private FSMState defaultState;

		
		void OnEnable ()
		{
       
			Reset (); 
		}

		void OnDisable ()
		{
			if (currentState != null)
				currentState.Exit ();
		}

		public void AddState (FSMState state)
		{
			if (state == null) {
				return;
			}

			// First State inserted is also the Initial state
			//   the state the machine is in when the simulation begins
			if (fsmStates.Count == 0) {
				fsmStates.Add (state);
				currentState = state;
				defaultState = state;
				return;
			}

			// Add the state to the List if it´s not inside it
			foreach (FSMState tmpState in fsmStates) {
				if (state.ID == tmpState.ID) {
					return;
				}
			}

			//If no state in the current then add the state to the list
			fsmStates.Add (state);
		}

		public void DeleteState (FSMStateID stateID)
		{
		
			if (stateID == FSMStateID.None) {
				return;
			}

			
			// Search the List and delete the state if it´s inside it
			foreach (FSMState state in fsmStates) {
				if (state.ID == stateID) {
					fsmStates.Remove (state);
					return;
				}
			}

		}

		public void PerformTransition ( FSMTransistion trans )
		{
			// Check for NullTransition before changing the current state
			if (trans == FSMTransistion.None) {
				return;
			}
			
			// Check if the currentState has the transition passed as argument
			FSMStateID id = currentState.GetOutputState (trans);
			if (id == FSMStateID.None) {
				return;
			}

			
			// Update the currentStateID and currentState		
			//currentStateID = id;
			foreach (FSMState state in fsmStates) {
				if (state.ID == id) {
					// Store previous state and call exit method.
					previousState = currentState;
					previousState.Exit ();

					// Update current state and call enter method.
					currentState = state;
					currentState.Enter ();

					break;
				}
			}
		}

		public void ClearStates ()
		{
			fsmStates.Clear ();
		}

		public void Reset ()
		{
			currentState = defaultState;
			if (currentState != null) {
				currentState.Enter ();
			}
		}


	}
}