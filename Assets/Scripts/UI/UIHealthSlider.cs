using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение слайдера здоровья
    /// </summary>
    public class UIHealthSlider : MonoBehaviour
    {
        /// <summary>
        /// Слайдер
        /// </summary>
        [SerializeField] private Slider slider;
        /// <summary>
        /// Изображения слайдера
        /// </summary>
        [SerializeField] private Image sliderImage;

        /// <summary>
        /// Цвет своей команды
        /// </summary>
        [SerializeField] private Color localTeamColor;
        /// <summary>
        /// Цвет вражеской команды
        /// </summary>
        [SerializeField] private Color otherTeamColor;

        /// <summary>
        /// Дестрактибл
        /// </summary>
        private Destructible destructible;


        private void OnDestroy()
        {
            if (destructible == null) return;

            destructible.HitPointChange -= OnHitPointChange;
        }


        /// <summary>
        /// Инициализовать
        /// </summary>
        /// <param name="destructible">Дестрактибл</param>
        public void Init(Destructible destructible, int destructibleTeamID, int localPlayerTeamID)
        {
            this.destructible = destructible;

            destructible.HitPointChange += OnHitPointChange;

            slider.maxValue = destructible.MaxHitPoint;
            slider.value = destructible.HitPoint;
            if (localPlayerTeamID == destructibleTeamID)
            {
                SetLocalColor();
            }
            else
            {
                SetOtherColor();
            }
        }


        /// <summary>
        /// Задать цвет своей команды
        /// </summary>
        private void SetLocalColor()
        {
            sliderImage.color = localTeamColor;
        }

        /// <summary>
        /// Задать цвет вражеской команды
        /// </summary>
        private void SetOtherColor()
        {
            sliderImage.color = otherTeamColor;
        }

        /// <summary>
        /// При изменении количества здоровья
        /// </summary>
        /// <param name="hitPoint">Здоровье</param>
        private void OnHitPointChange(int hitPoint)
        {
            slider.value = hitPoint;
        }
    }
}