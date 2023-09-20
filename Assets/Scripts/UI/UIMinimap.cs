using UnityEngine;
using UnityEngine.UI;

namespace NetworkTanks
{
    /// <summary>
    /// Миникарта
    /// </summary>
    public class UIMinimap : MonoBehaviour
    {
        /// <summary>
        /// Размер карты
        /// </summary>
        [SerializeField] private SizeMap sizeMap;

        /// <summary>
        /// Префаб значка танка
        /// </summary>
        [SerializeField] private UITankMark tankMarkPrefab;

        /// <summary>
        /// Фоновое изображение
        /// </summary>
        [SerializeField] private Image background;

        /// <summary>
        /// Массив меток танков
        /// </summary>
        private UITankMark[] tankMarks;
        /// <summary>
        /// Массив игроков
        /// </summary>
        private Vehicle[] vehicles;


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
            if (tankMarks == null) return;

            for (int i = 0; i < tankMarks.Length; i++)
            {
                if (vehicles[i] == null) continue;

                Vector3 normPos = sizeMap.GetNormPos(vehicles[i].transform.position);

                Vector3 posInMinimap = new Vector3(normPos.x * background.rectTransform.sizeDelta.x/* * 0.5f*/, normPos.z * background.rectTransform.sizeDelta.y/* * 0.5f*/, 0);

                tankMarks[i].transform.position = background.transform.position + posInMinimap;
            }
        }


        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnStartMatch()
        {
            vehicles = FindObjectsOfType<Vehicle>();

            tankMarks = new UITankMark[vehicles.Length];

            for (int i = 0; i < tankMarks.Length; i++)
            {
                tankMarks[i] = Instantiate(tankMarkPrefab);

                if (vehicles[i].TeamID == Player.Local.TeamID)
                {
                    tankMarks[i].SetLocalColor();
                }
                else
                {
                    tankMarks[i].SetOtherColor();
                }

                tankMarks[i].transform.SetParent(background.transform);
            }
        }

        /// <summary>
        /// При завершении матча
        /// </summary>
        private void OnEndMatch()
        {
            for (int i = 0; i < background.transform.childCount; i++)
            {
                Destroy(background.transform.GetChild(i).gameObject);
            }

            tankMarks = null;
        }
    }
}