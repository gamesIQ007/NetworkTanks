using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение количества здоровья
    /// </summary>
    public class UIHealthText : MonoBehaviour
    {
        /// <summary>
        /// Текст
        /// </summary>
        [SerializeField] private Text text;

        /// <summary>
        /// Уничтожаемый объект
        /// </summary>
        private Destructible destructible;


        #region Unity Events

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDestroy()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

            if (destructible != null)
            {
                destructible.HitPointChange -= OnHitPointChange;
            }
        }

        #endregion


        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            destructible = vehicle;

            destructible.HitPointChange += OnHitPointChange;

            text.text = destructible.HitPoint.ToString();
        }

        private void OnHitPointChange(int hitPoint)
        {
            text.text = hitPoint.ToString();
        }
    }
}