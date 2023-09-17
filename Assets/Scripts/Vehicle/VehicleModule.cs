using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Модуль транспорта
    /// </summary>
    public class VehicleModule : Destructible
    {
        /// <summary>
        /// Название
        /// </summary>
        [SerializeField] private string title;
        /// <summary>
        /// Броня
        /// </summary>
        [SerializeField] private Armor armor;
        /// <summary>
        /// Время восстановления
        /// </summary>
        [SerializeField] private float recoveredTime;

        /// <summary>
        /// Осталось времени до восстановления
        /// </summary>
        private float remainingRecoveryTime;


        private void Awake()
        {
            armor.SetDestrucible(this);
        }

        private void Start()
        {
            Destroyed += OnModuleDestroyed;
            enabled = false;
        }

        private void OnDestroy()
        {
            Destroyed -= OnModuleDestroyed;
        }

        private void Update()
        {
            if (isServer)
            {
                remainingRecoveryTime -= Time.deltaTime;

                if (remainingRecoveryTime <= 0)
                {
                    remainingRecoveryTime = 0.0f;

                    SvRecovery();

                    enabled = false;
                }
            }
        }


        /// <summary>
        /// При уничтожении модуля
        /// </summary>
        /// <param name="destructible">Дестрактибл</param>
        private void OnModuleDestroyed(Destructible destructible)
        {
            remainingRecoveryTime = recoveredTime;
            enabled = true;
        }
    }
}