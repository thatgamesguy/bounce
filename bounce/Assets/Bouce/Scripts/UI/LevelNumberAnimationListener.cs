using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Called during the level number transition animation.
    /// </summary>
    public class LevelNumberAnimationListener : MonoBehaviour
    {
        public LevelNumberAnimator levelNumberAnimator;

        public void OnAnimationComplete()
        {
            levelNumberAnimator.OnAnimationOver();
        }

    }
}
