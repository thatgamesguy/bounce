using UnityEngine;
using System.Collections;
using Bounce.CoroutineManager;
using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Responsible for playing menu animations and showing/hiding the additional menu.
    /// </summary>
	public class MainMenuView : MonoBehaviour, IMainMenuView 
	{	
		public float secondsBeforeButtonAnimationStarts;
		public float secondsBetweenButtonAnimations;
		public ButtonAnimationController[] buttonAnimations;	
		
		private static readonly int SHOW_HASH = Animator.StringToHash( "Show" );
		private static readonly int HIDE_HASH = Animator.StringToHash( "Hide" );
		private static readonly int SHOW_ADDITIONAL_HASH = Animator.StringToHash( "ShowAdditional" );
		private static readonly int HIDE_ADDITIONAL_HASH = Animator.StringToHash( "HideAdditional" );

		private Animator m_Animator;

		private bool m_IsVisible;
		private bool m_IsAdditionalVisible;

		private CM_Job m_ButtonBounceJob;

		void Awake()
		{
			m_Animator = GetComponent<Animator>();
		}

		void Start()
		{
			m_ButtonBounceJob = CM_Job.Make ( PlayButtonBounceAnimation() ).Repeatable();
		}

		public bool IsVisible()
		{
			return m_IsVisible;
		}

		public bool IsAdditionalVisible()
		{
			return m_IsAdditionalVisible;
		}

		public void Show()
		{
			m_Animator.SetTrigger( SHOW_HASH );

			m_ButtonBounceJob.Start();

			m_IsVisible = true;

			Events.instance.Raise( new MenuStatusChangedEvent( IsVisible (), IsAdditionalVisible() ) );
		}

		public void Hide()
		{
			m_Animator.SetTrigger( HIDE_HASH );
			m_IsVisible = false;
			m_IsAdditionalVisible = false;

			Events.instance.Raise( new MenuStatusChangedEvent( IsVisible (), IsAdditionalVisible() ) );
		}

        public void ShowAdditional()
		{
			m_Animator.SetTrigger( SHOW_ADDITIONAL_HASH );
			m_IsAdditionalVisible = true;

			Events.instance.Raise( new MenuStatusChangedEvent( IsVisible (), IsAdditionalVisible() ) );
		}

		public void HideAdditional()
		{
			m_Animator.SetTrigger( HIDE_ADDITIONAL_HASH );
			m_IsVisible = false;
			m_IsAdditionalVisible = false;

			Events.instance.Raise( new MenuStatusChangedEvent( IsVisible (), IsAdditionalVisible() ) );
		}

		private IEnumerator PlayButtonBounceAnimation()
		{
			yield return new WaitForSeconds( secondsBeforeButtonAnimationStarts );
			
			var wait = new WaitForSeconds( secondsBetweenButtonAnimations );

			foreach( var animation in buttonAnimations )
			{
				animation.PlayBouceAnimation();

				yield return wait;
			}

		}
	}
}