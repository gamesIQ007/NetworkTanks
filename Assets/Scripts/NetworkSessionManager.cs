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
        public static GameEventCollector Events => Instance.events;
        public static MatchController Match => Instance.matchController;

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

        /// <summary>
        /// Сборщик ивентов
        /// </summary>
        [SerializeField] private GameEventCollector events;
        /// <summary>
        /// Контроллер матчей
        /// </summary>
        [SerializeField] private MatchController matchController;


        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            events.SvOnAddPlayer();
        }
    }
}