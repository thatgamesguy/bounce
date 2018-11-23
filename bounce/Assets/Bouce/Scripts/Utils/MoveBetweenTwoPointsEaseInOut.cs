using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Moves between two points at the designated speed.
    /// </summary>
    public class MoveBetweenTwoPointsEaseInOut : MonoBehaviour 
	{

		public Vector2 pos1 = new Vector3(-4, 0, 0);
		public Vector2 pos2 = new Vector3(4, 0, 0);
		public float speed;
		private bool dirRight = true;

		void Update () {
			if (dirRight)
				transform.Translate (Vector2.right * speed * Time.deltaTime);
			else
				transform.Translate (-Vector2.right * speed * Time.deltaTime);

			if(transform.position.x >= pos2.x) {
				dirRight = false;
			} else if(transform.position.x <= pos1.x) {
				dirRight = true;
			}
		}


		private IEnumerator MoveObject (Vector2 startPos, Vector2 endPos, float time) 
		{
			var i = 0.0f;
			var rate = 1.0f / time;
			while (i < 1.0f) {
				i += Time.deltaTime * rate;
				transform.position = Vector2.Lerp(startPos, endPos, i);
				yield return null; 
			}
		}   
	}
}
