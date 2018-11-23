using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Draws line when player drags.
    /// </summary>
    public class CharacterLineView : MonoBehaviour, ICharacterLineView
    {
		public Material lineMaterial;
		public Color startColour;
		public Color endColour;
        private LineRenderer m_LineRenderer;

        void Awake()
        {
            m_LineRenderer = gameObject.AddComponent<LineRenderer>() as LineRenderer;
        }

        void Start()
        {
			m_LineRenderer.material = lineMaterial;
			m_LineRenderer.SetColors ( startColour, endColour );
            m_LineRenderer.SetWidth(0.2f, 0.2f);
            m_LineRenderer.SetVertexCount(2);
        }

        public void SetStart( Vector2 start )
        {
            m_LineRenderer.SetPosition( 0, start );
        }

        public void SetEnd( Vector2 end )
        {
            m_LineRenderer.SetPosition( 1, end );
        }

        public void Show()
        {
            m_LineRenderer.enabled = true;
        }

        public void Hide()
        {
            m_LineRenderer.enabled = false;
        }

    }
}