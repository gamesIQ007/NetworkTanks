using System.Collections.Generic;
using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Коллектор отображений информации о танках
    /// </summary>
    public class UITankInfoCollector : MonoBehaviour
    {
        /// <summary>
        /// Панель с информацией о танках
        /// </summary>
        [SerializeField] private Transform tankInfoPanel;

        /// <summary>
        /// Префаб блока с информацией о танке
        /// </summary>
        [SerializeField] private UITankInfo tankInfoPrefab;

        /// <summary>
        /// Массив блоков с информацией о танках
        /// </summary>
        private UITankInfo[] tankInfos;
        /// <summary>
        /// Список игроков без локального
        /// </summary>
        private List<Player> playerWithoutLocal;


        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnStartMatch;
            NetworkSessionManager.Match.MatchEnd += OnEndMatch;
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnStartMatch;
                NetworkSessionManager.Match.MatchEnd -= OnEndMatch;
            }
        }

        private void Update()
        {
            if (tankInfos == null) return;

            for (int i = 0; i < tankInfos.Length; i++)
            {
                if (tankInfos[i] == null) continue;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(tankInfos[i].Tank.transform.position + tankInfos[i].WorldOffset);

                if (screenPos.z > 0)
                {
                    tankInfos[i].transform.position = screenPos;
                }
            }
        }


        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnStartMatch()
        {
            Player[] players = FindObjectsOfType<Player>();

            playerWithoutLocal = new List<Player>(players.Length - 1);

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == Player.Local) continue;

                playerWithoutLocal.Add(players[i]);
            }

            tankInfos = new UITankInfo[playerWithoutLocal.Count];

            for (int i = 0; i < playerWithoutLocal.Count; i++)
            {
                tankInfos[i] = Instantiate(tankInfoPrefab);
                tankInfos[i].SetTank(playerWithoutLocal[i].ActiveVehicle);
                tankInfos[i].transform.SetParent(tankInfoPanel);
            }
        }

        /// <summary>
        /// При завершении матча
        /// </summary>
        private void OnEndMatch()
        {
            for (int i = 0; i < tankInfoPanel.transform.childCount; i++)
            {
                Destroy(tankInfoPanel.transform.GetChild(i).gameObject);
            }

            tankInfos = null;
        }
    }
}