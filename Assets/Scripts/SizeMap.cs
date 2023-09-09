using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Размер карты
    /// </summary>
    public class SizeMap : MonoBehaviour
    {
        /// <summary>
        /// Размер
        /// </summary>
        [SerializeField] private Vector2 size;
        public Vector2 Size => size;


        /// <summary>
        /// Получить нормализованную позицию
        /// </summary>
        /// <param name="pos">Исходная позиция</param>
        /// <returns>Нормализованная позиция</returns>
        public Vector3 GetNormPos(Vector3 pos)
        {
            return new Vector3(pos.x / size.x, 0, pos.z / size.y);
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 0, size.y));
        }
    }
}