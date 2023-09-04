using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение количества патронов
    /// </summary>
    public class UIAmmoText : MonoBehaviour
    {
        /// <summary>
        /// Текст
        /// </summary>
        [SerializeField] private Text text;

        /// <summary>
        /// Изображения снарядов
        /// </summary>
        [SerializeField] private Image[] images;

        /// <summary>
        /// Турель
        /// </summary>
        private Turret turret;


        #region Unity Events

        private void Start()
        {
            NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;
            }

            if (turret != null)
            {
                turret.AmmoChanged -= OnAmmoChanged;
            }
        }

        #endregion


        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            turret = vehicle.Turret;

            turret.AmmoChanged += OnAmmoChanged;

            text.text = turret.AmmoCount.ToString();

            for (int i = 0; i < images.Length; i++)
            {
                images[i].enabled = i == turret.IndexActiveWeapon;
            }
        }

        private void OnAmmoChanged(int ammo)
        {
            text.text = Player.Local.ActiveVehicle.Turret.AmmoCount.ToString();

            for (int i = 0; i < images.Length; i++)
            {
                images[i].enabled = i == turret.IndexActiveWeapon;
            }
        }
    }
}