using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Транспорт
    /// </summary>
    public class Vehicle : Destructible
    {
        /// <summary>
        /// Максимальная скорость
        /// </summary>
        [SerializeField] protected float maxLinearVelocity;

        [Header("Engine Sound")]
        /// <summary>
        /// Источник звука двигателя
        /// </summary>
        [SerializeField] private AudioSource engineSound;
        /// <summary>
        /// Модификатор звука
        /// </summary>
        [SerializeField] private float enginePitchModifier;
        
        [Header("Vehicle")]
        /// <summary>
        /// Точка, с которой идёт прицеливание
        /// </summary>
        [SerializeField] protected Transform zoomOpticalPosition;
        public Transform ZoomOpticalPosition => zoomOpticalPosition;

        /// <summary>
        /// Линейная скорость
        /// </summary>
        public virtual float LinearVelocity => 0;

        /// <summary>
        /// Нормализованная линейная скорость
        /// </summary>
        public float NormalizedLinearVelocity
        {
            get
            {
                if (Mathf.Approximately(0, LinearVelocity)) return 0;

                return Mathf.Clamp01(LinearVelocity / maxLinearVelocity);
            }
        }

        /// <summary>
        /// Входные данные с управления
        /// </summary>
        protected Vector3 targetInputControl;

        /// <summary>
        /// Задать данные с управления
        /// </summary>
        /// <param name="control">Вектор управления</param>
        public void SetTargetControl(Vector3 control)
        {
            targetInputControl = control.normalized;
        }


        protected virtual void Update()
        {
            UpdateEngineSFX();
        }


        /// <summary>
        /// Обновление звука двигателя
        /// </summary>
        private void UpdateEngineSFX()
        {
            if (engineSound != null)
            {
                engineSound.pitch = 1.0f + enginePitchModifier * NormalizedLinearVelocity;
                engineSound.volume = 0.5f + NormalizedLinearVelocity;
            }
        }
    }
}