using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(Projectile))]

    /// <summary>
    /// Движение снаряда
    /// </summary>
    public class ProjectileMovement : MonoBehaviour
    {
        /// <summary>
        /// Снаряд
        /// </summary>
        private Projectile projectile;

        /// <summary>
        /// Шаг перемещения
        /// </summary>
        private Vector3 step;


        private void Awake()
        {
            projectile = GetComponent<Projectile>();

            step = new Vector3();
        }


        /// <summary>
        /// Движение
        /// </summary>
        public void Move()
        {
            transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * projectile.Properties.Mass)).normalized;

            step = transform.forward * projectile.Properties.Velocity * Time.deltaTime;

            transform.position += step;
        }
    }
}