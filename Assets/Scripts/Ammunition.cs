using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Аммуниция
    /// </summary>
    public class Ammunition : NetworkBehaviour
    {
        /// <summary>
        /// Событие изменения количества патронов
        /// </summary>
        public event UnityAction<int> AmmoCountChanged;

        /// <summary>
        /// Свойства снаряда
        /// </summary>
        [SerializeField] private ProjectileProperties properties;
        public ProjectileProperties Properties => properties;

        /// <summary>
        /// Количество снарядов
        /// </summary>
        [SyncVar(hook = nameof(SyncAmmoCount))]
        [SerializeField] protected int syncAmmoCount;
        public int AmmoCount => syncAmmoCount;


        #region Server

        /// <summary>
        /// Добавить патроны
        /// </summary>
        /// <param name="count">Количество</param>
        [Server]
        public void SvAddAmmo(int count)
        {
            syncAmmoCount += count;
        }

        /// <summary>
        /// Отнять патроны
        /// </summary>
        /// <param name="count">Количество</param>
        /// <returns>Удачно ли отняты патроны</returns>
        [Server]
        public virtual bool SvDrawAmmo(int count)
        {
            if (syncAmmoCount == 0)
            {
                return false;
            }

            if (syncAmmoCount >= count)
            {
                syncAmmoCount -= count;

                return true;
            }

            return false;
        }

        #endregion


        #region Client

        /// <summary>
        /// Синхронизация количества снарядов
        /// </summary>
        /// <param name="oldValue">Старое количество</param>
        /// <param name="newValue">Новое количество</param>
        private void SyncAmmoCount(int oldValue, int newValue)
        {
            AmmoCountChanged?.Invoke(newValue);
        }

        #endregion
    }
}