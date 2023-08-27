using UnityEngine;

namespace NetworkTanks
{
    [RequireComponent(typeof(TrackTank))]

    /// <summary>
    /// Турель танка
    /// </summary>
    public class TankTurret : Turret
    {
        /// <summary>
        /// Танк
        /// </summary>
        private TrackTank tank;

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
        
        [Header("SFX")]
        /// <summary>
        /// Звук выстрела
        /// </summary>
        [SerializeField] private AudioSource fireSound;
        /// <summary>
        /// Вспышка
        /// </summary>
        [SerializeField] private ParticleSystem muzzle;
        /// <summary>
        /// Отбрасывающая сила
        /// </summary>
        [SerializeField] private float forceRecoil;

        /// <summary>
        /// Текущий угол поворота башни
        /// </summary>
        private float maskCurrentAngle;

        /// <summary>
        /// Ригид танка
        /// </summary>
        private Rigidbody tankRigidbody;


        #region Unity Events

        private void Start()
        {
            tank = GetComponent<TrackTank>();
            tankRigidbody = tank.GetComponent<Rigidbody>();

            maxTopAngle = -maxTopAngle;
        }

        protected override void Update()
        {
            base.Update();

            ControlTurretAim();
        }

        #endregion


        protected override void OnFire()
        {
            base.OnFire();

            GameObject projectile = Instantiate(projectilePrefab.gameObject);
            projectile.transform.position = launchPoint.position;
            projectile.transform.forward = launchPoint.forward;

            FireSfx();
        }


        /// <summary>
        /// Эффект от выстрела
        /// </summary>
        private void FireSfx()
        {
            fireSound.Play();
            muzzle.Play();

            tankRigidbody.AddForceAtPosition(-mask.forward * forceRecoil, mask.position, ForceMode.Impulse);
        }

        /// <summary>
        /// Управление прицеливанием башни
        /// </summary>
        private void ControlTurretAim()
        {
            // Башня
            Vector3 localPosition = tower.InverseTransformPoint(tank.NetAimPoint);
            localPosition.y = 0;
            Vector3 globalPosition = tower.TransformPoint(localPosition);
            tower.rotation = Quaternion.RotateTowards(tower.rotation, Quaternion.LookRotation((globalPosition - tower.position).normalized, tower.up), horizontalRotationSpeed * Time.deltaTime);

            // Маска
            mask.localRotation = Quaternion.identity;

            localPosition = mask.InverseTransformPoint(tank.NetAimPoint);
            localPosition.x = 0;
            globalPosition = mask.TransformPoint(localPosition);

            float targetAngle = -Vector3.SignedAngle((globalPosition - mask.position).normalized, mask.forward, mask.right);
            targetAngle = Mathf.Clamp(targetAngle, maxTopAngle, maxBottomAngle);

            maskCurrentAngle = Mathf.MoveTowards(maskCurrentAngle, targetAngle, verticalRotationSpeed * Time.deltaTime);

            mask.localRotation = Quaternion.Euler(maskCurrentAngle, 0, 0);
        }
    }
}