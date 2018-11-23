using UnityEngine;
using UnityEngine.UI;
using System;

namespace Bounce
{
    /// <summary>
    /// Animates the level numbers shown during a level transition.
    /// </summary>
    public class LevelNumberAnimator : MonoBehaviour
    {
        public Text[] numbers;
        public Animator[] animators;

        private static readonly int SHOW_HASH = Animator.StringToHash("Show");
        private static readonly int HIDE_HASH = Animator.StringToHash("Hide");

        private Action m_OnComplete;
        private int m_CurrentIndex = 0;

        public void StartAnimation(int firstNumber, int secondNumber, Action onComplete)
        {
            m_OnComplete = onComplete;

            numbers[m_CurrentIndex].text = firstNumber.ToString();
            animators[m_CurrentIndex].SetTrigger(HIDE_HASH);

            m_CurrentIndex = (m_CurrentIndex + 1) % numbers.Length;

            numbers[m_CurrentIndex].text = secondNumber.ToString();
            animators[m_CurrentIndex].SetTrigger(SHOW_HASH);

        }

        public void OnAnimationOver()
        {
            if (m_OnComplete != null)
            {
                m_OnComplete();
            }
        }
    }
}
