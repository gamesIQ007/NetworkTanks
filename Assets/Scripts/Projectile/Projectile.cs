using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Снаряд
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        /// <summary>
        /// Свойства снаряда
        /// </summary>
        [SerializeField] private ProjectileProperties properties;
        public ProjectileProperties Properties => properties;

        /// <summary>
        /// Движение снаряда
        /// </summary>
        [SerializeField] private ProjectileMovement movement;
        public ProjectileMovement Movement => movement;

        /// <summary>
        /// Проверка попадания снаряда
        /// </summary>
        [SerializeField] private ProjectileHit hit;
        public ProjectileHit Hit => hit;

        [Space(5)]
        /// <summary>
        /// Визуальная модель
        /// </summary>
        [SerializeField] private GameObject visualModel;

        [Space(5)]
        /// <summary>
        /// Время жизни
        /// </summary>
        [SerializeField] private float lifeTime;
        /// <summary>
        /// Время перед уничтожением
        /// </summary>
        [SerializeField] private float delayBeforeDestroy;

        /// <summary>
        /// Владелец
        /// </summary>
        public NetworkIdentity Owner { get; set; }


        #region Unity Events

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            hit.Check();

            movement.Move();

            if (hit.IsHit)
            {
                OnHit();
            }
        }

        #endregion


        /// <summary>
        /// Задать свойства
        /// </summary>
        /// <param name="properties">Свойства</param>
        public void SetProperties(ProjectileProperties properties)
        {
            this.properties = properties;
        }


        /// <summary>
        /// При столкновении
        /// </summary>
        private void OnHit()
        {
            transform.position = hit.RaycastHit.point;

            if (NetworkSessionManager.Instance.IsServer)
            {
                ProjectileHitResult hitResult = hit.GetHitResult();

                if (hitResult.Type == ProjectileHitType.Penetration || hitResult.Type == ProjectileHitType.ModulePenetration
                    || (hitResult.Type == ProjectileHitType.Nopenetration && properties.Type == ProjectileType.HighExplosive))
                {
                    SvTakeDamage(hitResult);

                    SvAddFrags();
                }

                Owner.GetComponent<Player>().SvInvokeProjectileHit(hitResult);
            }

            Destroy();
        }

        /// <summary>
        /// Нанесение урона
        /// </summary>
        /// <param name="hitResult">Результат пробивания</param>
        private void SvTakeDamage(ProjectileHitResult hitResult)
        {
            hit.HitArmor.Destructible.SvApplyDamage((int)hitResult.Damage);
        }

        /// <summary>
        /// Добавление фрагов
        /// </summary>
        private void SvAddFrags()
        {
            if (hit.HitArmor.Type == ArmorType.Module) return;

            if (hit.HitArmor.Destructible.HitPoint <= 0)
            {
                if (Owner != null)
                {
                    Player player = Owner.GetComponent<Player>();

                    if (player != null)
                    {
                        player.Frags++;
                    }
                }
            }
        }

        /// <summary>
        /// Уничтожение
        /// </summary>
        private void Destroy()
        {
            visualModel.SetActive(false);
            enabled = false;

            Destroy(gameObject, delayBeforeDestroy);
        }
    }
}