using Bounce.StateMachine;

namespace Bounce
{
    /// <summary>
    /// Implemented by any class that can perform transitions between FSM states.
    /// </summary>
    public interface IStateTransitioner 
    {
        void SetTransistion( FSMTransistion transition );
    }
}