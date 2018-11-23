using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Wrapper for playing a buttons bounce animation.
    /// </summary>
	[RequireComponent( typeof( Animator ) )]
	public class ButtonAnimationController : MonoBehaviour 
	{
        private static readonly int SHOW_HASH = Animator.StringToHash( "Bounce" );

		private Animator m_Animator;

		void Awake()
		{
			m_Animator = GetComponent<Animator>();
		}

		public void PlayBouceAnimation()
		{
			m_Animator.SetTrigger( SHOW_HASH );
		}

	}
}
