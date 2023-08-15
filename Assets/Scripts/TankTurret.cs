using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(TrackTank))]

    /// <summary>
    /// Турель танка
    /// </summary>
    public class TankTurret : MonoBehaviour
    {
        /// <summary>
        /// Танк
        /// </summary>
        private TrackTank tank;

        /// <summary>
        /// Прицел
        /// </summary>
        [SerializeField] private Transform aim;

        /// <summary>
        /// Башня
        /// </summary>
        [SerializeField] private Transform tower;
        /// <summary>
        /// Маска орудия
        /// </summary>
        [SerializeField] private Transform mask;

        /// <summary>
        /// Скорость вращения по горизонтали
        /// </summary>
        [SerializeField] private float horizontalRotationSpeed;
        /// <summary>
        /// Скорость вращения по вертикали
        /// </summary>
        [SerializeField] private float verticalRotationSpeed;

        /// <summary>
        /// Максимальный угол подъёма
        /// </summary>
        [SerializeField] private float maxTopAngle;
        /// <summary>
        /// Максимальный угол опускания
        /// </summary>
        [SerializeField] private float maxBottomAngle;

        /// <summary>
        /// Текущий угол поворота башни
        /// </summary>
        private float maskCurrentAngle;


        private void Start()
        {
            tank = GetComponent<TrackTank>();
            maxTopAngle = -maxTopAngle;
        }

        private void Update()
        {
            ControlTurretAim();
        }

        /// <summary>
        /// Управление прицеливанием башни
        /// </summary>
        private void ControlTurretAim()
        {
            // Башня
            Vector3 localPosition = tower.InverseTransformPoint(aim.position);
            localPosition.y = 0;
            Vector3 globalPosition = tower.TransformPoint(localPosition);
            tower.rotation = Quaternion.RotateTowards(tower.rotation, Quaternion.LookRotation((globalPosition - tower.position).normalized, tower.up), horizontalRotationSpeed * Time.deltaTime);

            // Маска
            mask.localRotation = Quaternion.identity;

            localPosition = mask.InverseTransformPoint(aim.position);
            localPosition.x = 0;
            globalPosition = mask.TransformPoint(localPosition);

            float targetAngle = -Vector3.SignedAngle((globalPosition - mask.position).normalized, mask.forward, mask.right);
            targetAngle = Mathf.Clamp(targetAngle, maxTopAngle, maxBottomAngle);

            maskCurrentAngle = Mathf.MoveTowards(maskCurrentAngle, targetAngle, verticalRotationSpeed * Time.deltaTime);

            mask.localRotation = Quaternion.Euler(maskCurrentAngle, 0, 0);
        }
    }
}