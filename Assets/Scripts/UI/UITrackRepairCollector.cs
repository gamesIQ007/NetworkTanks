using System.Collections.Generic;
using UnityEngine;

namespace NetworkTanks
{
    /// <summary>
    /// Сборщик информации о ремонта траков
    /// </summary>
    public class UITrackRepairCollector : MonoBehaviour
    {
        /// <summary>
        /// Панель с информацией о ремонте
        /// </summary>
        [SerializeField] private Transform trackRepairInfoPanel;

        /// <summary>
        /// Префаб блока с информацией о ремонте
        /// </summary>
        [SerializeField] private UITrackRepair trackRepairInfoPrefab;

        /// <summary>
        /// Массив блоков с информацией о ремонте
        /// </summary>
        private List<UITrackRepair> trackRepairInfos;
        /// <summary>
        /// Список модулей
        /// </summary>
        private VehicleModule[] vehicleModules;


        private void Start()
        {
            NetworkSessionManager.Match.MatchStart += OnStartMatch;
            NetworkSessionManager.Match.MatchEnd += OnEndMatch;

            trackRepairInfos = new List<UITrackRepair>();
        }

        private void OnDestroy()
        {
            if (NetworkSessionManager.Instance != null)
            {
                NetworkSessionManager.Match.MatchStart -= OnStartMatch;
                NetworkSessionManager.Match.MatchEnd -= OnEndMatch;
            }

            for (int i = 0; i < vehicleModules.Length; i++)
            {
                vehicleModules[i].Destroyed -= OnTrackDestroyed;
            }
        }

        private void Update()
        {
            if (trackRepairInfos == null) return;

            for (int i = 0; i < trackRepairInfos.Count; i++)
            {
                if (trackRepairInfos[i] == null) continue;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(trackRepairInfos[i].Tank.transform.position + trackRepairInfos[i].WorldOffset);

                if (screenPos.z > 0)
                {
                    trackRepairInfos[i].transform.position = screenPos;
                }
            }
        }


        /// <summary>
        /// При старте матча
        /// </summary>
        private void OnStartMatch()
        {
            vehicleModules = FindObjectsOfType<VehicleModule>();

            for (int i = 0; i < vehicleModules.Length; i++)
            {
                vehicleModules[i].Destroyed += OnTrackDestroyed;
            }
        }

        /// <summary>
        /// При завершении матча
        /// </summary>
        private void OnEndMatch()
        {
            for (int i = 0; i < trackRepairInfoPanel.transform.childCount; i++)
            {
                Destroy(trackRepairInfoPanel.transform.GetChild(i).gameObject);
            }

            vehicleModules = null;
            trackRepairInfos.Clear();
        }

        /// <summary>
        /// При уничтожении трака
        /// </summary>
        /// <param name="destructible">Дестрактибл</param>
        private void OnTrackDestroyed(Destructible destructible)
        {
            UITrackRepair trackRepair = Instantiate(trackRepairInfoPrefab);
            trackRepair.SetTank(destructible.gameObject.GetComponent<TrackTank>());
            trackRepair.SetRepairInfo((destructible as VehicleModule).RemainingRecoveryTime);
            trackRepair.transform.SetParent(trackRepairInfoPanel);

            trackRepairInfos.Add(trackRepair);
        }
    }
}