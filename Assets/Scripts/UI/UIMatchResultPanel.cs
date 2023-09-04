using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Обображение результатов матча
    /// </summary>
    public class UIMatchResultPanel : MonoBehaviour
    {
        /// <summary>
        /// Панель результатов
        /// </summary>
        [SerializeField] private GameObject resultPanel;

        /// <summary>
        /// Текст результата
        /// </summary>
        [SerializeField] private Text resultText;


        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnMatchStart;
            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
        }

        private void OnDestroy()
        {
            NetworkSessionManager.Match.MatchStart -= OnMatchStart;
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }


        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnMatchStart()
        {
            resultPanel.SetActive(false);
        }

        /// <summary>
        /// При завершении матча
        /// </summary>
        private void OnMatchEnd()
        {
            resultPanel.SetActive(true);

            int winTeamID = NetworkSessionManager.Match.WinTeamID;

            if (winTeamID == -1)
            {
                resultText.text = "Ничья";
                return;
            }

            if (winTeamID == Player.Local.TeamID)
            {
                resultText.text = "Победа";
                return;
            }
            else
            {
                resultText.text = "Поражение";
                return;
            }
        }
    }
}