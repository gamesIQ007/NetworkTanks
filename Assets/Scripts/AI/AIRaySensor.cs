using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// ИИ определения препятствий
    /// </summary>
    public class AIRaySensor : MonoBehaviour
    {
        /// <summary>
        /// Лучи
        /// </summary>
        [SerializeField] private Transform[] rays;

        /// <summary>
        /// Длина лучей
        /// </summary>
        [SerializeField] private float raycastDistance;
        public float RaycastDistance => raycastDistance;

        /// <summary>
        /// Рейкаст
        /// </summary>
        /// <returns>Есть препятствие? На каком расстоянии?</returns>
        public (bool, float) Raycast()
        {
            float dist = -1;

            foreach (var v in rays)
            {
                RaycastHit hit;

                if (Physics.Raycast(v.position, v.forward, out hit, raycastDistance))
                {
                    if (dist < 0 || hit.distance < dist)
                    {
                        dist = hit.distance;
                    }
                }
            }

            return (dist > 0, dist);
        }


        private void OnDrawGizmos()
        {
            foreach (var v in rays)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(v.position, v.position + v.forward * raycastDistance);
            }
        }
    }
}