using UnityEngine;
using System.Collections;
using Bounce.EventManagement;

namespace Bounce
{
    /// <summary>
    /// Base character control class. Responsible for managing the characters view and model.
    /// </summary>
	public class CharacterControl : MonoBehaviour
	{
	    private ICharacterMovementModel m_MovementModel;
	    private ICharacterMovementView m_MovementView;
		private ICharacterInteractionModel m_InteractionModel;
        private ICharacterTailView m_TailView;

	    protected virtual void Awake()
	    {
	        m_MovementModel = GetComponent<ICharacterMovementModel>();
	        m_MovementView = GetComponent<ICharacterMovementView>();
			m_InteractionModel = GetComponent<ICharacterInteractionModel>();
            m_TailView = GetComponent<ICharacterTailView>();
	    }

        protected virtual void Start()
        {
            if( m_TailView != null )
            {
                m_TailView.HideTail();
            }
        }

		protected virtual void Update()
		{
			if( IsMoving() && !IsVisible() )
			{
				Hide();
				RaisePlayerStatusChangeEvent( PlayerStatus.OffScreen );
			}

		}

		private bool IsMoving()
		{
			return m_MovementModel == null ? false : m_MovementModel.IsMoving();
		}

		private bool IsVisible()
		{
			return m_MovementView == null ? false : m_MovementView.IsVisible();
		}

	    protected void SetDirection( Vector2 dir )
	    {
			if ( m_MovementModel != null )
			{
				m_MovementModel.SetDirection( dir );
			}

			if( m_InteractionModel != null )
			{
				m_InteractionModel.EnableInteraction( m_MovementModel );
			}

            if ( m_TailView != null )
            {
                m_TailView.ShowTail();
            }
        }


	    protected void SetLocation( Vector2 point )
	    {
	        if( m_MovementModel != null )
	        {
	            m_MovementModel.StopMovement();
	            m_MovementModel.Place( point );
	        }

            if( m_MovementView != null )
	        {
   
                m_MovementView.Show();
	        }

            if ( m_TailView != null )
            {
                m_TailView.HideTail();
            }
        }

	    protected void Show( )
	    {
	        if( m_MovementView != null )
	        {
	            m_MovementView.Show();
	        }
        }

	    protected void Hide()
	    {
	        if( m_MovementView != null )
	        {
	            m_MovementView.Hide();
	        }

	        if(m_MovementModel != null)
	        {
	            m_MovementModel.StopMovement();
	        }

			if( m_InteractionModel != null )
			{
				m_InteractionModel.DisableInteraction();
			}
        }

		protected void RaisePlayerStatusChangeEvent( PlayerStatus status )
		{
			Events.instance.Raise( new PlayerStatusChangeEvent( status ) );
		}
			
	}
}