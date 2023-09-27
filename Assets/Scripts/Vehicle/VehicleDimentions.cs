using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(Vehicle))]

    /// <summary>
    /// Габариты транспорта
    /// </summary>
    public class VehicleDimentions : MonoBehaviour
    {
        /// <summary>
        /// Точки
        /// </summary>
        [SerializeField] private Transform[] points;

        /// <summary>
        /// Приоритетные точки выстрела
        /// </summary>
        [SerializeField] private Transform[] priorityFirePoints;

        /// <summary>
        /// Транспорт
        /// </summary>
        private Vehicle vehicle;
        public Vehicle Vehicle => vehicle;

        RaycastHit[] hits = new RaycastHit[10];


        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
        }


        /// <summary>
        /// Видно ли из точки
        /// </summary>
        /// <param name="source">Источник луча</param>
        /// <param name="point">Точка</param>
        /// <param name="color">Цвет</param>
        /// <returns>Видно ли</returns>
        public bool IsVisibleFromPoint(Transform source, Vector3 point, Color color)
        {
            bool visible = true;

            for (int i = 0; i < points.Length; i++)
            {
                int length = Physics.RaycastNonAlloc(point, (points[i].position - point).normalized, hits, Vector3.Distance(point, points[i].position));

                visible = true;
                for (int j = 0; j < length; j++)
                {
                    if (hits[j].collider.transform.root == source) continue;

                    if (hits[j].collider.transform.root == transform.root) continue;

                    visible = false;
                }

                if (visible)
                {
                    return visible;
                }
            }

            return false;
        }

        /// <summary>
        /// Получить приоритетную точку выстрела
        /// </summary>
        /// <returns>Точка выстрела</returns>
        public Transform GetPriorityFirePoint()
        {
            //temp
            return priorityFirePoints[0];
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (points == null) return;

            Gizmos.color = Color.blue;
            for (int i = 0; i < points.Length; i++)
            {
                Gizmos.DrawSphere(points[i].position, 0.2f);
            }
        }
#endif
    }
}