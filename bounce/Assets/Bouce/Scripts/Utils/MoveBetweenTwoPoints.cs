using UnityEngine;
using System.Collections;

namespace Bounce
{
    /// <summary>
    /// Moves between two points at the designated speed.
    /// </summary>
    public class MoveBetweenTwoPoints : MonoBehaviour
    {
        public Vector2 pos1 = new Vector3(-4, 0, 0);
        public Vector2 pos2 = new Vector3(4, 0, 0);
        public float speed = 1.0f;

        void Update()
        {
			transform.position = Vector3.Lerp (pos1, pos2, Mathf.PingPong(Time.time*speed, 1.0f));
        }
    }
}
