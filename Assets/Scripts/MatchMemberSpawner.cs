using UnityEngine;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Спавнер участников матча
    /// </summary>
    public class MatchMemberSpawner : NetworkBehaviour
    {
        /// <summary>
        /// Префаб бота
        /// </summary>
        [SerializeField] private GameObject botPrefab;

        /// <summary>
        /// Количество участников в команде
        /// </summary>
        [Range(0, 15)]
        [SerializeField] private int targetAmountMemberTeam;


        /// <summary>
        /// Респавн транспорт всех участников матча
        /// </summary>
        [Server]
        public void SvRespawnVehiclesAllMembers()
        {
            SvRespawnPlayerVehicles();
            SvRespawnBotVehicles();
        }

        /// <summary>
        /// Респавн транспорта игроков
        /// </summary>
        [Server]
        private void SvRespawnPlayerVehicles()
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

        /// <summary>
        /// Респавн транспорта ботов
        /// </summary>
        [Server]
        private void SvRespawnBotVehicles()
        {
            foreach (var b in FindObjectsOfType<Bot>())
            {
                NetworkServer.UnSpawn(b.gameObject);
                Destroy(b.gameObject);
            }

            int botAmount = targetAmountMemberTeam * 2 - MatchMemberList.Instance.MemberDataCount;

            for (int i = 0; i < botAmount; i++)
            {
                GameObject b = Instantiate(botPrefab);
                NetworkServer.Spawn(b);
            }
        }
    }
}