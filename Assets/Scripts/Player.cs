using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    [System.Serializable]
    /// <summary>
    /// Данные игрока
    /// </summary>
    public class PlayerData
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID;
        /// <summary>
        /// Ник
        /// </summary>
        public string Nickname;
        /// <summary>
        /// ID команды
        /// </summary>
        public int TeamID;

        public PlayerData(int id, string nickname, int teamID)
        {
            ID = id;
            Nickname = nickname;
            TeamID = teamID;
        }
    }


    /// <summary>
    /// Сериализация/десериализация данных игрока
    /// </summary>
    public static class PlayerDateReadWrite
    {
        /// <summary>
        /// Сериализация данных игрока
        /// </summary>
        /// <param name="writer">Писец</param>
        /// <param name="value">Данные игрока</param>
        public static void WritePlayerData(this NetworkWriter writer, PlayerData value)
        {
            writer.WriteInt(value.ID);
            writer.WriteString(value.Nickname);
            writer.WriteInt(value.TeamID);
        }

        /// <summary>
        /// Десериализация данных игрока
        /// </summary>
        /// <param name="reader">Чтец</param>
        /// <returns>Данные игрока</returns>
        public static PlayerData ReadPlayerData(this NetworkReader reader)
        {
            return new PlayerData(reader.ReadInt(), reader.ReadString(), reader.ReadInt());
        }
    }


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
        public event UnityAction<Vehicle> VehicleSpawned;

        /// <summary>
        /// Событие попадения
        /// </summary>
        public event UnityAction<ProjectileHitResult> ProjectileHit;

        /// <summary>
        /// Событие изменения количества фрагов (id игрока и количество его фрагов)
        /// </summary>
        public static UnityAction<int, int> ChangeFrags;

        [SyncVar(hook = nameof(OnFragsChanged))]
        /// <summary>
        /// Количество фрагов
        /// </summary>
        private int frags;
        public int Frags
        {
            set
            {
                frags = value;
                // На сервере
                ChangeFrags?.Invoke((int)netId, frags);
            }
            get
            {
                return frags;
            }
        }
        /// <summary>
        /// Изменение количества фрагов на клиенте
        /// </summary>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
        private void OnFragsChanged(int oldValue, int newValue)
        {
            ChangeFrags?.Invoke((int)netId, newValue);
        }

        /// <summary>
        /// Счётчик ID команд
        /// </summary>
        private static int teamIdCounter;

        /// <summary>
        /// Префаб транспорта
        /// </summary>
        [SerializeField] private Vehicle[] vehiclePrefab;
        /// <summary>
        /// Управление транспортом
        /// </summary>
        [SerializeField] private VehicleInputControl vehicleInputControl;

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
        /// Данные игрока
        /// </summary>
        private PlayerData data;
        public PlayerData Data => data;

        /// <summary>
        /// Задать ID команды
        /// </summary>
        /// <param name="teamId">ID команды</param>
        [Command]
        public void CmdSetTeamID(int teamId)
        {
            this.teamId = teamId;
        }


        /// <summary>
        /// Вызов результата попадания
        /// </summary>
        /// <param name="hitResult">Результат попадания</param>
        [Server]
        public void SvInvokeProjectileHit(ProjectileHitResult hitResult)
        {
            ProjectileHit?.Invoke(hitResult);

            RpcInvokeProjectileHit(hitResult.Type, hitResult.Damage, hitResult.Point);
        }
        [ClientRpc]
        public void RpcInvokeProjectileHit(ProjectileHitType type, float damage, Vector3 hitPoint)
        {
            ProjectileHitResult hitResult = new ProjectileHitResult();
            hitResult.Type = type;
            hitResult.Damage = damage;
            hitResult.Point = hitPoint;

            ProjectileHit?.Invoke(hitResult);
        }


        public override void OnStartServer()
        {
            base.OnStartServer();

            teamId = teamIdCounter % 2;
            teamIdCounter++;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            PlayerList.Instance.SvRemovePlayer(data);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isOwned)
            {
                CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

                NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

                data = new PlayerData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, teamId);

                CmdAddPlayerData(data);

                CmdUpdatePlayerData(data);
            }
        }

        /// <summary>
        /// Добавить данные пользователя
        /// </summary>
        /// <param name="data">Данные пользователя</param>
        [Command]
        private void CmdAddPlayerData(PlayerData data)
        {
            PlayerList.Instance.SvAddPlayer(data);
        }

        /// <summary>
        /// Обновить данные пользователя
        /// </summary>
        /// <param name="data">Данные пользователя</param>
        [Command]
        private void CmdUpdatePlayerData(PlayerData data)
        {
            this.data = data;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (isOwned)
            {
                NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
            }
        }

        /// <summary>
        /// При завершении матча
        /// </summary>
        private void OnMatchEnd()
        {
            if (ActiveVehicle != null)
            {
                ActiveVehicle.SetTargetControl(Vector3.zero);
                vehicleInputControl.enabled = false;
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
                    NetworkSessionManager.Match.SvRestartMatch();
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
            ActiveVehicle.TeamID = teamId;

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
            ActiveVehicle.Owner = netIdentity;
            ActiveVehicle.TeamID = teamId;

            if (ActiveVehicle != null && ActiveVehicle.isOwned && VehicleCamera.Instance != null)
            {
                VehicleCamera.Instance.SetTarget(ActiveVehicle);
            }

            vehicleInputControl.enabled = true;

            VehicleSpawned?.Invoke(ActiveVehicle);
        }
    }
}