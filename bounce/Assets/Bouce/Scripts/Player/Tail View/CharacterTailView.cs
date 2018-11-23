using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Draws the players tail. Disables players tail when player not visible.
    /// </summary>
    public class CharacterTailView : MonoBehaviour, ICharacterTailView
    {
        private static readonly float TAIL_ON_TIME = 1f;

        private TrailRenderer m_TrailRenderer;

        void Awake()
        {
            m_TrailRenderer = GetComponent<TrailRenderer>();
        }

        public void HideTail()
        {
            m_TrailRenderer.time = -TAIL_ON_TIME;
        }

        public void ShowTail()
        {
            m_TrailRenderer.time = TAIL_ON_TIME;
        }
    }
}
