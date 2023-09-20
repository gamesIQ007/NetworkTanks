using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение информации о танке
    /// </summary>
    public class UITankInfo : MonoBehaviour
    {
        /// <summary>
        /// Слайдер здоровья
        /// </summary>
        [SerializeField] private UIHealthSlider healthSlider;

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


        /// <summary>
        /// Задать танк
        /// </summary>
        /// <param name="tank">Танк</param>
        public void SetTank(Vehicle tank)
        {
            this.tank = tank;

            healthSlider.Init(tank, tank.TeamID, Player.Local.TeamID);
        }
    }
}