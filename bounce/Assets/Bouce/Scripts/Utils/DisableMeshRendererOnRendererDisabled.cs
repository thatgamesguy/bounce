using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Disables an object mesh renderer if the specified sprite renderer is also disabled.
    /// </summary>
    public class DisableMeshRendererOnRendererDisabled : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        private MeshRenderer m_MeshRenderer;

        void Awake()
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
        }
     
        void LateUpdate()
        {
			if( spriteRenderer.enabled && spriteRenderer.color.a > 0 )
            {
                m_MeshRenderer.enabled = true;
            }
			else if( !spriteRenderer.enabled || spriteRenderer.color.a == 0 )
            {
                m_MeshRenderer.enabled = false;
            }
        }
    }
}
