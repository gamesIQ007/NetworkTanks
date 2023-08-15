using UnityEngine;

namespace NetworkTanks
{
    [System.Serializable]

    /// <summary>
    /// Параметр кривой
    /// </summary>
    public class ParameterCurve
    {
        /// <summary>
        /// Кривая
        /// </summary>
        [SerializeField] private AnimationCurve curve;

        /// <summary>
        /// Продолжительность
        /// </summary>
        [SerializeField] private float duration = 1.0f;

        /// <summary>
        /// Пройденное время
        /// </summary>
        private float expiredTime;


        /// <summary>
        /// Двигаться вперёд
        /// </summary>
        /// <param name="deltaTime">Дельта времени</param>
        /// <returns>Значение на кривой</returns>
        public float MoveTowards(float deltaTime)
        {
            expiredTime += deltaTime;

            return curve.Evaluate(expiredTime / duration);
        }

        /// <summary>
        /// Сброс
        /// </summary>
        /// <returns>Значение на кривой</returns>
        public float Reset()
        {
            expiredTime = 0;

            return curve.Evaluate(0);
        }

        /// <summary>
        /// Показать значение между
        /// </summary>
        /// <param name="startValue">Стартовое значение</param>
        /// <param name="endValue">Конечное значение</param>
        /// <param name="currentValue">Текущее значение</param>
        /// <returns>Значение между</returns>
        public float GetValueBetween(float startValue, float endValue, float currentValue)
        {
            if (curve.length == 0 || startValue == endValue) return 0;

            float startTime = curve.keys[0].time;
            float endTime = curve.keys[curve.length - 1].time;

            float currentTime = Mathf.Lerp(startTime, endTime, (currentValue - startValue) / (endValue - startValue));

            return curve.Evaluate(currentTime);
        }
    }
}