using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Moves between two points at the designated speed in a pendulum motion (slow down at paths edges).
    /// </summary>
    public class MoveBetweenTwoPointsPendulum : MonoBehaviour 
	{
		public Vector2 pos1 = new Vector3( 0,0 );
		public Vector2 pos2 = new Vector3( 0,0 );

		public float speed = 1.0f;

		void Update() 
		{
			transform.position = Vector2.Lerp ( pos1, pos2, ( Mathf.Sin( speed * Time.time ) + 1.0f ) / 2.0f );
		}
	}
}