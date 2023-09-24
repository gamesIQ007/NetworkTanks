using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    /// <summary>
    /// Список участников матча
    /// </summary>
    public class MatchMemberList : NetworkBehaviour
    {
		/// <summary>
		/// Синглтон
		/// </summary>
		public static MatchMemberList Instance;

		/// <summary>
		/// Событие при обновлении списка участников матча
		/// </summary>
		public static UnityAction<List<MatchMemberData>> UpdateList;

        /// <summary>
        /// Список всех участников матча
        /// </summary>
        [SerializeField] private List<MatchMemberData> allMembersData = new List<MatchMemberData>();
		public int MemberDataCount => allMembersData.Count;


        private void Awake()
        {
			Instance = this;
        }


        public override void OnStartClient()
        {
            base.OnStartClient();

			allMembersData.Clear();
        }


		/// <summary>
		/// Команда на сервер добавить участника матча в список
		/// </summary>
		/// <param name="data">Данные участника матча</param>
		[Server]
		public void SvAddMatchMember(MatchMemberData data)
		{
			allMembersData.Add(data);

			RpcClearMatchMemberDataList();

			for (int i = 0; i < allMembersData.Count; i++)
            {
				RpcAddMatchMember(allMembersData[i]);
			}
		}

		/// <summary>
		/// Команда на сервер удалить участника матча из списка
		/// </summary>
		/// <param name="data">Данные участника матча</param>
		[Server]
		public void SvRemoveMatchMember(MatchMemberData data)
		{
			for (int i = 0; i < allMembersData.Count; i++)
			{
				if (allMembersData[i].ID == data.ID)
                {
					allMembersData.RemoveAt(i);
					break;
                }
			}

			RpcRemoveMatchMember(data);
		}

		/// <summary>
		/// Клиент, очистить список участников матча
		/// </summary>
		[ClientRpc]
		private void RpcClearMatchMemberDataList()
		{
			if (isServer) return;

			allMembersData.Clear();
		}

		/// <summary>
		/// Клиент, добавить участника матча в список
		/// </summary>
		/// <param name="data">Данные участника матча</param>
		[ClientRpc]
		private void RpcAddMatchMember(MatchMemberData data)
		{
			if (isClient && isServer)
            {
				UpdateList?.Invoke(allMembersData);
				return;
            }

			allMembersData.Add(data);

			UpdateList?.Invoke(allMembersData);
		}

		/// <summary>
		/// Клиент, удалить участника матча из списка
		/// </summary>
		/// <param name="data">Данные участника матча</param>
		[ClientRpc]
		private void RpcRemoveMatchMember(MatchMemberData data)
		{
			for (int i = 0; i < allMembersData.Count; i++)
			{
				if (allMembersData[i].ID == data.ID)
				{
					allMembersData.RemoveAt(i);
					break;
				}
			}

			UpdateList?.Invoke(allMembersData);
		}
	}
}