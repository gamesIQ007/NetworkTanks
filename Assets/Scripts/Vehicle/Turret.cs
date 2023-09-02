using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Турель
    /// </summary>
    public class Turret : NetworkBehaviour
    {
        /// <summary>
        /// Точка запуска снарядов
        /// </summary>
        [SerializeField] protected Transform launchPoint;
        public Transform LaunchPoint => launchPoint;

        /// <summary>
        /// Скорострельность
        /// </summary>
        [SerializeField] private float fireRate;

        /// <summary>
        /// Префаб снаряда
        /// </summary>
        [SerializeField] protected Projectile projectilePrefab;
        public Projectile ProjectilePrefab => projectilePrefab;

        /// <summary>
        /// Таймер перезарядки
        /// </summary>
        private float fireTimer;
        /// <summary>
        /// Нормализованное значение таймера перезарядки
        /// </summary>
        public float FireTimeNormalized => fireTimer / fireRate;

        /// <summary>
        /// Количество патронов
        /// </summary>
        [SyncVar]
        [SerializeField] protected int ammoCount;
        public int AmmoCount => ammoCount;

        /// <summary>
        /// Событие изменения количества патронов
        /// </summary>
        public UnityAction<int> AmmoChanged;

        /// <summary>
        /// Диапазон разброса стрельбы
        /// </summary>
        [SerializeField] private float spreadShotRange = 0.1f;
        public float SpreadShotRange => spreadShotRange;



        /// <summary>
        /// Добавить патроны
        /// </summary>
        /// <param name="count">Количество</param>
        [Server]
        public void SvAddAmmo(int count)
        {
            ammoCount += count;
            RpcAmmoChanged();
        }

        /// <summary>
        /// Отнять патроны
        /// </summary>
        /// <param name="count">Количество</param>
        /// <returns>Удачно ли отняты патроны</returns>
        [Server]
        protected virtual bool SvDrawAmmo(int count)
        {
            if (ammoCount == 0)
            {
                return false;
            }

            if (ammoCount >= count)
            {
                ammoCount -= count;
                RpcAmmoChanged();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Изменено количество патронов
        /// </summary>
        [ClientRpc]
        private void RpcAmmoChanged()
        {
            AmmoChanged?.Invoke(ammoCount);
        }

        /// <summary>
        /// Стрельба
        /// </summary>
        protected virtual void OnFire() { }

        /// <summary>
        /// Выстрел
        /// </summary>
        public void Fire()
        {
            if (isOwned == false) return;

            if (isClient)
            {
                CmdFire();
            }
        }

        [Command]
        private void CmdFire()
        {
            if (fireTimer > 0) return;
            if (SvDrawAmmo(1) == false) return;

            OnFire();

            fireTimer = fireRate;

            RpcFire();
        }
        [ClientRpc]
        private void RpcFire()
        {
            if (isServer) return;

            fireTimer = fireRate;

            OnFire();
        }

        protected virtual void Update()
        {
            if (fireTimer > 0)
            {
                fireTimer -= Time.deltaTime;
            }
        }
    }
}