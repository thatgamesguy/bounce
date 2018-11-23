using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bounce.StateMachine
{
	/// <summary>
	/// Abstract base class for a state
	/// Provides functionality for storing and retrieving transitions between states.
	/// </summary>
	public abstract class FSMState
	{
		// Each state has an ID that is used to identify the state to transition to.
		protected FSMStateID stateID;

		public FSMStateID ID { get { return stateID; } }

		// Stores the transition and the stateid of the state to transistion to.
		protected Dictionary<FSMTransistion, FSMStateID> map = new Dictionary<FSMTransistion, FSMStateID> ();

		public FSMState ()
		{

		}

		/// <summary>
		/// Called when entering the state (before Reason and Act). 
		/// Place any initialisation code here.
		/// </summary>
		public abstract void Enter ();

		/// <summary>
		/// Called when leaving a state. 
		/// Place any clean-up code here.
		/// </summary>
		public abstract void Exit ();

		/// <summary>
		/// Decides if the state should transition to another on its list.
		/// While the state is active this method is called each time step.
		/// </summary>
		public abstract void Reason ();

		/// <summary>
		/// Place the states implementation of the behaviour in this method.
		/// While the state is active this method is called each time step.
		/// </summary>
		public abstract void Act ();

		/// <summary>
		/// When implemented should return true when it is ok for the character to perform the actions in the Act method.
		/// Place behaviour specfic tests in this method.
		/// </summary>
		protected virtual bool OkToAct ()
	    {
	        return true;
	    }


		/// <summary>
		/// Adds a transition, stateID pair to the transition map. Every transition called 
		/// in the states Reason method should have a corresponding state id in the map.
		/// </summary>
		public void AddTransition (FSMTransistion transition, FSMStateID id)
		{
			// Ensure valid transition.
			if (transition == FSMTransistion.None || id == FSMStateID.None) {
				return;
			}

			// Ensure transition not already present in map.
			if (map.ContainsKey (transition)) {
				return;
			}
			
			map.Add (transition, id);
		}

		/// <summary>
		/// Deletes a transition, stateID pair from the transition map.
		/// </summary>
		public void DeleteTransition (FSMTransistion transition)
		{
			// Ensures valid transition.
			if (transition == FSMTransistion.None) {
				return;
			}
			
			// Ensure map contains transition before attempting delete.
			if (map.ContainsKey (transition)) {
				map.Remove (transition);
				return;
			}
		}

		public void ClearCurrentTransistions ()
		{
			map.Clear ();
		}

		/// <summary>
		/// This method returns the state that would be transitioned into based on the FSMTransition.
		/// </summary>
		public FSMStateID GetOutputState (FSMTransistion transition)
		{
			// Ensures valid transition.
			if (transition == FSMTransistion.None) {
				return FSMStateID.None;
			}
			
			// Ensure map contains transition before returning new state.
			if (map.ContainsKey (transition)) {
				return map [transition];
			}

			// Transition not in map.
			return FSMStateID.None;
			
		}

						
				
	}
}