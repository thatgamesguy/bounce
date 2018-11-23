using UnityEngine;
using System.Collections;

namespace Bounce.StateMachine
{
public abstract class FSMAction
{
    public abstract void PerformAction();
    protected virtual bool OkToAct(){ return true; }
    public abstract void Enter();
    public abstract void Exit();

}

}