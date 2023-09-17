using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Тип попадания
    /// </summary>
    public enum ProjectileHitType
    {
        /// <summary>
        /// Пробил
        /// </summary>
        Penetration,
        /// <summary>
        /// Не пробил
        /// </summary>
        Nopenetration,
        /// <summary>
        /// Рикошет
        /// </summary>
        Ricochet,
        /// <summary>
        /// Пробил по модулю
        /// </summary>
        ModulePenetration,
        /// <summary>
        /// Не пробил по модулю
        /// </summary>
        ModuleNoPenetration,
        /// <summary>
        /// Попадение в окружение
        /// </summary>
        Environment
    }


    /// <summary>
    /// Результат пробивания
    /// </summary>
    public class ProjectileHitResult
    {
        /// <summary>
        /// Тип попадания
        /// </summary>
        public ProjectileHitType Type;
        /// <summary>
        /// Урон
        /// </summary>
        public float Damage;
        /// <summary>
        /// Точка попадания
        /// </summary>
        public Vector3 Point;
    }


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
        /// Броня, по которой попали
        /// </summary>
        private Armor hitArmor;
        public Armor HitArmor => hitArmor;

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
                Armor armor = raycastHit.collider.GetComponent<Armor>();

                if (armor != null)
                {
                    hitArmor = armor;
                }

                isHit = true;
            }
        }

        /// <summary>
        /// Получить результат попадания
        /// </summary>
        /// <returns>Результат попадания</returns>
        public ProjectileHitResult GetHitResult()
        {
            ProjectileHitResult hitResult = new ProjectileHitResult();
            hitResult.Damage = 0;

            if (hitArmor == null)
            {
                hitResult.Type = ProjectileHitType.Environment;
                hitResult.Point = raycastHit.point;
                return hitResult;
            }

            float normalization = projectile.Properties.NormalizationAngle;

            if (projectile.Properties.Caliber > hitArmor.Thickness * 2)
            {
                normalization = (projectile.Properties.NormalizationAngle * 1.4f * projectile.Properties.Caliber) / hitArmor.Thickness;
            }

            float angle = Mathf.Abs(Vector3.SignedAngle(-projectile.transform.forward, raycastHit.normal, projectile.transform.right)) - normalization;
            float reducedArmor = hitArmor.Thickness / Mathf.Cos(angle * Mathf.Deg2Rad);
            float projectilePenetration = projectile.Properties.GetSpreadArmorPenetration();

            // Визуализация попадания
            Debug.DrawRay(raycastHit.point, -projectile.transform.forward, Color.red);
            Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.green);
            Debug.DrawRay(raycastHit.point, projectile.transform.right, Color.yellow);

            if (angle > projectile.Properties.RicochetAngle && projectile.Properties.Caliber < hitArmor.Thickness * 3 && hitArmor.Type == ArmorType.Vehicle)
            {
                hitResult.Type = ProjectileHitType.Ricochet;
            }
            else
            {
                if (projectilePenetration >= reducedArmor)
                {
                    hitResult.Type = ProjectileHitType.Penetration;

                    hitResult.Damage = projectile.Properties.GetSpreadDamage();
                }
                else
                {
                    if (projectilePenetration < reducedArmor)
                    {
                        hitResult.Type = ProjectileHitType.Nopenetration;

                        if (projectile.Properties.Type == ProjectileType.HighExplosive)
                        {
                            hitResult.Type = ProjectileHitType.Penetration;

                            hitResult.Damage = projectile.Properties.GetSpreadDamage();
                        }
                    }
                }
            }

            Debug.Log($"Armor: {hitArmor.Thickness}, reducedArmor: {reducedArmor}, angle: {angle}, normalization: {normalization}, penetration: {projectilePenetration}, type: {hitResult.Type}");

            if (hitArmor.Type == ArmorType.Module)
            {
                if (hitResult.Type == ProjectileHitType.Penetration)
                {
                    hitResult.Type = ProjectileHitType.ModulePenetration;
                }
                if (hitResult.Type == ProjectileHitType.Nopenetration)
                {
                    hitResult.Type = ProjectileHitType.ModuleNoPenetration;
                }
            }

            hitResult.Point = raycastHit.point;

            return hitResult;
        }
    }
}