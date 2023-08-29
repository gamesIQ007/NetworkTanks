using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Сборщик ивентов
    /// </summary>
    public class GameEventCollector : NetworkBehaviour
    {
        /// <summary>
        /// Событие спавна транспорта игрока
        /// </summary>
        public UnityAction<Vehicle> PlayerVehicleSpawned;


        /// <summary>
        /// Добавление игрока
        /// </summary>
        [Server]
        public void SvOnAddPlayer()
        {
            RpcOnAddPlayer();
        }
        [ClientRpc]
        private void RpcOnAddPlayer()
        {
            Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
        }

        private void OnPlayerVehicleSpawned(Vehicle vehicle)
        {
            PlayerVehicleSpawned?.Invoke(vehicle);
        }
    }
}