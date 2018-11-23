using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Responsible for showing and hiding the characters sprite.
    /// </summary>
	public class CharacterMovementView : MonoBehaviour, ICharacterMovementView
	{
	    public SpriteRenderer spriteRenderer;

	    private Collider2D m_Collider;

	    void Awake()
	    {
	        m_Collider = GetComponent<Collider2D>();
	    }

	    void Start()
	    {
	        Hide();
	    }

	    public void Show()
	    {
	        spriteRenderer.enabled = true;
	        m_Collider.enabled = true;
	    }

	    public void Hide()
	    {
	        spriteRenderer.enabled = false;
	        m_Collider.enabled = false;
	    }

		public bool IsVisible()
		{
			return spriteRenderer.isVisible;
		}
	}
}