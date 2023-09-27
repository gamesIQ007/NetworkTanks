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
        /// Событие смены типа патронов
        /// </summary>
        public event UnityAction<int> UpdateSelectedAmmunition;

        /// <summary>
        /// Событие выстрела
        /// </summary>
        public event UnityAction Shot;

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
        /// Аммуниция
        /// </summary>
        [SerializeField] protected Ammunition[] ammunition;
        public Ammunition[] Ammunition => ammunition;

        /// <summary>
        /// Таймер перезарядки
        /// </summary>
        private float fireTimer;
        /// <summary>
        /// Нормализованное значение таймера перезарядки
        /// </summary>
        public float FireTimeNormalized => fireTimer / fireRate;
        
        /// <summary>
        /// Индекс выбранного из аммуниции снаряда
        /// </summary>
        [SyncVar]
        [SerializeField] private int syncSelectedAmmunitionIndex;
        public int SelectedAmmunitionIndex => syncSelectedAmmunitionIndex;

        public ProjectileProperties SelectedProjectile => ammunition[syncSelectedAmmunitionIndex].Properties;

        /// <summary>
        /// Диапазон разброса стрельбы
        /// </summary>
        [SerializeField] private float spreadShotRange = 0.1f;
        public float SpreadShotRange => spreadShotRange;

        /// <summary>
        /// Индекс активного оружия
        /// </summary>
        private int indexActiveWeapon;
        public int IndexActiveWeapon => indexActiveWeapon;


        /// <summary>
        /// Задать выбранный снаряд
        /// </summary>
        /// <param name="index">Индекс снаряда</param>
        public void SetSelectProjectile(int index)
        {
            if (isOwned == false) return;

            if (index < 0 || index > ammunition.Length) return;

            syncSelectedAmmunitionIndex = index;

            if (isClient)
            {
                CmdReloadAmmunition();
            }

            UpdateSelectedAmmunition?.Invoke(index);
        }

        /// <summary>
        /// Перезарядка аммуниции
        /// </summary>
        [Command]
        private void CmdReloadAmmunition()
        {
            fireTimer = fireRate;
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
            SvFire();
        }
        [ClientRpc]
        private void RpcFire()
        {
            if (isServer) return;

            fireTimer = fireRate;

            OnFire();

            Shot?.Invoke();
        }
        [Server]
        public void SvFire()
        {
            if (fireTimer > 0) return;
            if (ammunition[syncSelectedAmmunitionIndex].SvDrawAmmo(1) == false) return;

            OnFire();

            fireTimer = fireRate;

            RpcFire();

            Shot?.Invoke();
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