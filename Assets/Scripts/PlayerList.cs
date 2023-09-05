using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Список игроков
    /// </summary>
    public class PlayerList : NetworkBehaviour
    {
		/// <summary>
		/// Синглтон
		/// </summary>
		public static PlayerList Instance;

		/// <summary>
		/// Событие при обновлении списка игроков
		/// </summary>
		public static UnityAction<List<PlayerData>> UpdatePlayerList;

        /// <summary>
        /// Список всех игроков
        /// </summary>
        [SerializeField] private List<PlayerData> m_AllPlayersData = new List<PlayerData>();
		public List<PlayerData> AllUsersData => m_AllPlayersData;


        private void Awake()
        {
			Instance = this;
        }


        public override void OnStartClient()
        {
            base.OnStartClient();

			m_AllPlayersData.Clear();
        }


		/// <summary>
		/// Команда на сервер добавить игрока в список
		/// </summary>
		/// <param name="data">Данные игрока</param>
		[Server]
		public void SvAddPlayer(PlayerData data)
		{
			m_AllPlayersData.Add(data);

			RpcClearPlayerDataList();

			for (int i = 0; i < m_AllPlayersData.Count; i++)
            {
				RpcAddPlayer(m_AllPlayersData[i]);
			}
		}

		/// <summary>
		/// Команда на сервер удалить игрока из списка
		/// </summary>
		/// <param name="data">Данные игрока</param>
		[Server]
		public void SvRemovePlayer(PlayerData data)
		{
			for (int i = 0; i < m_AllPlayersData.Count; i++)
			{
				if (m_AllPlayersData[i].ID == data.ID)
                {
					m_AllPlayersData.RemoveAt(i);
					break;
                }
			}

			RpcRemovePlayer(data);
		}

		/// <summary>
		/// Клиент, очистить список игроков
		/// </summary>
		[ClientRpc]
		private void RpcClearPlayerDataList()
		{
			if (isServer) return;

			m_AllPlayersData.Clear();
		}

		/// <summary>
		/// Клиент, добавить игрока в список
		/// </summary>
		/// <param name="data">Данные игрока</param>
		[ClientRpc]
		private void RpcAddPlayer(PlayerData data)
		{
			if (isClient && isServer)
            {
				UpdatePlayerList?.Invoke(m_AllPlayersData);
				return;
            }

			m_AllPlayersData.Add(data);

			UpdatePlayerList?.Invoke(m_AllPlayersData);
		}

		/// <summary>
		/// Клиент, удалить игрока из списка
		/// </summary>
		/// <param name="data">Данные игрока</param>
		[ClientRpc]
		private void RpcRemovePlayer(PlayerData data)
		{
			for (int i = 0; i < m_AllPlayersData.Count; i++)
			{
				if (m_AllPlayersData[i].ID == data.ID)
				{
					m_AllPlayersData.RemoveAt(i);
					break;
				}
			}

			UpdatePlayerList?.Invoke(m_AllPlayersData);
		}
	}
}