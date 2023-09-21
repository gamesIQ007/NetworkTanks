using UnityEngine;
using Mirror;

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

        /// <summary>
        /// Турель
        /// </summary>
        public Turret Turret;

        /// <summary>
        /// Смотритель
        /// </summary>
        public VehicleViewer Viewer;

        /// <summary>
        /// ID команды
        /// </summary>
        public int TeamID;

        /// <summary>
        /// Точка прицеливания
        /// </summary>
        [SyncVar]
        private Vector3 netAimPoint;
        public Vector3 NetAimPoint
        {
            get => netAimPoint;
            set
            {
                netAimPoint = value;
                CmdSetNetAimPoint(value);
            }
        }
        /// <summary>
        /// Задать точку прицеливания
        /// </summary>
        /// <param name="v">Точка прицеливания</param>
        [Command]
        private void CmdSetNetAimPoint(Vector3 v)
        {
            netAimPoint = v;
        }


        protected virtual void Update()
        {
            UpdateEngineSFX();
        }


        /// <summary>
        /// Выстрел
        /// </summary>
        public void Fire()
        {
            Turret.Fire();
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


        /// <summary>
        /// Задать видимость
        /// </summary>
        /// <param name="visible">Видимость</param>
        public void SetVisible(bool visible)
        {
            if (visible)
            {
                SetLayerToAll("Default");
            }
            else
            {
                SetLayerToAll("Ignore Main Camera");
            }
        }

        /// <summary>
        /// Применить слой ко всему
        /// </summary>
        /// <param name="layerName">Имя слоя</param>
        private void SetLayerToAll(string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);

            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }


        /// <summary>
        /// Владелец
        /// </summary>
        [SyncVar(hook = "T")]
        public NetworkIdentity Owner;
        private void T(NetworkIdentity oldValue, NetworkIdentity newValue)
        {

        }
    }
}