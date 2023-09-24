using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Игрок
    /// </summary>
    public class Player : MatchMember
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
        /// Префаб транспорта
        /// </summary>
        [SerializeField] private Vehicle[] vehiclePrefab;
        /// <summary>
        /// Управление транспортом
        /// </summary>
        [SerializeField] private VehicleInputControl vehicleInputControl;


        public override void OnStartServer()
        {
            base.OnStartServer();

            teamId = MatchController.GetNextTeam();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            MatchMemberList.Instance.SvRemoveMatchMember(data);
        }


        public override void OnStartClient()
        {
            base.OnStartClient();

            if (isOwned)
            {
                CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

                NetworkSessionManager.Match.MatchStart += OnMatchStart;
                NetworkSessionManager.Match.MatchEnd += OnMatchEnd;

                data = new MatchMemberData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, teamId, netIdentity);

                CmdAddPlayerData(data);

                CmdUpdateData(data);
            }
        }

        /// <summary>
        /// Добавить данные пользователя
        /// </summary>
        /// <param name="data">Данные пользователя</param>
        [Command]
        private void CmdAddPlayerData(MatchMemberData data)
        {
            MatchMemberList.Instance.SvAddMatchMember(data);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (isOwned)
            {
                NetworkSessionManager.Match.MatchStart -= OnMatchStart;
                NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
            }
        }

        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnMatchStart()
        {
            vehicleInputControl.enabled = true;
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


        private void Start()
        {
            vehicleInputControl.enabled = false;
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

            vehicleInputControl.enabled = false;

            VehicleSpawned?.Invoke(ActiveVehicle);
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
    }
}