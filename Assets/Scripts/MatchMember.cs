using UnityEngine;
using UnityEngine.Events;
using Mirror;

namespace NetworkTanks
{
    [System.Serializable]
    /// <summary>
    /// Данные участника матча
    /// </summary>
    public class MatchMemberData
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
        /// <summary>
        /// Участник
        /// </summary>
        public NetworkIdentity Member;

        public MatchMemberData(int id, string nickname, int teamID, NetworkIdentity member)
        {
            ID = id;
            Nickname = nickname;
            TeamID = teamID;
            Member = member;
        }
    }


    /// <summary>
    /// Сериализация/десериализация данных участника матча
    /// </summary>
    public static class MatchMemberDataExtention
    {
        /// <summary>
        /// Сериализация данных учасника матча
        /// </summary>
        /// <param name="writer">Писец</param>
        /// <param name="value">Данные учасника матча</param>
        public static void WriteMatchMemberData(this NetworkWriter writer, MatchMemberData value)
        {
            writer.WriteInt(value.ID);
            writer.WriteString(value.Nickname);
            writer.WriteInt(value.TeamID);
            writer.WriteNetworkIdentity(value.Member);
        }

        /// <summary>
        /// Десериализация данных учасника матча
        /// </summary>
        /// <param name="reader">Чтец</param>
        /// <returns>Данные участника матча</returns>
        public static MatchMemberData ReadMatchMemberData(this NetworkReader reader)
        {
            return new MatchMemberData(reader.ReadInt(), reader.ReadString(), reader.ReadInt(), reader.ReadNetworkIdentity());
        }
    }


    /// <summary>
    /// Участник боя
    /// </summary>
    public class MatchMember : NetworkBehaviour
    {
        /// <summary>
        /// Событие изменения количества фрагов (id участника матча и количество его фрагов)
        /// </summary>
        public static event UnityAction<MatchMember, int> ChangeFrags;

        /// <summary>
        /// Активный транспорт
        /// </summary>
        public Vehicle ActiveVehicle { get; set; }


        #region Data

        /// <summary>
        /// Данные участника матча
        /// </summary>
        protected MatchMemberData data;
        public MatchMemberData Data => data;

        /// <summary>
        /// Обновить данные
        /// </summary>
        /// <param name="data">Данные</param>
        [Command]
        protected void CmdUpdateData(MatchMemberData data)
        {
            this.data = data;
        }

        #endregion


        #region Frags

        [SyncVar(hook = nameof(OnFragsChanged))]
        /// <summary>
        /// Количество фрагов
        /// </summary>
        protected int fragsAmount;

        /// <summary>
        /// Изменение количества фрагов на клиенте
        /// </summary>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
        protected void OnFragsChanged(int oldValue, int newValue)
        {
            ChangeFrags?.Invoke(this, newValue);
        }

        /// <summary>
        /// Сбросить количество фрагов
        /// </summary>
        [Server]
        public void SvResetFrags()
        {
            fragsAmount = 0;
        }

        /// <summary>
        /// Добавить фраг
        /// </summary>
        [Server]
        public void SvAddFrags()
        {
            fragsAmount++;

            ChangeFrags?.Invoke(this, fragsAmount);
        }

        #endregion


        #region Nickname

        /// <summary>
        /// Ник
        /// </summary>
        [SyncVar(hook = nameof(OnNicknameChanged))]
        protected string nickname;
        public string Nickname => nickname;

        /// <summary>
        /// Изменение ника
        /// </summary>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
        protected void OnNicknameChanged(string oldValue, string newValue)
        {
            gameObject.name = newValue; // На клиенте
        }

        /// <summary>
        /// Задать ник
        /// </summary>
        /// <param name="name">Ник</param>
        [Command] // На сервере
        public void CmdSetName(string name)
        {
            nickname = name;
            gameObject.name = name;
        }

        #endregion


        #region TeamID

        /// <summary>
        /// ID команды
        /// </summary>
        [SyncVar]
        protected int teamId;
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

        #endregion
    }
}