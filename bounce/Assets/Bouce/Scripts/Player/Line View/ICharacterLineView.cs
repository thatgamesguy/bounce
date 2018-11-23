using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Implemented by any object responsible for showing the line when a player drags.
    /// </summary>
    public interface ICharacterLineView 
    {
        void SetStart( Vector2 start );
        void SetEnd( Vector2 end );
        void Show();
        void Hide();
    }
}