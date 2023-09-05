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


        private void Start()
        {
            PlayerList.UpdatePlayerList += OnUpdatePlayerList;
            Player.ChangeFrags += OnChangeFrags;
        }

        private void OnDestroy()
        {
            PlayerList.UpdatePlayerList -= OnUpdatePlayerList;
            Player.ChangeFrags -= OnChangeFrags;
        }


        /// <summary>
        /// Обновить список игроков
        /// </summary>
        /// <param name="playerData">Список данных игроков</param>
        private void OnUpdatePlayerList(List<PlayerData> playerData)
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

        private void AddPlayerLable(PlayerData data, UIPlayerLable playerLable, Transform parent)
        {
            UIPlayerLable uiPlayerLable = Instantiate(playerLable);
            uiPlayerLable.transform.SetParent(parent);
            uiPlayerLable.Init(data.ID, data.Nickname);

            allPlayerLable.Add(uiPlayerLable);
        }

        /// <summary>
        /// При изменении фрагов
        /// </summary>
        /// <param name="playerNetId">ID игрока</param>
        /// <param name="frag">Количество фрагов</param>
        private void OnChangeFrags(int playerNetId, int frag)
        {
            for (int i = 0; i < allPlayerLable.Count; i++)
            {
                if (allPlayerLable[i].NetID == playerNetId)
                {
                    allPlayerLable[i].UpdateFrag(frag);
                }
            }
        }
    }
}