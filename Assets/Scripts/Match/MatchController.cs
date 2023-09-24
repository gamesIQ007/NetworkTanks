using System.Collections;
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
        /// Счётчик ID команд
        /// </summary>
        private static int teamIdCounter;

        /// <summary>
        /// Получить номер следующей команды
        /// </summary>
        /// <returns>Номер следующей команды</returns>
        public static int GetNextTeam()
        {
            return teamIdCounter++ % 2;
        }

        /// <summary>
        /// Сбросить счётчик команд
        /// </summary>
        public static void ResetTeamCounter()
        {
            teamIdCounter = 1;
        }


        /// <summary>
        /// Событие начала матча
        /// </summary>
        public event UnityAction MatchStart;
        /// <summary>
        /// Событие окончания матча
        /// </summary>
        public event UnityAction MatchEnd;

        /// <summary>
        /// Событие начала матча (сервер)
        /// </summary>
        public event UnityAction SvMatchStart;
        /// <summary>
        /// Событие окончания матча (сервер)
        /// </summary>
        public event UnityAction SvMatchEnd;

        /// <summary>
        /// Спавнер участников матча
        /// </summary>
        [SerializeField] private MatchMemberSpawner spawner;
        /// <summary>
        /// Задержка между спавном и началом матча
        /// </summary>
        [SerializeField] private float delayAfterSpawnBeforeStartMatch = 0.5f;

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

            spawner.SvRespawnVehiclesAllMembers();

            StartCoroutine(StartEventMatchWithDelay(delayAfterSpawnBeforeStartMatch));
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
        /// Запуск матча с задержкой
        /// </summary>
        /// <param name="delay">Задержка</param>
        /// <returns>Задержка</returns>
        private IEnumerator StartEventMatchWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            foreach (var c in matchConditions)
            {
                c.OnServerMatchStart(this);
            }

            SvMatchStart?.Invoke();

            RpcMatchStart();
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