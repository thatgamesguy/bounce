using UnityEngine;
using UnityEngine.UI;

namespace Bounce
{
    /// <summary>
    /// Resets scroll rect position on game start.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectPosition : MonoBehaviour
    {
        private ScrollRect m_ScrollRect;

        void Awake()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
        }

        void Start()
        {
            m_ScrollRect.verticalNormalizedPosition = 1;
        }
    }
}
