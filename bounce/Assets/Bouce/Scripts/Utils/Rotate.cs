using UnityEngine;

namespace Bounce
{
    /// <summary>
    /// Rotates shape by speed.
    /// </summary>
    public class Rotate : MonoBehaviour
    {
        public float speed;
        public float direction = -1;

        void Update()
        {
            transform.Rotate(Vector3.forward * direction * (speed * Time.deltaTime));
        }
    }
}
