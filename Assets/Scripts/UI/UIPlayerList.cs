using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Отображение списка игроков
    /// </summary>
    public class UIPlayerList : MonoBehaviour
    {
        /// <summary>
        /// Панель игроков своей команды
        /// </summary>
        [SerializeField] private Transform localTeamPanel;
        /// <summary>
        /// Панель игроков вражеской команды
        /// </summary>
        [SerializeField] private Transform otherTeamPanel;

        /// <summary>
        /// Префаб метки об игроке
        /// </summary>
        [SerializeField] private UIPlayerLable playerLablePrefab;

        /// <summary>
        /// Список всех меток игроков
        /// </summary>
        private List<UIPlayerLable> allPlayerLable = new List<UIPlayerLable>();

        /// <summary>
        /// Массив транспортов
        /// </summary>
        private Vehicle[] vehicles;


        private void Start()
        {
            MatchMemberList.UpdateList += OnUpdatePlayerList;
            Player.ChangeFrags += OnChangeFrags;

            NetworkSessionManager.Match.MatchStart += OnStartMatch;
        }

        private void OnDestroy()
        {
            MatchMemberList.UpdateList -= OnUpdatePlayerList;
            Player.ChangeFrags -= OnChangeFrags;

            if (NetworkSessionManager.Instance != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnStartMatch;
            }
        }


        /// <summary>
        /// Обновить список игроков
        /// </summary>
        /// <param name="playerData">Список данных игроков</param>
        private void OnUpdatePlayerList(List<MatchMemberData> playerData)
        {
            for (int i = 0; i < localTeamPanel.childCount; i++)
            {
                Destroy(localTeamPanel.GetChild(i).gameObject);
            }
            for (int i = 0; i < otherTeamPanel.childCount; i++)
            {
                Destroy(otherTeamPanel.GetChild(i).gameObject);
            }

            allPlayerLable.Clear();

            for (int i = 0; i < playerData.Count; i++)
            {
                if (playerData[i].TeamID == Player.Local.TeamID)
                {
                    AddPlayerLable(playerData[i], playerLablePrefab, localTeamPanel);
                }
                if (playerData[i].TeamID != Player.Local.TeamID)
                {
                    AddPlayerLable(playerData[i], playerLablePrefab, otherTeamPanel);
                }
            }
        }

        /// <summary>
        /// Добавить метку об участнике
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="playerLable">Метка</param>
        /// <param name="parent">Родитель</param>
        private void AddPlayerLable(MatchMemberData data, UIPlayerLable playerLable, Transform parent)
        {
            UIPlayerLable uiPlayerLable = Instantiate(playerLable);
            uiPlayerLable.transform.SetParent(parent);
            uiPlayerLable.Init(data.ID, data.Nickname);

            allPlayerLable.Add(uiPlayerLable);
        }

        /// <summary>
        /// При изменении фрагов
        /// </summary>
        /// <param name="member">Участник матча</param>
        /// <param name="frag">Количество фрагов</param>
        private void OnChangeFrags(MatchMember member, int frag)
        {
            for (int i = 0; i < allPlayerLable.Count; i++)
            {
                if (allPlayerLable[i].NetID == member.netId)
                {
                    allPlayerLable[i].UpdateFrag(frag);
                }
            }

            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i].HitPoint == 0)
                {
                    for (int j = 0; j < allPlayerLable.Count; j++)
                    {
                        if (allPlayerLable[j].NetID == vehicles[i].netId)
                        {
                            allPlayerLable[j].UpdateNickname();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnStartMatch()
        {
            vehicles = FindObjectsOfType<Vehicle>();
        }
    }
}