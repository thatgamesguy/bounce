using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Responsible for updating the shapes view and model.
    /// </summary>
	public class ShapeControl : MonoBehaviour
	{
	    private IShapeView m_ShapeView;

	    void Awake()
	    {
	        m_ShapeView = GetComponent<IShapeView>();
	    }

		void Start()
		{
			if( m_ShapeView != null )
			{
				m_ShapeView.SetInteractable( false );
			}

		}
			
		public void Reset()
		{
			if ( m_ShapeView != null )
			{
				m_ShapeView.SetInteractable( true );
				m_ShapeView.Show();
				m_ShapeView.PlayShowAnimation();
			}
		}

		public void Disable()
		{
			if( m_ShapeView != null )
			{
				m_ShapeView.SetInteractable( false );
				m_ShapeView.Hide();
			}
		}

	    protected void OnHit()
	    {
	        if( m_ShapeView != null )
	        {
				m_ShapeView.PlayHitAnimation();
	        }
	    }

	    protected void OnHitAnimationStarted()
	    {
	        if ( m_ShapeView != null )
	        {
	            m_ShapeView.Show();
                m_ShapeView.SetInteractable( false );
            }
	    }

	    protected void OnHitAnimationFinished()
	    {
	        if (m_ShapeView != null)
	        {
	            m_ShapeView.Hide();
	        }
	    }
	}
}
