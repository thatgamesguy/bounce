using UnityEngine;
using System.Collections;

namespace Bounce.StateMachine
{
    public abstract class FSMReason
    {
        public FSMTransistion transition { get; private set; }
        public FSMStateID goToState { get; private set; }

        private IStateTransitioner m_Controller;
        private FSMStateSwitchLogger m_DebugLogger;

        public FSMReason( FSMTransistion transition, FSMStateID goToState, IStateTransitioner controller )
        {
            this.transition = transition;
            this.goToState = goToState;
            m_Controller = controller;

            m_DebugLogger = new FSMStateSwitchLogger(this);
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }


        public abstract bool ChangeState();

        protected void PerformTransition( bool log = true )
        {
            if( log )
            {
                LogTransisiton();
            }

            m_Controller.SetTransistion(transition);
        }

        protected void LogTransisiton()
        {
            m_DebugLogger.Log();
        }
    }

}