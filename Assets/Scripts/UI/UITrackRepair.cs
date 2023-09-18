using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Информация о ремонте трака
    /// </summary>
    public class UITrackRepair : MonoBehaviour
    {
        /// <summary>
        /// Слайдер с ходом ремонта
        /// </summary>
        [SerializeField] private Slider slider;

        /// <summary>
        /// Смещение панели с информацией
        /// </summary>
        [SerializeField] private Vector3 worldOffset;
        public Vector3 WorldOffset => worldOffset;

        /// <summary>
        /// Танк
        /// </summary>
        private Vehicle tank;
        public Vehicle Tank => tank;



        private void Update()
        {
            slider.value += Time.deltaTime;

            if (slider.value >= slider.maxValue)
            {
                Destroy(gameObject);
            }
        }


        /// <summary>
        /// Задать время ремонта
        /// </summary>
        /// <param name="repairTime">Время ремонта</param>
        public void SetRepairInfo(float repairTime)
        {
            slider.maxValue = repairTime;
        }

        /// <summary>
        /// Задать танк
        /// </summary>
        /// <param name="tank">Танк</param>
        public void SetTank(Vehicle tank)
        {
            this.tank = tank;
        }
    }
}