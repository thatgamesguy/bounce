using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Implement by any object that will be responsible for drawing the players tail.
    /// </summary>
    public interface ICharacterTailView 
    {
        void HideTail();
        void ShowTail();
    }
}
