using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение захвата базы
    /// </summary>
    public class UICaptureBase : MonoBehaviour
    {
        /// <summary>
        /// Условие захвата базы
        /// </summary>
        [SerializeField] private ConditionCaptureBase conditionCaptureBase;

        /// <summary>
        /// Слайдер захвата своей базы
        /// </summary>
        [SerializeField] private Slider localSlider;
        /// <summary>
        /// Слайдер захвата чужой базы
        /// </summary>
        [SerializeField] private Slider otherSlider;


        private void Update()
        {
            if (Player.Local == null) return;

            if (Player.Local.TeamID == TeamSide.TeamRed)
            {
                UpdateSlider(localSlider, conditionCaptureBase.RedBaseCaptureLevel);
                UpdateSlider(otherSlider, conditionCaptureBase.BlueBaseCaptureLevel);
            }

            if (Player.Local.TeamID == TeamSide.TeamBlue)
            {
                UpdateSlider(localSlider, conditionCaptureBase.BlueBaseCaptureLevel);
                UpdateSlider(otherSlider, conditionCaptureBase.RedBaseCaptureLevel);
            }
        }


        /// <summary>
        /// Обновить слайдер
        /// </summary>
        /// <param name="slider">Слайдер</param>
        /// <param name="value">Значение</param>
        private void UpdateSlider(Slider slider, float value)
        {
            if (value == 0)
            {
                slider.gameObject.SetActive(false);
            }
            else
            {
                if (slider.gameObject.activeSelf == false)
                {
                    slider.gameObject.SetActive(true);
                }

                slider.value = value;
            }
        }
    }
}