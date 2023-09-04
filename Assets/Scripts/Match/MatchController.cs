using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Интерфейс условий матча
    /// </summary>
    public interface IMatchCondition
    {
        /// <summary>
        /// Условие выполнено
        /// </summary>
        bool IsTriggered { get; }

        /// <summary>
        /// При старте матча
        /// </summary>
        /// <param name="controller">Контроллер матчей</param>
        void OnServerMatchStart(MatchController controller);
        /// <summary>
        /// При завершении матча
        /// </summary>
        /// <param name="controller">Контроллер матчей</param>
        void OnServerMatchEnd(MatchController controller);
    }

    /// <summary>
    /// Контроллер матчей
    /// </summary>
    public class MatchController : NetworkBehaviour
    {
        /// <summary>
        /// Событие начала матча
        /// </summary>
        public UnityAction MatchStart;
        /// <summary>
        /// Событие окончания матча
        /// </summary>
        public UnityAction MatchEnd;

        /// <summary>
        /// Событие начала матча (сервер)
        /// </summary>
        public UnityAction SvMatchStart;
        /// <summary>
        /// Событие окончания матча (сервер)
        /// </summary>
        public UnityAction SvMatchEnd;

        /// <summary>
        /// Матч активен?
        /// </summary>
        [SyncVar]
        private bool matchActive;
        public bool IsMatchActive => matchActive;

        /// <summary>
        /// Условия завершения матча
        /// </summary>
        private IMatchCondition[] matchConditions;

        /// <summary>
        /// ID победившей команды
        /// </summary>
        public int WinTeamID = -1;


        private void Awake()
        {
            matchConditions = GetComponentsInChildren<IMatchCondition>();
        }

        private void Update()
        {
            if (isServer)
            {
                if (matchActive)
                {
                    foreach (var c in matchConditions)
                    {
                        if (c.IsTriggered)
                        {
                            SvEndMatch();
                            break;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Перезапуск матча
        /// </summary>
        [Server]
        public void SvRestartMatch()
        {
            if (matchActive) return;

            matchActive = true;

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

            foreach (var c in matchConditions)
            {
                c.OnServerMatchStart(this);
            }

            SvMatchStart?.Invoke();

            RpcMatchStart();
        }

        /// <summary>
        /// Матч завершён
        /// </summary>
        [Server]
        public void SvEndMatch()
        {
            foreach (var c in matchConditions)
            {
                c.OnServerMatchEnd(this);

                if (c is ConditionTeamDeathmatch)
                {
                    WinTeamID = (c as ConditionTeamDeathmatch).WinTeamID;
                }

                if (c is ConditionCaptureBase)
                {
                    if ((c as ConditionCaptureBase).RedBaseCaptureLevel == 100)
                    {
                        WinTeamID = TeamSide.TeamBlue;
                    }

                    if ((c as ConditionCaptureBase).BlueBaseCaptureLevel == 100)
                    {
                        WinTeamID = TeamSide.TeamRed;
                    }
                }
            }

            matchActive = false;

            SvMatchEnd?.Invoke();

            RpcMatchEnd(WinTeamID);
        }


        /// <summary>
        /// Матч стартовал (клиент)
        /// </summary>
        [ClientRpc]
        private void RpcMatchStart()
        {
            MatchStart?.Invoke();
        }

        /// <summary>
        /// Матч завершился (клиент)
        /// </summary>
        [ClientRpc]
        private void RpcMatchEnd(int winTeamID)
        {
            WinTeamID = winTeamID;
            MatchEnd?.Invoke();
        }
    }
}