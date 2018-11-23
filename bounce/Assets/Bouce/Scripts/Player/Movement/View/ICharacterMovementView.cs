using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Implement by any object that will be responsible for displaying the character.
    /// </summary>
    public interface ICharacterMovementView
	{
	    void Show();
	    void Hide();

		bool IsVisible();
	}
}