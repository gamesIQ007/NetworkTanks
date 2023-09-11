using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(Projectile))]

    /// <summary>
    /// Проверка попадания снаряда
    /// </summary>
    public class ProjectileHit : MonoBehaviour
    {
        /// <summary>
        /// Небольшое увеличение длины рейкаста для увеличения точности
        /// </summary>
        private const float RAY_ADVANCE = 1.1f;

        /// <summary>
        /// Снаряд
        /// </summary>
        private Projectile projectile;

        /// <summary>
        /// Есть столкновение?
        /// </summary>
        private bool isHit;
        public bool IsHit => isHit;

        /// <summary>
        /// Дестрактибл, по которому попало
        /// </summary>
        private Destructible hitDestructible;
        public Destructible HitDestructible => hitDestructible;

        /// <summary>
        /// Попадание рейкаста
        /// </summary>
        private RaycastHit raycastHit;
        public RaycastHit RaycastHit => raycastHit;



        private void Awake()
        {
            projectile = GetComponent<Projectile>();
        }


        /// <summary>
        /// Проверка
        /// </summary>
        public void Check()
        {
            if (isHit) return;

            if (Physics.Raycast(transform.position, transform.forward, out raycastHit, projectile.Properties.Velocity * Time.deltaTime * RAY_ADVANCE))
            {
                var destructible = raycastHit.transform.root.GetComponent<Destructible>();

                if (destructible)
                {
                    hitDestructible = destructible;
                }

                isHit = true;
            }
        }
    }
}