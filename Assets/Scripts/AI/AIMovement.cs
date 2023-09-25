using UnityEngine;
using UnityEngine.AI;

namespace NetworkTanks
{
    /// <summary>
    /// Расширения для трансформ
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Получить позицию X и Z
        /// </summary>
        /// <param name="t">Трансформа</param>
        /// <returns>Позиция x и z</returns>
        public static Vector3 GetPositionZX(this Transform t)
        {
            var x = t.position;
            x.y = 0;
            return x;
        }
    }


    /// <summary>
    /// Расширения для вектора
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Получить позицию X и Z
        /// </summary>
        /// <param name="t">Вектор</param>
        /// <returns>Позиция x и z</returns>
        public static Vector3 GetPositionZX(this Vector3 t)
        {
            var x = t;
            x.y = 0;
            return x;
        }
    }


    [RequireComponent(typeof(Vehicle))]

    /// <summary>
    /// Двинение ИИ
    /// </summary>
    public class AIMovement : MonoBehaviour
    {
        /// <summary>
        /// Передний сенсор
        /// </summary>
        [SerializeField] private AIRaySensor sensorForward;
        /// <summary>
        /// Задний сенсор
        /// </summary>
        [SerializeField] private AIRaySensor sensorBackward;
        /// <summary>
        /// Левый сенсор
        /// </summary>
        [SerializeField] private AIRaySensor sensorLeft;
        /// <summary>
        /// Правый сенсор
        /// </summary>
        [SerializeField] private AIRaySensor sensorRight;

        /// <summary>
        /// Дистанция остановки от точки назначения
        /// </summary>
        [SerializeField] private float stopDistance;
        /// <summary>
        /// Частота обновления пути
        /// </summary>
        [SerializeField] private float pathUpdateRate;

        /// <summary>
        /// Цель движения
        /// </summary>
        private Vector3 target;
        /// <summary>
        /// Следующая точка движения
        /// </summary>
        private Vector3 nextPathPoint;

        /// <summary>
        /// Транспорт
        /// </summary>
        private Vehicle vehicle;

        /// <summary>
        /// Путь
        /// </summary>
        private NavMeshPath path;
        /// <summary>
        /// Индекс угла пути
        /// </summary>
        private int cornerIndex;

        /// <summary>
        /// Таймер обновления пути
        /// </summary>
        private float timerUpdatePath;

        /// <summary>
        /// Есть путь
        /// </summary>
        private bool hasPath;
        public bool HasPath => hasPath;

        /// <summary>
        /// Достигнута точка назначения
        /// </summary>
        private bool reachedDestination;
        public bool ReachedDestination => reachedDestination;


        private void Awake()
        {
            vehicle = GetComponent<Vehicle>();
            path = new NavMeshPath();
        }

        private void Update()
        {
            SetDestination(GameObject.FindGameObjectWithTag("Finish").transform.position);

            if (pathUpdateRate > 0)
            {
                timerUpdatePath += Time.deltaTime;

                if (timerUpdatePath >= pathUpdateRate)
                {
                    CalculatePath(target);
                    timerUpdatePath = 0;
                }
            }

            UpdateTarget();

            MoveToTarget();
        }


        /// <summary>
        /// Задать точку назначения
        /// </summary>
        /// <param name="target">Точка назначения</param>
        public void SetDestination(Vector3 target)
        {
            if (this.target == target && hasPath) return;

            this.target = target;

            CalculatePath(target);
        }

        /// <summary>
        /// Сбросить путь
        /// </summary>
        public void ResetPath()
        {
            hasPath = false;
            reachedDestination = false;
        }


        /// <summary>
        /// Обновление цели
        /// </summary>
        private void UpdateTarget()
        {
            if (hasPath == false) return;

            nextPathPoint = path.corners[cornerIndex];

            if (Vector3.Distance(transform.position, nextPathPoint) < stopDistance)
            {
                if (path.corners.Length - 1 > cornerIndex)
                {
                    cornerIndex++;
                    nextPathPoint = path.corners[cornerIndex];
                }
                else
                {
                    hasPath = false;
                    reachedDestination = true;
                }
            }

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }
        }

        /// <summary>
        /// Рассчитать путь
        /// </summary>
        /// <param name="target">Цель</param>
        private void CalculatePath(Vector3 target)
        {
            NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            
            hasPath = path.corners.Length > 0;
            reachedDestination = false;
            cornerIndex = 1;
        }

        /// <summary>
        /// Двигаться к цели
        /// </summary>
        private void MoveToTarget()
        {
            if (nextPathPoint == null) return;

            if (reachedDestination)
            {
                vehicle.SetTargetControl(new Vector3(0, 1, 0));
                return;
            }

            float turnControl = 0;
            float forwardThrust = 1;

            var referenceDirection = GetReferenceMovementDirectionZX();
            var tankDir = GetTankDirectionZX();

            var forwardSensorState = sensorForward.Raycast();
            var leftSensorState = sensorLeft.Raycast();
            var rightSensorState = sensorRight.Raycast();

            if (forwardSensorState.Item1)
            {
                forwardThrust = 0;

                if (leftSensorState.Item1 == false)
                {
                    turnControl = -1;
                    forwardThrust = -0.2f;
                }
                else if (rightSensorState.Item1 == false)
                {
                    turnControl = 1;
                    forwardThrust = -0.2f;
                }
                else
                {
                    forwardThrust = -1;
                }
            }
            else
            {
                turnControl = Mathf.Clamp(Vector3.SignedAngle(tankDir, referenceDirection, Vector3.up), -45.0f, 45.0f) / 45.0f;

                float minSideDistance = 2;

                if (leftSensorState.Item1 && leftSensorState.Item2 < minSideDistance && turnControl < 0)
                {
                    turnControl = -turnControl;
                }
                if (rightSensorState.Item1 && rightSensorState.Item2 < minSideDistance && turnControl > 0)
                {
                    turnControl = -turnControl;
                }
            }

            vehicle.SetTargetControl(new Vector3(turnControl, 0, forwardThrust));
        }

        /// <summary>
        /// Получить направление танка в осях X и Z
        /// </summary>
        /// <returns>Направление танка в осях X и Z</returns>
        private Vector3 GetTankDirectionZX()
        {
            var tankDir = vehicle.transform.forward.GetPositionZX();
            tankDir.Normalize();
            return tankDir;
        }

        /// <summary>
        /// Получить направление движения к цели
        /// </summary>
        /// <returns>Направление движения к цели</returns>
        private Vector3 GetReferenceMovementDirectionZX()
        {
            var tankPos = vehicle.transform.GetPositionZX();
            var targetPos = nextPathPoint.GetPositionZX();

            return (targetPos - tankPos).normalized;
        }
    }
}