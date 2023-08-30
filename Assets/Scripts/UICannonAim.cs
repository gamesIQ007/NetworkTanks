using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Интерфейс прицеливания пушки
    /// </summary>
    public class UICannonAim : MonoBehaviour
    {
        /// <summary>
        /// Изображение прицела
        /// </summary>
        [SerializeField] private Image aim;

        /// <summary>
        /// Слайдер перезарядки
        /// </summary>
        [SerializeField] private Image reloadSlider;

        /// <summary>
        /// Скорость смещения прицела
        /// </summary>
        [SerializeField] private float interpolationSpeed;

        /// <summary>
        /// Позиция прицела
        /// </summary>
        private Vector3 aimPosition;


        private void Update()
        {
            if (Player.Local == null) return;
            if (Player.Local.ActiveVehicle == null) return;

            Vehicle v = Player.Local.ActiveVehicle;

            reloadSlider.fillAmount = v.Turret.FireTimeNormalized;

            aimPosition = VehicleInputControl.TraceAimPointWithoutPlayerVehicle(v.Turret.LaunchPoint.position, v.Turret.LaunchPoint.forward);

            Vector3 result = Camera.main.WorldToScreenPoint(aimPosition);

            if (result.z > 0)
            {
                result.z = 0;

                //aim.transform.position = result;
                aim.transform.position = Vector3.Lerp(aim.transform.position, result, Time.deltaTime * interpolationSpeed);
            }
        }
    }
}