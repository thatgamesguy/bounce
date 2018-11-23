using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Methods called during animation.
    /// </summary>
    public class InteractableAnimationListener : MonoBehaviour
    {
        private IInteractable m_Interactable;
    
	    void Awake ()
        {
            m_Interactable = transform.parent.parent.GetComponent<IInteractable>();
	    }
	
	    public void OnAnimationStart()
        {
            if ( m_Interactable != null )
            {
                m_Interactable.OnAnimationStarted();
            }
        }

        public void OnAnimationFinished()
        {
            if( m_Interactable != null )
            {
                m_Interactable.OnAnimationFinished();
            }
        }
    }
}