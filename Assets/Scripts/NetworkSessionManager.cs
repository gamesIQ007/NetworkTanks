using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Менеджер сетевых сессий
    /// </summary>
    public class NetworkSessionManager : NetworkManager
    {
        /// <summary>
        /// Синглтон
        /// </summary>
        public static NetworkSessionManager Instance => singleton as NetworkSessionManager;

        /// <summary>
        /// Являемся сервером?
        /// </summary>
        public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
        /// <summary>
        /// Являемся клиентом?
        /// </summary>
        public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);

        /// <summary>
        /// Зоны спавна
        /// </summary>
        [SerializeField] private SphereArea[] spawnZoneRed;
        [SerializeField] private SphereArea[] spawnZoneBlue;

        /// <summary>
        /// Точки спавна
        /// </summary>
        public Vector3 RandomSpawnPointRed => spawnZoneRed[Random.Range(0, spawnZoneRed.Length)].RandomInside;
        public Vector3 RandomSpawnPointBlue => spawnZoneBlue[Random.Range(0, spawnZoneBlue.Length)].RandomInside;
    }
}