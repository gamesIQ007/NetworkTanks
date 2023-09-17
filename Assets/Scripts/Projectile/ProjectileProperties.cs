using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Тип снаряда
    /// </summary>
    public enum ProjectileType
    {
        /// <summary>
        /// Бронебойные
        /// </summary>
        ArmorPiercing,
        /// <summary>
        /// Фугасный
        /// </summary>
        HighExplosive,
        /// <summary>
        /// Подкалиберный
        /// </summary>
        Subcaliber
    }

    [CreateAssetMenu]
    /// <summary>
    /// Свойства снаряда
    /// </summary>
    public class ProjectileProperties : ScriptableObject
    {
        /// <summary>
        /// Тип снаряда
        /// </summary>
        [SerializeField] private ProjectileType type;
        public ProjectileType Type => type;

        [Header("Common")]
        /// <summary>
        /// Префаб снаряда
        /// </summary>
        [SerializeField] private Projectile projectilePrefab;
        public Projectile ProjectilePrefab => projectilePrefab;

        /// <summary>
        /// Иконка
        /// </summary>
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;

        [Header("Movement")]
        /// <summary>
        /// Скорость
        /// </summary>
        [SerializeField] private float velocity;
        public float Velocity => velocity;

        /// <summary>
        /// Масса
        /// </summary>
        [SerializeField] private float mass;
        public float Mass => mass;

        /// <summary>
        /// Сила, прикладываемая к объекту попадания
        /// </summary>
        [SerializeField] private float impactForce;
        public float ImpactForce => impactForce;

        [Header("Damage")]
        /// <summary>
        /// Урон
        /// </summary>
        [SerializeField] private float damage;
        public float Damage => damage;

        /// <summary>
        /// Разброс урона
        /// </summary>
        [Range(0.0f, 1.0f)]
        [SerializeField] private float damageSpread;
        public float DamageSpred => damageSpread;

        [Header("Caliber")]
        /// <summary>
        /// Калибр
        /// </summary>
        [SerializeField] private float caliber;
        public float Caliber => caliber;

        [Header("Armor Penetration")]
        /// <summary>
        /// Пробивание брони
        /// </summary>
        [SerializeField] private float armorPenetration;
        public float ArmorPenetration => armorPenetration;

        /// <summary>
        /// Разброс пробивания
        /// </summary>
        [Range(0.0f, 1.0f)]
        [SerializeField] private float armorPenetrationSpread;
        public float ArmorPenetrationSpread => armorPenetrationSpread;

        /// <summary>
        /// Угол нормализации
        /// </summary>
        [Range(0.0f, 90.0f)]
        [SerializeField] private float normalizationAngle;
        public float NormalizationAngle => normalizationAngle;

        /// <summary>
        /// Угол рикошета
        /// </summary>
        [Range(0.0f, 90.0f)]
        [SerializeField] private float ricochetAngle;
        public float RicochetAngle => ricochetAngle;


        /// <summary>
        /// Получить урон, в зависимости от разброса
        /// </summary>
        /// <returns>Урон</returns>
        public float GetSpreadDamage()
        {
            return damage * Random.Range(1 - damageSpread, 1 + damageSpread);
        }

        /// <summary>
        /// Получить пробивание, в зависимости от разброса
        /// </summary>
        /// <returns>Пробивание</returns>
        public float GetSpreadArmorPenetration()
        {
            return armorPenetration * Random.Range(1 - armorPenetrationSpread, 1 + armorPenetrationSpread);
        }
    }
}