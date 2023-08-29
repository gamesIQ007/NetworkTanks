using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Игрок
    /// </summary>
    public class Player : NetworkBehaviour
    {
        public static Player Local
        {
            get
            {
                var x = NetworkClient.localPlayer;

                if (x != null)
                {
                    return x.GetComponent<Player>();
                }

                return null;
            }
        }

        /// <summary>
        /// Событие спавна транспорта
        /// </summary>
        public UnityAction<Vehicle> VehicleSpawned;

        /// <summary>
        /// Счётчик ID команд
        /// </summary>
        private static int teamIdCounter;

        /// <summary>
        /// Префаб транспорта
        /// </summary>
        [SerializeField] private Vehicle[] vehiclePrefab;

        /// <summary>
        /// Активный транспорт
        /// </summary>
        public Vehicle ActiveVehicle { get; set; }

        /// <summary>
        /// Ник
        /// </summary>
        [Header("Player")]
        [SyncVar(hook = nameof(OnNicknameChanged))]
        public string Nickname;
        private void OnNicknameChanged(string oldValue, string newValue)
        {
            gameObject.name = "Player_" + newValue; // На клиенте
        }
        /// <summary>
        /// Задать ник
        /// </summary>
        /// <param name="name">Ник</param>
        [Command] // На сервере
        public void CmdSetName(string name)
        {
            Nickname = name;
            gameObject.name = "Player_" + name;
        }

        /// <summary>
        /// ID команды
        /// </summary>
        [SyncVar]
        [SerializeField] private int teamId;
        public int TeamID => teamId;

        /// <summary>
        /// Задать ID команды
        /// </summary>
        /// <param name="teamId">ID команды</param>
        [Command]
        public void CmdSetTeamID(int teamId)
        {
            this.teamId = teamId;
        }


        public override void OnStartServer()
        {
            base.OnStartServer();

            teamId = teamIdCounter % 2;
            teamIdCounter++;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isOwned)
            {
                CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);
            }
        }


        private void Update()
        {
            if (isLocalPlayer)
            {
                if (ActiveVehicle != null)
                {
                    ActiveVehicle.SetVisible(!VehicleCamera.Instance.IsZoom);
                }
            }

            if (isServer)
            {
                if (Input.GetKeyDown(KeyCode.F9))
                {
                    foreach (var p in FindObjectsOfType<Player>())
                    {
                        if (p.ActiveVehicle != null)
                        {
                            NetworkServer.UnSpawn(p.ActiveVehicle.gameObject);
                            Destroy(p.ActiveVehicle.gameObject);
                            p.ActiveVehicle = null;
                        }
                    }

                    foreach (var p in FindObjectsOfType<Player>())
                    {
                        p.SvSpawnClientVehicle();
                    }
                }
            }

            if (isOwned)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    if (Cursor.lockState != CursorLockMode.Locked)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
            }
        }


        [Server]
        /// <summary>
        /// Заспавнить клиентский транспорт
        /// </summary>
        public void SvSpawnClientVehicle()
        {
            if (ActiveVehicle != null) return;

            GameObject playerVehicle = Instantiate(vehiclePrefab[Random.Range(0, vehiclePrefab.Length)].gameObject, transform.position, Quaternion.identity);

            playerVehicle.transform.position = teamId % 2 == 0 ?
                NetworkSessionManager.Instance.RandomSpawnPointRed :
                NetworkSessionManager.Instance.RandomSpawnPointBlue;

            NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);

            ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
            ActiveVehicle.Owner = netIdentity;

            RpcSetVehicle(ActiveVehicle.netIdentity);
        }

        [ClientRpc]
        /// <summary>
        /// Задать клиенту активный транспорт
        /// </summary>
        private void RpcSetVehicle(NetworkIdentity vehicle)
        {
            if (vehicle == null) return;

            ActiveVehicle = vehicle.GetComponent<Vehicle>();

            if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
            {
                VehicleCamera.Instance.SetTarget(ActiveVehicle);
            }

            VehicleSpawned?.Invoke(ActiveVehicle);
        }
    }
}