using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Уничтожаемый объект
    /// </summary>
    public class Destructible : NetworkBehaviour
    {
        /// <summary>
        /// Событие изменения здоровья
        /// </summary>
        public event UnityAction<int> HitPointChanged;
        /// <summary>
        /// Событие: уничтожен
        /// </summary>
        public event UnityAction<Destructible> Destroyed;
        /// <summary>
        /// Событие: восстановлен
        /// </summary>
        public event UnityAction<Destructible> Recovered;

        /// <summary>
        /// Событие: уничтожен
        /// </summary>
        [SerializeField] private UnityEvent EventDestroyed;
        /// <summary>
        /// Событие: восстановлен
        /// </summary>
        [SerializeField] private UnityEvent EventRecovered;

        /// <summary>
        /// Максимальное количество здоровья
        /// </summary>
        [SerializeField] private int maxHitPoint;
        public int MaxHitPoint => maxHitPoint;

        /// <summary>
        /// Текущее количество здоровья
        /// </summary>
        [SerializeField] private int currentHitPoint; // дебаг
        public int HitPoint => currentHitPoint;
        [SyncVar(hook = nameof(SyncHitPoint))]
        private int syncCurrentHitPoint;


        #region Server

        public override void OnStartServer()
        {
            base.OnStartServer();

            syncCurrentHitPoint = maxHitPoint;
            currentHitPoint = maxHitPoint;
        }


        /// <summary>
        /// Нанести урон
        /// </summary>
        /// <param name="damage">Урон</param>
        [Server]
        public void SvApplyDamage(int damage)
        {
            syncCurrentHitPoint -= damage;

            if (syncCurrentHitPoint <= 0)
            {
                syncCurrentHitPoint = 0;

                RpcDestroy();
            }
        }

        /// <summary>
        /// Восстановление
        /// </summary>
        [Server]
        public void SvRecovery()
        {
            syncCurrentHitPoint = maxHitPoint;
            currentHitPoint = maxHitPoint;

            RpcRecovery();
        }

        #endregion


        #region Client

        /// <summary>
        /// Изменить количество здоровья
        /// </summary>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
        private void SyncHitPoint(int oldValue, int newValue)
        {
            currentHitPoint = newValue;
            HitPointChanged?.Invoke(newValue);
        }

        /// <summary>
        /// Уничтожение
        /// </summary>
        [ClientRpc]
        private void RpcDestroy()
        {
            Destroyed?.Invoke(this);
            EventDestroyed?.Invoke();
        }

        /// <summary>
        /// Восстановление
        /// </summary>
        [ClientRpc]
        private void RpcRecovery()
        {
            Recovered?.Invoke(this);
            EventRecovered?.Invoke();
        }

        #endregion
    }
}