using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Область сферы
    /// </summary>
    public class SphereArea : MonoBehaviour
    {
        /// <summary>
        /// Радиус
        /// </summary>
        [SerializeField] private float radius;

        /// <summary>
        /// Цвет
        /// </summary>
        [SerializeField] private Color color = Color.green;

        /// <summary>
        /// Случайная точка внутри сферы
        /// </summary>
        public Vector3 RandomInside
        {
            get
            {
                var pos = Random.insideUnitSphere * radius + transform.position;
                pos.y = transform.position.y;
                return pos;
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}