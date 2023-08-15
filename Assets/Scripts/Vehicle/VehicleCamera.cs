using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(Camera))]

    /// <summary>
    /// Камера от третьего лица
    /// </summary>
    public class VehicleCamera : MonoBehaviour
    {
        /// <summary>
        /// Цель слежения камеры
        /// </summary>
        [SerializeField] private Vehicle target;
        /// <summary>
        /// Смещение камеры
        /// </summary>
        [SerializeField] private Vector3 offset;

        [Header("Sensitive Limit")]
        /// <summary>
        /// Чувствительность вращения
        /// </summary>
        [SerializeField] private float rotateSensitive;
        /// <summary>
        /// Чувствительность отдаления/приближения
        /// </summary>
        [SerializeField] private float scrollSensitive;

        [Header("Distance")]
        /// <summary>
        /// Расстояние до цели
        /// </summary>
        [SerializeField] private float distance;
        /// <summary>
        /// Максимальное расстояние до цели
        /// </summary>
        [SerializeField] private float maxDistance;
        /// <summary>
        /// Минимальное расстояние до цели
        /// </summary>
        [SerializeField] private float minDistance;
        /// <summary>
        /// Интенсивность интерполяции дистанции
        /// </summary>
        [SerializeField] private float distanceLerpRate;
        /// <summary>
        /// Смещение расстояния от коллизии объекта с камерой
        /// </summary>
        [SerializeField] private float distanceOffsetFromCollisionHit;

        [Header("Rotation Limit")]
        /// <summary>
        /// Максимальный предел наклона по оси Y
        /// </summary>
        [SerializeField] private float maxVerticalAngle;
        /// <summary>
        /// Минимальный предел наклона по оси Y
        /// </summary>
        [SerializeField] private float minVerticalAngle;
        
        [Header("Zoom Optical")]
        /// <summary>
        /// Маска эффекта приближения
        /// </summary>
        [SerializeField] private GameObject zoomMaskEffect;
        /// <summary>
        /// FOV при приближении
        /// </summary>
        [SerializeField] private float zoomedFOV;
        /// <summary>
        /// Максимальный вертикальный угол при приближении
        /// </summary>
        [SerializeField] private float zoomedMaxVerticalAngle;

        private new Camera camera;

        /// <summary>
        /// Вектор внешнего управления вращением
        /// </summary>
        private Vector2 rotationControl;

        /// <summary>
        /// На сколько передвинута мышь по оси X
        /// </summary>
        private float deltaRotationX;
        /// <summary>
        /// На сколько передвинута мышь по оси Y
        /// </summary>
        private float deltaRotationY;
        /// <summary>
        /// Текущее расстояние до цели
        /// </summary>
        private float currentDistance;

        /// <summary>
        /// FOV по умолчанию
        /// </summary>
        private float defaultFOV;
        /// <summary>
        /// Зум включен
        /// </summary>
        private bool isZoom;
        /// <summary>
        /// Максимальный вертикальный угол по умолчанию
        /// </summary>
        private float defaultMaxVerticalAngle;
        /// <summary>
        /// Последняя дистанция
        /// </summary>
        private float lastDistance;


        #region Unity Events

        private void Start()
        {
            camera = GetComponent<Camera>();
            defaultFOV = camera.fieldOfView;
            defaultMaxVerticalAngle = maxVerticalAngle;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            UpdateControl();

            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            isZoom = distance <= minDistance;

            // Расчёт вращения и позиции
            deltaRotationX += rotationControl.x * rotateSensitive;
            deltaRotationY += rotationControl.y * rotateSensitive;
            deltaRotationY = ClampAngle(deltaRotationY, minVerticalAngle, maxVerticalAngle);

            Quaternion finalRotation = Quaternion.Euler(deltaRotationY, deltaRotationX, 0);
            Vector3 finalPosition = target.transform.position - (finalRotation * Vector3.forward * distance);
            finalPosition = AddLocalOffset(finalPosition);

            // Расчёт дистанции
            float targetDistance = distance;

            RaycastHit hit;
            if (Physics.Linecast(target.transform.position + new Vector3(0, offset.y, 0), finalPosition, out hit))
            {
                float distanceToHit = Vector3.Distance(target.transform.position + new Vector3(0, offset.y, 0), hit.point);

                if (hit.transform != target)
                {
                    if (distanceToHit < distance)
                    {
                        targetDistance = distanceToHit - distanceOffsetFromCollisionHit;
                    }
                }
            }

            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * distanceLerpRate);
            currentDistance = Mathf.Clamp(currentDistance, minDistance, distance);

            // Корректировка позиции камеры
            finalPosition = target.transform.position - (finalRotation * Vector3.forward * currentDistance);

            //Применение трансформы
            transform.rotation = finalRotation;
            transform.position = finalPosition;
            transform.position = AddLocalOffset(transform.position);

            // Зум
            zoomMaskEffect.SetActive(isZoom);

            if (isZoom)
            {
                transform.position = target.ZoomOpticalPosition.position;
                camera.fieldOfView = zoomedFOV;
                maxVerticalAngle = zoomedMaxVerticalAngle;
            }
            else
            {
                camera.fieldOfView = defaultFOV;
                maxVerticalAngle = defaultMaxVerticalAngle;
            }
        }

        #endregion


        #region Public API

        /// <summary>
        /// Обновление управления
        /// </summary>
        public void UpdateControl()
        {
            rotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            distance += -Input.mouseScrollDelta.y * scrollSensitive;

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isZoom = !isZoom;

                if (isZoom)
                {
                    lastDistance = distance;
                    distance = minDistance;
                }
                else
                {
                    distance = lastDistance;
                    currentDistance = lastDistance;
                }
            }
        }

        /// <summary>
        /// Задать цель
        /// </summary>
        /// <param name="target">Цель</param>
        public void SetTarget(Vehicle vehicle)
        {
            target = vehicle;
        }

        #endregion


        /// <summary>
        /// Ограничение угла поворота камеры
        /// </summary>
        /// <param name="angle">Угол</param>
        /// <param name="min">Минимум</param>
        /// <param name="max">Максимум</param>
        /// <returns>Итоговый угол поворота камеры</returns>
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// Добавление локального смещения
        /// </summary>
        /// <param name="position">Позиция смещения</param>
        /// <returns>Итоговая позиция смещения</returns>
        private Vector3 AddLocalOffset(Vector3 position)
        {
            Vector3 result = position;
            result.y += offset.y;
            result += transform.right * offset.x;
            result += transform.forward * offset.z;
            return result;
        }
    }
}