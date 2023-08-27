using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Снаряд
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        /// <summary>
        /// Визуальная модель
        /// </summary>
        [SerializeField] private GameObject visualModel;

        /// <summary>
        /// Скорость
        /// </summary>
        [SerializeField] private float velocity;
        /// <summary>
        /// Время жизни
        /// </summary>
        [SerializeField] private float lifeTime;
        /// <summary>
        /// Масса
        /// </summary>
        [SerializeField] private float mass;

        /// <summary>
        /// Урон
        /// </summary>
        [SerializeField] private float damage;
        /// <summary>
        /// Разброс урона
        /// </summary>
        [Range(0.0f, 1.0f)]
        [SerializeField] private float damageScatter;

        /// <summary>
        /// Сила, прикладываемая к объекту попадания
        /// </summary>
        [SerializeField] private float impactForce;

        /// <summary>
        /// Небольшое увеличение длины рейкаста для увеличения точности
        /// </summary>
        private const float RayAdvance = 1.1f;


        #region Unity Events

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            UpdateProjectile();
        }

        #endregion


        /// <summary>
        /// Обновление снаряда
        /// </summary>
        private void UpdateProjectile()
        {
            transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(Time.deltaTime * mass)).normalized;

            Vector3 step = transform.forward * velocity * Time.deltaTime;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, velocity * Time.deltaTime * RayAdvance))
            {
                transform.position = hit.point;

                var destructible = hit.transform.root.GetComponent<Destructible>();

                if (destructible)
                {
                    if (NetworkSessionManager.Instance.IsServer)
                    {
                        float dmg = damage + Random.Range(-damageScatter, damageScatter) * damage;

                        destructible.SvApplyDamage((int)dmg);
                    }
                }

                OnProjectileLifeEnd(hit.collider, hit.point, hit.normal);

                return;
            }

            transform.position += step;
        }

        /// <summary>
        /// При окончании жизни снаряда
        /// </summary>
        /// <param name="col">Коллайдер</param>
        /// <param name="pos">Позиция</param>
        /// <param name="normal">Нормаль</param>
        private void OnProjectileLifeEnd(Collider col, Vector3 pos, Vector3 normal)
        {
            visualModel.SetActive(false);
            enabled = false;
        }
    }
}