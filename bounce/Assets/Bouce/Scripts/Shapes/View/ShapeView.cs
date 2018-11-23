using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Responsible for showing and hiding a shape.
    /// </summary>
	public class ShapeView : MonoBehaviour, IShapeView
	{
	    public Animator animator;
	    public SpriteRenderer spriteRenderer;

	    private static readonly int HIT_HASH = Animator.StringToHash( "Hit" );
	    private static readonly int SHOW_HASH = Animator.StringToHash( "Show" );

	    private Collider2D m_Collider2D;

	    void Awake()
	    {
	        m_Collider2D = GetComponent<Collider2D>();
	    }

	    public void Show()
	    {
	        spriteRenderer.enabled = true;
	    }

	    public void Hide()
	    {
	        spriteRenderer.enabled = false;
	    }

		public void SetInteractable( bool interactable )
		{
			m_Collider2D.enabled = interactable;
		}

		public void PlayShowAnimation()
		{
			animator.SetTrigger( SHOW_HASH );	
		}
			
	    public void PlayHitAnimation()
	    {
	        animator.SetTrigger( HIT_HASH );
	    }
	}
}