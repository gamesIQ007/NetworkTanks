using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(Vehicle))]

    /// <summary>
    /// Маскировка транспорта
    /// </summary>
    public class VehicleCamouflage : MonoBehaviour
    {
        /// <summary>
        /// Базовая дистанция
        /// </summary>
        [SerializeField] private float baseDistance;
        [Range(0.0f, 1.0f)]
        /// <summary>
        /// Процент маскировки
        /// </summary>
        [SerializeField] private float percent;
        /// <summary>
        /// Скорость изменения процента маскировки
        /// </summary>
        [SerializeField] private float percentLerpRate;
        /// <summary>
        /// Процент маскировки при стрельбе
        /// </summary>
        [SerializeField] private float percentOnFire;

        /// <summary>
        /// Процент маскировки целевой
        /// </summary>
        private float targetPercent;

        /// <summary>
        /// Текущая дистанция маскировки
        /// </summary>
        private float currentDistance;
        public float CurrentDistance => currentDistance;

        /// <summary>
        /// Транспорт
        /// </summary>
        private Vehicle vehicle;


        private void Start()
        {
            if (NetworkSessionManager.Instance.IsServer == false) return;

            vehicle = GetComponent<Vehicle>();
            vehicle.Turret.Shot += OnShot;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                if (NetworkSessionManager.Instance.IsServer == false) return;

                vehicle.Turret.Shot -= OnShot;
            }
        }

        private void Update()
        {
            if (NetworkSessionManager.Instance.IsServer == false) return;

            if (vehicle.NormalizedLinearVelocity > 0.01f)
            {
                targetPercent = 0.5f;
            }
            if (vehicle.NormalizedLinearVelocity <= 0.01f)
            {
                targetPercent = 1.0f;
            }

            percent = Mathf.MoveTowards(percent, targetPercent, Time.deltaTime * percentLerpRate);
            percent = Mathf.Clamp01(percent);

            currentDistance = baseDistance * percent;
        }

        /// <summary>
        /// При выстреле
        /// </summary>
        private void OnShot()
        {
            percent = percentOnFire;
        }
    }
}